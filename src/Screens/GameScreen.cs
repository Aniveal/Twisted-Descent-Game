using Meridian2.Columns;
using Meridian2.Enemy;
using Meridian2.Gui;
using Meridian2.Theseus;
using Meridian2.Treasures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Screens;

public class GameScreen : Screen {
    private const float FixedTimeStep = 1 / 60f;
    private readonly SpriteBatch _batch;
    private double _fixedTickAccumulator;

    private readonly Map _map;
    public ColumnsManager ColumnsManager;
    public EnemyManager EnemyManager;
    public DiverseManager diverseManager;
    public RopeGame Game;

    public GuiManager GuiManager;

    public SpearsController SpearsController;
    //public List<DummyRectangle> walls = new List<DummyRectangle>();

    public TheseusManager TheseusManager;
    public World World;


    public GameScreen(RopeGame game) : base(game) {
        Game = getGame();
        Camera = new Camera(Game.GraphicsDevice);

        _batch = new SpriteBatch(Game.GraphicsDevice);

        World = new World(Vector2.Zero);
        ColumnsManager = new ColumnsManager();


        TheseusManager = new TheseusManager(Game, World);
        EnemyManager = new EnemyManager(Game, World, TheseusManager.Player, 1);
        SpearsController = new SpearsController(game, ColumnsManager, TheseusManager.Player);
        GuiManager = new GuiManager(game, SpearsController);
        diverseManager = new DiverseManager();


        _map = new Map(game, World, ColumnsManager, EnemyManager);
    }

    public override void Initialize() {
        base.Initialize();

        _map.Initialize();

        TheseusManager.Initialize();
        EnemyManager.Initialize();

        GuiManager.Initialize();

        //Create dummy walls
        var w = Game.GraphicsDevice.Viewport.Width;
        var h = Game.GraphicsDevice.Viewport.Height;
        //int thick = w / 40;

        //TODO: move columns addition into a world generation class
        // columnsManager.Add(new Column(Game, World, new Vector2(2, 2), 0.4f, Game.ColumnTexture));
        // columnsManager.Add(new Column(Game, World, new Vector2(1, 2), 0.4f, Game.ColumnTexture));
        // columnsManager.Add(new Column(Game, World, new Vector2(1, 3), 0.4f, Game.ColumnTexture));
        // columnsManager.Add(new FragileColumn(Game, World, new Vector2(4, 6), 0.4f, Game.ColumnTexture));
        // columnsManager.Add(new ElectricColumn(Game, World, new Vector2(6, 9), 0.4f, Game.ColumnTexture));
        // columnsManager.Add(new ElectricColumn(Game, World, new Vector2(6, 4), 0.4f, Game.ColumnTexture));
        // columnsManager.Add(new FragileColumn(Game, World, new Vector2(8, 3), 0.4f, Game.ColumnTexture));
        // columnsManager.Add(new Amphora(Game, World, new Vector2(2, 4), 0.2f));
        // columnsManager.Add(new Amphora(Game, World, new Vector2(2, 9), 0.2f));

        _map.LoadContent();

        TheseusManager.LoadContent();
        EnemyManager.LoadContent();
        GuiManager.LoadContent();
        SpearsController.LoadContent();
        diverseManager.LoadContent();
    }

    public void FixedUpdate(GameTime gameTime) {
        _fixedTickAccumulator += gameTime.ElapsedGameTime.TotalSeconds;

        while (_fixedTickAccumulator >= FixedTimeStep) {
            World.Step(FixedTimeStep);
            _fixedTickAccumulator -= FixedTimeStep;
        }
    }

    public override void Update(GameTime gameTime) {
        // Progress world physics
        World.Step(gameTime.ElapsedGameTime);
        World.Step(gameTime.ElapsedGameTime);
        World.Step(gameTime.ElapsedGameTime);
        World.Step(gameTime.ElapsedGameTime);

        base.Update(gameTime);
        _map.Update(gameTime);

        TheseusManager.Update(gameTime);
        EnemyManager.Update(gameTime);
        SpearsController.Update(gameTime);
        diverseManager.Update(gameTime);
        Diagnostics.Instance.Update(gameTime, TheseusManager.Player);

        //putting it here cuz otherwise we'll forget about it the day when columns actually need updating. same for gui
        //columnsManager.Update(gameTime);
        //guiManager.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch = null)
    {
        

        Camera.Pos = TheseusManager.Player.Body.Position;
        base.Draw(gameTime);

        _batch.Begin(SpriteSortMode.FrontToBack);

        _map.Draw(gameTime, _batch, Camera);
        ColumnsManager.Draw(gameTime, _batch, Camera);

        TheseusManager.Draw(gameTime, _batch, Camera);
        EnemyManager.Draw(gameTime, _batch, Camera);
        diverseManager.Draw(gameTime, _batch, Camera);

        //foreach (DummyRectangle rec in walls)
        //{
        //    rec.Draw(_batch);
        //}
        GuiManager.Draw(gameTime, _batch, Camera);

        var ropeRed = new Color(170, 54, 54);
        Diagnostics.Instance.Draw(_batch, Game.Font, new Vector2(10, 20), ropeRed);
        _batch.End();
    }
}