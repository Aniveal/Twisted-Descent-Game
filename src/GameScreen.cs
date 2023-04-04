﻿using Meridian2.Columns;
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

    private Map _map;
    //public List<DummyRectangle> walls = new List<DummyRectangle>();

    public TheseusManager theseusManager;
    public ColumnsManager columnsManager;


    public GameScreen(RopeGame game) : base(game) {
        Game = getGame();
        
        _batch = new SpriteBatch(Game.GraphicsDevice);
        _map = new Map(game);
        World = new World(Vector2.Zero);
        
        columnsManager = new ColumnsManager();
        theseusManager = new TheseusManager(Game, World);

    }

    public override void Initialize() {
        base.Initialize();

        _map.Initialize();
        
        theseusManager.Initialize();

        //Create dummy walls
        int w = Game.GraphicsDevice.Viewport.Width;
        int h = Game.GraphicsDevice.Viewport.Height;
        //int thick = w / 40;

        //TODO: move columns addition into a world generation class
        columnsManager.Add(new Column(Game, World, new Vector2(2, 2), 0.8f, Game.ColumnTexture));
        columnsManager.Add(new Column(Game, World, new Vector2(1, 2), 0.8f, Game.ColumnTexture));
        columnsManager.Add(new Column(Game, World, new Vector2(1, 3), 0.8f, Game.ColumnTexture));
        columnsManager.Add(new FragileColumn(Game, World, new Vector2(4, 6), 0.8f, Game.ColumnTexture));
        columnsManager.Add(new ElectricColumn(Game, World, new Vector2(6, 9), 0.8f, Game.ColumnTexture));
        columnsManager.Add(new ElectricColumn(Game, World, new Vector2(6, 4), 0.8f, Game.ColumnTexture));
        columnsManager.Add(new FragileColumn(Game, World, new Vector2(8, 3), 0.8f, Game.ColumnTexture));
        columnsManager.Add(new Amphora(Game, World, new Vector2(2, 4), 0.4f));
        columnsManager.Add(new Amphora(Game, World, new Vector2(2, 9), 0.4f));

        _map.LoadContent();
       
        theseusManager.LoadContent();
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
        Diagnostics.Instance.Update(gameTime, theseusManager.player);

        //putting it here cuz otherwise we'll forget about it the day when columns actually need updating
        //columnsManager.Update(gameTime);
    }

    public override void Draw(GameTime gameTime) {
        base.Draw(gameTime);

        _batch.Begin();

        _map.Draw(gameTime, _batch);
        columnsManager.DrawFirst(gameTime, _batch);

        theseusManager.Draw(gameTime, _batch);

        columnsManager.DrawSecond(gameTime, _batch);

        //foreach (DummyRectangle rec in walls)
        //{
        //    rec.Draw(_batch);
        //}
        Diagnostics.Instance.Draw(_batch, Game.Font, new Vector2(10,10), Color.Red);
        _batch.End();

        
    }
}