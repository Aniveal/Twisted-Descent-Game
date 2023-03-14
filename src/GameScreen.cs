using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        _world = new World();
        _batch = new SpriteBatch(_game.GraphicsDevice);
        _rope = new Rope(_game, _world, new Vector2(300, 50), 100, 20, 10);
    }

    public override void Update(GameTime gameTime) {
        base.Update(gameTime);

        _rope.Update(gameTime);
    }

    public override void Draw() {
        base.Draw();

        _batch.Begin();
        _rope.Draw(_batch);
        _batch.End();
    }
}