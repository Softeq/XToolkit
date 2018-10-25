// Developed by Softeq Development Corporation
// http://www.softeq.com

using Foundation;
using UIKit;
using ObjCRuntime;
using System;

namespace ToastBindings
{
    delegate void Completion (bool value);
    
    [BaseType(typeof(UIView))]
    [Category]
    interface Toast
    {
        [Export("makeToast:")]
        void MakeToast(NSString text);

        [Export("showToast:")]
        void ShowToast(UIView view);
        
        [Export("showToast:duration:position:completion:")]
        void ShowToast(UIView view, double duration, NSString position, CompletionDelegate completion);

        [Export("hideToast:")]
        void HideToast(UIView view);
    }

    [BaseType(typeof(NSObject))]
    interface CSToastManager
    {
        [Static]
        [Export("setQueueEnabled:")]
        void SetQueueEnabled(bool value);

        [Static]
        [Export("isQueueEnabled")]
        bool IsQueueEnabled { get; }
            
        [Static]
        [Export("setSharedStyle:")]
        void SetSharedStyle(CSToastStyle sharedStyle);
        
        [Static]
        [Export("setTapToDismissEnabled:")]
        void SetTapToDismissEnabled(bool tapToDismissEnabled);
        
        [Static]
        [Export ("defaultPosition")]
        NSString DefaultPosition { get; }
        
        [Static]
        [Export ("defaultDuration")]
        double DefaultDuration { get; }
    }
    
    [BaseType(typeof(NSObject))]
    [DisableDefaultCtor]
    interface CSToastStyle
    {
        [Export("initWithDefaultStyle")]
        IntPtr Constructor();
        
        [Export ("verticalPadding", ArgumentSemantic.Assign)]
        nfloat VerticalPadding { get; set; }
    }
    
    delegate void CompletionDelegate (bool didTap);
}
