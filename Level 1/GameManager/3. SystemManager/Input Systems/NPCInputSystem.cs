using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ECS_Framework
{
    /// <summary>
    /// <see cref="System"/> that updates the state of entities based on keyboard input.
    /// </summary>
    public class NPCInputSystem : System
    {
        private List<Entity> entities;
        private List<StateComponent> states;
        private List<NPCInputComponent> inputs;

        private List<MovementComponent> movements;

        /// <summary>
        /// Initializes a new instance of the PlayerInputSystem class.
        /// </summary>
        public NPCInputSystem()
        {
            entities = new List<Entity>();
            states = new List<StateComponent>();
            inputs = new List<NPCInputComponent>();
            movements = new List<MovementComponent>();
        }

        /// <summary>
        /// Adds an entity to the system.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        public override void AddEntity(Entity entity)
        {
            StateComponent state = entity.GetComponent<StateComponent>();
            NPCInputComponent input = entity.GetComponent<NPCInputComponent>();
            MovementComponent movement = entity.GetComponent<MovementComponent>();
            if (state == null || input == null || movement == null)
            {
                return;
            }

            entities.Add(entity);
            states.Add(state);
            inputs.Add(input);
            movements.Add(movement);
        }

        /// <summary>
        /// Removes an entity from the system.
        /// </summary>
        /// <param name="entity">The entity to be removed.</param>
        public override void RemoveEntity(Entity entity)
        {
            int index = entities.IndexOf(entity);
            if (index != -1)
            {
                entities.RemoveAt(index);
                states.RemoveAt(index);
                inputs.RemoveAt(index);
                movements.RemoveAt(index);
            }
        }

        /// <summary>
        /// Updates the system's state based on input from the keyboard.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                inputs[i].Update(movements[i].Position.X);
                UpdateEntityState(gameTime, inputs[i], states[i]);
            }
        }

        /// <summary>
        /// Updates the state of an entity based on its input and current state.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="input">The input component for the entity.</param>
        /// <param name="state">The state component for the entity.</param>
        private void UpdateEntityState(GameTime gameTime, NPCInputComponent input, StateComponent state)
        {
            if (state.CurrentSuperState != SuperState.IsOnGround)
            {
                return;
            }
            switch (state.CurrentState)
            {
                case State.Idle:
                    state.CurrentState = State.WalkRight;
                    break;
                case State.WalkLeft:
                    if (input.IsRight)
                    {
                        state.CurrentState = State.WalkRight;
                    }
                    break;
                case State.WalkRight:
                    if (input.IsLeft)
                    {
                        state.CurrentState = State.WalkLeft;
                    }
                    break;
                default:
                    break;
            }
            
        }
    }
}