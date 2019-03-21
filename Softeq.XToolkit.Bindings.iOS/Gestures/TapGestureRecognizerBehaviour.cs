using UIKit;

namespace Softeq.XToolkit.Bindings.iOS.Gestures
{
    public class TapGestureRecognizerBehaviour
        : GestureRecognizerBehavior<UITapGestureRecognizer>
    {
        protected override void HandleGesture(UITapGestureRecognizer gesture)
        {
            FireCommand();
        }

        public TapGestureRecognizerBehaviour(UIView target, uint numberOfTapsRequired = 1,
            uint numberOfTouchesRequired = 1,
            bool cancelsTouchesInView = true)
        {
            var tap = new UITapGestureRecognizer(HandleGesture)
            {
                NumberOfTapsRequired = numberOfTapsRequired,
                NumberOfTouchesRequired = numberOfTouchesRequired,
                CancelsTouchesInView = cancelsTouchesInView
            };

            AddGestureRecognizer(target, tap);
        }
    }
}
