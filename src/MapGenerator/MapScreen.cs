using Meridian2.Columns;
using Meridian2.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2; 

public class MapScreen : Screen {
    private readonly SpriteBatch _batch;

    private readonly float _camMovementSpeed = 200;

    private readonly Map _map;
    public ColumnsManager ColumnsManager;
    public RopeGame Game;

    public World World;
    //public List<DummyRectangle> walls = new List<DummyRectangle>();


    public MapScreen(RopeGame game) : base(game) {
        Game = getGame();

        Camera = new Camera(Game.GraphicsDevice);
        Camera.Scale = 10.0f;

        _batch = new SpriteBatch(Game.GraphicsDevice);

        World = new World(Vector2.Zero);
        ColumnsManager = new ColumnsManager();
        _map = new Map(game, World, ColumnsManager, new EnemyManager(game, World, null, 0));
    }

    public override void Initialize() {
        base.Initialize();

        _map.Initialize();
        //Create dummy walls
        var w = Game.GraphicsDevice.Viewport.Width;
        var h = Game.GraphicsDevice.Viewport.Height;


        _map.LoadContent();
    }

    public override void Update(GameTime gameTime) {
        // Progress world physics
        World.Step(gameTime.ElapsedGameTime);
        World.Step(gameTime.ElapsedGameTime);
        World.Step(gameTime.ElapsedGameTime);
        World.Step(gameTime.ElapsedGameTime);

        base.Update(gameTime);
        _map.Update(gameTime);

        processInput(gameTime);

        //putting it here cuz otherwise we'll forget about it the day when columns actually need updating
        //columnsManager.Update(gameTime);
    }

    public override void Draw(GameTime gameTime) {
        base.Draw(gameTime);

        _batch.Begin();

        _map.Draw(gameTime, _batch, Camera);

        //foreach (DummyRectangle rec in walls)
        //{
        //    rec.Draw(_batch);
        //}
        Diagnostics.Instance.Draw(_batch, Game.Font, new Vector2(10, 10), Color.Red);
        _batch.End();
    }

    private void processInput(GameTime gameTime) {
        var camMove = Vector2.Zero;
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.W)) camMove.Y -= _camMovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (keyboard.IsKeyDown(Keys.S)) camMove.Y += _camMovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (keyboard.IsKeyDown(Keys.A)) camMove.X -= _camMovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (keyboard.IsKeyDown(Keys.D)) camMove.X += _camMovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (keyboard.IsKeyDown(Keys.Add)) Camera.Scale += 5 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (keyboard.IsKeyDown(Keys.Subtract)) Camera.Scale -= 5 * (float)gameTime.ElapsedGameTime.TotalSeconds;

        Camera.Move(camMove);
    }
}