using System;
using System.Diagnostics;
using System.Linq;
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
        private readonly RopeGame _game;
        private Rope _rope;
        private World _world;
        private DistanceJoint _ropeConnection;

        private Texture2D idle;
        private Texture2D running_l;
        private Texture2D running_r;
        private Texture2D running_f;
        private Texture2D running_b;
        private readonly Point _playerSize = new(1, 2);
        private float PlayerForce = 10f;

        //How many milliseconds between footsteps
        private float footstepSoundDelayMax = 400f;
        //Current value of delay
        private float footstepSoundDelayCurrent = 0f;

        public Body Body;

        public Vector2 orientation;

        public double DashTimer = 0;
        private const int DashCoolDown = 5000;
        private const int DashUsageTime = 400;
        private bool Dash = false;
        private bool isWalking = false;
        private bool isPulling = false;
        private Vector2 input = Vector2.Zero;

        public Boolean isImmune = false;
        private double immuneTimer = 0;
        private double immuneCooldown = 3000;

        public Player(RopeGame game, World world, Rope rope)
        { 
            _rope = rope;
            _game = game;
            _world = world;
        }

        public void Initialize()
        {
            Body = _world.CreateEllipse((float)_playerSize.X / 2, (float)_playerSize.X / 4, 20, 0.01f,
                _rope.GetEndPosition(), 0f, BodyType.Dynamic);
            Body.FixedRotation = true;
            Body.LinearDamping = 1f;
            Body.Tag = this;

            Body.Mass = 10;
            // Disable rope collision
            foreach (Fixture fixture in Body.FixtureList)
            {
                fixture.CollisionGroup = -1;
            }

            LinkToRope();
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

            if(isImmune)
            {
                immuneTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if(immuneTimer > immuneCooldown)
                {
                    isImmune = false;
                    immuneTimer = 0;
                }
            }

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
                PlayerForce = 30f;
            }
            if (Dash & DashTimer >= DashUsageTime)
            {
                Dash = false;
                PlayerForce = 10f;
                DashTimer = 0;
            }
            if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
            {
                input.X += 1;
                isWalking = true;
            }

            if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
            {
                input.X -= 1;
                isWalking = true;
            }

            if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S))
            {
                input.Y += 1;
                isWalking = true;
            }

            if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W))
            {
                input.Y -= 1;
                isWalking = true;
            }


            //Footstep Sound:
            if (footstepSoundDelayCurrent >= 0)
                footstepSoundDelayCurrent -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (isWalking && footstepSoundDelayCurrent < 0)
            {
                _game.soundEngine.playGravelFootstep();
                footstepSoundDelayCurrent = footstepSoundDelayMax;
            }

            if (input.LengthSquared() > 1)
            {
                input.Normalize();
            }

            Vector2 movement = input * (float)gameTime.ElapsedGameTime.TotalMilliseconds * PlayerForce;

            Body.ApplyForce(movement);
            orientation = input;

            var ropeJointDistance = (_ropeConnection.WorldAnchorB - _ropeConnection.WorldAnchorA).Length();

            if (_ropeConnection != null) {
                // Extend rope if force on joint is too strong
                if (ropeJointDistance > Rope.TextureHeight) {
                    // Remove player joint
                    _world.Remove(_ropeConnection);
                    _ropeConnection = null;
                    
                    _rope.AppendSegment();
                    LinkToRope();
                } else if (movement == Vector2.Zero) {
                    // _world.Remove(_ropeConnection);
                    // _ropeConnection = null;
                    // _rope.RemoveSegment();
                    // LinkToRope();
                }
            }
            
            ropeJointDistance = (_ropeConnection.WorldAnchorB - _ropeConnection.WorldAnchorA).Length();
            Diagnostics.Instance.SetForce(ropeJointDistance);
            if (keyboard.IsKeyDown(Keys.P)) {
                isPulling = true;
                if (ropeJointDistance < Rope.TextureHeight*2) {
                    _world.Remove(_ropeConnection);
                    _ropeConnection = null;
                    _rope.RemoveSegment();
                    LinkToRope();
                }

                _rope.Pull(gameTime);
            } else {
                isPulling = false;
            }
        }

        private void LinkToRope() {
            _ropeConnection = JointFactory.CreateDistanceJoint(_world, _rope.LastSegment().Body, Body, 
                new Vector2(Rope.TextureWidth / 2, Rope.TextureHeight),
                new Vector2((float)_playerSize.X / 2, (float)_playerSize.X / 4));
            _ropeConnection.Length = Rope.RopeJointLength;
            _ropeConnection.Frequency = 15;
            _ropeConnection.DampingRatio = Rope.RopeJointDampingRatio;
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
            camera.Pos = Body.Position;
            Rectangle spritePos = camera.getScreenRectangle(Body.Position.X, Body.Position.Y - _playerSize.Y*2 + (float)_playerSize.X / 4, _playerSize.X, _playerSize.Y);

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
                spritePos,
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
                    spritePos,
                    new Rectangle(idle_frame_idx * 512, 0, 512, 768),
                    Color.White
                );
            }
        }
    }
}