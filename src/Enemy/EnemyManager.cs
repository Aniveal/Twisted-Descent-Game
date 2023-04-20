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
                Enemy enemy = new Enemy(_game, _world, _player);
                Vector2 initpos = new Vector2(-5, -5);
                int difficultyLevel = 3; // TODO change difficulty to random when initialize
                enemy.Initialize(initpos, difficultyLevel);
                enemies.Add(enemy);
            }
            
        }

        public void AddEnemy(Vector2 pos, int diff) {
            Enemy enemy = new Enemy(_game, _world, _player);
            enemy.Initialize(pos, diff);
            enemy.LoadContent();
            enemies.Add(enemy);
        }

        public void LoadContent()
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.isAlive == false)
                {
                    continue;
                }
                enemy.LoadContent();
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Enemy enemy in enemies.ToList())
            {
                if (enemy.isAlive == false)
                {
                    enemies.Remove(enemy);
                    enemy.Destroy();
                }
                enemy.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.isAlive == false)
                {
                    continue;
                }
                enemy.Draw(gameTime, batch, camera);
            }
            
        }
    }
}
