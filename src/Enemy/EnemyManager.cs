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
        private Player _player;
        public Enemy enemy;
        public List<Enemy> enemies = new List<Enemy>();
        private int _numOfEnemies;

        public EnemyManager(RopeGame game, World world, Player player, int numOfEnemies)
        {
            _game = game;
            _world = world;
            _player = player;
            _numOfEnemies = numOfEnemies;
        }

        public void Initialize()
        {
            for (int i = 0; i < _numOfEnemies ; i++)
            {
                enemy = new Enemy(_game, _world, _player);
                Vector2 initpos = new Vector2(-5, -5);
                int difficultyLevel = 2; // TODO change difficulty to random when initialize
                enemy.Initialize(initpos, difficultyLevel);
                enemies.Add(enemy);
            }
            
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
