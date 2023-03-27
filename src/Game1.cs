using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Meridian2;

/* LOADING A MAP:
 * In Map.cs, set the directory and filename
 */

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

    private Vector2 last_input;
    private float time_last_input;



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
            input += new Vector2(1, 0);        // weird numbers make it fit to the tiles (get's normalized anyways)
        }
        if (keyboard.IsKeyDown(Keys.Left))
        {
            input += new Vector2(-1, 0);
        }
        if (keyboard.IsKeyDown(Keys.Down))
        {
            input += new Vector2(0, 1);
        }
        if (keyboard.IsKeyDown(Keys.Up))
        {
            input += new Vector2(0, -1);
        }
        
        if (input.LengthSquared() > 1)
        {
            input.Normalize();
        }

        // single key press might cover ~10 frames -> input given 10 times
        // --> only read input once every 0.2 seconds (unless different input is given)
        float timedelta = (float)gameTime.TotalGameTime.TotalSeconds - time_last_input;
        if (last_input != input || timedelta > 0.12)
        {
            last_input = input;
            time_last_input = (float)gameTime.TotalGameTime.TotalSeconds;

            var keys = keyboard.GetPressedKeys();
            if (keys.Length > 0)
            {
                Debug.WriteLine(keys[0]);

                if (keys[0].ToString() == "D1")
                {
                    _map.place_rock_1();
                }
                else if (keys[0].ToString() == "D2")
                {
                    _map.place_rock_2();
                }
                else if (keys[0].ToString() == "D3")
                {
                    _map.place_rock_3();
                }
                else if (keys[0].ToString() == "D4")
                {
                    _map.place_rock_4();
                }
                else if (keys[0].ToString() == "D0")
                {
                    _map.place_rock_0();
                }
                else if (keys[0].ToString() == "C")
                {
                    _map.place_column();
                }
                else if (keys[0].ToString() == "S")
                {
                    _map.WriteMapToFile();
                }

            }
            
            Globals.UpdateTile(new((int)input.X, (int)input.Y));
        }

        Globals.UpdateTime(gameTime);

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();

        _map.Draw();

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
