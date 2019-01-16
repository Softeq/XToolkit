// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Android.Graphics;
using Android.Views;

namespace Softeq.XToolkit.Tooltips.Droid
{
    public class Tooltip
    {
        public enum AnimationType
        {
            FromTargetView,
            FromTop,
            FromBottom,
            Grow,
            FadeInOut,
            None
        }

        /// <summary>
        /// <para>View Id to be set to this tooltip root view</para>
        /// -1 stands for default id
        /// </summary>
        public int Id { get; set; } = -1;

        /// <summary>
        /// Returns <c>true</c> if view Id is set for this tooltip
        /// </summary>
        public bool IdIsSpecified => Id != -1;

        /// <summary>
        /// Custom View to be used as tooltip content instead of simple text view
        /// </summary>
        public View CustomView { get; set; }

        /// <summary>
        /// Text to be used as content of this tooltip
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Typeface for content text view. Typeface is not applied for CustomView
        /// </summary>
        public Typeface Typeface { get; set; }

        private float _textSizePx = -1;
        /// <summary>
        /// <para>Text size in pixels for content text view. TextSizePx is not applied for CustomView</para>
        /// -1 stands for default value from xml layout. Zero and negative values are not accepted, default is used instead
        /// </summary>
        public float TextSizePx
        {
            get { return _textSizePx; }
            set { _textSizePx = value > 0 ? value : -1; }
        }

        /// <summary>
        /// Returns <c>true</c> if custom text size is set by TextSizePx
        /// </summary>
        public bool HasCustomTextSize => TextSizePx > 0;

        /// <summary>
        /// Text color for content text view. TextColor is not applied for CustomView
        /// </summary>
        public Color TextColor { get; set; }

        /// <summary>
        /// Background color of tooltip view. Default value is White
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Indicates whether to show arrow pointer. Default value is <c>true</c>
        /// </summary>
        public bool ShowPointer { get; set; } = true;

        /// <summary>
        /// <para>If set to <c>true</c> tooltip view will be centered with target view</para>
        /// Default value is <c>false</c>
        /// </summary>
        public bool CenterWithTargetView { get; set; }

        /// <summary>
        /// <para>Vertical offset of tooltip view in pixels</para>
        /// Tooltip view is moved down if value is positive and up if value is negative
        /// </summary>
        public int VerticalOffsetPx { get; set; }

        /// <summary>
        /// <para>Indicates whether to show tooltip below target view when there is enough space on both sides</para>
        /// Default value is <c>false</c> (would rather show tooltip above target view)
        /// </summary>
        public bool PreferToShowBelow { get; set; }

        /// <summary>
        /// <para>Indicates whether to show shadow for tooltip view</para>
        /// Default value is <c>false</c>
        /// </summary>
        public bool ShouldShowShadow { get; set; }

        private int _shadowSizePx = 10;
        /// <summary>
        /// <para>Size of shadow blur in pixels. Value is ignored if <c>ShouldShowShadow = false</c></para>
        /// Default value is 10px. Negative values are not accepted, 0 is used instead
        /// </summary>
        public int ShadowSizePx
        {
            get { return _shadowSizePx; }
            set { _shadowSizePx = value > 0 ? value : 0; }
        }

        private int _minScreenSpacingPx;
        /// <summary>
        /// <para>Minimum space in pixels from tooltip view to screen edges</para>
        /// Default value is 0. Negative values are not accepted, 0 is used instead
        /// </summary>
        public int MinScreenSpacingPx
        {
            get { return _minScreenSpacingPx; }
            set { _minScreenSpacingPx = value > 0 ? value : 0; }
        }

        private int _horizontalPaddingPx = -1;
        /// <summary>
        /// <para>Horizontal padding inside tooltip view. If not set, default value from xml is used</para>
        /// Negative values are treated as 0
        /// </summary>
        public int HorizontalPaddingPx
        {
            get { return _horizontalPaddingPx; }
            set { _horizontalPaddingPx = value > 0 ? value : 0; }
        }

        /// <summary>
        /// Returns <c>true</c> if custom horizontal padding is set by HorizontalPaddingPx
        /// </summary>
        public bool HasCustomHorizontalPadding => HorizontalPaddingPx != -1;

        private int _verticalPaddingPx = -1;
        /// <summary>
        /// <para>Vertical padding inside tooltip view. If not set, default value from xml is used</para>
        /// Negative values are treated as 0
        /// </summary>
        public int VerticalPaddingPx
        {
            get { return _verticalPaddingPx; }
            set { _verticalPaddingPx = value > 0 ? value : 0; }
        }

        /// <summary>
        /// Returns <c>true</c> if custom vertical padding is set by VerticalPaddingPx
        /// </summary>
        public bool HasCustomVerticalPadding => VerticalPaddingPx != -1;

        private int _maxTextWidthPx = -1;
        /// <summary>
        /// <para>Max width of content text view in pixels. MaxTextWidthPx is not applied for CustomView</para>
        /// -1 stands for default value from xml layout. Zero and negative values are not accepted, default is used instead
        /// </summary>
        public int MaxTextWidthPx
        {
            get { return _maxTextWidthPx; }
            set { _maxTextWidthPx = value > 0 ? value : -1; }
        }

        /// <summary>
        /// Returns <c>true</c> if custom max text width is set by MaxTextWidthPx
        /// </summary>
        public bool HasCustomMaxWidth => MaxTextWidthPx != -1;

        private int _cornerRadiusPx = -1;
        /// <summary>
        /// <para>Corner radius of tooltip view in pixels. If not set, default value from xml is used</para>
        /// Negative values are treated as 0
        /// </summary>
        public int CornerRadiusPx
        {
            get { return _cornerRadiusPx; }
            set { _cornerRadiusPx = value > 0 ? value : 0; }
        }

        /// <summary>
        /// Returns <c>true</c> if custom corner radius is set by CornerRadiusPx
        /// </summary>
        public bool HasCustomCornerRadius => CornerRadiusPx != -1;

        private int _pointerSpacingPx;
        /// <summary>
        /// <para>Space in pixels between tooltip arrow pointer and target view</para>
        /// Default value is 0. Negative values are not accepted, 0 is used instead
        /// </summary>
        public int PointerSpacingPx
        {
            get { return _pointerSpacingPx; }
            set { _pointerSpacingPx = value > 0 ? value : 0; }
        }

        /// <summary>
        /// <para>Animation of tooltip appearing and disappearing</para>
        /// Default value is <c>FadeInOut</c>
        /// </summary>
        public AnimationType Animation { get; set; } = AnimationType.FadeInOut;

        private TimeSpan _autoDismissTimeout = TimeSpan.Zero;
        /// <summary>
        /// <para>Timeout after which tooltip will automatically disappear</para>
        /// Default value is Zero - tooltip will never be dismissed automatically
        /// </summary>
        public TimeSpan AutoDismissTimeout
        {
            get { return _autoDismissTimeout; }
            set { _autoDismissTimeout = value > TimeSpan.Zero ? value : TimeSpan.Zero; }
        }
    }
}
