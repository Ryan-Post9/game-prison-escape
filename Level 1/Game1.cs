
//https://www.industrian.net/tutorials/monogame-using-the-content-pipeline/
//https://www.industrian.net/tutorials/using-sprite-sheets/
//https://www.industrian.net/tutorials/texture2d-and-drawing-sprites/
//http://rbwhitaker.wikidot.com/monogame-rotating-sprites

using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Diagnostics;

using System.Linq; //this will come in later
using TiledCS;
using System.Reflection.Emit;

namespace Level_1;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    Texture2D box;
    Texture2D platBrown;
    Texture2D platGrey;

    Texture2D characterIdle;
    Texture2D characterRun;
    Texture2D characterLand;
    Texture2D characterJump;
    Texture2D characterRoll;
    Texture2D characterDodge;
    float timer;
    int threshold;

   
    Rectangle[] idleSourceRectangles;
    Rectangle[] runSourceRectangles;
    Rectangle[] jumpSourceRectangles;
    Rectangle[] dodgeSourceRectangles;
    byte[] animationFrames;

    float x = 60;
    float y = 400;

    bool isLanded = true;
    Vector2[] objects = { new Vector2(200, 90), new Vector2(500, 90), new Vector2(500, 65)};
    Vector2 boxVelocity = new Vector2(0, 0);
    Vector2 boxLoc = new Vector2(100,100);
    public Vector2 Position
    {
        get { return position; }
        set { position = value; }
    }
    Vector2 position;

   
    public Vector2 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }
    Vector2 velocity;

    bool isJumping = false;
    private bool wasJumping;
    private float jumpTime;
    private const float MaxJumpTime = 0.35f;
    private const float JumpLaunchVelocity = -3500.0f;
    private const float GravityAcceleration = 3400.0f;
    private const float MaxFallSpeed = 550.0f;
    private const float JumpControlPower = 0.14f;

    private const float MoveAcceleration = 13000.0f;
    private const float MaxMoveSpeed = 1750.0f;
    private const float GroundDragFactor = 0.48f;
    private const float AirDragFactor = 0.58f;

    bool isDodging = false;
    private bool wasDodging;
    private float dodgeTime;
    //float movement = 0;
    int animationType = 0;
    byte previousAnimationIndex;
    byte currentAnimationIndex;
    
    private float angle = 0;
    Vector2 origin = new Vector2(0, 0);

    // initialize the tiled map
    private TiledMap map;
    private Dictionary<int, TiledTileset> tilesets;
    private Texture2D tilesetTexture;

    // used to flip/rotate sprites as necessary later
    [Flags]
    enum Trans
    {
        None = 0,
        Flip_H = 1 << 0,
        Flip_V = 1 << 1,
        Flip_D = 1 << 2,

        Rotate_90 = Flip_D | Flip_H,
        Rotate_180 = Flip_H | Flip_V,
        Rotate_270 = Flip_V | Flip_D,

        Rotate_90AndFlip_H = Flip_H | Flip_V | Flip_D,
    }

    // create a layer to check for ground
    private TiledLayer groundLayer;
    private TiledLayer brownPlatLayer;
    private TiledLayer boxLayer;
    bool hitGround;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic herez

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        //Add objects
        

        map = new TiledMap(Content.RootDirectory + "\\level.tmx");
        tilesets = map.GetTiledTilesets(Content.RootDirectory + "/");
        tilesetTexture = Content.Load<Texture2D>("Terrain (16x16)");
        groundLayer = map.Layers.First(l => l.name == "Ground");
        var boxLayer = map.Layers.First(l => l.name == "Box");
        int tileX = 0;
        int tileY = 0;
        for (var j = 0; j < boxLayer.height; j++)
        {
            for (var i = 0; i < boxLayer.width; i++)
            {
                // Assuming the default render order is used which is from right to bottom
                var index = (j * boxLayer.width) + i;
                var gid = boxLayer.data[index]; // The tileset tile index
                tileX = i * map.TileWidth;
                tileY = j * map.TileHeight;
                if(gid != 0)
                {
                    break;
                }
            }
        }
        boxLoc = new Vector2(tileX-40, 70);
        Debug.WriteLine(boxLoc.Y);
        brownPlatLayer = map.Layers.First(l => l.name == "Brown Plat");

        // TODO: use this.Content to load your game content here
        characterIdle = Content.Load<Texture2D>("Character Idle 48x48");
        characterRun = Content.Load<Texture2D>("run cycle 48x48");
        characterJump = Content.Load<Texture2D>("player jump 48x48");
        characterDodge = Content.Load<Texture2D>("Player Roll 48x48");

       

        box = Content.Load<Texture2D>("Idle");
        platBrown = Content.Load<Texture2D>("Brown On (32x8)");
        platGrey = Content.Load<Texture2D>("Grey On (32x8)");

        timer = 0;
        // Set an initial threshold of 250ms, you can change this to alter the speed of the animation (lower number = faster animation).
        threshold = 250;
        // Three sourceRectangles contain the coordinates of Alex's three down-facing sprites on the charaset.
        idleSourceRectangles = new Rectangle[10];
        for(int i = 0; i < 10; i++)
        {
            idleSourceRectangles[i] = new Rectangle(0 + i*48, 0, 48, 64);
        }

        runSourceRectangles = new Rectangle[8];
        for (int i = 0; i < 8; i++)
        {
            runSourceRectangles[i] = new Rectangle(0 + i * 48, 0, 48, 64);
        }

        jumpSourceRectangles = new Rectangle[3];
        for (int i = 0; i < 3; i++)
        {
            jumpSourceRectangles[i] = new Rectangle(0 + i * 48, 0, 48, 64);
        }

        dodgeSourceRectangles = new Rectangle[7];
        for (int i = 0; i < 7; i++)
        {
            dodgeSourceRectangles[i] = new Rectangle(0 + i * 48, 0, 48, 64);
        }

        animationFrames = new byte[] {10, 8, 3, 8, 10, 7, 7, 3 };
        previousAnimationIndex = 2;
        currentAnimationIndex = 1;
    }

    public void ApplyPhysics(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Base velocity is a combination of horizontal movement control and
        // acceleration downward due to gravity.
        velocity.X += x * MoveAcceleration * elapsed;
        velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

        //boxVelocity.X += objects[2].X * MoveAcceleration * elapsed;
        boxVelocity.Y = MathHelper.Clamp(boxVelocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
       
       
        velocity.Y = DoJump(velocity.Y, gameTime);
        velocity.X = DoDodge(velocity.X, gameTime);
        
        // Apply pseudo-drag horizontally.
        if (y > 100)
            velocity.X *= GroundDragFactor;
        else
            velocity.X *= AirDragFactor;

        // Prevent the player from running faster than his top speed.            
        velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);
        //velocity = DetectCollision(velocity, gameTime);

        //if (objects[2].Y > 100)
        //   boxVelocity.X *= GroundDragFactor;
        //else
        if (objects[2].Y <= 100)
            boxVelocity.Y *= AirDragFactor;
        boxVelocity.X = MathHelper.Clamp(boxVelocity.X, -MaxMoveSpeed, MaxMoveSpeed);

        // Apply velocity.
        if (hitGround && !isJumping)
        {

        }
        else
        {
            y += velocity.Y * elapsed;
        }
        
        //x += velocity.X * elapsed;
    }
    private float DoJump(float velocityY, GameTime gameTime)
    {
        // If the player wants to jump
        if (isJumping)
        {
            // Begin or continue a jump
            if ((!wasJumping && isLanded) || jumpTime > 0.0f)
            {
                isLanded = false;
                jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // If we are in the ascent of the jump
            if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
            {
                // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
            }
            else
            {
                // Reached the apex of the jump
                jumpTime = 0.0f;
            }
        }
        else
        {
            // Continues not jumping or cancels a jump in progress
            jumpTime = 0.0f;
        }
        wasJumping = isJumping;

        return velocityY;
    }

        private float DoDodge(float velocityX, GameTime gameTime)
    {
        // If the player wants to jump
        if (isDodging)
        {
            // Begin or continue a jump
            if ((!wasDodging) || dodgeTime > 0.0f)
            {
                dodgeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // If we are in the ascent of the jump
            if (0.0f < dodgeTime && dodgeTime <= MaxJumpTime)
            {
                // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                velocityX = 100 * (1.0f - (float)Math.Pow(dodgeTime / MaxJumpTime, JumpControlPower));
            }
            else
            {
                // Reached the apex of the jump
                jumpTime = 0.0f;
            }
        }
        else
        {
            // Continues not jumping or cancels a jump in progress
            jumpTime = 0.0f;
        }
        wasJumping = isJumping;

        return velocityX;
    }

    protected override void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        //Exit if you hit end
        if (x > 760 && (y < 161 && y > 80))
        {
            Exit();
        }
        //Don't go off screen
        if(x < 0)
        {
            x = 10;
        }
        else if (y < 0)
        {
            y = 10;
        }
        else if(y > 800)
        {
            x = 60;
            y = 400;
        }
        else if (x > 765)
        {
            x = 750;
        }
        //Box Collisions
        Rectangle player = new Rectangle((int)x, (int)y - 24, idleSourceRectangles[currentAnimationIndex].Width, idleSourceRectangles[currentAnimationIndex].Height);
        var boxRect = new Rectangle((int)boxLoc.X, (int)boxLoc.Y, 28, 24);
        if (player.Intersects(boxRect))
        {
            if ((animationType == 5 || animationType == 6) && !Keyboard.GetState().IsKeyDown(Keys.D) && !Keyboard.GetState().IsKeyDown(Keys.A))
            {
                boxVelocity.Y = -700;
            }
            else if (animationType == 5)
            {
                boxVelocity.X = 700;
                boxVelocity.Y = velocity.Y;
            }
            else if (animationType == 6)
            {
                boxVelocity.X = -700;
                boxVelocity.Y = velocity.Y;
            }
            else
            {
                if (y < boxLoc.Y - 45)
                {
                    isLanded = true;
                    y = boxLoc.Y - 45;
                }
                else if (boxLoc.Y >= y - 10)
                {
                    if (animationType == 1 && x + 24 < boxLoc.X)
                    {
                        x = boxLoc.X - 48;
                        boxLoc.X += 5;
                    }
                    else if (animationType == 3 && x > boxLoc.X)
                    {
                        x = boxLoc.X + 24;
                        boxLoc.X -= 5;
                    }
                }
                //boxVelocity.X = velocity.X;
                //boxVelocity.Y = velocity.Y;
            }
        }
        foreach (var obj in groundLayer.objects)
        {
            var objRect = new Rectangle((int)obj.x, (int)obj.y, (int)obj.width, (int)obj.height);
            // can access as either a Rectangle or the direct obj calls
            bool xoverlap = (boxLoc.X < objRect.Right) && (boxLoc.X + 32 > objRect.Left);
            // a little wiggle room in these calcs in case the player is falling fast enough to skip through
            bool yoverlap = (boxLoc.Y + 32 - obj.y < 15) && (boxLoc.Y + 32 - obj.y > -15);

            //Debug.WriteLine(obj.y + " " + obj.height + " " + obj.width);
            if (xoverlap && yoverlap)
            {
                //Debug.WriteLine(obj.y + " " + obj.height);
                if (boxLoc.Y < obj.y && boxVelocity.Y >= 0)
                {
                    boxVelocity.Y = 0;
                    boxVelocity.X = 0;
                    //boxLoc = obj.y - 46;
                }
                else
                {
                    boxVelocity.X = 0;
                    boxVelocity.Y = MathHelper.Clamp(boxVelocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
                    //Fall down
                    if (animationType == 2 && x + 16 < obj.x)
                    {
                        x = obj.x - 20;
                    }
                    else if (animationType == 7 && x > obj.x + obj.width - 30)
                    {
                        x = obj.x + obj.width;
                    }
                    else if ((animationType == 2 || animationType == 7))
                    {
                        boxLoc.Y = obj.y + 30;
                    }
                }
                hitGround = true;
                isLanded = true;
                break;
            }

        }
       /* if (boxRect.Intersects(solidPlat))
        {
            if (objects[2].Y < objects[0].Y - 60)
            {
                boxVelocity.X = 0;
                boxVelocity.Y = 0;
            }
        }*/
       /*
        else if (boxRect.Intersects(otherPlat))
        {

            if (boxVelocity.Y > 0)
            {
                if (objects[2].Y < objects[0].Y)
                {
                    boxVelocity.X = 0;
                    boxVelocity.Y = 0;
                }
                else
                {
                    boxVelocity.X = 0;
                    boxVelocity.Y = MathHelper.Clamp(boxVelocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
                }

            }

        }*/
        boxLoc.X += boxVelocity.X * elapsed;
        boxLoc.Y += boxVelocity.Y * elapsed;

        // Check player ground collision
        hitGround = false;
        foreach (var obj in brownPlatLayer.objects)
        {
            var objRect = new Rectangle((int)obj.x, (int)obj.y, (int)obj.width, (int)obj.height);
            // can access as either a Rectangle or the direct obj calls
            bool xoverlap = (x < objRect.Right) && (x + 32 > objRect.Left);
            // a little wiggle room in these calcs in case the player is falling fast enough to skip through
            bool yoverlap = (y + 32 - obj.y < 15) && (y + 32 - obj.y > -15);

            if (xoverlap && yoverlap)
            {
                hitGround = true;
                isLanded = true;
                y = obj.y - 40;
                // once a collision has been detected, no need to check the other objects
                break;
            }
        }
        foreach (var obj in groundLayer.objects)
        {
            var objRect = new Rectangle((int)obj.x, (int)obj.y, (int)obj.width, (int)obj.height);
            // can access as either a Rectangle or the direct obj calls
            bool xoverlap = (x < objRect.Right) && (x + 32 > objRect.Left);
            // a little wiggle room in these calcs in case the player is falling fast enough to skip through
            bool yoverlap = (y + 32 - obj.y < 15) && (y + 32 - obj.y > -15);
            
            //Debug.WriteLine(obj.y + " " + obj.height + " " + obj.width);
            if (xoverlap && yoverlap)
            {
                //Debug.WriteLine(obj.y + " " + obj.height);
                if (y < obj.y && velocity.Y >= 0)
                {
                    //Debug.WriteLine("Yup");
                    isLanded = true;
                    hitGround = true;
                    y = obj.y - 46;
                }
                else
                {
                    //Debug.WriteLine("Nope");
                    //Fall down
                    if (animationType == 2 && x + 16 < obj.x)
                    {
                        x = obj.x - 20;
                    }
                    else if (animationType == 7 && x > obj.x + obj.width - 30)
                    {
                        x = obj.x + obj.width;
                    }
                    else if ((animationType == 2 || animationType == 7))
                    {
                        y = obj.y+30;
                    }
                }
                hitGround = true;
                isLanded = true;
                break;
            }
            
        }
        if(hitGround == false)
        {
            isLanded = false;
        }
        //Debug.WriteLine(hitGround);
        int currentAnim = animationType;
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
       
        if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Left) ||
            Keyboard.GetState().IsKeyDown(Keys.A))
        {
            animationType = 3;
            x -= 5;
        }
        else if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed ||
                 Keyboard.GetState().IsKeyDown(Keys.Right) ||
                 Keyboard.GetState().IsKeyDown(Keys.D))
        {
            animationType = 1;
            x += 5;
        }
        else
        {
            if (animationType == 3 || animationType == 4 || animationType == 6 || animationType == 7)
            {
                animationType = 4;
            }
            else
            {
                animationType = 0;
            }
            
        }
        if (
            Keyboard.GetState().IsKeyDown(Keys.Up) ||
            Keyboard.GetState().IsKeyDown(Keys.W))
        {
            isJumping = true;
            if (animationType == 3 || animationType == 4 || animationType == 6 || animationType == 7)
                animationType = 7;
            else
            {
                animationType = 2;
            }
        }
        if (
            Keyboard.GetState().IsKeyDown(Keys.Space))
        {
            isDodging = true;
            if(animationType == 3 || animationType == 4 || animationType == 6 || animationType == 7)
                animationType = 6;
            else
            {
                animationType = 5;
            }
        }
        ApplyPhysics(gameTime);
        // TODO: Add your update logic here
        if (timer > threshold)
        {
            if (currentAnimationIndex == animationFrames[animationType]-1)
            {
                previousAnimationIndex = currentAnimationIndex;
                currentAnimationIndex = 1;
            }
            else
            {
                previousAnimationIndex = currentAnimationIndex;
                currentAnimationIndex += 1;
            }
            timer = 0;
        }
        else
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }
        if(currentAnim != animationType)
        {
            previousAnimationIndex = 2;
            currentAnimationIndex = 1;
        }
        /*if(y > 1500)
        {
            x = 60;
            y = 400;
            isLanded = true;
            hitGround = true;
        }*/
        isJumping = false;
        isDodging = false;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        var tileLayers = map.Layers.Where(x => x.type == TiledLayerType.TileLayer);
        foreach (var layer in tileLayers)
        {
            for (var y = 0; y < layer.height; y++)
            {
                for (var x = 0; x < layer.width; x++)
                {
                    // Assuming the default render order is used which is from right to bottom
                    var index = (y * layer.width) + x;
                    var gid = layer.data[index]; // The tileset tile index
                    var tileX = x * map.TileWidth;
                    var tileY = y * map.TileHeight;

                    // Gid 0 is used to tell there is no tile set
                    if (gid == 0)
                    {
                        continue;
                    }

                    // Helper method to fetch the right TieldMapTileset instance
                    // This is a connection object Tiled uses for linking the correct tileset to the 
                    // gid value using the firstgid property
                    var mapTileset = map.GetTiledMapTileset(gid);

                    // Retrieve the actual tileset based on the firstgid property of the connection object 
                    // we retrieved just now
                    var tileset = tilesets[mapTileset.firstgid];

                    // Use the connection object as well as the tileset to figure out the source rectangle
                    var rect = map.GetSourceRect(mapTileset, tileset, gid);

                    // Create destination and source rectangles
                    var source = new Rectangle(rect.x, rect.y, rect.width, rect.height);
                    var destination = new Rectangle(tileX, tileY, map.TileWidth, map.TileHeight);

                    // You can use the helper methods to get information to handle flips and rotations
                    Trans tileTrans = Trans.None;
                    if (map.IsTileFlippedHorizontal(layer, x, y)) tileTrans |= Trans.Flip_H;
                    if (map.IsTileFlippedVertical(layer, x, y)) tileTrans |= Trans.Flip_V;
                    if (map.IsTileFlippedDiagonal(layer, x, y)) tileTrans |= Trans.Flip_D;

                    SpriteEffects effects = SpriteEffects.None;
                    double rotation = 0f;
                    switch (tileTrans)
                    {
                        case Trans.Flip_H: effects = SpriteEffects.FlipHorizontally; break;
                        case Trans.Flip_V: effects = SpriteEffects.FlipVertically; break;

                        case Trans.Rotate_90:
                            rotation = Math.PI * .5f;
                            destination.X += map.TileWidth;
                            break;

                        case Trans.Rotate_180:
                            rotation = Math.PI;
                            destination.X += map.TileWidth;
                            destination.Y += map.TileHeight;
                            break;

                        case Trans.Rotate_270:
                            rotation = Math.PI * 3 / 2;
                            destination.Y += map.TileHeight;
                            break;

                        case Trans.Rotate_90AndFlip_H:
                            effects = SpriteEffects.FlipHorizontally;
                            rotation = Math.PI * .5f;
                            destination.X += map.TileWidth;
                            break;

                        default:
                            break;
                    }
                    if(layer.name == "Ground")
                    {
                        //new Vector2(destination.x, destination.y)
                    }
                    else if (layer.name == "Box")
                    {
                        _spriteBatch.Draw(box, boxLoc, new Rectangle(0, 0, 28, 24), Color.White, angle, origin, 1.0f, SpriteEffects.None, 1);
                       // _spriteBatch.Draw(box, destination, source, Color.White,
                       //    (float)rotation, Vector2.Zero, effects, 0);
                    }
                    else
                    {
                            // Render sprite at position tileX, tileY using the rect
                            _spriteBatch.Draw(tilesetTexture, destination, source, Color.White,
                                (float)rotation, Vector2.Zero, effects, 0);
                    }
                    
                }
            }
        }

       // _spriteBatch.Draw(box, objects[2], new Rectangle(0, 0, 28, 24), Color.White, angle, origin, 1.0f, SpriteEffects.None, 1);
       // _spriteBatch.Draw(platGrey, objects[0], new Rectangle(0, 0, 32, 8), Color.White, angle, origin, 1.0f, SpriteEffects.None, 1);
       // _spriteBatch.Draw(platBrown, objects[1], new Rectangle(0, 0, 32, 8), Color.White, angle, origin, 1.0f, SpriteEffects.None, 1);

        if (animationType == 0)
        {
            _spriteBatch.Draw(characterIdle, new Vector2(x, y), idleSourceRectangles[currentAnimationIndex], Color.White, angle, origin, 1.0f, SpriteEffects.None, 1);
        }
        else if (animationType == 4)
        {
            _spriteBatch.Draw(characterIdle, new Vector2(x, y), idleSourceRectangles[currentAnimationIndex], Color.White, angle, origin, 1.0f, SpriteEffects.FlipHorizontally, 1);
        }
        else if (animationType == 1)
        {
            _spriteBatch.Draw(characterRun, new Vector2(x, y), runSourceRectangles[currentAnimationIndex], Color.White);
        }
        else if (animationType == 2)
        {
            _spriteBatch.Draw(characterJump, new Vector2(x, y), runSourceRectangles[currentAnimationIndex], Color.White);
        }
        else if (animationType == 3)
        {
            _spriteBatch.Draw(characterRun, new Vector2(x, y), runSourceRectangles[currentAnimationIndex], Color.White, angle, origin, 1.0f, SpriteEffects.FlipHorizontally, 1);
        }
        else if (animationType == 5)
        {
            _spriteBatch.Draw(characterDodge, new Vector2(x, y), dodgeSourceRectangles[currentAnimationIndex], Color.White, angle, origin, 1.0f, SpriteEffects.None, 1);
        }
        else if (animationType == 6)
        {
            _spriteBatch.Draw(characterDodge, new Vector2(x, y), dodgeSourceRectangles[currentAnimationIndex], Color.White, angle, origin, 1.0f, SpriteEffects.FlipHorizontally, 1);
        }
        else if (animationType == 7)
        {
            _spriteBatch.Draw(characterJump, new Vector2(x, y), runSourceRectangles[currentAnimationIndex], Color.White, angle, origin, 1.0f, SpriteEffects.FlipHorizontally, 1);
        }
        //_spriteBatch.Draw(characterIdle, new Vector2(100, 100), Color.White);


        _spriteBatch.End();
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}

