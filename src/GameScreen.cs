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

    public GameScreen(RopeGame game) : base(game) {
        this._game = getGame();
    }

    public override void Init() {
        base.Init();

        _world = new World(Vector2.Zero);
        _batch = new SpriteBatch(_game.GraphicsDevice);
        _rope = new Rope(_game, _world, new Vector2(_game.GraphicsDevice.Viewport.Width / 2f, 20), 150);
    }

    public override void Update(GameTime gameTime) {
        // Progress world physics
        _world.Step(gameTime.ElapsedGameTime);
        _world.Step(gameTime.ElapsedGameTime);
        _world.Step(gameTime.ElapsedGameTime);
        _world.Step(gameTime.ElapsedGameTime);

        base.Update(gameTime);
        _rope.Update(gameTime);
        Diagnostics.Instance.Update(gameTime);
    }

    public override void Draw() {
        base.Draw();

        _batch.Begin();
        Diagnostics.Instance.Draw(_batch, _game.Font, new Vector2(10,10), Color.Red);
        _rope.Draw(_batch);
        _batch.End();
    }
}