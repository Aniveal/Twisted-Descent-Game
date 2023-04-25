using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Meridian2.GameElements;
using Meridian2.Columns;

namespace Meridian2.Gui {
    public class SpearsGui : DrawableGameElement {

        private RopeGame _game;
        private GameData _data;

        private SpearsController _controler;

        private const int size = 100;

        private Texture2D boxTexture;
        private Texture2D icon_basic;
        private Texture2D icon_electric;
        private Texture2D icon_fragile;

        public SpearsGui(RopeGame game, GameData data, SpearsController controler) {
            _game = game;
            _data = data;
            _controler = controler;
        } 

        public void LoadContent() {
            boxTexture = _game.Content.Load<Texture2D>("Sprites/UI/box");
            icon_basic = _game.Content.Load<Texture2D>("Sprites/UI/letter_b");
            icon_electric = _game.Content.Load<Texture2D>("Sprites/UI/letter_e");
            icon_fragile = _game.Content.Load<Texture2D>("Sprites/UI/letter_f");
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
            int width = _game.GraphicsDevice.Viewport.Width;
            int height = _game.GraphicsDevice.Viewport.Height;

            //assuming 3 spears
            //basic spear
            //Rectangle rec = new Rectangle((width-size)/2-size, height-2*size, size, size);
            Rectangle rec = new Rectangle((width-size)/2-size, height-size, size, size);
            if (_controler.selected == 0) {
                batch.Draw(boxTexture, rec, null, Color.Red, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            } else {
                batch.Draw(boxTexture, rec, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            }
            batch.Draw(icon_basic, rec, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);

            //electric
            rec = new Rectangle((width-size)/2, height-size, size, size);
            if (_controler.selected == 1) {
                batch.Draw(boxTexture, rec, null, Color.Red, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            } else {
                batch.Draw(boxTexture, rec, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            }
            batch.Draw(icon_electric, rec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);

            //fragile
            rec = new Rectangle((width-size)/2+size, height-size, size, size);
            if (_controler.selected == 2) {
                batch.Draw(boxTexture, rec, null, Color.Red, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            } else {
                batch.Draw(boxTexture, rec, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            }
            batch.Draw(icon_fragile, rec, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            
            //numbers
            batch.DrawString(_game.Font, _data.spears[0].ToString(), new Vector2((width)/2-size, height-size-20), Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            batch.DrawString(_game.Font, _data.spears[1].ToString(), new Vector2((width)/2, height-size-20), Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            batch.DrawString(_game.Font, _data.spears[2].ToString(), new Vector2((width)/2+size, height-size-20), Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }


    }
}