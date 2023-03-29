using Meridian2.Columns;
using Meridian2.Interfaces;
using Meridian2.Theseus;
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

    public ColumnsManager columnsManager;


    public GameScreen(RopeGame game) : base(game) {
        Game = getGame();
        
        _map = new Map(game);
        Player = new Player(this);
        World = new World(Vector2.Zero);
        _batch = new SpriteBatch(Game.GraphicsDevice);

        columnsManager = new ColumnsManager();

        Rope = new Rope(this, new Vector2(Game.GraphicsDevice.Viewport.Width / 2f, 20), 150);
        _amphora = new Amphora(Game, World, new Vector2(Game.GraphicsDevice.Viewport.Width / 2f + 50, 420), 10);
        _amph2 = new Amphora(Game, World, new Vector2(Game.GraphicsDevice.Viewport.Width / 2f -100, 270), 10);
    }

    public override void Initialize() {
        base.Initialize();

        _amphora.Initialize();
        _amph2.Initialize();
        _map.Initialize();
        Rope.Initialize();
        Player.Initialize();

        //Create dummy walls
        int w = Game.GraphicsDevice.Viewport.Width;
        int h = Game.GraphicsDevice.Viewport.Height;
        //int thick = w / 40;

        //walls.Add(new DummyRectangle(Game, World, new Vector2(2 * w / 10, 0), thick, 6 * h / 10, Game.rectangleTexture));
        //walls.Add(new DummyRectangle(Game, World, new Vector2(0, 6 * h / 10), 2 * w / 10, thick, Game.rectangleTexture));
        //walls.Add(new DummyRectangle(Game, World, new Vector2(7 * w / 10, 5 * h / 10), 3 * w / 10, thick, Game.rectangleTexture));
        //walls.Add(new DummyRectangle(Game, World, new Vector2(7 * w / 10, 5 * h / 10), thick, 6 * h / 10, Game.rectangleTexture));
        //walls.Add(new DummyRectangle(Game, World, new Vector2(3 * w / 10, 5 * h / 10), thick, 6 * h / 10, Game.rectangleTexture));
        //walls.Add(new DummyRectangle(Game, World, new Vector2(8 * w / 10, 8 * h / 10), 2 * w / 10, thick, Game.rectangleTexture));
        //walls.Add(new DummyRectangle(Game, World, new Vector2(9 * w / 10, 1 * h / 10), 7 * w / 10, thick, Game.rectangleTexture));

        columnsManager.Add(new Column(Game, World, new Vector2(8 * w / 10, 2 * h/10), 10, Game.ColumnTexture));
        columnsManager.Add(new Column(Game, World, new Vector2(1 * w / 10, 2 * h / 10), 10, Game.ColumnTexture));
        columnsManager.Add(new Column(Game, World, new Vector2(1 * w / 10, 3 * h / 10), 10, Game.ColumnTexture));
        columnsManager.Add(new FragileColumn(Game, World, new Vector2(4 * w / 10, 6 * h / 10), 10, Game.ColumnTexture));
        columnsManager.Add(new ElectricColumn(Game, World, new Vector2(6 * w / 10, 9 * h / 10), 10, Game.ColumnTexture));
        columnsManager.Add(new ElectricColumn(Game, World, new Vector2(6 * w / 10, 4 * h / 10), 10, Game.ColumnTexture));
        columnsManager.Add(new FragileColumn(Game, World, new Vector2(8 * w / 10, 3 * h / 10), 10, Game.ColumnTexture));
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
        Diagnostics.Instance.Update(gameTime, Player);

        //putting it here cuz otherwise we'll forget about it the day when columns actually need updating
        //columnsManager.Update(gameTime);
    }

    public override void Draw(GameTime gameTime) {
        base.Draw(gameTime);

        _batch.Begin();

        _map.Draw(gameTime, _batch);
        columnsManager.DrawFirst(gameTime, _batch);
        Player.Draw(gameTime, _batch);
        Rope.Draw(gameTime, _batch);
        columnsManager.DrawSecond(gameTime, _batch);

        _amphora.Draw(gameTime, _batch);
        _amph2.Draw(gameTime, _batch);
        //foreach (DummyRectangle rec in walls)
        //{
        //    rec.Draw(_batch);
        //}
        Diagnostics.Instance.Draw(_batch, Game.Font, new Vector2(10,10), Color.Red);
        _batch.End();

        
    }
}