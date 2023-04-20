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
                batch.Draw(boxTexture, rec, Color.Red);
            } else {
                batch.Draw(boxTexture, rec, Color.Black);
            }
            batch.Draw(icon_basic, rec, Color.Black);

            //electric
            rec = new Rectangle((width-size)/2, height-size, size, size);
            if (_controler.selected == 1) {
                batch.Draw(boxTexture, rec, Color.Red);
            } else {
                batch.Draw(boxTexture, rec, Color.Black);
            }
            batch.Draw(icon_electric, rec, Color.White);

            //fragile
            rec = new Rectangle((width-size)/2+size, height-size, size, size);
            if (_controler.selected == 2) {
                batch.Draw(boxTexture, rec, Color.Red);
            } else {
                batch.Draw(boxTexture, rec, Color.Black);
            }
            batch.Draw(icon_fragile, rec, Color.Black);
            
            //numbers
            batch.DrawString(_game.Font, _data.spears[0].ToString(), new Vector2((width)/2-size, height-size-20), Color.Yellow);
            batch.DrawString(_game.Font, _data.spears[1].ToString(), new Vector2((width)/2, height-size-20), Color.Yellow);
            batch.DrawString(_game.Font, _data.spears[2].ToString(), new Vector2((width)/2+size, height-size-20), Color.Yellow);
        }


    }
}