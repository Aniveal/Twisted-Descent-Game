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


namespace Meridian2
{
    public class MapScreen : Screen
    {
        public RopeGame Game;
        private SpriteBatch _batch;
        public World World;

        private Map _map;
        //public List<DummyRectangle> walls = new List<DummyRectangle>();


        public MapScreen(RopeGame game) : base(game)
        {
            Game = getGame();

            _batch = new SpriteBatch(Game.GraphicsDevice);
            _map = new Map(game);
            World = new World(Vector2.Zero);

        }

        public override void Initialize()
        {
            base.Initialize();

            _map.Initialize();


            //Create dummy walls
            int w = Game.GraphicsDevice.Viewport.Width;
            int h = Game.GraphicsDevice.Viewport.Height;
            //int thick = w / 40;

            //walls.Add(new DummyRectangle(Game, World, new Vector2(2 * w / 10, 0), thick, 6 * h / 10, Game.rectangleTexture));
            //walls.Add(new DummyRectangle(Game, World, new Vector2(0, 6 * h / 10), 2 * w / 10, thick, Game.rectangleTexture));
            //walls.Add(new DummyRectangle(Game, World, new Vector2(7 * w / 10, 5 * h / 10), 3 * w / 10, thick, Game.rectangleTexture));
            //walls.Add(new DummyRectangle(Game, World, new Vector2(7 * w / 10, 5 * h / 10), thick, 6 * h / 10, Game.rectangleTexture));
            //walls.Add(new DummyRectangle(Game, World, new Vector2(3 * w / 10, 5 * h / 10), thick, 6 * h / 10, Game.rectangleTexture));
            //walls.Add(new DummyRectangle(Game, World, new Vector2(8 * w / 10, 8 * h / 10), 2 * w / 10, thick, Game.rectangleTexture));
            //walls.Add(new DummyRectangle(Game, World, new Vector2(9 * w / 10, 1 * h / 10), 7 * w / 10, thick, Game.rectangleTexture));

            
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

            //putting it here cuz otherwise we'll forget about it the day when columns actually need updating
            //columnsManager.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _batch.Begin();

            _map.Draw(gameTime, _batch);

            //foreach (DummyRectangle rec in walls)
            //{
            //    rec.Draw(_batch);
            //}
            Diagnostics.Instance.Draw(_batch, Game.Font, new Vector2(10, 10), Color.Red);
            _batch.End();


        }
    }
}
