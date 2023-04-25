using Meridian2.Columns;
using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Gui; 

public class SpearsGui : DrawableGameElement {
    private const int Size = 100;

    private Texture2D _boxTexture;

    private readonly SpearsController _controler;
    private readonly GameData _data;

    private readonly RopeGame _game;
    private Texture2D _iconBasic;
    private Texture2D _iconElectric;
    private Texture2D _iconFragile;

    public SpearsGui(RopeGame game, GameData data, SpearsController controler) {
        _game = game;
        _data = data;
        _controler = controler;
    }

    public void LoadContent() {
        _boxTexture = _game.Content.Load<Texture2D>("Sprites/UI/box");
        _iconBasic = _game.Content.Load<Texture2D>("Sprites/UI/letter_b");
        _iconElectric = _game.Content.Load<Texture2D>("Sprites/UI/letter_e");
        _iconFragile = _game.Content.Load<Texture2D>("Sprites/UI/letter_f");
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        var width = _game.GraphicsDevice.Viewport.Width;
        var height = _game.GraphicsDevice.Viewport.Height;

        //assuming 3 spears
        //basic spear
        //Rectangle rec = new Rectangle((width-size)/2-size, height-2*size, size, size);
        var rec = new Rectangle((width - Size) / 2 - Size, height - Size, Size, Size);
        if (_controler.Selected == 0)
            batch.Draw(_boxTexture, rec, null, Color.Red, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        else
            batch.Draw(_boxTexture, rec, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        batch.Draw(_iconBasic, rec, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);

        //electric
        rec = new Rectangle((width - Size) / 2, height - Size, Size, Size);
        if (_controler.Selected == 1)
            batch.Draw(_boxTexture, rec, null, Color.Red, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        else
            batch.Draw(_boxTexture, rec, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        batch.Draw(_iconElectric, rec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);

        //fragile
        rec = new Rectangle((width - Size) / 2 + Size, height - Size, Size, Size);
        if (_controler.Selected == 2)
            batch.Draw(_boxTexture, rec, null, Color.Red, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        else
            batch.Draw(_boxTexture, rec, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        batch.Draw(_iconFragile, rec, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);

        //numbers
        batch.DrawString(_game.Font, _data.Spears[0].ToString(), new Vector2(width / 2 - Size, height - Size - 20),
            Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        batch.DrawString(_game.Font, _data.Spears[1].ToString(), new Vector2(width / 2, height - Size - 20),
            Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        batch.DrawString(_game.Font, _data.Spears[2].ToString(), new Vector2(width / 2 + Size, height - Size - 20),
            Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
    }
}