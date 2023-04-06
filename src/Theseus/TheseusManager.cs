using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Theseus
{
    public class TheseusManager
    {
        private RopeGame _game;
        private World _world;
        public Player player;
        public Rope rope;

        public TheseusManager(RopeGame game, World world)
        {
            _game = game;
            _world = world;
            rope = new Rope(_game, _world, new Vector2(0, 0), 150);
            player = new Player(_game, _world, rope);
        }

        public void Initialize()
        {
            rope.Initialize();
            player.Initialize();
        }

        public void LoadContent()
        {
            rope.LoadContent();
            player.LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            player.Update(gameTime);
            rope.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
            rope.Draw(gameTime, batch, camera);
            player.Draw(gameTime, batch, camera);
        }
    }
}
