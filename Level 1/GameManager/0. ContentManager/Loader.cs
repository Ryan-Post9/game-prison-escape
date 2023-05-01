using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using System;

namespace ECS_Framework
{
    /// <summary>
    /// Handles loading and retrieval of game assets, including textures and tile maps.
    /// </summary>
    public class Loader
    {
        // Textures
        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        // TiledMap
        public static TileHandler tiledHandler { get; private set; }
        public static Dictionary<string, List<Rectangle>> obstacles { get; private set; }
        private static Dictionary<string, List<LevelID>> terrainToLevel = new Dictionary<string, List<LevelID>>();

        //Debug box
        public static Texture2D collisionBox;

        /// <summary>
        /// Loads game assets into memory.
        /// </summary>
        /// <param name="content">The ContentManager to load assets with.</param>
        public static void LoadContent(ContentManager content)
        {
            // Player
            textures.Add("player_idle", content.Load<Texture2D>("Player/Virtual Guy/Idle"));
            textures.Add("player_walking", content.Load<Texture2D>("Player/Virtual Guy/Walking"));
            textures.Add("player_jump", content.Load<Texture2D>("Player/Virtual Guy/Jump"));
            textures.Add("player_double_jump", content.Load<Texture2D>("Player/Virtual Guy/Double Jump"));
            textures.Add("player_fall", content.Load<Texture2D>("Player/Virtual Guy/Fall"));
            textures.Add("player_slide", content.Load<Texture2D>("Player/Virtual Guy/Wall Jump"));
            textures.Add("player_death", content.Load<Texture2D>("Player/Virtual Guy/Hit"));

            // Enemy
            textures.Add("enemy_idle", content.Load<Texture2D>("Player/Mask Dude/Idle"));
            textures.Add("enemy_walking", content.Load<Texture2D>("Player/Mask Dude/Walking"));
            textures.Add("enemy_jump", content.Load<Texture2D>("Player/Mask Dude/Jump"));
            textures.Add("enemy_double_jump", content.Load<Texture2D>("Player/Mask Dude/Double Jump"));
            textures.Add("enemy_fall", content.Load<Texture2D>("Player/Mask Dude/Fall"));
            textures.Add("enemy_slide", content.Load<Texture2D>("Player/Mask Dude/Wall Jump"));
            textures.Add("enemy_death", content.Load<Texture2D>("Player/Mask Dude/Hit"));

            //Traps
            textures.Add("trap", content.Load<Texture2D>("Traps/Idle"));

            //Collictible Items
            textures.Add("apple_idle", content.Load<Texture2D>("Items/Fruits/Apple"));

            //Collectible Collected
            textures.Add("fruits_death", content.Load<Texture2D>("Items/Fruits/Collected"));

            // Background
            textures.Add("bg_green", content.Load<Texture2D>("Background/BG_Green"));
            textures.Add("bg_yellow", content.Load<Texture2D>("Background/BG_Yellow"));
            textures.Add("WireMesh", content.Load<Texture2D>("WireMesh"));
            textures.Add("PrisonEscape", content.Load<Texture2D>("PrisonEscape"));
            textures.Add("PrisonHome", content.Load<Texture2D>("PrisonHome"));
            textures.Add("Terrain2", content.Load<Texture2D>("Terrain (16x16)"));
            textures.Add("Terrain", content.Load<Texture2D>("TiledMap/Prison_B"));
            //textures.Add("Terrain (16x16)", content.Load<Texture2D>("TiledMap/Terrain (16x16)"));
            // Add more terrain types here

            // Map Terrains to their Level
            AddTerrain("PrisonHome", LevelID.HomeScreen);
            AddTerrain("Terrain", LevelID.Level1);
            AddTerrain("Terrain", LevelID.Level2);
            AddTerrain("Terrain", LevelID.Level3);
            AddTerrain("Terrain2", LevelID.Level5);
            //AddTerrain("Terrain (16x16)", LevelID.Level4);
            //Map more Levels to terrains here

            //Load TiledMaps
            tiledHandler = new TileHandler(content);
            foreach (LevelID level in LevelID.GetValues(typeof(LevelID)))
            {
                string levelName = level.ToString();
                tiledHandler.Load(
                    Path.Combine(content.RootDirectory, "TiledMap", $"{levelName}.tmx"),
                    Path.Combine(content.RootDirectory, "TiledMap", " "),
                    levelName,
                    GetTerrain(level)
                );

                // Save collision boxes for each level
                tiledHandler.GetLayersBoundsInMap();
            }

            //Box to debug Collisions
            GraphicsDevice graphicsDevice = ((IGraphicsDeviceService)content.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            collisionBox = new Texture2D(graphicsDevice, 1, 1);
            collisionBox.SetData(new[] { Color.White });
        }

        /// <summary>
        /// Retrieves a loaded texture by name.
        /// </summary>
        /// <param name="textureName">The name of the texture to retrieve.</param>
        /// <returns>The loaded texture, or null if the texture was not found.</returns>
        public static Texture2D GetTexture(string textureName)
        {
            if (textures.ContainsKey(textureName))
            {
                return textures[textureName];
            }

            return null;
        }

        /// <summary>
        /// Associates a terrain type with a level.
        /// </summary>
        /// <param name="terrain">The name of the terrain type.</param>
        /// <param name="levelID">The ID of the level.</param>
        private static void AddTerrain(string terrain, LevelID levelID)
        {
            if (!terrainToLevel.ContainsKey(terrain))
            {
                terrainToLevel[terrain] = new List<LevelID>();
            }
            terrainToLevel[terrain].Add(levelID);
        }

        /// <summary>
        /// Retrieves the name of the terrain associated with a given level.
        /// </summary>
        /// <param name="levelID">The ID of the level.</param>
        /// <returns>The name of the associated terrain, or null if no terrain is associated with the level.</returns>
        private static string GetTerrain(LevelID levelID)
        {
            foreach (var key in terrainToLevel.Keys)
            {
                foreach (var value in terrainToLevel[key])
                {
                    if (value == levelID)
                    {
                        return key;
                    }
                }
            }
            Console.WriteLine($"There is no Terrain associated with {levelID.ToString()}"); //Debug message
            return null;
        }
    }
}