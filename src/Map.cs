using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Meridian2

{
	public class Map
	{
        private Texture2D empty_tile;
        private Texture2D rock;

        public Point TILE_SIZE = new(80, 80); // pixels
        public int[,] TILE_TYPE = new int[100, 100]; // contains type of each tile: 0 for empty_tile, 1 for rock_tile

        /* Initialization */
        public Map()
		{
            empty_tile = Globals.Content.Load<Texture2D>("empty_tile");
            rock = Globals.Content.Load<Texture2D>("rock");


            Random rng = new Random();
            for (int x = 0; x < TILE_TYPE.GetLength(0); x++)
            {
                for (int y = 0; y < TILE_TYPE.GetLength(1); y++)
                {
                    float type = rng.Next(0, 20); // 1 out of 20 tiles should contain a rock
                    TILE_TYPE[x, y] = (type == 1) ? 1 : 0; // if neq 1, set it to zero (empty tile)
                }
            }
        }

        /* Draw */
        public void Draw()
        {

            // Point a = MapToScreen(new(10, 10));  // Debug code for ScreenToMap, MapToScreen
            // Point b = ScreenToMap(a);
            // Debug.WriteLine(a + " -- " + b);

            // only draw planes that are visible on screen:
            int h = Globals.graphics.PreferredBackBufferHeight;
            int w = Globals.graphics.PreferredBackBufferWidth;

            int addon_tiles = 2;

            int x_min = ScreenToMap(new(0, 0)).X - addon_tiles;
            int x_max = ScreenToMap(new(h, w)).X + addon_tiles;
            int y_min = ScreenToMap(new(w, 0)).Y - addon_tiles;
            int y_max = ScreenToMap(new(0, h)).Y + addon_tiles;

            x_min = (x_min < 0) ? 0 : x_min;
            x_max = (x_max >= TILE_TYPE.GetLength(0)) ? TILE_TYPE.GetLength(0) - 1 : x_max;
            y_min = (y_min < 0) ? 0 : y_min;
            y_max = (y_max >= TILE_TYPE.GetLength(1)) ? TILE_TYPE.GetLength(0) - 1 : y_max;

            for (int x = x_min; x < x_max; x++)
            {
                for (int y = y_min; y < y_max; y++)
                {
                    Point screen_pos = MapToScreen(new(x, y));
                    Rectangle tile_pos = new Rectangle(screen_pos.X, screen_pos.Y, TILE_SIZE.X, TILE_SIZE.Y);
                    //_spriteBatch.Draw(empty_tile, tile_pos, null, c, 0.0f, Vector2.Zero, SpriteEffects.None, 2);

                    //Globals.SpriteBatch.Draw(empty_tile, tile_pos, Color.White);
                    Globals.SpriteBatch.Draw(empty_tile, tile_pos, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);



                    if (TILE_TYPE[x, y] == 1)  
                    {
                        Rectangle rock_pos = new Rectangle(screen_pos.X, screen_pos.Y - TILE_SIZE.Y/2, TILE_SIZE.X, TILE_SIZE.Y);
                        
                        //Globals.SpriteBatch.Draw(rock, rock_pos, Color.White);
                        Globals.SpriteBatch.Draw(rock, rock_pos, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.1f);
                    }
                }
            }
        }

        /* Helper Functions */
        // Isometric Math: https://clintbellanger.net/articles/isometric_math/
        // Note: our tiles have slightly different shape, (they're twice as high: take 1/4 of the height instead of 1/2)

        // MapToScreen: take index of a tile as an input (e.g. (2,1)) returns pixel position.
        public Point MapToScreen(Point map_coordinates)
        {
            int half_tile = TILE_SIZE.X / 2;
            int quater_tile = TILE_SIZE.X / 4;

            var ScreenX = (int)((map_coordinates.X - map_coordinates.Y) * half_tile);
            var ScreenY = (int)((map_coordinates.X + map_coordinates.Y) * quater_tile);
            return new(ScreenX + (int)Globals.CameraPosition.X, ScreenY + (int)Globals.CameraPosition.Y);
        }

        // ScreenToMap: takes pixel position, returns the index of the tile at this position.
        public Point ScreenToMap(Point ScreenPos) 
        {
            ScreenPos.X -= (int)Globals.CameraPosition.X;
            ScreenPos.Y -= (int)Globals.CameraPosition.Y;

            int half_tile = TILE_SIZE.X / 2;
            int quater_tile = TILE_SIZE.X / 4;
            int mapX = (int)((ScreenPos.X / half_tile + ScreenPos.Y / quater_tile) / 2);
            int mapY = (int)((ScreenPos.Y / quater_tile - ScreenPos.X / half_tile) / 2);
            return new(mapX, mapY);
        }
    }
}

