using Microsoft.Xna.Framework;

namespace ECS_Framework
{
    /// <summary>
    /// The possible interaptable States an object can be in.
    /// </summary>
    public enum ObjectState
    {
        Idle,
        WalkLeft,
        WalkRight,
        Jump,
        DoubleJump,
        Slide,
    }

    /// <summary>
    /// The possible continious SuperStates an object can be in.
    /// </summary>
    public enum SuperState
    {
        OnGround,
        IsFalling,
        IsJumping,
        IsDoubleJumping,
    }

    /// <summary>
    /// <see cref="Component"/> that stores the current state and super state of an object, as well as its state ID, jump counter, and horizontal direction.
    /// </summary>
    public class StateComponent : Component
    {
        //State
        public ObjectState currentState { get; private set; }
        public ObjectState previousState { get; private set; }

        //SuperState
        public SuperState currentSuperState { get; private set; }
        public SuperState previousSuperState { get; private set; }

        //ID
        public string stateID { get; private set; }

        //Jump Counter
        public int JumpsPerformed = 0;

        //Flags for movement restrictions
        public bool _canMoveLeft, _canMoveRight;

        //Horizontal direction
        private int _horizontalDirection = 1;

        /// <summary>
        /// Gets or sets a value indicating whether the entity can move to the left.
        /// </summary>
        public bool CanMoveLeft
        {
            get { return _canMoveLeft; }
            set
            {
                _canMoveLeft = value;
                if (!_canMoveLeft)
                {
                    SetState(ObjectState.Slide);
                    JumpsPerformed = 2;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the entity can move to the right.
        /// </summary>
        public bool CanMoveRight
        {
            get { return _canMoveRight; }
            set
            {
                _canMoveRight = value;
                if (!_canMoveRight)
                {
                    SetState(ObjectState.Slide);
                    JumpsPerformed = 2;
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal direction of the entity.
        /// </summary>
        public int HorizontalDirection { get => _horizontalDirection; set => _horizontalDirection = value; }

        /// <summary>
        /// Initializes a new instance of the StateComponent class with the default state and super state.
        /// </summary>
        public StateComponent()
        {
            SetState(ObjectState.Idle);
            SetSuperState(SuperState.IsFalling);
            UpdateStateID();
            _canMoveRight = true;
            _canMoveLeft = true;
        }

        /// <summary>
        /// Sets the current state to the specified state and updates its ID.
        /// </summary>
        /// <param name="newState">The new state to set.</param>
        public void SetState(ObjectState newState)
        {
            previousState = currentState;
            currentState = newState;
            UpdateStateID();
        }

        /// <summary>
        /// Checks if the current state is the specified state.
        /// </summary>
        /// <param name="state">The state to check against.</param>
        /// <returns>True if the current state is the specified state, false otherwise.</returns>
        public bool IsState(ObjectState state)
        {
            return currentState == state;
        }

        /// <summary>
        /// Sets the current super state to the specified super state.
        /// </summary>
        /// <param name="newSuperState">The new super state to set.</param>
        public void SetSuperState(SuperState newSuperState)
        {
            previousSuperState = currentSuperState;
            currentSuperState = newSuperState;
            UpdateStateID();
        }

        /// <summary>
        /// Checks if the current super state is the specified super state.
        /// </summary>
        /// <param name="superState">The super state to check against.</param>
        /// <returns>True if the current super state is the specified super state, false otherwise.</returns>
        public bool IsSuperState(SuperState superState)
        {
            return currentSuperState == superState;
        }

        /// <summary>
        /// Updates the state ID based on the current state and super state. 
        /// Used for identifying current animation
        /// </summary>
        public void UpdateStateID()
        {
            switch (currentSuperState)
            {
                case SuperState.OnGround:
                    stateID = "idle";
                    if (currentState == ObjectState.WalkLeft || currentState == ObjectState.WalkRight)
                    {
                        stateID = "walking";
                    }
                    break;

                case SuperState.IsFalling:
                    stateID = "fall";
                    if (currentState == ObjectState.Slide)
                    {
                        stateID = "slide";
                    }
                    break;

                case SuperState.IsJumping:
                    stateID = "jump";
                    break;

                case SuperState.IsDoubleJumping:
                    stateID = "double_jump";
                    break;
            }
        }
    }
}