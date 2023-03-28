
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

    float x = 100;
    float y = 100;

    bool isLanded = true;
    Vector2[] objects = { new Vector2(200, 90), new Vector2(500, 90), new Vector2(500, 65)};
    Vector2 boxVelocity = new Vector2(0, 0);
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
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

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
        velocity = DetectCollision(velocity, gameTime);

        //if (objects[2].Y > 100)
        //   boxVelocity.X *= GroundDragFactor;
        //else
        if (objects[2].Y <= 100)
            boxVelocity.Y *= AirDragFactor;
        boxVelocity.X = MathHelper.Clamp(boxVelocity.X, -MaxMoveSpeed, MaxMoveSpeed);

        // Apply velocity.
        
        y += velocity.Y * elapsed;
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



    private Vector2 DetectCollision(Vector2 velocity, GameTime gameTime)
    {
        //Time elapsed for position changes
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Rectangle player = new Rectangle((int)x, (int)y-24, idleSourceRectangles[currentAnimationIndex].Width, idleSourceRectangles[currentAnimationIndex].Height);
        Rectangle solidPlat = new Rectangle((int)objects[0].X, (int)objects[0].Y, 32, 8);
        Rectangle otherPlat = new Rectangle((int)objects[1].X, (int)objects[1].Y, 32, 8);
        Rectangle boxRect = new Rectangle((int)objects[2].X, (int)objects[2].Y, 28, 24);
        //Player Collisions 
        if (player.Intersects(solidPlat))
        {
            //2 & 7
            //On top of obstacle
            if (player.Y < objects[0].Y-45 && velocity.Y >= 0)
            {
                isLanded = true;
                y = objects[0].Y - 46;
            }
            else
            {
                //Fall down
                if (animationType == 2 && player.X + 32 < solidPlat.X)
                {
                    x = solidPlat.X - player.Width;
                }
                else if (animationType == 7 && player.X > solidPlat.X)
                {
                    x = solidPlat.X + 32;
                }
                else if((animationType == 2 || animationType == 7) && player.Y < objects[0].Y - 45)
                {

                }
                else if((animationType == 2 || animationType == 7))
                {
                    y = 90;
                }
            }
        }
        if (player.Intersects(otherPlat))
        {
           
            if (velocity.Y > 0)
            {
                if (player.Y < objects[0].Y - 45 && velocity.Y >= 0)
                {
                    isLanded = true;
                    y = objects[0].Y - 45;

                }
                else
                {
                    velocity.X = 0;
                    velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
                }
                
            }
            
        }
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
                if(player.Y < objects[2].Y - 45)
                {
                    isLanded = true;
                    y = objects[2].Y - 45;
                }
                else if (objects[2].Y >= y - 10) 
                {
                    if(animationType == 1 && player.X+24 < boxRect.X)
                    {
                        x = boxRect.X - player.Width;
                        objects[2].X += 5;
                    }
                    else if (animationType == 3 && player.X > boxRect.X)
                    {
                        x = boxRect.X + 24;
                        objects[2].X -= 5;
                    }
                }
                //boxVelocity.X = velocity.X;
                //boxVelocity.Y = velocity.Y;
            }
        }

        //Box Collisions
       
        if (boxRect.Intersects(solidPlat))
        {
            if (objects[2].Y < objects[0].Y - 60)
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

        }
        objects[2].X += boxVelocity.X * elapsed;
        objects[2].Y += boxVelocity.Y * elapsed;
        if (objects[2].Y >= 120)
        {
            boxVelocity.Y = 0;
            objects[2].Y = 120;
        }
       

        return velocity;
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
        if(y > 100)
        {
            y = 100;
            isLanded = true;
        }
        isJumping = false;
        isDodging = false;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();
        _spriteBatch.Draw(box, objects[2], new Rectangle(0, 0, 28, 24), Color.White, angle, origin, 1.0f, SpriteEffects.None, 1);
        _spriteBatch.Draw(platGrey, objects[0], new Rectangle(0, 0, 32, 8), Color.White, angle, origin, 1.0f, SpriteEffects.None, 1);
        _spriteBatch.Draw(platBrown, objects[1], new Rectangle(0, 0, 32, 8), Color.White, angle, origin, 1.0f, SpriteEffects.None, 1);
        _spriteBatch.Draw(platGrey, new Vector2(0,135), new Rectangle(0, 0, 1500, 10), Color.Green, angle, origin, 1.0f, SpriteEffects.None, 1);

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

