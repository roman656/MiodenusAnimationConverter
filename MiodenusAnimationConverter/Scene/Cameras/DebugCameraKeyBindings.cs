using OpenTK.Windowing.GraphicsLibraryFramework;

namespace MiodenusAnimationConverter.Scene.Cameras
{
    public static class DebugCameraKeyBindings
    {
        public const Keys FovChangeKey = Keys.LeftAlt;
        public const Keys SwitchCoordinateSystemForMovementKey = Keys.L;
        public const Keys ForwardMoveKey = Keys.W;
        public const Keys LeftMoveKey = Keys.A;
        public const Keys BackMoveKey = Keys.S;
        public const Keys RightMoveKey = Keys.D;
        public const Keys MoveUpKey = Keys.Space;
        public const Keys MoveDownKey = Keys.LeftShift;
        public const Keys LeftTiltKey = Keys.Q;
        public const Keys RightTiltKey = Keys.E;
        public static readonly Keys[] ResetKeys = { Keys.C, Keys.R };
    }
}