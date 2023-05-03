using Meridian2.Columns;
using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Gui; 

public class SpearsGui : DrawableGameElement {
    private const int Size_Texture = 100;
    private const int Size_Icon = 30;

    private Texture2D _boxTexture;

    private readonly SpearsController _controler;
    private readonly GameData _data;

    private readonly RopeGame _game;
    private Texture2D _iconBasic;
    private Texture2D _textureBasic;
    private Texture2D _iconElectric;
    private Texture2D _textureElectric;
    private Texture2D _iconFragile;
    private Texture2D _textureFragile;

    public SpearsGui(RopeGame game, GameData data, SpearsController controler) {
        _game = game;
        _data = data;
        _controler = controler;
    }

    public void LoadContent() {
        _boxTexture = _game.Content.Load<Texture2D>("Sprites/UI/box");
        _iconBasic = _game.Content.Load<Texture2D>("Sprites/UI/letter_b");
        _textureBasic = _game.Content.Load<Texture2D>("Sprites/Columns/column");     
        _iconElectric = _game.Content.Load<Texture2D>("Sprites/UI/letter_e");
        _textureElectric = _game.Content.Load<Texture2D>("Sprites/Columns/lightning_column");
        _iconFragile = _game.Content.Load<Texture2D>("Sprites/UI/letter_f");
        _textureFragile = _game.Content.Load<Texture2D>("Sprites/Columns/fragile_column");
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        var width = _game.GraphicsDevice.Viewport.Width;
        var height = _game.GraphicsDevice.Viewport.Height;

        //assuming 3 spears
        //basic spear
        //Rectangle rec = new Rectangle((width-size)/2-size, height-2*size, size, size);
        var rec_texture = new Rectangle((width - Size_Texture) / 2 - Size_Texture, height - Size_Texture, Size_Texture, Size_Texture);
        var rec_icon = new Rectangle((width - Size_Texture) / 2 - Size_Texture, height - Size_Texture / 2, Size_Icon, Size_Icon);

        if (_controler.Selected == 0)
            batch.Draw(_boxTexture, rec_texture, null, Color.Red, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        else
            batch.Draw(_boxTexture, rec_texture, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);

        batch.Draw(_iconBasic, rec_icon, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        batch.Draw(_textureBasic, rec_texture, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);

        //electric
        rec_texture = new Rectangle((width - Size_Texture) / 2, height - Size_Texture, Size_Texture, Size_Texture);
        rec_icon = new Rectangle((width - Size_Texture) / 2, height - Size_Texture / 2, Size_Icon, Size_Icon);

        if (_controler.Selected == 1)
            batch.Draw(_boxTexture, rec_texture, null, Color.Red, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        else
            batch.Draw(_boxTexture, rec_texture, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);

        batch.Draw(_textureElectric, rec_texture, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        batch.Draw(_iconElectric, rec_icon, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);

        //fragile
        rec_texture = new Rectangle((width - Size_Texture) / 2 + Size_Texture, height - Size_Texture, Size_Texture, Size_Texture);
        rec_icon = new Rectangle((width - Size_Texture) / 2 + Size_Texture, height - Size_Texture / 2, Size_Icon, Size_Icon);

        if (_controler.Selected == 2)
            batch.Draw(_boxTexture, rec_texture, null, Color.Red, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        else
            batch.Draw(_boxTexture, rec_texture, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);

        batch.Draw(_textureFragile, rec_texture, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        batch.Draw(_iconFragile, rec_icon, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);

        //numbers
        batch.DrawString(_game.Font, _data.Spears[0].ToString(), new Vector2(width / 2 - Size_Texture, height - Size_Texture - 20),
            Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        batch.DrawString(_game.Font, _data.Spears[1].ToString(), new Vector2(width / 2, height - Size_Texture - 20),
            Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        batch.DrawString(_game.Font, _data.Spears[2].ToString(), new Vector2(width / 2 + Size_Texture, height - Size_Texture - 20),
            Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
    }
}