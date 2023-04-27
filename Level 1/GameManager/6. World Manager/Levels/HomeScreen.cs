using System.Collections.Generic;

namespace ECS_Framework
{
    /// <summary>
    /// Implements the <see cref="LevelInitializer"/> interface to provide initialization logic for Level 1.
    /// </summary>
    public class HomeScreenInitializer : LevelInitializer
    {
        /// <summary>
        /// Returns a list of entities to be created and added to the level.
        /// </summary>
        /// <returns>A list of entities.</returns>
        public List<Entity> GetObjects()
        {
            List<Entity> objects = new List<Entity>();
            objects.Add(EntityFactory.CreateParallaxBackground("PrisonHome", new Vector2(0, 0)));
            return objects;
        }
    }
}