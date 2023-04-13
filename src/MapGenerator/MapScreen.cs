using Meridian2.Columns;
using Meridian2.Theseus;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Meridian2
{
    public class MapScreen : Screen
    {
        public RopeGame Game;
        private SpriteBatch _batch;
        public World World;
        public Camera Camera;

        float camMovementSpeed = 200;

        private Map _map;
        //public List<DummyRectangle> walls = new List<DummyRectangle>();


        public MapScreen(RopeGame game) : base(game)
        {
            Game = getGame();

            Camera = new Camera(Game.GraphicsDevice);
            Camera.Scale = 10.0f;

            _batch = new SpriteBatch(Game.GraphicsDevice);
           
            World = new World(Vector2.Zero);
            _map = new Map(game, World);

        }

        public override void Initialize()
        {
            base.Initialize();

            _map.Initialize();
            //Create dummy walls
            int w = Game.GraphicsDevice.Viewport.Width;
            int h = Game.GraphicsDevice.Viewport.Height;

            
            _map.LoadContent();

        }

        public override void Update(GameTime gameTime)
        {
            // Progress world physics
            World.Step(gameTime.ElapsedGameTime);
            World.Step(gameTime.ElapsedGameTime);
            World.Step(gameTime.ElapsedGameTime);
            World.Step(gameTime.ElapsedGameTime);

            base.Update(gameTime);
            _map.Update(gameTime);

            processInput(gameTime);

            //putting it here cuz otherwise we'll forget about it the day when columns actually need updating
            //columnsManager.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _batch.Begin();

            _map.Draw(gameTime, _batch, Camera);

            //foreach (DummyRectangle rec in walls)
            //{
            //    rec.Draw(_batch);
            //}
            Diagnostics.Instance.Draw(_batch, Game.Font, new Vector2(10, 10), Color.Red);
            _batch.End();


        }

        private void processInput(GameTime gameTime)
        {
            Vector2 camMove = Vector2.Zero;
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.W))
            {
                camMove.Y -= camMovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                camMove.Y += camMovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                camMove.X -= camMovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                camMove.X += camMovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (keyboard.IsKeyDown(Keys.Add))
            {
                Camera.Scale += 5 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (keyboard.IsKeyDown(Keys.Subtract))
            {
                Camera.Scale -= 5 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            Camera.Move(camMove);
        }


    }
}
