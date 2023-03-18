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
            for (int x = 0; x < TILE_TYPE.GetLength(0); x++)
            {
                for (int y = 0; y < TILE_TYPE.GetLength(1); y++)
                {
                    Point screen_pos = MapToScreen(new(x, y));
                    Rectangle tile_pos = new Rectangle(screen_pos.X, screen_pos.Y, TILE_SIZE.X, TILE_SIZE.Y);
                    //_spriteBatch.Draw(empty_tile, tile_pos, null, c, 0.0f, Vector2.Zero, SpriteEffects.None, 2);
                    Globals.SpriteBatch.Draw(empty_tile, tile_pos, Color.White);

                    if (TILE_TYPE[x, y] == 1)  
                    {
                        Rectangle rock_pos = new Rectangle(screen_pos.X, screen_pos.Y - TILE_SIZE.Y/2, TILE_SIZE.X, TILE_SIZE.Y);
                        Globals.SpriteBatch.Draw(rock, rock_pos, Color.White);
                    }

                }
            }
        }

        /* Helper Functions */
        // Isometric Math: https://clintbellanger.net/articles/isometric_math/
        // Note: our tiles have slightly different shape

        // MapToScreen: take index of a tile as an input (e.g. (2,1)) returns pixel position.
        public Point MapToScreen(Point map_coordinates)
        {
            int half_tile = TILE_SIZE.X / 2;
            int quater_tile = TILE_SIZE.X / 4;
            var ScreenX = (int)((map_coordinates.X - map_coordinates.Y) * half_tile);
            var ScreenY = (int)((map_coordinates.X + map_coordinates.Y) * quater_tile);
            return new(ScreenX, ScreenY);
        }

        // ScreenToMap: takes pixel position, returns the index of the tile at this position.
        public Point ScreenToMap(Point ScreenPos) 
        {
            int half_tile = TILE_SIZE.X / 2;
            int quater_tile = TILE_SIZE.X / 4;
            int mapX = (int)((ScreenPos.X / half_tile + ScreenPos.Y / quater_tile) / 2);
            int mapY = (int)((ScreenPos.Y / quater_tile - ScreenPos.X / half_tile) / 2);
            return new(mapX, mapY);
        }
    }
}

