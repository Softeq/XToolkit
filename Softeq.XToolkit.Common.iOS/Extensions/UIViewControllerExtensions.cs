// Developed by Softeq Development Corporation
// http://www.softeq.com

using CoreGraphics;
using UIKit;

namespace Softeq.XToolkit.Common.iOS.Extensions
{
    public static class UIViewControllerExtensions
    {
        public static UIViewController TopViewController
        {
            get
            {
                for (var i = 0; i < UIApplication.SharedApplication.Windows.Length; i++)
                {
                    var rootViewController = UIApplication.SharedApplication.Windows[i].RootViewController;
                    if (rootViewController != null)
                    {
                        return GetTopViewController(rootViewController);
                    }
                }

                return null;
            }
        }

        public static void AddAsChild(this UIViewController child, UIViewController parent, UIView targetView = null)
        {
            parent.AddChildViewController(child);
            if (targetView == null)
            {
                parent.View.AddSubview(child.View);
            }
            else
            {
                targetView.AddSubview(child.View);
            }

            child.DidMoveToParentViewController(parent);
        }

        public static void SetBackButtonTitle(this UIViewController controller, string title)
        {
            controller.NavigationItem.BackBarButtonItem =
                new UIBarButtonItem(title, UIBarButtonItemStyle.Plain, null);
        }

        public static void SetRightBarButtonItem(this UIViewController controller, UIButton button)
        {
            button.SizeToFit();
            var buttonHeight = controller.NavigationController.NavigationBar.Frame.Height;
            var view = new UIView
            {
                Frame = new CGRect(0, 0, button.Bounds.Width, buttonHeight)
            };

            view.AddSubview(button);
            button.Frame = new CGRect(2, 0, button.Bounds.Width + 12, view.Frame.Height);

            controller.NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(view), false);
        }

        private static UIViewController GetTopViewController(UIViewController viewController)
        {
            if (viewController.PresentedViewController != null)
            {
                var presentedViewController = viewController.PresentedViewController;
                return GetTopViewController(presentedViewController);
            }

            if (viewController is UINavigationController navigationController)
            {
                return GetTopViewController(navigationController.VisibleViewController);
            }

            if (viewController is UITabBarController tabBarController)
            {
                return GetTopViewController(tabBarController.SelectedViewController);
            }

            return viewController;
        }
    }
}