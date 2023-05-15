using Meridian2.GameElements;
using Meridian2.Theseus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using static System.Net.Mime.MediaTypeNames;

namespace Meridian2.Treasures {
    public class BodyWithText : DrawableGameElement {

        private RopeGame _game;
        private World _world;
        private Body _body;
        private DiverseManager dm;
        private String _text;
        public bool onDisplay = false;
        public bool finished = false;
        private bool destroyed = false;
        private SpriteFont _font;           // font set in map.cs, set to Fonts/tutorial_text
        private int margin = 300;
        Vector2 stringSize;
        private float duration = 4; //duration in seconds
        private double messageStart = 0;

        // Red color: new Color(170, 54, 54)
        // Light Brown color: new Color(147, 124, 119)
        private Color text_color = new Color(147, 124, 119);
        private Color text_background_color = new Color(32, 33, 33);

        private int text_outline_size = 1; // pixels, size of dark outline around the text

        public BodyWithText(RopeGame game, Vector2 position, float radius, World world, String text, SpriteFont font) {
            _world = world;
            _text = text;
            _body = _world.CreateCircle(radius, 0, position);
            _body.Tag = this;
            _body.OnCollision += OnCollision;
            _font = font;
            _game = game;

            stringSize = _font.MeasureString(text);
        }

        public void SetManager(DiverseManager manager) {
            dm = manager;
        }

        public void Destroy() {
            if (destroyed) return;
            _world.Remove(_body);
            destroyed = true;
        }

        private bool OnCollision(Fixture sender, Fixture other, Contact contact) {
            Body collider;
            if (sender.Body.Tag == this)
                collider = other.Body;
            else
                collider = sender.Body;
            if (collider.Tag is Player) {
                onDisplay = true;
                dm.SetActiveText(this);
            }
            return false;
        }


        public override void Update(GameTime gameTime) {
            if (onDisplay && !finished) {
                if (messageStart == 0) {
                    messageStart = gameTime.TotalGameTime.Seconds;
                }
                if (messageStart + duration < gameTime.TotalGameTime.Seconds) {
                    finished = true;
                    onDisplay = false;
                }
            }
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
            if (onDisplay) {
                //TODO draw text
                int viewportWidth = _game.GraphicsDevice.Viewport.Width;
                Vector2 text_position = new Vector2(viewportWidth / 2f - stringSize.X / 2, margin);
                batch.DrawString(_font, _text, text_position, text_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);

                // Drawing a black outline around the text:
                batch.DrawString(_font, _text, new Vector2(text_position.X - text_outline_size, text_position.Y - text_outline_size), text_background_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
                batch.DrawString(_font, _text, new Vector2(text_position.X - text_outline_size, text_position.Y + text_outline_size), text_background_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
                batch.DrawString(_font, _text, new Vector2(text_position.X + text_outline_size, text_position.Y - text_outline_size), text_background_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
                batch.DrawString(_font, _text, new Vector2(text_position.X + text_outline_size, text_position.Y + text_outline_size), text_background_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);

            }
        }
    }
}
