namespace ECS_Framework
{
    /// <summary>
    /// Holds common game constants
    /// </summary>
    public static class GameConstants
    {
        // Player-related constants
        public const int PLAYER_MAX_HP = 3;

        // Entity-related constant
        public const float GRAVITY = 2000f;
        public const int OTHER_HP = 1;
        public const float SpeedY = -500f;
        public const float SpeedX = 100f;

        // Other game constants
        public const int SCREEN_WIDTH = 640;
        public const int SCREEN_HEIGHT = 368;
        public const float AnimationFPS = 20f;
        public const float FPS = 60f;

        public const float MaxJumpTime = 0.35f;
        public const float JumpLaunchVelocity = -3500.0f;
        public const float GravityAcceleration = 3400.0f;
        public const float MaxFallSpeed = 550.0f;
        public const float JumpControlPower = 0.14f;
        public const float MoveAcceleration = 13000.0f;
        public const float MaxMoveSpeed = 1750.0f;
        public const float GroundDragFactor = 0.48f;
        public const float AirDragFactor = 0.58f;
        //Debug
        public const bool DisplayCollisionBoxes = false;
    }
}
