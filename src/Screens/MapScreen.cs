using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;
using TwistedDescent.Columns;
using TwistedDescent.Enemy;
using TwistedDescent.Gui;
using TwistedDescent.Theseus;
using TwistedDescent.Treasures;

namespace TwistedDescent.Screens;

public class MapScreen : Screen
{
    private SpriteBatch _batch;

    private readonly float _camMovementSpeed = 200;

    private Map _map;
    public ColumnsManager ColumnsManager;
    public RopeGame Game;
    //public List<DummyRectangle> walls = new List<DummyRectangle>();

    DiverseManager diverseManager;


    public MapScreen(RopeGame game) : base(game)
    {
        Game = getGame();
        Camera = new Camera(Game.GraphicsDevice);

        

        _batch = new SpriteBatch(Game.GraphicsDevice);

        World = new World(Vector2.Zero);
        ColumnsManager = new ColumnsManager(game);

        diverseManager = new DiverseManager();

        //debugging items
        //diverseManager.Add(new HealthChest(Game, World, new Vector2(2,2)));
        //diverseManager.Add(new Amphora(Game, World, new Vector2(2,2), 1));


        _map = new Map(game, World, ColumnsManager, null, diverseManager);
    }

    public void createNewLevel(int difficulty)
    {

        _batch = new SpriteBatch(Game.GraphicsDevice);

        World = new World(Vector2.Zero);
        ColumnsManager = new ColumnsManager(Game);
        diverseManager = new DiverseManager();

        Game.GameData.currentDifficulty = difficulty;

        _map = new Map(Game, World, ColumnsManager, null, diverseManager);
        _map.Initialize();

        _map.LoadContent();
        diverseManager.LoadContent();
        ColumnsManager.LoadContent();

    }


    public override void Initialize()
    {
        base.Initialize();
        _map.Initialize();

        Camera.SetCameraPos(new Vector2(0, 0));


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

        diverseManager.LoadContent();
        ColumnsManager.LoadContent();

        Camera.isInMapMode = true;
    }

    public override void Update(GameTime gameTime)
    {
        // Progress world physics
        World.Step(gameTime.ElapsedGameTime);
        World.Step(gameTime.ElapsedGameTime);
        World.Step(gameTime.ElapsedGameTime);
        World.Step(gameTime.ElapsedGameTime);

        base.Update(gameTime);
        _map.Update(gameTime);

        diverseManager.Update(gameTime);

        //putting it here cuz otherwise we'll forget about it the day when columns actually need updating. same for gui
        ColumnsManager.Update(gameTime);
        //guiManager.Update(gameTime);

        // Update Game Timer
        Game.GameData.DecayTime(gameTime);

        Vector2 movement = new Vector2(0, 0);

        //Player input
        var keyboard = Keyboard.GetState();
        if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
        {
            movement.X += 1;
        }

        if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
        {
            movement.X -= 1;
        }

        if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S))
        {
            movement.Y += 1;
        }

        if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W))
        {
            movement.Y -= 1;
        }

        if (keyboard.IsKeyDown(Keys.N))
        {
            Camera.Scale += 50 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        if (keyboard.IsKeyDown(Keys.M))
        {
            Camera.Scale -= 50 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        if (keyboard.IsKeyDown(Keys.Space))
        {
            createNewLevel(40);
        }

        if(movement.X == 0 && movement.Y == 0)
        {
            return;
        }


        movement.Normalize();

        movement = movement * 50 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Camera.Move(movement);

        Game.GameData.TimeLeft = 60;

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch = null)
    {
        base.Draw(gameTime);

        _batch.Begin(SpriteSortMode.FrontToBack);

        _map.Draw(gameTime, _batch, Camera);
        ColumnsManager.Draw(gameTime, _batch, Camera);

        diverseManager.Draw(gameTime, _batch, Camera);

        //foreach (DummyRectangle rec in walls)
        //{
        //    rec.Draw(_batch);
        //}

        // var ropeRed = new Color(170, 54, 54);
        //if (_map.levelFinished) {
        //    _batch.DrawString(Game.Font, "LEVEL COMPLETE", new Vector2(300, 300), ropeRed, 0f, Vector2.Zero, 1f,
        //        SpriteEffects.None, 1f);
        //}
        // Diagnostics.Instance.Draw(_batch, Game.Graphics.GraphicsDevice, Game.Font, new Vector2(10, 20), ropeRed);
        _batch.End();
    }
}