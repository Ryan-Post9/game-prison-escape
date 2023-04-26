using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ECS_Framework
{
    /// <summary>
    /// Component representing the input state of the NPC entity.
    /// </summary>
    public class NPCInputComponent : Component
    {
        /// <summary>
        /// Indicates whether the left key is currently pressed.
        /// </summary>
        public bool IsLeft { get; private set; }
        
        /// <summary>
        /// Indicates whether the right key is currently pressed.
        /// </summary>
        public bool IsRight { get; private set; }
        
        public float left;
        public float right;

        public NPCInputComponent(float Left, float Right)
        {
            left = Left;
            right = Right;
        }

        public void Update(float position)
        {
            if (position <= left)
            {
                IsRight = true;
                IsLeft = false;
            }
            if (position >= right)
            {
                IsRight = false;
                IsLeft = true;
            }
        }

    }
}