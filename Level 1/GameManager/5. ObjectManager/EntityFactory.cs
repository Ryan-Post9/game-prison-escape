using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ECS_Framework
{
    /// <summary>
    /// Enum that holds entity types
    /// </summary>
    public enum EntityType
    {
        Player,
        Coin,
        WalkingEnemy,
        PortalToNextLevel,
        Trap,
        // Add more entity types as needed
    }

    /// <summary>
    /// A factory class for creating entities.
    /// </summary>
    static class EntityFactory
    {
        //Parallax Background
        /// <summary>
        /// Creates a parallax background entity.
        /// </summary>
        /// <param name="texture">The texture to use for the background.</param>
        /// <param name="velocity">The velocity of the background.</param>
        /// <returns>The parallax background entity.</returns>
        public static Entity CreateParallaxBackground(string texture, Vector2 velocity)
        {
            Entity background = new Entity();

            //Define the desired area of parallax
            int viewX = GameConstants.SCREEN_WIDTH;
            int viewY = GameConstants.SCREEN_HEIGHT;

            //Parallax
            background.AddComponent(new ParallaxComponent(texture, velocity, Vector2.Zero, viewX, viewY));
            return background;
        }

        //Player
        /// <summary>
        /// Creates a player entity.
        /// </summary>
        /// <remarks>
        /// Player must be first Level entity to initialize to be first in PlayerEntityCollisionSystem.
        /// </remarks>
        /// <param name="position">The initial position of the player.</param>
        /// <returns>The player entity.</returns>
        public static Entity CreatePlayer(Vector2 position, bool haveKey = false)
        {
            //Empty Player
            Entity player = new Entity();
            player.AddComponent(new EntityTypeComponent(EntityType.Player));
            player.AddComponent(new CoinComponent(haveKey));
            // Animations
            AnimatedComponent animation = new AnimatedComponent();
            animation.AddAnimation("player_idle", "idle", 1, 11, 20);
            animation.AddAnimation("player_walking", "walking", 1, 12, 20);
            animation.AddAnimation("player_jump", "jump", 1, 1, 20);
            animation.AddAnimation("player_double_jump", "double_jump", 1, 6, 20);
            animation.AddAnimation("player_fall", "fall", 1, 1, 20);
            animation.AddAnimation("player_slide", "slide", 1, 5, 20);
            animation.AddAnimation("player_death", "death", 1, 7, 20);
            player.AddComponent(animation);

            // States
            player.AddComponent(new StateComponent());
            player.AddComponent(new PlayerInputComponent());

            // Position and transforms
            player.AddComponent(new MovementComponent(position));

            // Collisions
            player.AddComponent(new CollisionBoxComponent(
                    position: position,
                    width: 32,
                    height: 32,
                    vertTopOffset: 8,
                    vertBottomOffset: 0,
                    horLeftOffset: 4,
                    horRightOffset: 6));

            return player;
        }
        public static Entity CreateCop(Vector2 position, float left, float right)
        {
            //Empty Player
            Entity cop = new Entity();
            cop.AddComponent(new EntityTypeComponent(EntityType.WalkingEnemy));
            // Animations
            AnimatedComponent animation = new AnimatedComponent();
            animation.AddAnimation("enemy_idle", "idle", 1, 11, 20);
            animation.AddAnimation("enemy_walking", "walking", 1, 12, 20);
            animation.AddAnimation("enemy_jump", "jump", 1, 1, 20);
            animation.AddAnimation("enemy_double_jump", "double_jump", 1, 6, 20);
            animation.AddAnimation("enemy_fall", "fall", 1, 1, 20);
            animation.AddAnimation("enemy_slide", "slide", 1, 5, 20);
            animation.AddAnimation("enemy_death", "death", 1, 7, 20);
            cop.AddComponent(animation);

            // States
            cop.AddComponent(new StateComponent());
            cop.AddComponent(new NPCInputComponent(left,right));

            // Position and transforms
            cop.AddComponent(new MovementComponent(position));

            // Collisions
            cop.AddComponent(new CollisionBoxComponent(
                    position: position,
                    width: 32,
                    height: 32,
                    vertTopOffset: 8,
                    vertBottomOffset: 0,
                    horLeftOffset: 4,
                    horRightOffset: 6));

            return cop;
        }
        public static Entity CreateApple(Vector2 position)
        {
            //Empty coin
            Entity coin = new Entity();
            coin.AddComponent(new EntityTypeComponent(EntityType.Coin));

            //Animations
            AnimatedComponent animation = new AnimatedComponent();
            animation.AddAnimation("apple_idle", "idle", 1, 4, 20);
            animation.AddAnimation("fruits_death", "death", 1, 6, 20);
            coin.AddComponent(animation);

            // States
            coin.AddComponent(new StateComponent(State.Idle, SuperState.IsOnGround));

            // Position and transforms
            coin.AddComponent(new MovementComponent(position));

            // Collisions
            coin.AddComponent(new CollisionBoxComponent(
                    position: position,
                    width: 32,
                    height: 32,
                    vertTopOffset: 7,
                    vertBottomOffset: 11,
                    horLeftOffset: 10,
                    horRightOffset: 10));

            return coin;
        }

        public static Entity CreatePortal(Vector2 position)
        {
            //Empty coin
            Entity coin = new Entity();
            coin.AddComponent(new EntityTypeComponent(EntityType.PortalToNextLevel));

            //Animations
            AnimatedComponent animation = new AnimatedComponent();
            animation.AddAnimation("apple_idle", "idle", 1, 17, 20);
            animation.AddAnimation("fruits_death", "death", 1, 6, 20);
            coin.AddComponent(animation);

            // States
            coin.AddComponent(new StateComponent(State.Idle, SuperState.IsOnGround));

            // Position and transforms
            coin.AddComponent(new MovementComponent(position));

            // Collisions
            coin.AddComponent(new CollisionBoxComponent(
                    position: position,
                    width: 32,
                    height: 32,
                    vertTopOffset: 7,
                    vertBottomOffset: 11,
                    horLeftOffset: 10,
                    horRightOffset: 10));

            return coin;
        }
        public static Entity CreateTrap(Vector2 position)
        {
            //Empty coin
            Entity trap = new Entity();
            trap.AddComponent(new EntityTypeComponent(EntityType.Trap));

            //Animations
            AnimatedComponent animation = new AnimatedComponent();
            animation.AddAnimation("trap", "idle", 1, 1, 20);
            animation.AddAnimation("trap", "fall", 1, 1, 20);
            trap.AddComponent(animation);


            // States
            trap.AddComponent(new StateComponent(State.Idle, SuperState.IsFalling));

            // Position and transforms
            trap.AddComponent(new MovementComponent(position));

            // Collisions
            trap.AddComponent(new CollisionBoxComponent(
                    position: position,
                    width: 16,
                    height: 16,
                    vertTopOffset: 8,
                    vertBottomOffset: 0,
                    horLeftOffset: 0,
                    horRightOffset: 0));

            return trap;
        }
        /*
        public static Entity CreateSpotlight(Vector2 position)
        {
            //Empty coin
            Entity spotlight = new Entity();
            spotlight.AddComponent(new EntityTypeComponent(EntityType.Trap));

            //Animations
            AnimatedComponent animation = new AnimatedComponent();
            animation.AddAnimation("trap", "idle", 1, 1, 20);
            animation.AddAnimation("trap", "fall", 1, 1, 20);
            spotlight.AddComponent(animation);


            // States
            spotlight.AddComponent(new StateComponent(State.Idle, SuperState.IsFalling));

            // Position and transforms
            spotlight.AddComponent(new MovementComponent(position));

            // Collisions
            spotlight.AddComponent(new CollisionBoxComponent(
                    position: position,
                    width: 16,
                    height: 16,
                    vertTopOffset: 8,
                    vertBottomOffset: 0,
                    horLeftOffset: 0,
                    horRightOffset: 0));

            return spotlight;
        }
        */
    }
}
