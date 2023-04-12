using Meridian2.Columns;
using Meridian2.Theseus;
using Meridian2.Gui;
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
    public Camera Camera;

    private Map _map;
    //public List<DummyRectangle> walls = new List<DummyRectangle>();

    public TheseusManager theseusManager;
    public ColumnsManager columnsManager;

    public GuiManager guiManager;
    public SpearsController spearsController;


    public GameScreen(RopeGame game) : base(game) {
        Game = getGame();
        Camera = new Camera(Game.GraphicsDevice);

        _batch = new SpriteBatch(Game.GraphicsDevice);
        _map = new Map(game);
        World = new World(Vector2.Zero);
        
        columnsManager = new ColumnsManager();
        theseusManager = new TheseusManager(Game, World);
        guiManager = new GuiManager(game);
        spearsController = new SpearsController(game, columnsManager, theseusManager.player);
    }

    public override void Initialize() {
        base.Initialize();

        _map.Initialize();
        
        theseusManager.Initialize();

        guiManager.Initialize();

        //Create dummy walls
        int w = Game.GraphicsDevice.Viewport.Width;
        int h = Game.GraphicsDevice.Viewport.Height;
        //int thick = w / 40;

        //TODO: move columns addition into a world generation class
        columnsManager.Add(new Column(Game, World, new Vector2(2, 2), 0.4f, Game.ColumnTexture));
        columnsManager.Add(new Column(Game, World, new Vector2(1, 2), 0.4f, Game.ColumnTexture));
        columnsManager.Add(new Column(Game, World, new Vector2(1, 3), 0.4f, Game.ColumnTexture));
        columnsManager.Add(new FragileColumn(Game, World, new Vector2(4, 6), 0.4f, Game.ColumnTexture));
        columnsManager.Add(new ElectricColumn(Game, World, new Vector2(6, 9), 0.4f, Game.ColumnTexture));
        columnsManager.Add(new ElectricColumn(Game, World, new Vector2(6, 4), 0.4f, Game.ColumnTexture));
        columnsManager.Add(new FragileColumn(Game, World, new Vector2(8, 3), 0.4f, Game.ColumnTexture));
        columnsManager.Add(new Amphora(Game, World, new Vector2(2, 4), 0.2f));
        columnsManager.Add(new Amphora(Game, World, new Vector2(2, 9), 0.2f));

        _map.LoadContent();
       
        theseusManager.LoadContent();
        guiManager.LoadContent();
        spearsController.LoadContent();
    }

    public override void Update(GameTime gameTime) {
        // Progress world physics
        World.Step(gameTime.ElapsedGameTime);
        World.Step(gameTime.ElapsedGameTime);
        World.Step(gameTime.ElapsedGameTime);
        World.Step(gameTime.ElapsedGameTime);

        base.Update(gameTime);
        _map.Update(gameTime);

        theseusManager.Update(gameTime);
        spearsController.Update(gameTime);
        Diagnostics.Instance.Update(gameTime, theseusManager.player);

        //putting it here cuz otherwise we'll forget about it the day when columns actually need updating. same for gui
        //columnsManager.Update(gameTime);
        //guiManager.Update(gameTime);
    }

    public override void Draw(GameTime gameTime) {
        base.Draw(gameTime);

        _batch.Begin();

        _map.Draw(gameTime, _batch, Camera);
        columnsManager.DrawFirst(gameTime, _batch, Camera);

        theseusManager.Draw(gameTime, _batch, Camera);

        columnsManager.DrawSecond(gameTime, _batch, Camera);

        //foreach (DummyRectangle rec in walls)
        //{
        //    rec.Draw(_batch);
        //}
        guiManager.Draw(gameTime, _batch, Camera);

        Diagnostics.Instance.Draw(_batch, Game.Font, new Vector2(10,10), Color.Red);
        _batch.End();
    }
}