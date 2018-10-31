using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace SWRevealBindings
{
	// @interface SWRevealViewController : UIViewController
	[BaseType (typeof(UIViewController))]
	interface SWRevealViewController
	{
		// -(id)initWithRearViewController:(UIViewController *)rearViewController frontViewController:(UIViewController *)frontViewController;
		[Export ("initWithRearViewController:frontViewController:")]
		IntPtr Constructor (UIViewController rearViewController, UIViewController frontViewController);

		// @property (nonatomic) UIViewController * rearViewController;
		[Export ("rearViewController", ArgumentSemantic.Assign)]
		UIViewController RearViewController { get; set; }

		// -(void)setRearViewController:(UIViewController *)rearViewController animated:(BOOL)animated;
		[Export ("setRearViewController:animated:")]
		void SetRearViewController (UIViewController rearViewController, bool animated);

		// @property (nonatomic) UIViewController * rightViewController;
		[Export ("rightViewController", ArgumentSemantic.Assign)]
		UIViewController RightViewController { get; set; }

		// -(void)setRightViewController:(UIViewController *)rightViewController animated:(BOOL)animated;
		[Export ("setRightViewController:animated:")]
		void SetRightViewController (UIViewController rightViewController, bool animated);

		// @property (nonatomic) UIViewController * frontViewController;
		[Export ("frontViewController", ArgumentSemantic.Assign)]
		UIViewController FrontViewController { get; set; }

		// -(void)setFrontViewController:(UIViewController *)frontViewController animated:(BOOL)animated;
		[Export ("setFrontViewController:animated:")]
		void SetFrontViewController (UIViewController frontViewController, bool animated);

		// -(void)pushFrontViewController:(UIViewController *)frontViewController animated:(BOOL)animated;
		[Export ("pushFrontViewController:animated:")]
		void PushFrontViewController (UIViewController frontViewController, bool animated);

		// @property (nonatomic) FrontViewPosition frontViewPosition;
		[Export ("frontViewPosition", ArgumentSemantic.Assign)]
		FrontViewPosition FrontViewPosition { get; set; }

		// -(void)setFrontViewPosition:(FrontViewPosition)frontViewPosition animated:(BOOL)animated;
		[Export ("setFrontViewPosition:animated:")]
		void SetFrontViewPosition (FrontViewPosition frontViewPosition, bool animated);

		// -(void)revealToggle:(id)sender __attribute__((ibaction));
		[Export ("revealToggle:")]
		void RevealToggle (NSObject sender);

		// -(void)rightRevealToggle:(id)sender __attribute__((ibaction));
		[Export ("rightRevealToggle:")]
		void RightRevealToggle (NSObject sender);

		// -(void)revealToggleAnimated:(BOOL)animated;
		[Export ("revealToggleAnimated:")]
		void RevealToggleAnimated (bool animated);

		// -(void)rightRevealToggleAnimated:(BOOL)animated;
		[Export ("rightRevealToggleAnimated:")]
		void RightRevealToggleAnimated (bool animated);

        // -(UIPanGestureRecognizer *)panGestureRecognizer;
        [Export("panGestureRecognizer")]
        //[Verify (MethodToProperty)]
        UIPanGestureRecognizer PanGestureRecognizer();

        // -(UITapGestureRecognizer *)tapGestureRecognizer;
        [Export("tapGestureRecognizer")]
        //[Verify (MethodToProperty)]
        UITapGestureRecognizer TapGestureRecognizer();

		// @property (nonatomic) CGFloat rearViewRevealWidth;
		[Export ("rearViewRevealWidth")]
		nfloat RearViewRevealWidth { get; set; }

		// @property (nonatomic) CGFloat rightViewRevealWidth;
		[Export ("rightViewRevealWidth")]
		nfloat RightViewRevealWidth { get; set; }

		// @property (nonatomic) CGFloat rearViewRevealOverdraw;
		[Export ("rearViewRevealOverdraw")]
		nfloat RearViewRevealOverdraw { get; set; }

		// @property (nonatomic) CGFloat rightViewRevealOverdraw;
		[Export ("rightViewRevealOverdraw")]
		nfloat RightViewRevealOverdraw { get; set; }

		// @property (nonatomic) CGFloat rearViewRevealDisplacement;
		[Export ("rearViewRevealDisplacement")]
		nfloat RearViewRevealDisplacement { get; set; }

		// @property (nonatomic) CGFloat rightViewRevealDisplacement;
		[Export ("rightViewRevealDisplacement")]
		nfloat RightViewRevealDisplacement { get; set; }

		// @property (nonatomic) CGFloat draggableBorderWidth;
		[Export ("draggableBorderWidth")]
		nfloat DraggableBorderWidth { get; set; }

		// @property (nonatomic) BOOL bounceBackOnOverdraw;
		[Export ("bounceBackOnOverdraw")]
		bool BounceBackOnOverdraw { get; set; }

		// @property (nonatomic) BOOL bounceBackOnLeftOverdraw;
		[Export ("bounceBackOnLeftOverdraw")]
		bool BounceBackOnLeftOverdraw { get; set; }

		// @property (nonatomic) BOOL stableDragOnOverdraw;
		[Export ("stableDragOnOverdraw")]
		bool StableDragOnOverdraw { get; set; }

		// @property (nonatomic) BOOL stableDragOnLeftOverdraw;
		[Export ("stableDragOnLeftOverdraw")]
		bool StableDragOnLeftOverdraw { get; set; }

		// @property (nonatomic) BOOL presentFrontViewHierarchically;
		[Export ("presentFrontViewHierarchically")]
		bool PresentFrontViewHierarchically { get; set; }

		// @property (nonatomic) CGFloat quickFlickVelocity;
		[Export ("quickFlickVelocity")]
		nfloat QuickFlickVelocity { get; set; }

		// @property (nonatomic) NSTimeInterval toggleAnimationDuration;
		[Export ("toggleAnimationDuration")]
		double ToggleAnimationDuration { get; set; }

		// @property (nonatomic) SWRevealToggleAnimationType toggleAnimationType;
		[Export ("toggleAnimationType", ArgumentSemantic.Assign)]
		SWRevealToggleAnimationType ToggleAnimationType { get; set; }

		// @property (nonatomic) CGFloat springDampingRatio;
		[Export ("springDampingRatio")]
		nfloat SpringDampingRatio { get; set; }

		// @property (nonatomic) NSTimeInterval replaceViewAnimationDuration;
		[Export ("replaceViewAnimationDuration")]
		double ReplaceViewAnimationDuration { get; set; }

		// @property (nonatomic) CGFloat frontViewShadowRadius;
		[Export ("frontViewShadowRadius")]
		nfloat FrontViewShadowRadius { get; set; }

		// @property (nonatomic) CGSize frontViewShadowOffset;
		[Export ("frontViewShadowOffset", ArgumentSemantic.Assign)]
		CGSize FrontViewShadowOffset { get; set; }

		// @property (nonatomic) CGFloat frontViewShadowOpacity;
		[Export ("frontViewShadowOpacity")]
		nfloat FrontViewShadowOpacity { get; set; }

		// @property (nonatomic) UIColor * frontViewShadowColor;
		[Export ("frontViewShadowColor", ArgumentSemantic.Assign)]
		UIColor FrontViewShadowColor { get; set; }

		// @property (nonatomic) BOOL clipsViewsToBounds;
		[Export ("clipsViewsToBounds")]
		bool ClipsViewsToBounds { get; set; }

		// @property (nonatomic) BOOL extendsPointInsideHit;
		[Export ("extendsPointInsideHit")]
		bool ExtendsPointInsideHit { get; set; }

		[Wrap ("WeakDelegate")]
		SWRevealViewControllerDelegate Delegate { get; set; }

		// @property (nonatomic, weak) id<SWRevealViewControllerDelegate> delegate;
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	}

    // @protocol SWRevealViewControllerDelegate <NSObject>
    [BaseType(typeof(NSObject))]
    [Model]
	interface SWRevealViewControllerDelegate
	{
		// @optional -(void)revealController:(SWRevealViewController *)revealController willMoveToPosition:(FrontViewPosition)position;
		[Export ("revealController:willMoveToPosition:")]
		void willMoveToPosition (SWRevealViewController revealController, FrontViewPosition position);

		// @optional -(void)revealController:(SWRevealViewController *)revealController didMoveToPosition:(FrontViewPosition)position;
		[Export ("revealController:didMoveToPosition:")]
		void didMoveToPosition (SWRevealViewController revealController, FrontViewPosition position);

		// @optional -(void)revealController:(SWRevealViewController *)revealController animateToPosition:(FrontViewPosition)position;
		[Export ("revealController:animateToPosition:")]
		void animateToPosition (SWRevealViewController revealController, FrontViewPosition position);

		// @optional -(BOOL)revealControllerPanGestureShouldBegin:(SWRevealViewController *)revealController;
		[Export ("revealControllerPanGestureShouldBegin:")]
		bool RevealControllerPanGestureShouldBegin (SWRevealViewController revealController);

		// @optional -(BOOL)revealControllerTapGestureShouldBegin:(SWRevealViewController *)revealController;
		[Export ("revealControllerTapGestureShouldBegin:")]
		bool RevealControllerTapGestureShouldBegin (SWRevealViewController revealController);

		// @optional -(BOOL)revealController:(SWRevealViewController *)revealController panGestureRecognizerShouldRecognizeSimultaneouslyWithGestureRecognizer:(UIGestureRecognizer *)otherGestureRecognizer;
		[Export ("revealController:panGestureRecognizerShouldRecognizeSimultaneouslyWithGestureRecognizer:")]
		bool panGestureRecognizerShouldRecognizeSimultaneouslyWithGestureRecognizer (SWRevealViewController revealController, UIGestureRecognizer otherGestureRecognizer);

		// @optional -(BOOL)revealController:(SWRevealViewController *)revealController tapGestureRecognizerShouldRecognizeSimultaneouslyWithGestureRecognizer:(UIGestureRecognizer *)otherGestureRecognizer;
		[Export ("revealController:tapGestureRecognizerShouldRecognizeSimultaneouslyWithGestureRecognizer:")]
		bool tapGestureRecognizerShouldRecognizeSimultaneouslyWithGestureRecognizer (SWRevealViewController revealController, UIGestureRecognizer otherGestureRecognizer);

		// @optional -(void)revealControllerPanGestureBegan:(SWRevealViewController *)revealController;
		[Export ("revealControllerPanGestureBegan:")]
		void RevealControllerPanGestureBegan (SWRevealViewController revealController);

		// @optional -(void)revealControllerPanGestureEnded:(SWRevealViewController *)revealController;
		[Export ("revealControllerPanGestureEnded:")]
		void RevealControllerPanGestureEnded (SWRevealViewController revealController);

		// @optional -(void)revealController:(SWRevealViewController *)revealController panGestureBeganFromLocation:(CGFloat)location progress:(CGFloat)progress overProgress:(CGFloat)overProgress;
		[Export ("revealController:panGestureBeganFromLocation:progress:overProgress:")]
		void panGestureBeganFromLocation (SWRevealViewController revealController, nfloat location, nfloat progress, nfloat overProgress);

		// @optional -(void)revealController:(SWRevealViewController *)revealController panGestureMovedToLocation:(CGFloat)location progress:(CGFloat)progress overProgress:(CGFloat)overProgress;
		[Export ("revealController:panGestureMovedToLocation:progress:overProgress:")]
		void panGestureMovedToLocation (SWRevealViewController revealController, nfloat location, nfloat progress, nfloat overProgress);

		// @optional -(void)revealController:(SWRevealViewController *)revealController panGestureEndedToLocation:(CGFloat)location progress:(CGFloat)progress overProgress:(CGFloat)overProgress;
		[Export ("revealController:panGestureEndedToLocation:progress:overProgress:")]
		void panGestureEndedToLocation (SWRevealViewController revealController, nfloat location, nfloat progress, nfloat overProgress);

		// @optional -(void)revealController:(SWRevealViewController *)revealController willAddViewController:(UIViewController *)viewController forOperation:(SWRevealControllerOperation)operation animated:(BOOL)animated;
		[Export ("revealController:willAddViewController:forOperation:animated:")]
		void willAddViewController (SWRevealViewController revealController, UIViewController viewController, SWRevealControllerOperation operation, bool animated);

		// @optional -(void)revealController:(SWRevealViewController *)revealController didAddViewController:(UIViewController *)viewController forOperation:(SWRevealControllerOperation)operation animated:(BOOL)animated;
		[Export ("revealController:didAddViewController:forOperation:animated:")]
		void didAddViewController (SWRevealViewController revealController, UIViewController viewController, SWRevealControllerOperation operation, bool animated);

		// @optional -(id<UIViewControllerAnimatedTransitioning>)revealController:(SWRevealViewController *)revealController animationControllerForOperation:(SWRevealControllerOperation)operation fromViewController:(UIViewController *)fromVC toViewController:(UIViewController *)toVC;
		[Export ("revealController:animationControllerForOperation:fromViewController:toViewController:")]
		UIViewControllerAnimatedTransitioning animationControllerForOperation (SWRevealViewController revealController, SWRevealControllerOperation operation, UIViewController fromVC, UIViewController toVC);

		// @optional -(void)revealController:(SWRevealViewController *)revealController panGestureBeganFromLocation:(CGFloat)location progress:(CGFloat)progress;
		[Export ("revealController:panGestureBeganFromLocation:progress:")]
		void panGestureBeganFromLocation (SWRevealViewController revealController, nfloat location, nfloat progress);

		// @optional -(void)revealController:(SWRevealViewController *)revealController panGestureMovedToLocation:(CGFloat)location progress:(CGFloat)progress;
		[Export ("revealController:panGestureMovedToLocation:progress:")]
		void panGestureMovedToLocation (SWRevealViewController revealController, nfloat location, nfloat progress);

		// @optional -(void)revealController:(SWRevealViewController *)revealController panGestureEndedToLocation:(CGFloat)location progress:(CGFloat)progress;
		[Export ("revealController:panGestureEndedToLocation:progress:")]
		void panGestureEndedToLocation (SWRevealViewController revealController, nfloat location, nfloat progress);
	}

	[Static]
	//[Verify (ConstantsInterfaceAssociation)]
	partial interface Constants
	{
		// extern NSString *const SWSegueRearIdentifier;
		[Field ("SWSegueRearIdentifier", "__Internal")]
		NSString SWSegueRearIdentifier { get; }

		// extern NSString *const SWSegueFrontIdentifier;
		[Field ("SWSegueFrontIdentifier", "__Internal")]
		NSString SWSegueFrontIdentifier { get; }

		// extern NSString *const SWSegueRightIdentifier;
		[Field ("SWSegueRightIdentifier", "__Internal")]
		NSString SWSegueRightIdentifier { get; }
	}

	// @interface SWRevealViewControllerSegueSetController : UIStoryboardSegue
	[BaseType (typeof(UIStoryboardSegue))]
	interface SWRevealViewControllerSegueSetController
	{
	}

	// @interface SWRevealViewControllerSeguePushController : UIStoryboardSegue
	[BaseType (typeof(UIStoryboardSegue))]
	interface SWRevealViewControllerSeguePushController
	{
	}
}
