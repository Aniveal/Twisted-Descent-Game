﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

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
        public Tile[,] TileMap; // contains a prototype object for each tile coordinate

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

        public void CreateMapBody(Tile tile) {
            if (tile.finalPrototype == null)
                return;

            Vector2 p = MapToWorld(tile.x, tile.y);
            //TODO: make computation of l dependent on map_scaling only, find adequate formula
            Vector2 pn = MapToWorld(tile.x, tile.y + 1);
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
                default:
                    break;
            }
        }

        public void Initialize() 
        {
            MapGenerator mapGenerator = new MapGenerator(_game);

            Debug.WriteLine("Initializing Map");

            int mapX = 50;
            int mapY = 50;

            List<Vector3> openings = new List<Vector3>{
                new Vector3(0, 25, 4),
                new Vector3(25, 0, 2),
            new Vector3(mapY - 1, 20, 3)};

            //The new Map generation
            TileMap = mapGenerator.createRoom(mapX, mapY, openings).tileMap;

            Debug.WriteLine(TileMap);

            //create bodies for tiles
            foreach (Tile t in TileMap) {
                CreateMapBody(t);
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
            // Point a = MapToScreen(new(10, 10));  // Debug code for ScreenToMap, MapToScreen
            // Point b = ScreenToMap(a);
            // Debug.WriteLine(a + " -- " + b);

            int c = 0;

            // only draw planes that are visible on screen:
            int h = _game._graphics.PreferredBackBufferHeight;
            int w = _game._graphics.PreferredBackBufferWidth;

            int addonTiles = 4;

            int xMin = ScreenToMap(new(0, 0)).X - addonTiles;
            int xMax = ScreenToMap(new(h, w)).X + addonTiles;
            int yMin = ScreenToMap(new(w, 0)).Y - addonTiles;
            int yMax = ScreenToMap(new(0, h)).Y + addonTiles;

            xMin = (xMin < 0) ? 0 : xMin;
            xMax = (xMax >= TileMap.GetLength(0)) ? TileMap.GetLength(0) - 1 : xMax;
            yMin = (yMin < 0) ? 0 : yMin;
            yMax = (yMax >= TileMap.GetLength(1)) ? TileMap.GetLength(0) - 1 : yMax;


            
            foreach (Tile t in TileMap)
            {
                Point screenPos = MapToScreen(new(t.x, t.y));
                Vector2 pos = MapToWorld(new(t.x, t.y));
                //Rectangle tilePos = new Rectangle(screenPos.X + _game._graphics.PreferredBackBufferWidth / 2 - TileSize.X, screenPos.Y, TileSize.X, TileSize.Y);
                Rectangle tilePos = camera.getScreenRectangle(pos.X, pos.Y-map_scaling*3f, 2*map_scaling, 2*map_scaling);
                //batch.Draw(_ground, tilePos, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
                batch.Draw(_ground, tilePos, Color.White);

                if (t.finalPrototype != null)
                {
                    c++;
                    batch.Draw(t.finalPrototype.texture, tilePos, Color.White);
                }
            }
        }

        public Map(RopeGame game, World world) {
            _game = game;
            _world = world;
        }
    }
}