using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Formats.Asn1.AsnWriter;

namespace TwistedDescent.Screens; 

public class FinalScreen : Screen {
    private readonly List<Component> _components;
    private SpriteBatch _spriteBatch;
    public GraphicsDeviceManager Graphics;

    private ContentManager _content;

    private Texture2D _bg;
    private Texture2D _skull_img;
    private Texture2D _name_selection_box;
    private Texture2D _name_selection_arrow;
    private Texture2D _name_selection_arrow_down;
    private Texture2D _menu_selection_highlight;

    private SpriteFont font;

    private bool _timedOut;

    private int w;
    private int h;
    private int n_kills;
    private int name_selection_margin = 15; // pixels, (horizontal) distance between boxes containing the letters for name selection
    private int arrows_vertical_margin = 4; // pixels, (vertical) distance between arrows and box
    private int name_selection_size = 80;
    private int letter_width;

    private int thumstick_direction = 0;
    private int thumbstick_slowdown = 0;
    private int thumbstick_slowdown_threshold = 20;

    private Vector2 margin;

    private Color font_color = new Color(154, 134, 129);
    private Color name_active_color = new Color(150, 150, 150); // color used for name selection box / arrows when its selected (active)

    public String playerName;
    private String letter0 = "B";
    private String letter1 = "E";
    private String letter2 = "N";
    private int active_letter = 1;      // == 0, 1, 2 for letters 3 for Save button


    public FinalScreen(RopeGame game, ContentManager content, bool timedOut) : base(game)
    {
        font = content.Load<SpriteFont>("Fonts/damn");
        _content = content;

        _bg = content.Load<Texture2D>("Sprites/UI/menu_background");
        _skull_img = content.Load<Texture2D>("Sprites/UI/skull_icon");
        _name_selection_box = content.Load<Texture2D>("Sprites/UI/name_selection_background");
        _name_selection_arrow = content.Load<Texture2D>("Sprites/UI/name_selection_arrow");
        _name_selection_arrow_down = content.Load<Texture2D>("Sprites/UI/name_selection_arrow_down");
        _menu_selection_highlight = content.Load<Texture2D>("Sprites/UI/menu_selection_highlight");

        w = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
        h = game.GraphicsDevice.PresentationParameters.BackBufferHeight;

        n_kills = this.getGame().GameData.Kills;
        margin = new Vector2(w / 16, h / 9);
        letter_width = (int) font.MeasureString(letter0).X;

        _timedOut = timedOut;
    }
    
    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        spriteBatch.Draw(_bg, new Rectangle(0, 0, w, h), Color.White);

        // Writing the Text:

        String death_count_msg = "During the run, you killed "  + n_kills + " Enemies.";
        String enter_name_msg = "Enter your name for the leaderboard!";
        

        float font_height = font.MeasureString(death_count_msg).Y;

        spriteBatch.DrawString(font, death_count_msg, new Vector2(margin.X, margin.Y), font_color);
        spriteBatch.DrawString(font, enter_name_msg, new Vector2(margin.X, margin.Y + font_height), font_color);

        // Name Selection UI:
        String save_str = "Save";
        int save_width = (int)font.MeasureString(save_str).X;

        int upper_arrows_y = (int) (margin.Y + 5 * font_height / 2);
        int name_selection_y = upper_arrows_y + arrows_vertical_margin + name_selection_size / 2; // arrows sprite is half as high as name selection box height
        int lower_arrows_y = name_selection_y + 2 * arrows_vertical_margin + name_selection_size;

        int name_selection_width = 3 * name_selection_size + 7 * name_selection_margin + save_width;
        int x1_pos = (w - name_selection_width) / 2;
        int x2_pos = x1_pos + name_selection_size + name_selection_margin;
        int x3_pos = x2_pos + name_selection_size + name_selection_margin;
        int x_save_pos = x3_pos + name_selection_size + 5 * name_selection_margin; // gap between letter selection and save is twice as wide

        Color pos1_color = (active_letter == 0) ? name_active_color : Color.White;
        Color pos2_color = (active_letter == 1) ? name_active_color : Color.White;
        Color pos3_color = (active_letter == 2) ? name_active_color : Color.White;

        // upper arrows
        spriteBatch.Draw(_name_selection_arrow, new Rectangle(x1_pos, upper_arrows_y, name_selection_size, name_selection_size / 2), pos1_color);
        spriteBatch.Draw(_name_selection_arrow, new Rectangle(x2_pos, upper_arrows_y, name_selection_size, name_selection_size / 2), pos2_color);
        spriteBatch.Draw(_name_selection_arrow, new Rectangle(x3_pos, upper_arrows_y, name_selection_size, name_selection_size / 2), pos3_color);

        // background box
        spriteBatch.Draw(_name_selection_box, new Rectangle(x1_pos, name_selection_y, name_selection_size, name_selection_size), pos1_color);
        spriteBatch.Draw(_name_selection_box, new Rectangle(x2_pos, name_selection_y, name_selection_size, name_selection_size), pos2_color);
        spriteBatch.Draw(_name_selection_box, new Rectangle(x3_pos, name_selection_y, name_selection_size, name_selection_size), pos3_color);

        // lower arrows
        spriteBatch.Draw(_name_selection_arrow_down, new Rectangle(x1_pos, lower_arrows_y, name_selection_size, name_selection_size / 2), pos1_color);
        spriteBatch.Draw(_name_selection_arrow_down, new Rectangle(x2_pos, lower_arrows_y, name_selection_size, name_selection_size / 2), pos2_color);
        spriteBatch.Draw(_name_selection_arrow_down, new Rectangle(x3_pos, lower_arrows_y, name_selection_size, name_selection_size / 2), pos3_color);

        // letters
        float letters_x_offset = (name_selection_size - letter_width) / 2;
        float letters_y_offset = (name_selection_size - font_height + 10) / 2;
        spriteBatch.DrawString(font, letter0, new Vector2(x1_pos + letters_x_offset, name_selection_y + letters_y_offset), font_color);
        spriteBatch.DrawString(font, letter1, new Vector2(x2_pos + letters_x_offset, name_selection_y + letters_y_offset), font_color);
        spriteBatch.DrawString(font, letter2, new Vector2(x3_pos + letters_x_offset, name_selection_y + letters_y_offset), font_color);

        // Save Button
        if (active_letter == 3)
            spriteBatch.Draw(_menu_selection_highlight, new Rectangle(x_save_pos - 10, name_selection_y, save_width + 20, (int) font_height), Color.White);
        
        spriteBatch.DrawString(font, save_str, new Vector2(x_save_pos, name_selection_y + letters_y_offset), font_color);

        spriteBatch.End();
    }



    public override void Update(GameTime gameTime) {

        // Keyboard / DPad input support:

       if (Input.IsButtonPressed(Buttons.DPadUp, true) || Input.IsKeyPressed(Keys.Up, true) || Input.IsKeyPressed(Keys.W, true))   // UP
        {
            increase_button(1);
        } 
       else if (Input.IsButtonPressed(Buttons.DPadDown, true) || Input.IsKeyPressed(Keys.Down, true) || Input.IsKeyPressed(Keys.S, true))  // DOWN
        {
            increase_button(-1);
        }
       else if (Input.IsButtonPressed(Buttons.DPadRight, true) || Input.IsKeyPressed(Keys.Right, true) || Input.IsKeyPressed(Keys.D, true))    // RIGHT
        {
            active_letter += 1;
            if (active_letter > 3)
                active_letter = 0;
        }
       else if (Input.IsButtonPressed(Buttons.DPadLeft, true) || Input.IsKeyPressed(Keys.Left, true) || Input.IsKeyPressed(Keys.A, true))      // LEFT
        {
            active_letter -= 1;
            if (active_letter < 0)
                active_letter = 3;
        }
       else if (Input.IsButtonPressed(Buttons.A, true) || Input.IsKeyPressed(Keys.Enter, true))
        {
            if (active_letter == 3)
                save_game();
        }

        // thumbstick input support:

        Vector2 leftThumbStick = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
        float ThumbStickThreshold = 0.85f;   // how far thumbstick is pushed to count as a input

        if (leftThumbStick.Y >= ThumbStickThreshold)            // UP
        {
            if (joystick_input(1))
                increase_button(1);
        }
        else if (leftThumbStick.Y <= -ThumbStickThreshold)    // DOWN
        {
            if (joystick_input(2))
                increase_button(-1);
        }
        else if (leftThumbStick.X >= ThumbStickThreshold)    // RIGHT
        {
            if (joystick_input(3))
            {
                active_letter += 1;
                if (active_letter > 3)
                    active_letter = 0;
            }
        }
        else if (leftThumbStick.X <= - ThumbStickThreshold)     // LEFT
        {
            if (joystick_input(4))
            {
                active_letter -= 1;
                if (active_letter < 0)
                    active_letter = 3;
            }
        }
    }

    private void save_game() {
        // update leaderboard:
        int score = base.getGame().GameData.Score;
        playerName = letter0 + "." + letter1 + "." + letter2;
        _game.leaderBoard.Add(new KeyValuePair<string, int>(playerName, score));
        _game.leaderBoard = _game.leaderBoard.OrderByDescending(x => x.Value).ToList().GetRange(0, Math.Min(5, _game.leaderBoard.Count()));
        File.WriteAllLines("leaderBoard.txt", base.getGame().leaderBoard.Select(x => $"{x.Key},{x.Value}"));

        // return to menu screen:
        base.getGame()._menuScreen = new MenuScreen(base.getGame(), base.getGame().GraphicsDevice, _content);
        base.getGame()._menuScreen.Initialize();
        base.getGame().ChangeState(RopeGame.State.MainMenu);
        return;
    }

    private bool joystick_input(int cur_direction) {   // tests if duration since same input has been long enough, if so, returns true (allow action)
        bool allow_action = false;
        if (thumstick_direction != cur_direction || thumbstick_slowdown >= thumbstick_slowdown_threshold)
        {
            thumbstick_slowdown = 0;
        }
        if (thumbstick_slowdown == 0)
        {
            allow_action = true;
        }
        thumstick_direction = cur_direction;
        thumbstick_slowdown += 1;
        return allow_action;
    }

    private void increase_button(int amount) {  // amount usually should be 1 or -1 (next or previous letter)
        if (active_letter > 2)
            return;

        String current_letter = (active_letter == 0) ? letter0 : ((active_letter == 1) ? letter1 : letter2);
        int ascii_value = (int) char.Parse(current_letter);

        ascii_value += amount;          // ascii value of A is 65, value of Z is 90
        if (ascii_value < 65)
            ascii_value = 90;
        else if (ascii_value > 90)
            ascii_value = 65;

        String new_letter = ((char)ascii_value).ToString();

        if (active_letter == 0)
            letter0 = new_letter;
        else if (active_letter == 1)
            letter1 = new_letter;
        else if (active_letter == 2)
            letter2 = new_letter;

        return;
    }

}