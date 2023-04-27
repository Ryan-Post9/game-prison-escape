using System.Collections.Generic;

namespace ECS_Framework
{
    /// <summary>
    /// Implements the <see cref="LevelInitializer"/> interface to provide initialization logic for Level 1.
    /// </summary>
    public class Level4Initializer : LevelInitializer
    {
        /// <summary>
        /// Returns a list of entities to be created and added to the level.
        /// </summary>
        /// <returns>A list of entities.</returns>
        public List<Entity> GetObjects()
        {
            List<Entity> objects = new List<Entity>();
            objects.Add(EntityFactory.CreateParallaxBackground("WireMesh", new Vector2(0, 0)));
            objects.Add(EntityFactory.CreateApple(new Vector2(10, 30)));
            objects.Add(EntityFactory.CreateCop(new Vector2(250, 250), 200, 300));
            //objects.Add(EntityFactory.CreatePortal(new Vector2(524, 80)));
            objects.Add(EntityFactory.CreatePlayer(new Vector2(100, 100)));
            return objects;
        }
    }
}