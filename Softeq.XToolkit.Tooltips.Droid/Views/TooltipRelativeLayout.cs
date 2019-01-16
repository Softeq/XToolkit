// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Softeq.XToolkit.Tooltips.Droid.Views
{
    public class TooltipRelativeLayout : RelativeLayout
    {
        /// <summary>
        /// <para>Specifies if several tooltips can be displayed simultaneously</para>
        /// Default value is <c>false</c>. True value can only be applied if <c>CloseTooltipOnTapAnywhere = false</c> 
        /// </summary>
        public bool AllowSeveralTooltips = false;

        /// <summary>
        /// <para>Specifies whether any touches dismiss all displayed tooltips</para>
        /// Default value is <c>true</c>
        /// </summary>
        public bool CloseTooltipOnTapAnywhere = true;

        public TooltipRelativeLayout(Context context) : base(context) { }

        public TooltipRelativeLayout(Context context, IAttributeSet attrs) : base(context, attrs) { }

        public TooltipRelativeLayout(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { }

        private TooltipView AddTooltipView(Tooltip tooltip, View targetView)
        {
            TooltipView tooltipView = new TooltipView(Context);
            tooltipView.SetTooltip(tooltip, targetView);
            AddView(tooltipView);
            return tooltipView;
        }

        /// <summary>
        /// Displays tooltip view for targetView with settings specified by tooltip parameter
        /// </summary>
        /// <returns>Tooltip view</returns>
        public TooltipView ShowTooltipForView(Tooltip tooltip, View targetView)
        {
            if (!AllowSeveralTooltips)
            {
                CloseAllTooltips();
            }

            return AddTooltipView(tooltip, targetView);
        }

        /// <summary>
        /// Displays tooltip view for target with resId with settings specified by tooltip parameter
        /// </summary>
        /// <returns>Tooltip view</returns>
        public TooltipView ShowTooltipForViewResId(Activity activity, Tooltip tooltip, int resId)
        {
            if (!AllowSeveralTooltips)
            {
                CloseAllTooltips();
            }

            View decorView = activity.Window.DecorView;
            View view = decorView.FindViewById(resId);
            if (view == null)
            {
                throw new Exception("View not found");
            }

            return AddTooltipView(tooltip, view);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (CloseTooltipOnTapAnywhere)
            {
                CloseAllTooltips();
            }
            return false;
        }

        /// <summary>
        /// Dismisses all displayed tooltips
        /// </summary>
        /// <param name="animated">If set to <c>true</c> tooltips disappear with animation</param>
        public void CloseAllTooltips(bool animated = true)
        {
            if (!animated)
            {
                RemoveAllViews();
            }
            else
            {
                var tooltips = new List<TooltipView>();
                var childCount = ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = GetChildAt(i);
                    if (child is TooltipView)
                    {
                        tooltips.Add(child as TooltipView);
                    }
                }
                foreach (var tooltip in tooltips)
                {
                    tooltip?.RemoveTooltip();
                }
            }
        }

        /// <summary>
        /// Shows new tooltip only if no other tooltips are displayed, otherwise dismisses all tooltips and returns <c>null</c>
        /// </summary>
        public TooltipView ShowIfNoTooltipsOrHideAll(Tooltip tooltip, View targetView)
        {
            if (ChildCount > 0)
            {
                CloseAllTooltips();
                return null;
            }
            else
            {
                return AddTooltipView(tooltip, targetView);
            }
        }
    }
}
