﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2 {
    public class Map : IGameObject {
        private Texture2D _ground;
        private List<Texture2D> _column;
        private List<Texture2D> _rockTextures;

        public Point TileSize = new(160, 160); // pixels
        public int[,] TileType = new int[100, 100]; // contains type of each tile: 0 for empty_tile, 1 for rock_tile

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

        public void Initialize() {
            for (int x = 0; x < TileType.GetLength(0); x++) {
                for (int y = 0; y < TileType.GetLength(1); y++) {
                    TileType[x, y] = 0;

                    // Walls
                    if (x == 0) {
                        TileType[x, y] = 5;
                    }
                    
                    if (x == TileType.GetLength(0) - 1) {
                        TileType[x, y] = 7;
                    }
                    
                    if (y == 0) {
                        TileType[x, y] = 6;
                    }

                    if (y == TileType.GetLength(1) - 1) {
                        TileType[x, y] = 8;
                    }
                }
            }
            
            // Corners
            TileType[0, 0] = 11;
            TileType[TileType.GetLength(0) - 1, 0] = 10;
            TileType[0, TileType.GetLength(1) - 1] = 12;
            TileType[TileType.GetLength(0) - 1, TileType.GetLength(1) - 1] = 9;
        }

        public void LoadContent() {
            _ground = Globals.Content.Load<Texture2D>("ground");

            _column = new List<Texture2D> {
                Globals.Content.Load<Texture2D>("column"),
                Globals.Content.Load<Texture2D>("column_lower"),
                Globals.Content.Load<Texture2D>("column_upper")
            };

            _rockTextures = new List<Texture2D> {
                Globals.Content.Load<Texture2D>("wall_1b"), // 1
                Globals.Content.Load<Texture2D>("wall_1r"), // 2
                Globals.Content.Load<Texture2D>("wall_1f"), // 3 
                Globals.Content.Load<Texture2D>("wall_1l"), // 4
                Globals.Content.Load<Texture2D>("wall_2lf"), // 5
                Globals.Content.Load<Texture2D>("wall_2rf"), // 6 
                Globals.Content.Load<Texture2D>("wall_2rb"), // 7
                Globals.Content.Load<Texture2D>("wall_2lb"), // 8
                Globals.Content.Load<Texture2D>("wall_3b"), // 9
                Globals.Content.Load<Texture2D>("wall_3r"), // 10
                Globals.Content.Load<Texture2D>("wall_3f"), // 11
                Globals.Content.Load<Texture2D>("wall_3l"), // 12
                Globals.Content.Load<Texture2D>("wall_4") // 13
            };
        }

        public void Update(GameTime gameTime) {
            // Do nothing
        }

        public void Draw(GameTime gameTime, SpriteBatch batch) {
            // Point a = MapToScreen(new(10, 10));  // Debug code for ScreenToMap, MapToScreen
            // Point b = ScreenToMap(a);
            // Debug.WriteLine(a + " -- " + b);

            // only draw planes that are visible on screen:
            int h = Globals.Graphics.PreferredBackBufferHeight;
            int w = Globals.Graphics.PreferredBackBufferWidth;

            int addonTiles = 2;

            int xMin = ScreenToMap(new(0, 0)).X - addonTiles;
            int xMax = ScreenToMap(new(h, w)).X + addonTiles;
            int yMin = ScreenToMap(new(w, 0)).Y - addonTiles;
            int yMax = ScreenToMap(new(0, h)).Y + addonTiles;

            xMin = (xMin < 0) ? 0 : xMin;
            xMax = (xMax >= TileType.GetLength(0)) ? TileType.GetLength(0) - 1 : xMax;
            yMin = (yMin < 0) ? 0 : yMin;
            yMax = (yMax >= TileType.GetLength(1)) ? TileType.GetLength(0) - 1 : yMax;

            for (int x = xMin; x < xMax; x++) {
                for (int y = yMin; y < yMax; y++) {
                    Point screenPos = MapToScreen(new(x, y));
                    Rectangle tilePos = new Rectangle(screenPos.X, screenPos.Y, TileSize.X, TileSize.Y);

                    batch.Draw(_ground, tilePos, null, Color.White, 0.0f, Vector2.Zero,
                        SpriteEffects.None, 0.9f);

                    int tileType = TileType[x, y];
                    if (tileType > 0) {
                        batch.Draw(_rockTextures[tileType - 1], tilePos, Color.White);
                    } else if (tileType < 0) {
                        batch.Draw(_column[(-1 * tileType) - 1], tilePos, Color.White);
                    }
                }
            }
        }
    }
}