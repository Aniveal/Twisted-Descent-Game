using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Meridian2;

//Hello from Timo
// Hi Timo, Hello from Beni

/* -- TODO --
 * 
 *  - Use ScreenToMap function to stop player from moving over rocks
 *      - don't let the player start on a rock!
 *      - MapTiles need to be accessible when computing player position update (make the map a global?)
 * 
 *  - Make camera move with player
 * 
 *  - Separate between foreground background:
 *      - so far 4 layers: rocks (infront of player), player, rocks (behind player), basic tiles
 *      - need to know player position to know which rocks are infront / behind him 
 */

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Map _map;
    private Player _player;

    
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

        Globals.Content = Content;
        _map = new();
        _player = new();

        base.Initialize();
    }


    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.SpriteBatch = _spriteBatch;
        
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

        Globals.Update(gameTime);
        _player.Update(input);

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        _map.Draw();
        _player.Draw();
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
