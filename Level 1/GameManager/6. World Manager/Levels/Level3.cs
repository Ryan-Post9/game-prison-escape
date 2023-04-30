using System.Collections.Generic;

namespace ECS_Framework
{
    /// <summary>
    /// Implements the <see cref="LevelInitializer"/> interface to provide initialization logic for Level 2.
    /// </summary>
    public class Level3Initializer : LevelInitializer
    {
        /// <summary>
        /// Returns a list of entities to be created and added to the level.
        /// </summary>
        /// <returns>A list of entities.</returns>
        public List<Entity> GetObjects()
        {
            List<Entity> objects = new List<Entity>();
            objects.Add(EntityFactory.CreateParallaxBackground("WireMesh", new Vector2(0,0)));
            objects.Add(EntityFactory.CreateApple(new Vector2(550, 200)));
            objects.Add(EntityFactory.CreateTrap(new Vector2(600, 200)));
            objects.Add(EntityFactory.CreateTrap(new Vector2(400, 200)));
            objects.Add(EntityFactory.CreateTrap(new Vector2(500, 200)));
            objects.Add(EntityFactory.CreateCop(new Vector2(400,150), 320, 600));
            objects.Add(EntityFactory.CreatePlayer(new Vector2(20, 200)));
            return objects;
        }
    }
}