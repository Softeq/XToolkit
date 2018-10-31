using System;
using ObjCRuntime;

namespace SWRevealBindings
{
	[Native]
    public enum FrontViewPosition : long
	{
		LeftSideMostRemoved,
		LeftSideMost,
		LeftSide,
		Left,
		Right,
		RightMost,
		RightMostRemoved
	}

	[Native]
    public enum SWRevealToggleAnimationType : long
	{
		Spring,
		EaseOut
	}

	public enum SWRevealControllerOperation : long
	{
		None,
		ReplaceRearController,
		ReplaceFrontController,
		ReplaceRightController
	}
}
