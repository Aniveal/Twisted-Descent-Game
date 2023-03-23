using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2;

public class GameScreen : Screen {
    private RopeGame _game;
    private SpriteBatch _batch;
    private World _world;
    private Rope _rope;
    
    private Map _map;
    private Player _player;

    public GameScreen(RopeGame game) : base(game) {
        _game = getGame();
        
        _map = new Map();
        _player = new Player();
        _world = new World(Vector2.Zero);
        _batch = new SpriteBatch(_game.GraphicsDevice);
        _rope = new Rope(_game, _world, new Vector2(_game.GraphicsDevice.Viewport.Width / 2f, 20), 150);
    }

    public override void Initialize() {
        base.Initialize();
    
        _map.Initialize();
        _player.Initialize();
        _rope.Initialize();
        
        _map.LoadContent();
        _player.LoadContent();
        _rope.LoadContent();
    }

    public override void Update(GameTime gameTime) {
        // Progress world physics
        _world.Step(gameTime.ElapsedGameTime);
        _world.Step(gameTime.ElapsedGameTime);
        _world.Step(gameTime.ElapsedGameTime);
        _world.Step(gameTime.ElapsedGameTime);

        base.Update(gameTime);
        _map.Update(gameTime);
        _player.Update(gameTime);
        _rope.Update(gameTime);
        Diagnostics.Instance.Update(gameTime);
    }

    public override void Draw(GameTime gameTime) {
        base.Draw(gameTime);

        _batch.Begin();
        _map.Draw(gameTime, _batch);
        _player.Draw(gameTime, _batch);
        _rope.Draw(gameTime, _batch);
        Diagnostics.Instance.Draw(_batch, _game.Font, new Vector2(10,10), Color.Red);
        _batch.End();
    }
}