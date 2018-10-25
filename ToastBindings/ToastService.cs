// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using ToastBindings;
using UIKit;

namespace ToastBindings
{
    public static class ToastService
    {
        public static void HideToast(UIView view, UIView toast)
        {
            view.HideToast(toast);
        }

        public static void ShowToast(UIView parentView, UIView toastView, Action completion = null)
        {
            if (completion == null)
            {
                parentView.ShowToast(toastView);
            }
            else
            {
                parentView.ShowToast(
                    toastView,
                    CSToastManager.DefaultDuration,
                    CSToastManager.DefaultPosition,
                    x => { completion(); });
            }
        }
    }
}