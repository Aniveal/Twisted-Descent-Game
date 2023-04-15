using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using Meridian2.GameElements;
using Meridian2.Theseus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace Meridian2.Enemy
{
    public class Enemy : DrawableGameElement
    {
        private readonly RopeGame _game;
        private World _world;
        private Player _player;

        private Texture2D idle;
        private Texture2D running_l;
        private Texture2D running_r;
        private Texture2D running_f;
        private Texture2D running_b;
        private readonly Point _enemySize = new(1, 2);
        private float EnemyForce = 0.01f;
        private int _difficultyLevel;
        private float FollowDistance = 100;
        private float AngerDistance = 200;

        public Body Body;
        public Vector2 orientation;

        private bool isWalking = false;
        private Vector2 input = Vector2.Zero;

        public Enemy(RopeGame game, World world, Player player)
        { 
            _game = game;
            _world = world;
            _player = player;
        }

        public void Initialize(Vector2 initpos, int difficultyLevel)
        {
            Body = _world.CreateEllipse((float)_enemySize.X / 2, (float)_enemySize.X / 4, 20, 0.01f,
                initpos, 0f, BodyType.Dynamic);
            Body.FixedRotation = true;
            Body.LinearDamping = 1f;
            Body.Tag = this;
            Body.OnCollision += OnCollision;

            _difficultyLevel = difficultyLevel;
        }

        public void LoadContent()
        {
            idle = _game.Content.Load<Texture2D>("Enemy");
            running_l = _game.Content.Load<Texture2D>("Enemy");
            running_r = _game.Content.Load<Texture2D>("Enemy");
            running_f = _game.Content.Load<Texture2D>("Enemy");
            running_b = _game.Content.Load<Texture2D>("Enemy");
        }

        public void Electrify() {
            //TODO: play animation (change color to yellow?), take damage
        }

        protected bool OnCollision(Fixture sender, Fixture other, Contact contact) {
            Body collider;
            if (sender.Body.Tag == this) {
                collider = other.Body;
            } else {
                collider = sender.Body;
            }
            if (collider.Tag == null) {
                return true;
            }
            ///player collision
            if (collider.Tag is Player) {
                _game.gameData.health -= 1; //TODO: do stuff when health reaches 0
            }
            // If colliding with rope, and rope electrified
            if (collider.Tag is RopeSegment) {
                if (((RopeSegment) collider.Tag).elecIntensity > 0) {
                    Electrify();
                }
                
            }

            return true;
        }
        

        public override void Update(GameTime gameTime)
        {
            input = Vector2.Zero;
            isWalking = false;
            if(_difficultyLevel == 0) // enemies does not move
            {

            }

            if (_difficultyLevel == 1) // enemies moves random
            {
                Random rnd = new Random();
                int r = rnd.Next(0, 3);
                if (r == 0)
                    input.X += 1;
                if (r == 1)
                    input.X -= 1;
                if (r == 2)
                    input.Y += 1;
                if (r == 3)
                    input.Y -= 1;
            }

            if (_difficultyLevel == 2) // enemies chase you
            {
 
                var currentDistance = Vector2.Distance(this.Body.Position, _player.Body.Position);
                if (currentDistance > FollowDistance)
                {
                    if (this.Body.Position.X < _player.Body.Position.X)
                    {
                        input.X += 1;
                    }
                    if (this.Body.Position.X > _player.Body.Position.X)
                    {
                        input.X -= 1;
                    }
                    if (this.Body.Position.Y < _player.Body.Position.Y)
                    {
                        input.Y += 1;
                    }
                    if (this.Body.Position.Y > _player.Body.Position.Y)
                    {
                        input.Y -= 1;
                    }
                }
            }

            if (_difficultyLevel == 3) // enemies chase you for a while
            {
                var currentDistance = Vector2.Distance(this.Body.Position, _player.Body.Position);
                if (currentDistance > AngerDistance)
                {
                    return;
                }
                if (currentDistance > FollowDistance)
                {
                    if (this.Body.Position.X < _player.Body.Position.X)
                    {
                        input.X += 1;
                    }
                    if (this.Body.Position.X > _player.Body.Position.X)
                    {
                        input.X -= 1;
                    }
                    if (this.Body.Position.Y < _player.Body.Position.Y)
                    {
                        input.Y += 1;
                    }
                    if (this.Body.Position.Y > _player.Body.Position.Y)
                    {
                        input.Y -= 1;
                    }
                }
            }


            if (input.LengthSquared() > 1)
            {
                input.Normalize();
            }

            Vector2 movement = input * (float)gameTime.ElapsedGameTime.TotalMilliseconds * EnemyForce;

            Body.ApplyForce(movement);
            orientation = input;
          
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
            camera.Pos = Body.Position;
            Rectangle spritePos = camera.getScreenRectangle(Body.Position.X, Body.Position.Y - _enemySize.Y*2 + (float)_enemySize.X / 4, _enemySize.X, _enemySize.Y);

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