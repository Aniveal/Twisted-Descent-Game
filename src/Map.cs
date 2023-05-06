using System;
using System.Collections.Generic;
using System.Diagnostics;
using Meridian2.Columns;
using Meridian2.Enemy;
using Meridian2.GameElements;
using Meridian2.Theseus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using static System.Net.Mime.MediaTypeNames;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using Meridian2.Screens;
using Meridian2.Treasures;

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

    public int mapDifficulty = 1;

    MapGenerator _mapGenerator;

    //WIP, remove this as soon as possible
    public ColumnsManager Cm;

    public EnemyManager Em;

    public DiverseManager Dm;

    //The list of rooms to draw
    public List<Room> RoomList = new();

    public Point TileSize = new(160, 160); // pixels

    public bool levelFinished = false;

    public Map(RopeGame game, World world, ColumnsManager cm, EnemyManager em, DiverseManager dm) {
        _game = game;
        _world = world;
        Cm = cm;
        Em = em;
        Dm = dm;
    }


    /* Helper Functions */
    // Isometric Math: https://clintbellanger.net/articles/isometric_math/
    // Note: our tiles have slightly different shape, (they're twice as high: take 1/4 of the height instead of 1/2)


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

    protected bool OnCliffCollision(Fixture sender, Fixture other, Contact contact) {
        if (sender.Body.Tag is RopeSegment || other.Body.Tag is RopeSegment) {
            return false;
        }

        return true;
    }

    protected bool OnFinishCollision(Fixture sender, Fixture other, Contact contact) {
        if (sender.Body.Tag is Player || other.Body.Tag is Player) {

            levelFinished = true;

            _game._gameScreen.LoadNextLevel();
            
            return false;
        }
        return false;
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
                if (tile.FinalPrototype.Name == "finish") {
                    tile.Body = _world.CreateRectangle(l*0.5f, l*0.5f, 0, p + new Vector2(MapScaling, 0), (float)Math.PI / 4);
                    tile.Body.OnCollision += OnFinishCollision;
                }
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
            case (8, 8, 8, 8): //full cliff
                tile.Body = _world.CreateRectangle(l, l, 0, p + new Vector2(MapScaling, 0), (float)Math.PI / 4);
                break;
            case (8, 6, 8, 7): //bottom NOT cliff
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0));
                break;
            case (7, 8, 6, 8): //top NOT cliff
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0),
                    (float)Math.PI);
                break;
            case (8, 7, 7, 8): //left NOT cliff
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0),
                    (float)Math.PI / 2);
                break;
            case (6, 8, 8, 6): //right NOT cliff
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0),
                    (float)-Math.PI / 2);
                break;
            case (8, 0, 7, 7): //top right cliff
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 1.25f, -0.25f * MapScaling), (float)Math.PI / 4);
                break;
            case (7, 7, 0, 8): //bottom right cliff
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 1.25f, 0.25f * MapScaling), (float)-Math.PI / 4);
                break;
            case (6, 6, 8, 0): //top left cliff
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 0.75f, -0.25f * MapScaling), (float)-Math.PI / 4);
                break;
            case (0, 8, 7, 7): //bottom left cliff
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 0.75f, 0.25f * MapScaling), (float)Math.PI / 4);
                break;
            case (7, 0, 0, 7): //right quarter cliff
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p + new Vector2(MapScaling * 2, 0));
                break;
            case (0, 7, 0, 6): //bottom quarter cliff
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p + new Vector2(MapScaling, MapScaling));
                break;
            case (0, 6, 6, 0): //left quarter cliff
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p);
                break;
            case (6, 0, 7, 0): //top quarter cliff
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p + new Vector2(MapScaling, -MapScaling));
                break;
        }

        if (tile.Body != null) {
            tile.Body.Tag = tile;
            if (tile.FinalPrototype.IsCliff) {
                tile.Body.OnCollision += OnCliffCollision;
            }
        } 
    }


    public void Initialize() {
        _mapGenerator = new MapGenerator(_game);

        Debug.WriteLine("Initializing Map");

        _mapGenerator.createProceduralMap(_game.GameData.currentDifficulty);

        RoomList = _mapGenerator.RoomList;

        transferDataToManagers();

        //create bodies for tiles
        foreach (var r in RoomList)
        foreach (var t in r.TileMap)
            CreateMapBody(t);
    }

    public void transferDataToManagers() {
        var columnTexture = _game.Content.Load<Texture2D>("Sprites/Columns/column");
        var elecTexture = _game.Content.Load<Texture2D>("Sprites/Columns/lightning_column");
        var elecAnimationTexture = _game.Content.Load<Texture2D>("Sprites/Columns/lightning_column_animation");
        var fragileTexture = _game.Content.Load<Texture2D>("Sprites/Columns/fragile_column");
        var fragileAnimationTexture = _game.Content.Load<Texture2D>("Sprites/Columns/collapse_column");
        var brokenTexture = _game.Content.Load<Texture2D>("Sprites/Columns/fragile_column_broken");

        foreach (var r in RoomList) {
            Debug.WriteLine("Sending coords to ColumnsManager: " + r.Columns.Count);

            foreach (var v in r.Columns) {
                var worldCoords = MapToWorld(v.X + r.PosX, v.Y + r.PosY);

                double rand = RnGsus.Instance.NextDouble();
                int i = 0;

                while (i < r.columnWeight.Length && rand > r.columnWeight[i])
                    i++;

                switch (i) {
                    case 1:
                        Cm.Add(new FragileColumn(_world, worldCoords, 1.5f, fragileTexture, brokenTexture, fragileAnimationTexture));
                        break;
                    case 2:
                        Cm.Add(new ElectricColumn(_world, worldCoords, 1.5f, elecTexture, elecAnimationTexture));
                        break;
                    default:
                        Cm.Add(new Column(_world, worldCoords, 1.5f, columnTexture));
                        break;
                }

            }

            for (int i = 0; i < r.EnemyPositions.Count; i++)
            {
                Em.AddEnemy(MapToWorld(r.EnemyPositions[i].X + r.PosX, r.EnemyPositions[i].Y + r.PosY), RnGsus.Instance.Next(3) + 1); // pass map diff here as second arg
            }

            foreach(Vector2 pos in r.AmphoraPositions)
            {
                Amphora a = new Amphora(_game, _world, MapToWorld(pos.X + r.PosX, pos.Y + r.PosY), 0.5f);
                Dm.Add(a);
            }

            foreach (Vector2 pos in r.TreasurePositions)
            {
                
                Chest c;
                if(RnGsus.Instance.NextDouble() > 0.5)
                {
                    c = new HealthChest(_game, _world, MapToWorld(pos.X + r.PosX, pos.Y + r.PosY));
                }
                else c = new SpearsChest(_game, _world, MapToWorld(pos.X + r.PosX, pos.Y + r.PosY));
                
                Dm.Add(c);
            }
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

        var addonTiles = 2; 

        var xMin = Math.Floor(camera.getWorldPixel(new Vector2(0, 0)).X) - addonTiles;
        var xMax = Math.Ceiling(camera.getWorldPixel(new Vector2(w, h)).X) + addonTiles;
        var yMin = Math.Floor(camera.getWorldPixel(new Vector2(0,0)).Y) - addonTiles;
        var yMax = Math.Ceiling(camera.getWorldPixel(new Vector2(w, h)).Y) + addonTiles;

        //Debug.WriteLine(xMin + ", " + xMax + ", " + yMin + ", " + yMax);

        foreach (var r in RoomList) {
            //if (r.PosX > xMax || r.PosY > yMax || r.PosX + r.SizeX < xMin || r.PosY + r.SizeY < yMin)
                //continue;
            foreach (var t in r.TileMap) {
                //Only draw what is on the screen, NOT WORKING
                //if (t.X < xMin || t.X > xMax || t.Y < yMin || t.Y > yMax)
                    //continue;

                Vector2 screenPos;
                Vector2 pos;

                //Rectangle tilePos = new Rectangle(screenPos.X + _game._graphics.PreferredBackBufferWidth / 2 - TileSize.X, screenPos.Y, TileSize.X, TileSize.Y);
                //batch.Draw(_ground, tilePos, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
                //batch.Draw(_ground, tilePos, Color.White);
                if (t.FinalPrototype != null) {
                    //Shift the prototype up by one if it is a cliff
                    if (t.FinalPrototype.IsCliff) {
                        pos = MapToWorld(new Point(t.X + 1 + r.PosX, t.Y + 1 + r.PosY));
                        screenPos = camera.getScreenPoint(new Vector2(pos.X, pos.Y));
                    } else {
                        pos = MapToWorld(new Point(t.X + r.PosX, t.Y + r.PosY));
                        screenPos = camera.getScreenPoint(new Vector2(pos.X, pos.Y));
                    }
                    if (!camera.IsVisible(pos)) {
                        continue;
                    }

                    float layerDepthWalls = camera.getLayerDepth(screenPos.Y);
                    float layerDepthFloor = 0.24f;

                    var tilePos =
                        camera.getScreenRectangle(pos.X, pos.Y - MapScaling * 3f, 2 * MapScaling, 2 * MapScaling);
                    c++;

                    if (t.FinalPrototype.WallTex != null) {
                        if (t.FinalPrototype.Name == "Wall2l" || t.FinalPrototype.Name == "Wall2u" || t.FinalPrototype.Name == "Wall1lu") {
                            layerDepthWalls = camera.getLayerDepth(tilePos.Y + tilePos.Height * 0.625f);
                        } else if (t.FinalPrototype.Name == "Wall2r" || t.FinalPrototype.Name == "Wall2d" || t.FinalPrototype.Name == "Wall1rd") {
                            layerDepthWalls = camera.getLayerDepth(tilePos.Y + tilePos.Height * 0.875f);
                        }

                        batch.Draw(t.FinalPrototype.WallTex, tilePos, null, Color.White, 0f, Vector2.Zero,
                            SpriteEffects.None, layerDepthWalls);
                    }

                    if (t.FinalPrototype.GroundTex != null) {
                        // We put cliffs below ground (between 0.025 and 0.075)
                        if (t.FinalPrototype.IsCliff) {
                            layerDepthFloor = camera.getLayerDepth(tilePos.Y)/10f;
                        }

                        batch.Draw(t.FinalPrototype.GroundTex, tilePos, null, Color.White, 0f, Vector2.Zero,
                            SpriteEffects.None, layerDepthFloor);
                    }
                }
            }
        }
    }
}