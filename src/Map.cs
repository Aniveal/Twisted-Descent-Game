using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2 {
    public class Map : IGameObject {
        private Texture2D _emptyTile;
        private Texture2D _rock;

        public Point TileSize = new(80, 80); // pixels
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
            Random rng = new Random();
            for (int x = 0; x < TileType.GetLength(0); x++) {
                for (int y = 0; y < TileType.GetLength(1); y++) {
                    float type = rng.Next(0, 20); // 1 out of 20 tiles should contain a rock
                    TileType[x, y] = (type == 1) ? 1 : 0; // if neq 1, set it to zero (empty tile)
                }
            }
        }
        
        public void LoadContent() {
            _emptyTile = Globals.Content.Load<Texture2D>("empty_tile");
            _rock = Globals.Content.Load<Texture2D>("rock");
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
                    //_spriteBatch.Draw(empty_tile, tile_pos, null, c, 0.0f, Vector2.Zero, SpriteEffects.None, 2);

                    batch.Draw(_emptyTile, tilePos, null, Color.White, 0.0f, Vector2.Zero,
                        SpriteEffects.None, 0.9f);


                    if (TileType[x, y] == 1) {
                        Rectangle rockPos = new Rectangle(screenPos.X, screenPos.Y - TileSize.Y / 2, TileSize.X,
                            TileSize.Y);

                        batch.Draw(_rock, rockPos, null, Color.White, 0.0f, Vector2.Zero,
                            SpriteEffects.None, 0.1f);
                    }
                }
            }
        }
    }
}