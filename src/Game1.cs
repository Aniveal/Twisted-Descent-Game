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
 *  - Separate between foreground background:
 *      - so far 4 layers: rocks (infront of player), player, rocks (behind player), basic tiles
 *      - need to know player position to know which rocks are infront / behind him 
 *      
 *  - create more tiles
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
        _graphics.PreferredBackBufferWidth = 1024;
        _graphics.PreferredBackBufferHeight = 800;
        //_graphics.ToggleFullScreen();
        _graphics.ApplyChanges();

        Globals.Content = Content;
        Globals.graphics = _graphics;

        _map = new();
        _player = new();

        base.Initialize();
    }


    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.SpriteBatch = _spriteBatch;
        
    }

    private Vector2 cartesian_to_isometric(Vector2 vector) {
        return new Vector2(vector.X - vector.Y, (vector.X + vector.Y) / 2);
    }
    
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Vector2 input = Vector2.Zero;
        KeyboardState keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Right))     // makes player move similar to isometric perspective
        {
            input += new Vector2(9, -9);        // weird numbers make it fit to the tiles (get's normalized anyways)
        }
        if (keyboard.IsKeyDown(Keys.Left))
        {
            input += new Vector2(-9, 9);
        }
        if (keyboard.IsKeyDown(Keys.Down))
        {
            input += new Vector2(9, 9);
        }
        if (keyboard.IsKeyDown(Keys.Up))
        {
            input += new Vector2(-9, -9);
        }
        
        input = cartesian_to_isometric(input);
        
        if (input.LengthSquared() > 1)
        {
            input.Normalize();
        }

        Globals.UpdateTime(gameTime);
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
