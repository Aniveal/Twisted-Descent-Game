using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2;

public class GameScreen : Screen {
    public RopeGame Game;
    private SpriteBatch _batch;
    public World World;
    public Rope Rope;

    private Map _map;
    public Player Player;

    public GameScreen(RopeGame game) : base(game) {
        Game = getGame();
        
        _map = new Map();
        Player = new Player(this);
        World = new World(Vector2.Zero);
        _batch = new SpriteBatch(Game.GraphicsDevice);
        Rope = new Rope(this, new Vector2(Game.GraphicsDevice.Viewport.Width / 2f, 20), 150);
    }

    public override void Initialize() {
        base.Initialize();
    
        _map.Initialize();
        Rope.Initialize();
        Player.Initialize();

        _map.LoadContent();
        Rope.LoadContent();
        Player.LoadContent();
    }

    public override void Update(GameTime gameTime) {
        // Progress world physics
        World.Step(gameTime.ElapsedGameTime);
        World.Step(gameTime.ElapsedGameTime);
        World.Step(gameTime.ElapsedGameTime);
        World.Step(gameTime.ElapsedGameTime);

        base.Update(gameTime);
        _map.Update(gameTime);
        Player.Update(gameTime);
        Rope.Update(gameTime);
        Diagnostics.Instance.Update(gameTime);
    }

    public override void Draw(GameTime gameTime) {
        base.Draw(gameTime);

        _batch.Begin();
        _map.Draw(gameTime, _batch);
        Player.Draw(gameTime, _batch);
        Rope.Draw(gameTime, _batch);
        Diagnostics.Instance.Draw(_batch, Game.Font, new Vector2(10,10), Color.Red);
        _batch.End();
    }
}