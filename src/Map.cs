using System;
using System.IO;
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
        String directory = "/Users/benjaminglaus/Desktop/GameLab/Git/gamelab2023-meridian-team2/src/Content/Maps/";
        bool readMapFromFile = true;
        String MapFile = "map_27-03-2023_07-44-56.map";

        private Texture2D ground;

        private Texture2D[] column =
        {
            Globals.Content.Load<Texture2D>("column"),
            Globals.Content.Load<Texture2D>("column_lower"),
            Globals.Content.Load<Texture2D>("column_upper"),
        };

        private Texture2D[] rock_textures =
            {
                Globals.Content.Load<Texture2D>("wall_1b"),
                Globals.Content.Load<Texture2D>("wall_1r"),
                Globals.Content.Load<Texture2D>("wall_1f"),
                Globals.Content.Load<Texture2D>("wall_1l"),
                Globals.Content.Load<Texture2D>("wall_2lf"),
                Globals.Content.Load<Texture2D>("wall_2rf"),
                Globals.Content.Load<Texture2D>("wall_2rb"),
                Globals.Content.Load<Texture2D>("wall_2lb"),
                Globals.Content.Load<Texture2D>("wall_3b"),
                Globals.Content.Load<Texture2D>("wall_3r"),
                Globals.Content.Load<Texture2D>("wall_3f"),
                Globals.Content.Load<Texture2D>("wall_3l"),
                Globals.Content.Load<Texture2D>("wall_4"),
            };

        public Point TILE_SIZE = new(160, 160); // pixels
        public int[,] TILE_TYPE = new int[100, 100]; // contains type of each tile: 0 for empty_tile, 1 for rock_tile

        /* Initialization */
        public Map()
		{
            ground = Globals.Content.Load<Texture2D>("ground");
            if (readMapFromFile)
            {
                ReadMapFromFile(directory + MapFile);
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
                    Color c = (Globals.SelectedTile.X == x && Globals.SelectedTile.Y == y) ? Color.Red : Color.White;

                    Point screen_pos = MapToScreen(new(x, y));
                    Rectangle tile_pos = new Rectangle(screen_pos.X, screen_pos.Y, TILE_SIZE.X, TILE_SIZE.Y);

                    Globals.SpriteBatch.Draw(ground, tile_pos, null, c, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);

                    int tile_type = TILE_TYPE[x, y];
                    if (tile_type > 0)  
                    {
                        Rectangle rock_pos = new Rectangle(screen_pos.X, screen_pos.Y, TILE_SIZE.X, TILE_SIZE.Y);
                        //Globals.SpriteBatch.Draw(rock_textures[tile_type], rock_pos, null, c, 0.0f, Vector2.Zero, SpriteEffects.None, 0.1f);
                        Globals.SpriteBatch.Draw(rock_textures[tile_type - 1], rock_pos, c);
                    }
                    else if (tile_type < 0)
                    {
                        Rectangle rock_pos = new Rectangle(screen_pos.X, screen_pos.Y, TILE_SIZE.X, TILE_SIZE.Y);
                        Globals.SpriteBatch.Draw(column[(-1 * tile_type) - 1], rock_pos, c);
                    }
                }
            }
        }

        
        public void place_column() // iterate between 0,13
        {
            int x = Globals.SelectedTile.X;
            int y = Globals.SelectedTile.Y;
            TILE_TYPE[x, y] = ((TILE_TYPE[x, y] - 1) < -3 || (TILE_TYPE[x, y] - 1) >= 0) ? -1 : (TILE_TYPE[x, y] - 1);
            
            Debug.WriteLine(TILE_TYPE[x, y] + " : " + ((-1 * TILE_TYPE[x, y]) - 1));
        }

        public void place_rock_0() // iterate between 0,13
        {
            int x = Globals.SelectedTile.X;
            int y = Globals.SelectedTile.Y;
            TILE_TYPE[x, y] = 0;
        }

        public void place_rock_1() // iterate between 1,2,3,4
        {
            int x = Globals.SelectedTile.X;
            int y = Globals.SelectedTile.Y;
            TILE_TYPE[x, y] = 1 + (TILE_TYPE[x, y] % 4);

            Debug.WriteLine("Set to: " + TILE_TYPE[x, y]);
        }

        public void place_rock_2() // iterate between 5,6,7,8
        {
            int x = Globals.SelectedTile.X;
            int y = Globals.SelectedTile.Y;
            TILE_TYPE[x, y] = 5 + (TILE_TYPE[x, y] % 4);

            Debug.WriteLine("Set to: " + TILE_TYPE[x, y]);
        }

        public void place_rock_3() // iterate between 9,10,11,12
        {
            int x = Globals.SelectedTile.X;
            int y = Globals.SelectedTile.Y;
            TILE_TYPE[x, y] = 9 + (TILE_TYPE[x, y] % 4);

            Debug.WriteLine("Set to: " + TILE_TYPE[x, y]);
        }

        public void place_rock_4() // iterate between 0,13
        {
            int x = Globals.SelectedTile.X;
            int y = Globals.SelectedTile.Y;
            TILE_TYPE[x, y] = 13;
        }

        public void WriteMapToFile()
        {
            int rows = TILE_TYPE.GetLength(0);
            int columns = TILE_TYPE.GetLength(1);
            String currentTime = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");

            using (StreamWriter writer = new StreamWriter(directory + "map_" + currentTime + ".map"))
            {
                writer.WriteLine(rows);
                writer.WriteLine(columns);

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        writer.Write($"{TILE_TYPE[i, j]} ");
                    }
                    writer.WriteLine();
                }
            }
            return;
        }


        public void ReadMapFromFile(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                int rows = int.Parse(reader.ReadLine());
                int columns = int.Parse(reader.ReadLine());
                for (int i = 0; i < rows; i++)
                {
                    string[] values = reader.ReadLine().Split(' ');
                    for (int j = 0; j < columns; j++)
                    {
                        TILE_TYPE[i, j] = int.Parse(values[j]);
                    }
                }
            }
            return;
        }



        /* Helper Functions */
        // Isometric Math: https://clintbellanger.net/articles/isometric_math/
        // Note: our tiles have slightly different shape, (they're twice as high: take 1/4 of the height instead of 1/2)

        // MapToScreen: take index of a tile as an input (e.g. (2,1)) returns pixel position.
        public Point MapToScreen(Point map_coordinates)
        {
            int h = Globals.graphics.PreferredBackBufferHeight;
            int w = Globals.graphics.PreferredBackBufferWidth;

            int half_tile = TILE_SIZE.X / 2;
            int quater_tile = TILE_SIZE.X / 4;

            int x = map_coordinates.X - Globals.SelectedTile.X;
            int y = map_coordinates.Y - Globals.SelectedTile.Y;

            var ScreenX = (int)((x - y) * half_tile);
            var ScreenY = (int)((x + y) * quater_tile);
            return new(ScreenX + (w - TILE_SIZE.X)/2, ScreenY + (h - TILE_SIZE.Y) / 2);
        }

        // ScreenToMap: takes pixel position, returns the index of the tile at this position.
        public Point ScreenToMap(Point ScreenPos) 
        {
            int h = Globals.graphics.PreferredBackBufferHeight;
            int w = Globals.graphics.PreferredBackBufferWidth;

            int x = ScreenPos.X - (w - TILE_SIZE.X) / 2;
            int y = ScreenPos.Y - (h - TILE_SIZE.Y) / 2;

            int half_tile = TILE_SIZE.X / 2;
            int quater_tile = TILE_SIZE.X / 4;

            int mapX = (int)((x / half_tile + y / quater_tile) / 2);
            int mapY = (int)((y / quater_tile - x / half_tile) / 2);
            return new(mapX + Globals.SelectedTile.X, mapY + Globals.SelectedTile.Y);
        }
    }
}

