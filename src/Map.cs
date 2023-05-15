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

    public int mapLevel = 1;

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
            _game._transitionScreen.gameLoaded = false;
            _game._transitionScreen.timer = 0;
            _game.ChangeState(RopeGame.State.Transition);

            return false;
        }

        return false;
    }

    public void CreateMapBody(Tile tile) {
        if (tile.FinalPrototype == null)
            return;

        // Prepare vertices for rock walls
        var v_tl_1_4 = new Vector2(0.375f * MapScaling * 2, -0.1875f * MapScaling * 4);
        var v_tl_2_4 = new Vector2(0.25f * MapScaling * 2, -0.125f * MapScaling * 4);
        var v_tr_1_4 = new Vector2(0.625f * MapScaling * 2, -0.1875f * MapScaling * 4);
        var v_tr_2_4 = new Vector2(0.75f * MapScaling * 2, -0.125f * MapScaling * 4);
        var v_bl_1_4 = new Vector2(0.125f * MapScaling * 2, 0.0625f * MapScaling * 4);
        var v_bl_2_4 = new Vector2(0.25f * MapScaling * 2, 0.125f * MapScaling * 4);
        var v_br_1_4 = new Vector2(0.875f * MapScaling * 2, 0.0625f * MapScaling * 4);
        var v_br_2_4 = new Vector2(0.75f * MapScaling * 2, 0.125f * MapScaling * 4);
        var v_c = new Vector2(0.5f * MapScaling * 2, 0);
        var v_ct = new Vector2(0.5f * MapScaling * 2, -0.125f * MapScaling * 4);
        var v_ctl = new Vector2(0.375f * MapScaling * 2, -0.0625f * MapScaling * 4);
        var v_ctr = new Vector2(0.625f * MapScaling * 2, -0.0625f * MapScaling * 4);

        var v_t = new Vector2(0.5f * MapScaling * 2, -0.25f * MapScaling * 4);
        var v_l = new Vector2(0, 0);
        var v_b = new Vector2(0.5f * MapScaling * 2, 0.25f * MapScaling * 4);
        var v_r = new Vector2(MapScaling * 2, 0);

        var p = MapToWorld(tile.getX(), tile.getY());
        //TODO: make computation of l dependent on map_scaling only, find adequate formula
        var pn = MapToWorld(tile.getX(), tile.getY() + 1);
        var l = (p - pn).Length();

        switch (tile.FinalPrototype.Name) {
            case "finish":
                tile.Body = _world.CreateRectangle(l * 0.5f, l * 0.5f, 0, p + new Vector2(MapScaling, 0),
                    (float)Math.PI / 4);
                tile.Body.OnCollision += OnFinishCollision;
                break;
            case "FullWall": //fully walls tile
                tile.Body = _world.CreateRectangle(l, l, 0, p + new Vector2(MapScaling, 0), (float)Math.PI / 4);
                break;
            case "Wall2l": //topleft straight wall
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 0.75f, -0.25f * MapScaling), (float)-Math.PI / 4);
                break;
            case "Wall2u": //topright straight wall
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 1.25f, -0.25f * MapScaling), (float)Math.PI / 4);
                break;
            case "StartL": //topright straight wall
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 1.25f, -0.25f * MapScaling), (float)Math.PI / 4);
                break;
            case "StartR": //topright straight wall
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 1.25f, -0.25f * MapScaling), (float)Math.PI / 4);
                break;
            case "Wall2r": //bottom right straight wall
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 1.25f, 0.25f * MapScaling), (float)-Math.PI / 4);
                break;
            case "Wall2d": //bottom left wall
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 0.75f, 0.25f * MapScaling), (float)Math.PI / 4);
                break;
            case "Wall1rd": //bottom corner
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p + new Vector2(MapScaling, MapScaling));
                break;
            case "Wall1ru": //right corner
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p + new Vector2(MapScaling * 2, 0));
                break;
            case "Wall1lu": //top corner
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p + new Vector2(MapScaling, -MapScaling));
                break;
            case "Wall1ld": //left corner
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p);
                break;
            case "Wall3dr": //wall 3 open bot
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0));
                break;
            case "Wall3dl": //wall 3 open left
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0),
                    (float)Math.PI / 2);
                break;
            case "Wall3ur": //wall 3 open right
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0),
                    (float)-Math.PI / 2);
                break;
            case "Wall3ul": //wall 3 open top
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0),
                    (float)Math.PI);
                break;
            // rock walls
            case "RockWall_ud":
                tile.Body = _world.CreatePolygon(new Vertices { v_bl_1_4, v_bl_2_4, v_tr_2_4, v_tr_1_4 }, 0, p);
                break;
            case "RockWall_lr":
                tile.Body = _world.CreatePolygon(new Vertices { v_tl_2_4, v_br_2_4, v_br_1_4, v_tl_1_4 }, 0, p);
                break;
            case "RockWall_ur":
                tile.Body = _world.CreatePolygon(new Vertices { v_ctl, v_br_2_4, v_br_1_4, v_ctr, v_tr_2_4, v_tr_1_4 },
                    0, p);
                break;
            case "RockWall_bl":
                tile.Body = _world.CreatePolygon(new Vertices { v_bl_1_4, v_bl_2_4, v_ctr, v_tl_1_4, v_tl_2_4, v_ctl },
                    0, p);
                break;
            case "RockWall_ul":
                tile.Body = _world.CreatePolygon(new Vertices { v_tl_2_4, v_c, v_tr_2_4, v_tr_1_4, v_ct, v_tl_1_4 }, 0,
                    p);
                break;
            case "RockWall_dr":
                tile.Body = _world.CreatePolygon(new Vertices { v_bl_1_4, v_bl_2_4, v_c, v_br_2_4, v_br_1_4, v_ct }, 0,
                    p);
                break;
            case "WallToRock_01":
                tile.Body = _world.CreatePolygon(
                    new Vertices { v_bl_2_4, v_c, v_br_2_4, v_br_1_4, v_ctr, v_tr_2_4, v_t, v_l }, 0, p);
                break;
            case "WallToRock_02":
                tile.Body = _world.CreatePolygon(
                    new Vertices { v_tl_2_4, v_ctl, v_bl_1_4, v_bl_2_4, v_c, v_br_2_4, v_r, v_t }, 0, p);
                break;
            case "WallToRock_03":
                tile.Body = _world.CreatePolygon(
                    new Vertices { v_tl_2_4, v_c, v_br_2_4, v_b, v_r, v_tr_2_4, v_ctr, v_tl_1_4 }, 0, p);
                break;
            case "WallToRock_04":
                tile.Body = _world.CreatePolygon(
                    new Vertices { v_l, v_b, v_br_2_4, v_c, v_tr_2_4, v_tr_1_4, v_ctl, v_tl_2_4 }, 0, p);
                break;
            // cliffs
            case "FullCliff": //full cliff
                tile.Body = _world.CreateRectangle(l, l, 0, p + new Vector2(MapScaling, 0), (float)Math.PI / 4);
                break;
            case "Cliff_1rd": //bottom NOT cliff
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0));
                break;
            case "Cliff_1ul": //top NOT cliff
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0),
                    (float)Math.PI);
                break;
            case "Cliff_1dl": //left NOT cliff
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0),
                    (float)Math.PI / 2);
                break;
            case "Cliff_1dr": //right NOT cliff
                tile.Body = _world.CreatePolygon(buildWall3Polygon(MapScaling), 0, p + new Vector2(MapScaling, 0),
                    (float)-Math.PI / 2);
                break;
            case "Cliff_2u": //top right cliff
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 1.25f, -0.25f * MapScaling), (float)Math.PI / 4);
                break;
            case "Cliff_2r": //bottom right cliff
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 1.25f, 0.25f * MapScaling), (float)-Math.PI / 4);
                break;
            case "Cliff_2l": //top left cliff
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 0.75f, -0.25f * MapScaling), (float)-Math.PI / 4);
                break;
            case "Cliff_2d": //bottom left cliff
                tile.Body = _world.CreateRectangle(l, 0.5f * l, 0,
                    p + new Vector2(MapScaling * 0.75f, 0.25f * MapScaling), (float)Math.PI / 4);
                break;
            case "Cliff_3ur": //right quarter cliff
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p + new Vector2(MapScaling * 2, 0));
                break;
            case "Cliff_3dr": //bottom quarter cliff
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p + new Vector2(MapScaling, MapScaling));
                break;
            case "Cliff_3dl": //left quarter cliff
                tile.Body = _world.CreateCircle(l * 0.5f, 0, p);
                break;
            case "Cliff_3ul": //top quarter cliff
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

    public void InitializeTutorialMap() {
        _mapGenerator = new MapGenerator(_game);

        Debug.WriteLine("Initializing Map");

        _mapGenerator.CreateTutorialMap();

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

            int j = 0;
            foreach (var v in r.Columns) {
                var worldCoords = MapToWorld(v.X + r.PosX, v.Y + r.PosY);

                double rand = RnGsus.Instance.NextDouble();
                int i = 0;

                while (i < r.columnWeight.Length && rand > r.columnWeight[i])
                    i++;

                if (r is TutorialRoom) {
                    i = r.columnTypes[j];
                }

                switch (i) {
                    case 1:
                        Cm.Add(new FragileColumn(_world, worldCoords, 1.5f, fragileTexture, brokenTexture,
                            fragileAnimationTexture));
                        break;
                    case 2:
                        Cm.Add(new ElectricColumn(_world, worldCoords, 1.5f, elecTexture, elecAnimationTexture));
                        break;
                    default:
                        Cm.Add(new Column(_world, worldCoords, 1.5f, columnTexture));
                        break;
                }
                j++;
            }

            for (int i = 0; i < r.EnemyPositions.Count; i++) {
                Em.AddEnemy(MapToWorld(r.EnemyPositions[i].X + r.PosX, r.EnemyPositions[i].Y + r.PosY),
                    _game.GameData.currentDifficulty); // pass map diff here as second arg
            }
            

            foreach (Vector2 pos in r.AmphoraPositions) {
                Amphora a = new Amphora(_game, _world, MapToWorld(pos.X + r.PosX, pos.Y + r.PosY), 0.5f);
                Dm.Add(a);
            }

            foreach (Vector2 pos in r.TreasurePositions) {
                Chest c;
                if (r is TutorialRoom) {
                    int[] loot = { 10, 0, 0 };
                    c = new SpearsChest(_game, _world, MapToWorld(pos.X + r.PosX, pos.Y + r.PosY), loot);
                } else if (RnGsus.Instance.NextDouble() > 0.5) {
                    if (RnGsus.Instance.NextDouble() > 0.7) {
                        c = new HealthChest(_game, _world, MapToWorld(pos.X + r.PosX, pos.Y + r.PosY), 1, 2);
                    } else {
                        c = new HealthChest(_game, _world, MapToWorld(pos.X + r.PosX, pos.Y + r.PosY), 0, 1);
                    }
                } else c = new SpearsChest(_game, _world, MapToWorld(pos.X + r.PosX, pos.Y + r.PosY));

                Dm.Add(c);
            }

            if (r is TutorialRoom) {
                TutorialRoom tr = (TutorialRoom) r;
                Em.SetTutorialMode();
                for (int i = 0; i < tr.textPos.Count; i++) {
                    SpriteFont font = _game.Content.Load<SpriteFont>("Fonts/tutorial_text");
                    BodyWithText text = new BodyWithText(_game, MapToWorld(tr.textPos[i].X + r.PosX, tr.textPos[i].Y + r.PosY), 3, _world, tr.texts[i], font);
                    Dm.Add(text);
                }
                for (int i = 0; i < tr.linePos.Count; i+=2) {
                    GuideLine line = new GuideLine(_game, MapToWorld(tr.linePos[i].X + r.PosX, tr.linePos[i].Y + r.PosY), 
                        MapToWorld(tr.linePos[i+1].X + r.PosX, tr.linePos[i+1].Y + r.PosY));
                    Dm.Add(line);
                }
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
        var h = _game.GraphicsDevice.PresentationParameters.BackBufferHeight;
        var w = _game.GraphicsDevice.PresentationParameters.BackBufferWidth;

        var addonTiles = 2;

        var xMin = Math.Floor(camera.getWorldPixel(new Vector2(0, 0)).X) - addonTiles;
        var xMax = Math.Ceiling(camera.getWorldPixel(new Vector2(w, h)).X) + addonTiles;
        var yMin = Math.Floor(camera.getWorldPixel(new Vector2(0, 0)).Y) - addonTiles;
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
                        if (t.FinalPrototype.Name == "Wall2l" || t.FinalPrototype.Name == "Wall2u" ||
                            t.FinalPrototype.Name == "Wall1lu" || t.FinalPrototype.Name == "StartL" ||
                            t.FinalPrototype.Name == "StartR") {
                            layerDepthWalls = camera.getLayerDepth(tilePos.Y + tilePos.Height * 0.625f);
                        } else if (t.FinalPrototype.Name == "Wall2r" || t.FinalPrototype.Name == "Wall2d" ||
                                   t.FinalPrototype.Name == "Wall1rd") {
                            layerDepthWalls = camera.getLayerDepth(tilePos.Y + tilePos.Height * 0.875f);
                        }

                        batch.Draw(t.FinalPrototype.WallTex, tilePos, null, Color.White, 0f, Vector2.Zero,
                            SpriteEffects.None, layerDepthWalls);
                    }

                    if (t.FinalPrototype.GroundTex != null) {
                        // We put cliffs below ground (between 0.025 and 0.075)
                        if (t.FinalPrototype.IsCliff) {
                            layerDepthFloor = camera.getLayerDepth(tilePos.Y) / 10f;
                        }

                        batch.Draw(t.FinalPrototype.GroundTex, tilePos, null, Color.White, 0f, Vector2.Zero,
                            SpriteEffects.None, layerDepthFloor);
                    }
                }
            }
        }
    }
}