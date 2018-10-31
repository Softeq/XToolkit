// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Widget;
using Plugin.CurrentActivity;

namespace Softeq.XToolkit.Tooltips.Droid
{
    public class TooltipView : RelativeLayout, ViewTreeObserver.IOnPreDrawListener, View.IOnClickListener
    {
        private const string TranslationYCompat = "translationY";
        private const string TranslationXCompat = "translationX";
        private const string ScaleXCompat = "scaleX";
        private const string ScaleYCompat = "scaleY";
        private const string AlphaCompat = "alpha";

        private ImageView _topPointerView;
        private ViewGroup _contentHolder;
        private TextView _tooltipTextView;
        private ImageView _bottomPointerView;
        private View _shadowView;

        private Tooltip _tooltipController;
        private View _targetView;

        private bool _dimensionsKnown;
        private int _tooltipWidth;
        private int _targetViewRelativeY;
        private int _targetViewRelativeX;

        private Timer _autoDismissTimer;

        /// <summary>
        /// Additional action to be performed on tooltip click besides dismissing it
        /// </summary>
        public event EventHandler OnTooltipClicked;

        public TooltipView(Context context) : base(context) => Init();
        public TooltipView(Context context, IAttributeSet attrs)
            : base(context, attrs) => Init();
        public TooltipView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle) => Init();

        private void Init()
        {
            Inflate(Context, Resource.Layout.tooltip, this);

            _topPointerView = FindViewById<ImageView>(Resource.Id.tooltip_pointer_up);
            _contentHolder = (ViewGroup)FindViewById(Resource.Id.tooltip_contentholder);
            _tooltipTextView = FindViewById<TextView>(Resource.Id.tooltip_contenttv);
            _bottomPointerView = FindViewById<ImageView>(Resource.Id.tooltip_pointer_down);
            _shadowView = FindViewById(Resource.Id.tooltip_shadow);

            SetOnClickListener(this);
            ViewTreeObserver.AddOnPreDrawListener(this);
        }

        public bool OnPreDraw()
        {
            ViewTreeObserver.RemoveOnPreDrawListener(this);
            _dimensionsKnown = true;

            if (_tooltipController != null)
            {
                ApplyTooltipPosition();
            }
            return true;
        }

        /// <summary>
        /// Specify settings for tooltip view
        /// </summary>
        /// <param name="tooltipController">Tooltip controller with settings for this tooltip view</param>
        /// <param name="targetView">Target view to attach tooltip view to</param>
        public void SetTooltip(Tooltip tooltipController, View targetView)
        {
            _tooltipController = tooltipController;
            _targetView = targetView;

            //Set id if specified
            if (_tooltipController.IdIsSpecified)
            {
                Id = _tooltipController.Id;
            }

            // Set background color
            if (_tooltipController.Color != 0)
            {
                SetColor(_tooltipController.Color);
            }

            // Set custom view or set up default text view
            if (_tooltipController.CustomView != null)
            {
                SetCustomView(_tooltipController.CustomView);
            }
            else
            {
                SetUpTextView();
            }

            SetUpCustomSizes();
            SetUpShadow();

            if (_dimensionsKnown)
            {
                ApplyTooltipPosition();
            }
        }

        private void ApplyTooltipPosition()
        {
            //Set correct view width depending on shadow
            var shadowSize = _tooltipController.ShouldShowShadow
                                        ? _tooltipController.ShadowSizePx : 0;
            _tooltipWidth = _contentHolder.Width + 2 * shadowSize;
            LayoutParameters.Width = _tooltipWidth;

            // Find relative position of target view
            int[] targetViewScreenPosition = new int[2];
            _targetView.GetLocationOnScreen(targetViewScreenPosition);

            Rect windowDisplayFrame = new Rect();
            _targetView.GetWindowVisibleDisplayFrame(windowDisplayFrame);

            int[] parentViewScreenPosition = new int[2];
            ((View)Parent).GetLocationOnScreen(parentViewScreenPosition);

            int targetViewWidth = _targetView.Width;
            int targetViewHeight = _targetView.Height;

            _targetViewRelativeX = targetViewScreenPosition[0] - parentViewScreenPosition[0];
            _targetViewRelativeY = targetViewScreenPosition[1] - parentViewScreenPosition[1];
            int targetViewRelativeCenterX = _targetViewRelativeX + targetViewWidth / 2;

            var minScreenSpacingHorizontal = Math.Max(_tooltipController.MinScreenSpacingPx - shadowSize, 0);
            // Find tooltip view X position
            int tooltipViewX = Math.Max(minScreenSpacingHorizontal, targetViewRelativeCenterX - _tooltipWidth / 2);
            if (tooltipViewX + _tooltipWidth > windowDisplayFrame.Right - minScreenSpacingHorizontal)
            {
                tooltipViewX = windowDisplayFrame.Right - minScreenSpacingHorizontal - _tooltipWidth;
            }

            SetX(tooltipViewX);
            SetPointerCenterX(targetViewRelativeCenterX);

            //Find tooltip view Y position
            int tooltipViewY;

            if (_tooltipController.CenterWithTargetView)
            {
                int targetViewRelativeCenterY = _targetViewRelativeY + targetViewHeight / 2;
                var tooltipViewYInitial = targetViewRelativeCenterY - Height / 2 + _tooltipController.VerticalOffsetPx;
                var lowestTooltipY = windowDisplayFrame.Bottom - _tooltipController.MinScreenSpacingPx - Height;
                tooltipViewY = Math.Max(_tooltipController.MinScreenSpacingPx, Math.Min(tooltipViewYInitial, lowestTooltipY));
                SetPointersVisibility(_tooltipController.PreferToShowBelow);
            }
            else
            {
                int tooltipViewYIfAbove = _targetViewRelativeY - Height;
                int toolTipViewYIfBelow = Math.Max(_tooltipController.MinScreenSpacingPx, _targetViewRelativeY + targetViewHeight);

                bool showBelow = tooltipViewYIfAbove < _tooltipController.MinScreenSpacingPx ||
                               (_tooltipController.PreferToShowBelow &&
                               (toolTipViewYIfBelow + Height < windowDisplayFrame.Bottom - _tooltipController.MinScreenSpacingPx));

                SetPointersVisibility(showBelow);

                if (!_tooltipController.ShowPointer)
                {
                    tooltipViewYIfAbove += _bottomPointerView.MeasuredHeight;
                    toolTipViewYIfBelow -= _topPointerView.MeasuredHeight;
                }

                tooltipViewY = showBelow ? toolTipViewYIfBelow : tooltipViewYIfAbove;
            }

            if (_tooltipController.ShouldShowShadow)
            {
                // Update shadow size
                var layoutParams = _shadowView.LayoutParameters;
                layoutParams.Width = _contentHolder.Width + 2 * _tooltipController.ShadowSizePx;
                layoutParams.Height = Math.Min(_contentHolder.Height + 2 * _tooltipController.ShadowSizePx, Height);
                _shadowView.LayoutParameters = layoutParams;
            }

            // Show tooltip
            RevealTooltip(tooltipViewX, tooltipViewY);
        }

        private void SetPointerCenterX(int pointerCenterX)
        {
            _topPointerView.SetX(pointerCenterX - _topPointerView.MeasuredWidth / 2 - (int)GetX());
            _bottomPointerView.SetX(pointerCenterX - _bottomPointerView.MeasuredWidth / 2 - (int)GetX());
        }

        private void SetColor(Color color)
        {
            _topPointerView.SetColorFilter(color, PorterDuff.Mode.Multiply);
            _bottomPointerView.SetColorFilter(color, PorterDuff.Mode.Multiply);
            _contentHolder.Background.SetColorFilter(color, PorterDuff.Mode.Multiply);
        }

        private void SetPointersVisibility(bool showBelow)
        {
            _topPointerView.Visibility = showBelow && _tooltipController.ShowPointer ? ViewStates.Visible : ViewStates.Invisible;
            _bottomPointerView.Visibility = !showBelow && _tooltipController.ShowPointer ? ViewStates.Visible : ViewStates.Invisible;
        }

        private void SetCustomView(View view)
        {
            _contentHolder.RemoveAllViews();
            _contentHolder.AddView(view);
        }

        private void SetUpCustomSizes()
        {
            var horizontalPadding = _tooltipController.HasCustomHorizontalPadding
                                ? _tooltipController.HorizontalPaddingPx
                                : _contentHolder.PaddingLeft;
            var verticalPadding = _tooltipController.HasCustomVerticalPadding
                                 ? _tooltipController.VerticalPaddingPx
                                 : _contentHolder.PaddingTop;
            _contentHolder.SetPadding(horizontalPadding, verticalPadding,
                                      horizontalPadding, verticalPadding);

            if (_tooltipController.HasCustomCornerRadius)
            {
                GradientDrawable drawable = (GradientDrawable)_contentHolder.Background;
                drawable.SetCornerRadius(_tooltipController.CornerRadiusPx);
            }

            if (_tooltipController.PointerSpacingPx > 0)
            {
                _topPointerView.SetPadding(_topPointerView.PaddingLeft, _topPointerView.PaddingTop + _tooltipController.PointerSpacingPx,
                                           _topPointerView.PaddingRight, _topPointerView.PaddingBottom);
                _bottomPointerView.SetPadding(_bottomPointerView.PaddingLeft, _bottomPointerView.PaddingTop, _bottomPointerView.PaddingRight,
                                              _bottomPointerView.PaddingBottom + _tooltipController.PointerSpacingPx);
            }
        }

        private void SetUpShadow()
        {
            _shadowView.Visibility = !_tooltipController.ShouldShowShadow ? ViewStates.Gone : ViewStates.Visible;
        }

        private void SetUpTextView()
        {
            // Set text
            if (_tooltipController.Text != null)
            {
                _tooltipTextView.Text = _tooltipController.Text;
            }

            // Set font
            if (_tooltipController.Typeface != null)
            {
                _tooltipTextView.Typeface = _tooltipController.Typeface;
            }

            // Set text size
            if (_tooltipController.HasCustomTextSize)
            {
                _tooltipTextView.SetTextSize(ComplexUnitType.Px, _tooltipController.TextSizePx);
            }

            // Set text color
            if (_tooltipController.TextColor != 0)
            {
                _tooltipTextView.SetTextColor(_tooltipController.TextColor);
            }

            // Set max width if needed
            if (_tooltipController.HasCustomMaxWidth)
            {
                _tooltipTextView.SetMaxWidth(_tooltipController.MaxTextWidthPx);
            }
        }

        private void RevealTooltip(int tooltipViewX, int tooltipViewY)
        {
            if (_tooltipController.Animation == Tooltip.AnimationType.None)
            {
                TranslationY = tooltipViewY;
                TranslationX = tooltipViewX;
            }
            else
            {
                List<Animator> animators = new List<Animator>(3);

                if (_tooltipController.Animation == Tooltip.AnimationType.FromTargetView)
                {
                    animators.Add(ObjectAnimator.OfFloat(this, TranslationYCompat, _targetViewRelativeY + _targetView.Height / 2 - Height / 2, tooltipViewY));
                    animators.Add(ObjectAnimator.OfFloat(this, TranslationXCompat, _targetViewRelativeX + _targetView.Width / 2 - _tooltipWidth / 2, tooltipViewX));
                    animators.Add(ObjectAnimator.OfFloat(this, AlphaCompat, 0, 1));
                }
                else if (_tooltipController.Animation == Tooltip.AnimationType.FromTop)
                {
                    animators.Add(ObjectAnimator.OfFloat(this, TranslationYCompat, 0, tooltipViewY));
                    TranslationX = tooltipViewX;
                    animators.Add(ObjectAnimator.OfFloat(this, AlphaCompat, 0, 1));
                }
                else if (_tooltipController.Animation == Tooltip.AnimationType.FromBottom)
                {
                    Rect windowDisplayFrame = new Rect();
                    _targetView.GetWindowVisibleDisplayFrame(windowDisplayFrame);
                    animators.Add(ObjectAnimator.OfFloat(this, TranslationYCompat, windowDisplayFrame.Bottom, tooltipViewY));
                    TranslationX = tooltipViewX;
                    animators.Add(ObjectAnimator.OfFloat(this, AlphaCompat, 0, 1));
                }
                else if (_tooltipController.Animation == Tooltip.AnimationType.Grow)
                {
                    animators.Add(ObjectAnimator.OfFloat(this, ScaleXCompat, 0, 1));
                    animators.Add(ObjectAnimator.OfFloat(this, ScaleYCompat, 0, 1));
                    animators.Add(ObjectAnimator.OfFloat(this, AlphaCompat, 0, 1));
                    TranslationY = tooltipViewY;
                    TranslationX = tooltipViewX;
                }
                else if (_tooltipController.Animation == Tooltip.AnimationType.FadeInOut)
                {
                    animators.Add(ObjectAnimator.OfFloat(this, AlphaCompat, 0, 1));
                    TranslationY = tooltipViewY;
                    TranslationX = tooltipViewX;
                }

                AnimatorSet animatorSet = new AnimatorSet();
                animatorSet.PlayTogether(animators);

                animatorSet.Start();
                animatorSet.AnimationEnd += (sender, e) =>
                {
                    EnableAutomaticHidingIfNeeded();
                };
            }
        }

        private void EnableAutomaticHidingIfNeeded()
        {
            if (_tooltipController.AutoDismissTimeout > TimeSpan.Zero)
            {
                if (_autoDismissTimer != null)
                {
                    return;
                }
                _autoDismissTimer = new Timer(new TimerCallback((a) =>
                {
                    CrossCurrentActivity.Current.Activity.RunOnUiThread(RemoveTooltip);
                }), null, (int)_tooltipController.AutoDismissTimeout.TotalMilliseconds, Timeout.Infinite);
            }
        }

        private void StopAutoDismissTimer()
        {
            if (_autoDismissTimer != null)
            {
                _autoDismissTimer.Dispose();
                _autoDismissTimer = null;
            }
        }


        public void RemoveTooltip()
        {
            StopAutoDismissTimer();

            if (_tooltipController.Animation == Tooltip.AnimationType.None)
            {
                RemoveFromParent();
            }
            else
            {
                List<Animator> animators = new List<Animator>(3);
                if (_tooltipController.Animation == Tooltip.AnimationType.FromTargetView)
                {
                    animators.Add(ObjectAnimator.OfFloat(this, TranslationYCompat, GetY(), _targetViewRelativeY + _targetView.Height / 2 - Height / 2));
                    animators.Add(ObjectAnimator.OfFloat(this, TranslationXCompat, GetX(), _targetViewRelativeX + _targetView.Width / 2 - _tooltipWidth / 2));
                    animators.Add(ObjectAnimator.OfFloat(this, AlphaCompat, 1, 0));
                }
                else if (_tooltipController.Animation == Tooltip.AnimationType.FromTop)
                {
                    animators.Add(ObjectAnimator.OfFloat(this, TranslationYCompat, GetY(), 0));
                    animators.Add(ObjectAnimator.OfFloat(this, AlphaCompat, 1, 0));
                }
                else if (_tooltipController.Animation == Tooltip.AnimationType.FromBottom)
                {
                    Rect windowDisplayFrame = new Rect();
                    _targetView.GetWindowVisibleDisplayFrame(windowDisplayFrame);
                    animators.Add(ObjectAnimator.OfFloat(this, TranslationYCompat, GetY(), windowDisplayFrame.Bottom));
                    animators.Add(ObjectAnimator.OfFloat(this, AlphaCompat, 1, 0));
                }
                else if (_tooltipController.Animation == Tooltip.AnimationType.Grow)
                {
                    animators.Add(ObjectAnimator.OfFloat(this, ScaleXCompat, 1, 0));
                    animators.Add(ObjectAnimator.OfFloat(this, ScaleYCompat, 1, 0));
                    animators.Add(ObjectAnimator.OfFloat(this, AlphaCompat, 1, 0));
                }
                else if (_tooltipController.Animation == Tooltip.AnimationType.FadeInOut)
                {
                    animators.Add(ObjectAnimator.OfFloat(this, AlphaCompat, 1, 0));
                }

                AnimatorSet animatorSet = new AnimatorSet();
                animatorSet.PlayTogether(animators);
                animatorSet.Start();
                animatorSet.AnimationEnd += (sender, e) =>
                {
                    RemoveFromParent();
                };
            }
        }

        private void RemoveFromParent()
        {
            if (Parent != null)
            {
                ((IViewManager)Parent).RemoveView(this);
            }
        }

        public void OnClick(View view)
        {
            RemoveTooltip();
            OnTooltipClicked?.Invoke(this, null);
        }
    }
}
