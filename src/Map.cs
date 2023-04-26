using System;
using System.Collections.Generic;
using System.Diagnostics;
using Meridian2.Columns;
using Meridian2.Enemy;
using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2; 

public class Map : DrawableGameElement {
    private const float MapScaling = 2;
    private const int MapTranslation = -4;
    private readonly RopeGame _game;
    private List<Texture2D> _cliffTextures;
    private List<Texture2D> _column;
    private Texture2D _ground;
    private List<Texture2D> _rockTextures;
    private List<Texture2D> _wallTextures;
    private float _wallWidth = (float)Math.Sqrt(MapScaling * 2);
    private readonly World _world;

    //WIP, remove this as soon as possible
    public ColumnsManager Cm;

    public EnemyManager Em;

    //The list of rooms to draw
    public List<Room> RoomList = new();

    public Point TileSize = new(160, 160); // pixels

    public Map(RopeGame game, World world, ColumnsManager cm, EnemyManager em) {
        _game = game;
        _world = world;
        Cm = cm;
        Em = em;
    }


    /* Helper Functions */
    // Isometric Math: https://clintbellanger.net/articles/isometric_math/
    // Note: our tiles have slightly different shape, (they're twice as high: take 1/4 of the height instead of 1/2)

    // MapToScreen: take index of a tile as an input (e.g. (2,1)) returns pixel position.
    public Point MapToScreen(Point mapCoordinates) {
        var halfTile = TileSize.X / 2;
        var quaterTile = TileSize.X / 4;

        var screenX = (mapCoordinates.X - mapCoordinates.Y) * halfTile;
        var screenY = (mapCoordinates.X + mapCoordinates.Y) * quaterTile;
        return new Point(screenX + (int)_game._currentScreen.Camera.Pos.X,
            screenY + (int)_game._currentScreen.Camera.Pos.Y);
    }

    //Returns the world coordinates of the left angle of the ground level of the tile
    public Vector2 MapToWorld(Point mapCoordinates) {
        return new Vector2((mapCoordinates.X - mapCoordinates.Y + MapTranslation) * MapScaling,
            (mapCoordinates.X + mapCoordinates.Y + MapTranslation) * MapScaling);
    }

    //Returns the world coordinates of the left angle of the ground level of the tile
    public Vector2 MapToWorld(int x, int y) {
        return new Vector2((x - y + MapTranslation) * MapScaling, (x + y + MapTranslation) * MapScaling);
    }

    public Vector2 MapToWorld(float x, float y) {
        return new Vector2((x - y + MapTranslation) * MapScaling, (x + y + MapTranslation) * MapScaling);
    }

    // ScreenToMap: takes pixel position, returns the index of the tile at this position.
    public Point ScreenToMap(Point screenPos) {
        screenPos.X -= (int)_game._currentScreen.Camera.Pos.X;
        screenPos.Y -= (int)_game._currentScreen.Camera.Pos.Y;

        var halfTile = TileSize.X / 2;
        var quaterTile = TileSize.X / 4;
        var mapX = (screenPos.X / halfTile + screenPos.Y / quaterTile) / 2;
        var mapY = (screenPos.Y / quaterTile - screenPos.X / halfTile) / 2;
        return new Point(mapX, mapY);
    }

    //building a polygon of the shape of wall_3f
    private Vertices buildWall3Polygon(float scaling) {
        var v = new List<Vector2>();
        v.Add(new Vector2(-scaling, 0)); //left corner
        v.Add(new Vector2(-scaling * 0.5f, scaling * 0.5f)); //left end of hole
        v.Add(new Vector2(-scaling * 0.25f, scaling * 0.25f));
        v.Add(new Vector2(scaling * 0.25f, scaling * 0.25f));
        v.Add(new Vector2(scaling * 0.5f, scaling * 0.5f)); //right end of hole
        v.Add(new Vector2(scaling, 0)); //right corner
        v.Add(new Vector2(0, -scaling)); //top corner

        return new Vertices(v);
    }

    public void CreateMapBody(Tile tile) {
        if (tile.FinalPrototype == null)
            return;

        var p = MapToWorld(tile.getX(), tile.getY());
        //TODO: make computation of l dependent on map_scaling only, find adequate formula
        var pn = MapToWorld(tile.getX(), tile.getY() + 1);
        var l = (p - pn).Length();
        switch ((tile.FinalPrototype.Sockets[0], tile.FinalPrototype.Sockets[1], tile.FinalPrototype.Sockets[2],
                    tile.FinalPrototype.Sockets[3])) {
            case (0, 0, 0, 0): //no walls at all
                break;
            case (3, 3, 3, 3): //fully walls tile

                tile.Body = _world.CreateRectangle(l, l, 0, p + new Vector2(MapScaling, 0), (float)Math.PI / 4);
                break;
            case (2, 2, 3, 0): //topleft straight wall
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 0.75f, -0.25f * MapScaling), (float)-Math.PI / 4);
                break;
            case (3, 0, 1, 1): //topright straight wall
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 1.25f, -0.25f * MapScaling), (float)Math.PI / 4);
                break;
            case (1, 1, 0, 3): //bottom right straight wall
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 1.25f, 0.25f * MapScaling), (float)-Math.PI / 4);
                break;
            case (0, 3, 2, 2): //bottom left wall
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 0.75f, 0.25f * MapScaling), (float)Math.PI / 4);
                break;
            case (0, 1, 0, 2): //bottom corner
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p + new Vector2(MapScaling, MapScaling));
                break;
            case (1, 0, 0, 1): //right corner
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p + new Vector2(MapScaling * 2, 0));
                break;
            case (2, 0, 1, 0): //top corner
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p + new Vector2(MapScaling, -MapScaling));
                break;
            case (0, 2, 2, 0): //left corner
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p);
                break;
            case (3, 2, 3, 1): //wall 3 open bot
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0));
                break;
            case (3, 1, 1, 3): //wall 3 open left
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0),
                    (float)Math.PI / 2);
                break;
            case (2, 3, 3, 2): //wall 3 open right
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0),
                    (float)-Math.PI / 2);
                break;
            case (1, 3, 2, 3): //wall 3 open top
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0),
                    (float)Math.PI);
                break;
            //cliffs
            case (8,8,8,8): //full cliff
                tile.Body = _world.CreateRectangle(l, l, 0, p + new Vector2(MapScaling, 0), (float)Math.PI / 4);
                break;
            case (8,6,8,7): //bottom NOT cliff
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0));
                break;
            case (7,8,6,8): //top NOT cliff
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0),
                    (float)Math.PI);
                break;
            case (8,7,7,8): //left NOT cliff
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0),
                    (float)Math.PI / 2);
                break;
            case (6,8,8,6): //right NOT cliff
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0),
                    (float)-Math.PI / 2);
                break;
            case (8,0,7,7): //top right cliff
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 1.25f, -0.25f * MapScaling), (float)Math.PI / 4);
                break;
            case (7,7,0,8): //bottom right cliff
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 1.25f, 0.25f * MapScaling), (float)-Math.PI / 4);
                break;
            case (6,6,8,0): //top left cliff
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 0.75f, -0.25f * MapScaling), (float)-Math.PI / 4);
                break;
            case (0,8,7,7): //bottom left cliff
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 0.75f, 0.25f * MapScaling), (float)Math.PI / 4);
                break;
            case (7,0,0,7): //right quarter cliff
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p + new Vector2(MapScaling * 2, 0));
                break;
            case (0,7,0,6): //bottom quarter cliff
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p + new Vector2(MapScaling, MapScaling));
                break;
            case (0,6,6,0): //left quarter cliff
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p);
                break;
            case (6,0,7,0): //top quarter cliff
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p + new Vector2(MapScaling, -MapScaling));
                break;
        }

        if (tile.Body != null) tile.Body.Tag = tile;
    }

    public void Initialize() {
        var mapGenerator = new MapGenerator(_game);

        Debug.WriteLine("Initializing Map");


        //mapGenerator.hardcodedMap();
        mapGenerator.createProceduralMap(100, 60);

        RoomList = mapGenerator.RoomList;


        transferDataToManagers();

        //create bodies for tiles
        foreach (var r in RoomList)
        foreach (var t in r.TileMap)
            CreateMapBody(t);
    }

    public void transferDataToManagers() {
        //TODO: move column textures loading to adequate location? 
        //TODO: replace column_lower by column_base once the sprite is available AND modify Column.Draw() accordingly
        var columnLower = _game.Content.Load<Texture2D>("Sprites/Columns/column_lower");
        var columnUpper = _game.Content.Load<Texture2D>("Sprites/Columns/column");
        var columnTexture = new ColumnTextures(columnLower, columnUpper);
        var elecLower = _game.Content.Load<Texture2D>("Sprites/Columns/lightning_column_lower");
        var elecUpper = _game.Content.Load<Texture2D>("Sprites/Columns/lightning_column");
        var elecTexture = new ColumnTextures(elecLower, elecUpper);

        foreach (var r in RoomList) {
            Debug.WriteLine("Sending coords to ColumnsManager: " + r.Columns.Count);
            var typeCtr = 0;
            foreach (var v in r.Columns) {
                var worldCoords = MapToWorld(v.X + r.PosX, v.Y + r.PosY);
                switch (typeCtr % 8) {
                    case 2:
                        Cm.Add(new FragileColumn(_game, _world, worldCoords, 0.4f, _game.ColumnTexture));
                        break;
                    case 3:
                        Cm.Add(new ElectricColumn(_game, _world, worldCoords, 0.4f, elecTexture));
                        break;
                    default:
                        Cm.Add(new Column(_game, _world, worldCoords, 0.4f, columnTexture));
                        break;
                }

                typeCtr++;
            }

            foreach (var v in r.EnemyPositions)
                Em.AddEnemy(MapToWorld(v.X + r.PosX, v.Y + r.PosY), RnGsus.Instance.Next(3) + 1);
        }
    }

    public void LoadContent() {
        
    }

    public override void Update(GameTime gameTime) {
        // Do nothing
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        var c = 0;

        // only draw planes that are visible on screen:
        var h = _game.Graphics.PreferredBackBufferHeight;
        var w = _game.Graphics.PreferredBackBufferWidth;

        var addonTiles = 4;

        var xMin = ScreenToMap(new Point(0 - addonTiles, 0 - addonTiles)).X;
        var xMax = ScreenToMap(new Point(h + addonTiles, w + addonTiles)).X;
        var yMin = ScreenToMap(new Point(w - addonTiles, 0 - addonTiles)).Y;
        var yMax = ScreenToMap(new Point(0 + addonTiles, h + addonTiles)).Y;

        Debug.WriteLine(xMin + ", " + xMax + ", " + yMin + ", " + yMax);

        foreach (var r in RoomList)
            //if (r.posX > xMax || r.posY > yMax || r.posX + r.sizeX < xMin || r.posY + r.sizeY < yMin)
            //continue;
        foreach (var t in r.TileMap) {
            //Only draw what is on the screen, NOT WORKING
            //if (t.X + r.PosX < xMin || t.X + r.PosX > xMax || t.Y + r.PosY < yMin || t.Y + r.PosY > yMax) continue;
            Point screenPos;
            Vector2 pos;

            //Rectangle tilePos = new Rectangle(screenPos.X + _game._graphics.PreferredBackBufferWidth / 2 - TileSize.X, screenPos.Y, TileSize.X, TileSize.Y);
            //batch.Draw(_ground, tilePos, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
            //batch.Draw(_ground, tilePos, Color.White);
            if (t.FinalPrototype != null) {
                //Shift the prototype up by one if it is a cliff
                if (t.FinalPrototype.IsCliff) {
                    screenPos = MapToScreen(new Point(t.X + 1 + r.PosX, t.Y + 1 + r.PosY));
                    pos = MapToWorld(new Point(t.X + 1 + r.PosX, t.Y + 1 + r.PosY));
                } else {
                    screenPos = MapToScreen(new Point(t.X + r.PosX, t.Y + r.PosY));
                    pos = MapToWorld(new Point(t.X + r.PosX, t.Y + r.PosY));
                }

                var tilePos = camera.getScreenRectangle(pos.X, pos.Y - MapScaling * 3f, 2 * MapScaling, 2 * MapScaling);
                c++;

                batch.Draw(t.FinalPrototype.Texture, tilePos, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None,
                    0f);
            }
        }
    }
}