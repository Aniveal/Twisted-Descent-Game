using System.Collections.Generic;
using System.Linq;
using Meridian2.Theseus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Enemy;

public class EnemyManager {
    private readonly RopeGame _game;
    private readonly int _numOfEnemies;
    private readonly Player _player;
    private readonly World _world;
    public List<Enemy> Enemies = new();

    public EnemyManager(RopeGame game, World world, Player player, int numOfEnemies) {
        _game = game;
        _world = world;
        _player = player;
        _numOfEnemies = numOfEnemies;
    }

    public void Initialize() {
        // for (var i = 0; i < _numOfEnemies; i++) {
        //     var enemy = new Enemy(_game, _world, _player);
        //     var initpos = new Vector2(-5, -5);
        //     var difficultyLevel = 3; // TODO change difficulty to random when initialize
        //     enemy.Initialize(initpos, difficultyLevel);
        //     Enemies.Add(enemy);
        // }
    }

    public void AddEnemy(Vector2 pos, int diff)
    {
        Enemy enemy = new Enemy(_game, _world, _player);
        enemy.generateRandomAbilities(diff);
        enemy.Initialize(pos, diff);
        enemy.LoadContent();
        Enemies.Add(enemy);
    }

    public void SetTutorialMode() {
        foreach (Enemy e in Enemies) {
            e.SetTutorialMode();
        }
    }

    public void LoadContent() {
        foreach (var enemy in Enemies) {
            if (enemy.IsAlive == false) continue;
            enemy.LoadContent();
        }
    }

    public void Update(GameTime gameTime) {
        foreach (var enemy in Enemies.ToList()) {
            if (enemy.IsAlive == false) {
                Enemies.Remove(enemy);
                enemy.Destroy();
            }

            enemy.Update(gameTime);
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        foreach (var enemy in Enemies) {
            if (enemy.IsAlive == false) continue;
            if (!camera.IsVisible(enemy.Body.Position)) continue;
            enemy.Draw(gameTime, batch, camera);
        }
    }
}