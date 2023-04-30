using System.Collections.Generic;

namespace ECS_Framework
{
    /// <summary>
    /// Implements the <see cref="LevelInitializer"/> interface to provide initialization logic for Level 2.
    /// </summary>
    public class Level2Initializer : LevelInitializer
    {
        /// <summary>
        /// Returns a list of entities to be created and added to the level.
        /// </summary>
        /// <returns>A list of entities.</returns>
        public List<Entity> GetObjects()
        {
            List<Entity> objects = new List<Entity>();
            objects.Add(EntityFactory.CreateParallaxBackground("WireMesh", new Vector2(0,0)));
            objects.Add(EntityFactory.CreateTrap(new Vector2(150, 150)));
            objects.Add(EntityFactory.CreateTrap(new Vector2(50, 150)));
            objects.Add(EntityFactory.CreateTrap(new Vector2(250, 150)));
            objects.Add(EntityFactory.CreateApple(new Vector2(550, 30)));
            objects.Add(EntityFactory.CreateCop(new Vector2(300,150), 320, 550));
            objects.Add(EntityFactory.CreatePlayer(new Vector2(50, 20)));
            
            return objects;
        }
    }
}