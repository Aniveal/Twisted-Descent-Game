using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;

namespace Meridian2.Theseus
{
    public class Player : DrawableGameElement
    {
        private readonly GameScreen _gameScreen;
        private readonly RopeGame _game;

        private Texture2D idle;
        private Texture2D running_l;
        private Texture2D running_r;
        private Texture2D running_f;
        private Texture2D running_b;
        private readonly Point _playerSize = new(60, 120);
        private int PlayerForce = 5000;

        //How many milliseconds between footsteps
        private float footstepSoundDelayMax = 400f;
        //Current value of delay
        private float footstepSoundDelayCurrent = 0f;

        public Body Body;

        public double DashTimer = 0;
        private const int DashCoolDown = 5000;
        private const int DashUsageTime = 400;
        private bool Dash = false;
        private bool isWalking = false;
        private Vector2 input = Vector2.Zero;

        public Player(GameScreen gameScreen)
        { 
            _gameScreen = gameScreen;
            _game = gameScreen.Game;
        }

        public void Initialize()
        {
            Body = _gameScreen.World.CreateEllipse((float)_playerSize.X / 2, (float)_playerSize.X / 4, 20, 0.1f,
                _gameScreen.Rope.GetEndPosition(), 0f, BodyType.Dynamic);
            Body.FixedRotation = true;
            Body.LinearDamping = 1f;

            // Disable rope collision
            foreach (Fixture fixture in Body.FixtureList)
            {
                fixture.CollisionGroup = -1;
            }

            JointFactory.CreateRevoluteJoint(_gameScreen.World, _gameScreen.Rope.LastSegment().Body, Body,
                Vector2.Zero);
        }

        public void LoadContent()
        {
            idle = _game.Content.Load<Texture2D>("idle");
            running_l = _game.Content.Load<Texture2D>("running");
            running_r = _game.Content.Load<Texture2D>("running_r");
            running_f = _game.Content.Load<Texture2D>("running_f");
            running_b = _game.Content.Load<Texture2D>("running_b");
        }

        private Vector2 ScreenToIsometric(Vector2 vector)
        {
            var rotSin = Math.Sin(-Math.PI / 4);
            var rotCos = Math.Cos(-Math.PI / 4);

            // Rotate by 45 degrees
            var isoX = (float)(rotCos * vector.X - rotSin * vector.Y);
            var isoY = (float)(rotSin * vector.X + rotCos * vector.Y);

            // Stretch to 2:1 ratio
            return new Vector2(isoX - isoY, (isoX + isoY) / 2);
        }

        public override void Update(GameTime gameTime)
        {
            input = Vector2.Zero;
            DashTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            DashTimer = Math.Min(DashTimer, 5000);

            GamePadCapabilities gamePadCapabilities = GamePad.GetCapabilities(PlayerIndex.One);
            if (gamePadCapabilities.IsConnected)
            {
                input = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
                input.Y *= -1;
            }

            isWalking = false;

            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Space) & DashTimer >= DashCoolDown)
            {
                Dash = true;
                DashTimer = 0;
                PlayerForce = 50000;
            }
            if (Dash & DashTimer >= DashUsageTime)
            {
                Dash = false;
                PlayerForce = 5000;
                DashTimer = 0;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                input.X += 1;
                isWalking = true;
            }

            if (keyboard.IsKeyDown(Keys.Left))
            {
                input.X -= 1;
                isWalking = true;
            }

            if (keyboard.IsKeyDown(Keys.Down))
            {
                input.Y += 1;
                isWalking = true;
            }

            if (keyboard.IsKeyDown(Keys.Up))
            {
                input.Y -= 1;
                isWalking = true;
            }


            //Footstep Sound:
            if (footstepSoundDelayCurrent >= 0)
                footstepSoundDelayCurrent -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (isWalking && footstepSoundDelayCurrent < 0)
            {
                Globals.SoundEngine.playGravelFootstep();
                footstepSoundDelayCurrent = footstepSoundDelayMax;
            }

            if (keyboard.IsKeyDown(Keys.P))
            {
                _gameScreen.Rope.Pull(gameTime);
            }

            input = ScreenToIsometric(input);

            if (input.LengthSquared() > 1)
            {
                input.Normalize();
            }

            Vector2 movement = input * (float)gameTime.ElapsedGameTime.TotalMilliseconds * PlayerForce;

            Body.ApplyForce(movement);

            // TODO: Camera movement was temporarily disabled and should be handled in a separate class
            // If the player is within a rectangle of the center of the screen, move the player, else move the camera:
            // int fraction = 3; // rectangle in the center makes up 1/3 of the width and 1/3 of the width
            //
            // Vector2 updatedPosition = _playerSpritePosition + movement;
            // Vector2 updatedFeetpos = FeetPosition(updatedPosition);
            //
            // float lowerX = (fraction - 1) * (Globals.Graphics.PreferredBackBufferWidth / 2) / fraction;
            // float upperX = (fraction + 1) * (Globals.Graphics.PreferredBackBufferWidth / 2) / fraction;
            // float lowerY = (fraction - 1) * (Globals.Graphics.PreferredBackBufferHeight / 2) / fraction;
            // float upperY = (fraction + 1) * (Globals.Graphics.PreferredBackBufferHeight / 2) / fraction;
            //
            // if (updatedPosition.X < lowerX || updatedFeetpos.X > upperX || updatedPosition.Y < lowerY ||
            //     updatedFeetpos.Y > upperY) {
            //     Vector2 updatedCamera = Globals.CameraPosition - movement; // move the camera
            //     Globals.UpdateCamera(updatedCamera);
            // } else {
            //     _playerSpritePosition = updatedPosition; // move the player
            //     _playerPosition = FeetPosition(_playerSpritePosition);
            // }

        }

        public override void Draw(GameTime gameTime, SpriteBatch batch)
        {
            var playerSpriteX = Body.Position.X - (float)_playerSize.X / 2;
            var playerSpriteY = Body.Position.Y - _playerSize.Y;

            Rectangle spritePos = new Rectangle((int)playerSpriteX, (int)playerSpriteY, _playerSize.X, _playerSize.Y);


            float totalTime = (float)gameTime.TotalGameTime.TotalMilliseconds;

            if (isWalking)
            {
                float run_duration = 200f;
                int run_frame_idx = (int)(totalTime / run_duration) % 4;

                Texture2D running_sprite = running_f;

                if (input.X > 0 && input.X >= input.Y)
                {
                    running_sprite = running_r;
                }
                else if (input.X < 0 && input.X <= input.Y)
                {
                    running_sprite = running_l;
                }
                else if (input.Y < 0)
                {
                    running_sprite = running_b;
                }
                //running_sprite = (input.X > 0 && input.X > input.Y) ? running_r : running_l;

                batch.Draw(
                running_sprite,
                new Rectangle((int)playerSpriteX, (int)playerSpriteY, _playerSize.X, _playerSize.Y),
                new Rectangle(run_frame_idx * 512, 0, 512, 768),
                Color.White
            );
            }
            else
            {
                float idle_duration = 400f; //ms
                int idle_frame_idx = (int)(totalTime / idle_duration) % 2;

                batch.Draw(
                    idle,
                    new Rectangle((int)playerSpriteX, (int)playerSpriteY, _playerSize.X, _playerSize.Y),
                    new Rectangle(idle_frame_idx * 512, 0, 512, 768),
                    Color.White
                );
            }
        }
    }
}