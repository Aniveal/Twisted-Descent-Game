using System;
using System.Collections.Generic;
using System.Diagnostics;
using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Common;
using Meridian2.Columns;
using Meridian2.Enemy;

namespace Meridian2 {
    public class Map : DrawableGameElement {
        private readonly RopeGame _game;
        private World _world;
        private Texture2D _ground;
        private List<Texture2D> _column;
        private List<Texture2D> _rockTextures;


        private const float map_scaling = 2;
        private const int map_translation = -4;
        private float wallWidth = (float) Math.Sqrt(map_scaling*2);

        public Point TileSize = new(160, 160); // pixels

        //The list of rooms to draw
        public List<Room> roomList = new List<Room>();

        //WIP, remove this as soon as possible
        public ColumnsManager cm;

        public EnemyManager em;


        /* Helper Functions */
        // Isometric Math: https://clintbellanger.net/articles/isometric_math/
        // Note: our tiles have slightly different shape, (they're twice as high: take 1/4 of the height instead of 1/2)

        // MapToScreen: take index of a tile as an input (e.g. (2,1)) returns pixel position.
        public Point MapToScreen(Point map_coordinates) {
            int halfTile = TileSize.X / 2;
            int quaterTile = TileSize.X / 4;

            var screenX = (int)((map_coordinates.X - map_coordinates.Y) * halfTile);
            var screenY = (int)((map_coordinates.X + map_coordinates.Y) * quaterTile);
            return new(screenX + (int)Globals.CameraPosition.X, screenY + (int)Globals.CameraPosition.Y);
        }

        //Returns the world coordinates of the left angle of the ground level of the tile
        public Vector2 MapToWorld(Point map_coordinates) {
            return new Vector2((map_coordinates.X -  map_coordinates.Y+map_translation)*map_scaling, 
                (map_coordinates.X + map_coordinates.Y+map_translation)*map_scaling);
        }

        //Returns the world coordinates of the left angle of the ground level of the tile
        public Vector2 MapToWorld(int x, int y) {
            return new Vector2((x -  y +map_translation)*map_scaling, (x + y +map_translation)*map_scaling);
        }

        public Vector2 MapToWorld(float x, float y) {
            return new Vector2((x -  y +map_translation)*map_scaling, (x + y +map_translation)*map_scaling);
        }

        // ScreenToMap: takes pixel position, returns the index of the tile at this position.
        public Point ScreenToMap(Point screenPos) {
            screenPos.X -= (int)Globals.CameraPosition.X;
            screenPos.Y -= (int)Globals.CameraPosition.Y;

            int halfTile = TileSize.X / 2;
            int quaterTile = TileSize.X / 4;
            int mapX = (int)((screenPos.X / halfTile + screenPos.Y / quaterTile) / 2);
            int mapY = (int)((screenPos.Y / quaterTile - screenPos.X / halfTile) / 2);
            return new(mapX, mapY);
        }

        //building a polygon of the shape of wall_3f
        private Vertices buildWall3Polygon(float scaling) {
            List<Vector2> v = new List<Vector2>();
            v.Add(new Vector2(-scaling, 0)); //left corner
            v.Add(new Vector2(-scaling*0.5f, scaling*0.5f)); //left end of hole
            v.Add(new Vector2(-scaling*0.25f, scaling*0.25f));
            v.Add(new Vector2(scaling*0.25f, scaling*0.25f));
            v.Add(new Vector2(scaling*0.5f, scaling*0.5f)); //right end of hole
            v.Add(new Vector2(scaling, 0)); //right corner
            v.Add(new Vector2(0, -scaling)); //top corner

            return new Vertices(v);
        }

        public void CreateMapBody(Tile tile) {
            if (tile.finalPrototype == null)
                return;

            Vector2 p = MapToWorld(tile.getX(), tile.getY());
            //TODO: make computation of l dependent on map_scaling only, find adequate formula
            Vector2 pn = MapToWorld(tile.getX(), tile.getY() + 1);
            float l = (p-pn).Length();
            switch ((tile.finalPrototype.sockets[0], tile.finalPrototype.sockets[1], tile.finalPrototype.sockets[2], tile.finalPrototype.sockets[3])) {
                case (0, 0, 0, 0): //no walls at all
                    break;
                case (3,3,3,3): //fully walls tile
                    
                    tile.body = _world.CreateRectangle(l, l, 0, p + new Vector2(map_scaling, 0), (float) Math.PI/4);
                    break;
                case(2,2,3,0): //topleft straight wall
                    tile.body = _world.CreateRectangle(l, 0.5f*l, 0, p + new Vector2(map_scaling*0.75f, -0.25f*map_scaling), (float) -Math.PI/4);
                    break;
                case(3,0,1,1): //topright straight wall
                    tile.body = _world.CreateRectangle(l, 0.5f*l, 0, p + new Vector2(map_scaling*1.25f, -0.25f*map_scaling), (float) Math.PI/4);
                    break;
                case(1,1,0,3): //bottom right straight wall
                    tile.body = _world.CreateRectangle(l, 0.5f*l, 0, p + new Vector2(map_scaling*1.25f, 0.25f*map_scaling), (float) -Math.PI/4);
                    break;
                case(0,3,2,2): //bottom left wall
                    tile.body = _world.CreateRectangle(l, 0.5f*l, 0, p + new Vector2(map_scaling*0.75f, 0.25f*map_scaling), (float) Math.PI/4);
                    break;
                case(0,1,0,2): //bottom corner
                    tile.body = _world.CreateCircle(l*0.5f, 0, p + new Vector2(map_scaling, map_scaling));
                    break;
                case(1,0,0,1): //right corner
                    tile.body = _world.CreateCircle(l*0.5f, 0, p + new Vector2(map_scaling*2, 0));
                    break;
                case(2,0,1,0): //top corner
                    tile.body = _world.CreateCircle(l*0.5f, 0, p + new Vector2(map_scaling, -map_scaling));
                    break;
                case(0,2,2,0): //left corner
                    tile.body = _world.CreateCircle(l*0.5f, 0, p);
                    break;
                case(3,2,3,1): //wall 3 open bot
                    tile.body = _world.CreatePolygon(buildWall3Polygon(map_scaling), 0, p + new Vector2(map_scaling, 0));
                    break;
                case(3,1,1,3): //wall 3 open left
                    tile.body = _world.CreatePolygon(buildWall3Polygon(map_scaling), 0, p + new Vector2(map_scaling, 0), (float) Math.PI/2);
                    break;
                case(2,3,3,2): //wall 3 open right
                    tile.body = _world.CreatePolygon(buildWall3Polygon(map_scaling), 0, p + new Vector2(map_scaling, 0), (float) -Math.PI/2);
                    break;
                case(1,3,2,3): //wall 3 open top
                    tile.body = _world.CreatePolygon(buildWall3Polygon(map_scaling), 0, p + new Vector2(map_scaling, 0), (float) Math.PI);
                    break;
                default:
                    break;
            }
        }

        public void Initialize() 
        {
            MapGenerator mapGenerator = new MapGenerator(_game);

            Debug.WriteLine("Initializing Map");


            mapGenerator.hardcodedMap();

            roomList = mapGenerator.roomList;

            transferDataToManagers();

            //create bodies for tiles
            foreach (Room r in roomList)
                foreach (Tile t in r.tileMap)
                {
                    CreateMapBody(t);
                }

        }

        public void transferDataToManagers()
        {
            //TODO: move column textures loading to adequate location? 
            Texture2D columnLower = _game.Content.Load<Texture2D>("Sprites/Columns/column_lower");
            Texture2D columnUpper = _game.Content.Load<Texture2D>("Sprites/Columns/column_upper");
            ColumnTextures columnTexture = new ColumnTextures(columnLower, columnUpper);
            Texture2D elecLower = _game.Content.Load<Texture2D>("Sprites/Columns/lightning_column_lower");
            Texture2D elecUpper = _game.Content.Load<Texture2D>("Sprites/Columns/lightning_column_upper");
            ColumnTextures elecTexture = new ColumnTextures(elecLower, elecUpper);

            foreach(Room r in roomList)
            {
                Debug.WriteLine("Sending coords to ColumnsManager: " + r.columns.Count);
                int typeCtr = 0;
                foreach (Vector2 v in r.columns)
                { 
                    Vector2 worldCoords = MapToWorld(v.X + r.posX, v.Y + r.posY);
                    switch(typeCtr % 8) {
                        case(2):
                            cm.Add(new FragileColumn(_game, _world, worldCoords, 0.4f, _game.ColumnTexture));
                            break;
                        case(3):
                            cm.Add(new ElectricColumn(_game, _world, worldCoords, 0.4f, elecTexture));
                            break;
                        default:
                            cm.Add(new Column(_game, _world, worldCoords, 0.4f, columnTexture));
                            break;
                    }
                    typeCtr++;                    
                }

                foreach(Vector2 v in r.enemyPositions)
                {

                    em.AddEnemy(MapToWorld(v.X + r.posX, v.Y + r.posY), RNGsus.Instance.Next(3) + 1);
                }
                
            }
        }

        public void LoadContent() {
            _ground = _game.Content.Load<Texture2D>("ground");

            _column = new List<Texture2D> {
                _game.Content.Load<Texture2D>("column"),
                _game.Content.Load<Texture2D>("column_lower"),
                _game.Content.Load<Texture2D>("column_upper")
            };

            
            _rockTextures = new List<Texture2D> {
                _game.Content.Load<Texture2D>("Sprites/Rock/wall_1b"), // 1
                _game.Content.Load<Texture2D>("Sprites/Rock/wall_1r"), // 2
                _game.Content.Load<Texture2D>("Sprites/Rock/wall_1f"), // 3 
                _game.Content.Load<Texture2D>("Sprites/Rock/wall_1l"), // 4
                _game.Content.Load<Texture2D>("Sprites/Rock/wall_2lf"), // 5
                _game.Content.Load<Texture2D>("Sprites/Rock/wall_2rf"), // 6 
                _game.Content.Load<Texture2D>("Sprites/Rock/wall_2rb"), // 7
                _game.Content.Load<Texture2D>("Sprites/Rock/wall_2lb"), // 8
                _game.Content.Load<Texture2D>("Sprites/Rock/wall_3b"), // 9
                _game.Content.Load<Texture2D>("Sprites/Rock/wall_3r"), // 10
                _game.Content.Load<Texture2D>("Sprites/Rock/wall_3f"), // 11
                _game.Content.Load<Texture2D>("Sprites/Rock/wall_3l"), // 12
                _game.Content.Load<Texture2D>("Sprites/Rock/wall_4") // 13
            };
        }

        public override void Update(GameTime gameTime) {
            // Do nothing
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {

            int c = 0;

            // only draw planes that are visible on screen:
            int h = _game._graphics.PreferredBackBufferHeight;
            int w = _game._graphics.PreferredBackBufferWidth;

            int addonTiles = 4;

            int xMin = ScreenToMap(new(0, 0)).X - addonTiles;
            int xMax = ScreenToMap(new(h, w)).X + addonTiles;
            int yMin = ScreenToMap(new(w, 0)).Y - addonTiles;
            int yMax = ScreenToMap(new(0, h)).Y + addonTiles;

            /*
            xMin = (xMin < 0) ? 0 : xMin;
            xMax = (xMax >= TileMap.GetLength(0)) ? TileMap.GetLength(0) - 1 : xMax;
            yMin = (yMin < 0) ? 0 : yMin;
            yMax = (yMax >= TileMap.GetLength(1)) ? TileMap.GetLength(0) - 1 : yMax;
            */

            foreach(Room r in roomList)
            {
                //if (r.posX > xMax || r.posY > yMax || r.posX + r.sizeX < xMin || r.posY + r.sizeY < yMin)
                    //continue;

                foreach (Tile t in r.tileMap)
                {

                    //Only draw what is on the screen, NOT WORKING
                    //if (t.x < xMin || t.x > xMax || t.y < yMin || t.y > yMax) continue;

                    Point screenPos = MapToScreen(new(t.x + r.posX, t.y + r.posY));
                    Vector2 pos = MapToWorld(new(t.x + r.posX, t.y + r.posY));
                    //Rectangle tilePos = new Rectangle(screenPos.X + _game._graphics.PreferredBackBufferWidth / 2 - TileSize.X, screenPos.Y, TileSize.X, TileSize.Y);
                    Rectangle tilePos = camera.getScreenRectangle(pos.X, pos.Y - map_scaling * 3f, 2 * map_scaling, 2 * map_scaling);
                    //batch.Draw(_ground, tilePos, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
                    batch.Draw(_ground, tilePos, Color.White);

                    if (t.finalPrototype != null)
                    {
                        c++;
                        batch.Draw(t.finalPrototype.texture, tilePos, Color.White);
                    }
                }
                    
            }
            
            
        }

        public Map(RopeGame game, World world, ColumnsManager cm, EnemyManager em) {
            _game = game;
            _world = world;
            this.cm = cm;
            this.em = em;

        }
    }
}