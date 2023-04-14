using Meridian2.Theseus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Enemy
{
    public class EnemyManager
    {
        private RopeGame _game;
        private World _world;
        public Enemy enemy;

        public EnemyManager(RopeGame game, World world, Player player)
        {
            _game = game;
            _world = world;
            enemy = new Enemy(_game, _world, player);
        }

        public void Initialize(Vector2 initpos, int difficultyLevel)
        {
            enemy.Initialize(initpos, difficultyLevel);
        }

        public void LoadContent()
        {
            enemy.LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            enemy.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
            enemy.Draw(gameTime, batch, camera);
        }
    }
}
