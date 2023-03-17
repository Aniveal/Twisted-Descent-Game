using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Meridian2;

//Hello from Timo
// Hi Timo, Hello from Beni

/* -- TODO --
    - split into multiple files
        - File for Map and Tiles
        - File for Player, Control, Position
    - Fix confusion with Vector2 and Point (always use one, not both (see ScreenToMap, MapToScreen))
    - Fix ScreenToMap function (and maybe rename? TileIndexToPixelCoordinate, PixelCoordinateToTileIndex)
    - Use ScreenToMap function to stop player from moving over rocks
        - don't let the player start on a rock!
    - Use (and draw) better tiles (currently very low resolution)
    - Rock image sparate from tiles (draw them infront of player, but tiles should be behind the player)
        - overthink use of int[,] for tiles:
            - store rock infront of tile: draw empty everywhere, if tile_type[i,j] == 1 -> draw additional rock
            - what when player reaches [0,0] ?
    - Make camera move with player
 */

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // Assets
    Texture2D empty_tile;
    Texture2D rock_tile;
    Texture2D hero;

    // Constants
    Point TILE_SIZE = new(50, 100); // size of Tile images (should be of value (x, 2x) for some x)
    const int PLAYER_VELOCITY = 200;

    // Variables
    Vector2 playerPosition = new Vector2(100, 100);     // starting position of player
    int[,] tile_type = new int[80, 80];                 // contains type of each tile: 0 for empty_tile, 1 for rock_tile

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1200;
        _graphics.PreferredBackBufferHeight = 1024;
        //_graphics.ToggleFullScreen();
        _graphics.ApplyChanges();

        // Randomly choosing which tiles (rock or empty) to place
        Random rng = new Random();
        for (int x = 0; x < tile_type.GetLength(0); x++)
        {
            for (int y = 0; y < tile_type.GetLength(1); y++)
            {
                float type = rng.Next(0, 20); // 1 out of 20 tiles should contain a rock
                if (type == 1)
                {
                    tile_type[x, y] = 1;
                }
                else
                {
                    tile_type[x, y] = 0;
                }
            }
        }



        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        empty_tile = Content.Load<Texture2D>("empty_tile");
        rock_tile = Content.Load<Texture2D>("rock_tile");
        hero = Content.Load<Texture2D>("hero");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Vector2 input = Vector2.Zero;
        KeyboardState keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Right))     // makes player move similar to isometric perspective
        {
            input += new Vector2(9, -6);        // weird numbers make it fit to the tiles (get's normalized anyways)
        }
        if (keyboard.IsKeyDown(Keys.Left))
        {
            input += new Vector2(-9, 6);
        }
        if (keyboard.IsKeyDown(Keys.Down))
        {
            input += new Vector2(9, 6);
        }
        if (keyboard.IsKeyDown(Keys.Up))
        {
            input += new Vector2(-9, -6);
        }
        if (input.LengthSquared() > 1)
        {
            input.Normalize();
        }

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 updatedPos = playerPosition + input * deltaTime * PLAYER_VELOCITY;
        Point MapIndex = ScreenToMap(new Point((int)updatedPos.X, (int)updatedPos.Y));


        if (true)
        {
            playerPosition = updatedPos;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        Point MapIndex = ScreenToMap(new Point((int)playerPosition.X, (int)playerPosition.Y)); // Tile coordinate player is standing on
        Debug.WriteLine(MapIndex.ToString() + "  " + playerPosition.ToString());

        for (int x = 0; x < tile_type.GetLength(0); x++)
        {
            for (int y = 0; y < tile_type.GetLength(1); y++)
            {
                // Debug Code for MapToScreen function (turn tile that player is standing on to red):

                Color c = Color.White;
                if (MapIndex == new Point(x, y)) 
                {
                    c = Color.Red;
                }

                // DRAW TILES:

                Point screen_pos = MapToScreen(new(x, y));
                Rectangle tile_pos = new Rectangle(screen_pos.X, screen_pos.Y, TILE_SIZE.X, TILE_SIZE.Y);

                if (tile_type[x, y] == 1)    // tile should contain a rock
                {
                    _spriteBatch.Draw(rock_tile, tile_pos, c);
                }
                else  // tile should be empty
                {
                    _spriteBatch.Draw(empty_tile, tile_pos, c);
                }

            }
        }
        //_spriteBatch.Draw(hero, new Rectangle(0, 0, 200, 200), Color.White);
        _spriteBatch.Draw(hero, new Rectangle((int)playerPosition.X, (int)playerPosition.Y, TILE_SIZE.X, TILE_SIZE.Y), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }


    // maps tile coordinate (e.g. 4,5 (== index in tile_type 2d array) to the pixel coordinates of where sprite should be placed
    private Point MapToScreen(Point map_coordinates)
    {
        int ScreenX = (map_coordinates.X - map_coordinates.Y) * TILE_SIZE.X / 2 + 10 * TILE_SIZE.X;
        int ScreenY = (map_coordinates.Y + map_coordinates.X) * TILE_SIZE.Y / 6 - 5 * TILE_SIZE.Y;
        return new(ScreenX, ScreenY);
    }

    // maps pixel coordinates to tile coordinate (inverse of function above)
    private Point ScreenToMap(Point ScreenPos)      // This function is broke, math doesn't work out
    {
        Vector2 cursor = new(ScreenPos.X - (int)(10 * TILE_SIZE.X), ScreenPos.Y + (int)(5 * TILE_SIZE.Y));
        //var x = cursor.X + (6 * cursor.Y) - (TILE_SIZE.X / 2);
        var x = (3 / TILE_SIZE.Y) * cursor.Y + (1 / TILE_SIZE.X) * cursor.X;
        int mapX = (x < 0) ? -1 : (int)(x / TILE_SIZE.X);
        //var y = -cursor.X + (6 * cursor.Y) + (TILE_SIZE.X / 2);
        var y = (3 / TILE_SIZE.Y) * cursor.Y - (1 / TILE_SIZE.X) * cursor.X;
        int mapY = (y < 0) ? -1 : (int)(y / TILE_SIZE.X);

        return new(mapX, mapY);
    }
}
