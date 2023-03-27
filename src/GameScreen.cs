using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using tainicom.Aether.Physics2D;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2;

public class GameScreen : Screen {
    public RopeGame Game;
    private SpriteBatch _batch;
    public World World;
    public Rope Rope;

    private List<Column> _columns = new List<Column>();
    private Amphora _amphora;
    private Amphora _amph2;
    private Map _map;
    public Player Player;
    public List<DummyRectangle> walls = new List<DummyRectangle>();


    public GameScreen(RopeGame game) : base(game) {
        Game = getGame();
        
        _map = new Map();
        Player = new Player(this);
        World = new World(Vector2.Zero);
        _batch = new SpriteBatch(Game.GraphicsDevice);
        Rope = new Rope(this, new Vector2(Game.GraphicsDevice.Viewport.Width / 2f, 20), 150);
        _amphora = new Amphora(Game, World, new Vector2(Game.GraphicsDevice.Viewport.Width / 2f + 50, 420), 10);
        _amph2 = new Amphora(Game, World, new Vector2(Game.GraphicsDevice.Viewport.Width / 2f -100, 270), 10);
    }

    public override void Initialize() {
        base.Initialize();
    
        _amphora.Initialize();
        _amph2.Initialize();
        //_map.Initialize();
        Rope.Initialize();
        Player.Initialize();

        //Create dummy walls
        
        walls.Add(new DummyRectangle(Game, World, new Vector2(Game.GraphicsDevice.Viewport.Width/2, 20), Game.GraphicsDevice.Viewport.Width, 10, Game.rectangleTexture));
        walls.Add(new DummyRectangle(Game, World, new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height - 50), Game.GraphicsDevice.Viewport.Width, 10, Game.rectangleTexture));

        walls.Add(new DummyRectangle(Game, World, new Vector2(100, Game.GraphicsDevice.Viewport.Height / 2), 10, Game.GraphicsDevice.Viewport.Height, Game.rectangleTexture));
        walls.Add(new DummyRectangle(Game, World, new Vector2(Game.GraphicsDevice.Viewport.Width - 100, Game.GraphicsDevice.Viewport.Height / 2), 10, Game.GraphicsDevice.Viewport.Height, Game.rectangleTexture));

        _columns.Add(new Column(Game, World, new Vector2(Game.GraphicsDevice.Viewport.Width / 2f + 120, 200), 10, Game.ColumnTexture));
        _columns.Add(new FragileColumn(Game, World, new Vector2(Game.GraphicsDevice.Viewport.Width / 2f + 40, 520), 10, Game.ColumnTexture));
        _columns.Add(new ElectricColumn(Game, World, new Vector2(Game.GraphicsDevice.Viewport.Width / 2f - 80, 400), 10, Game.ColumnTexture));
        _columns.Add(new FragileColumn(Game, World, new Vector2(Game.GraphicsDevice.Viewport.Width / 2f - 150, 220), 10, Game.ColumnTexture));
        //_map.LoadContent();
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
        Diagnostics.Instance.Update(gameTime, Player);
    }

    public override void Draw(GameTime gameTime) {
        base.Draw(gameTime);

        _batch.Begin();

        //_map.Draw(gameTime, _batch);
        Player.Draw(gameTime, _batch);
        Rope.Draw(gameTime, _batch);

        _amphora.Draw(gameTime, _batch);
        _amph2.Draw(gameTime, _batch);
        foreach (Column c in _columns)
        {
            c.Draw(_batch);
        }
        foreach (DummyRectangle rec in walls)
        {
            rec.Draw(_batch);
        }
        Diagnostics.Instance.Draw(_batch, Game.Font, new Vector2(10,10), Color.Red);
        _batch.End();

        
    }
}