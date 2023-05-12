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

namespace Meridian2.Treasures {
    public class BodyWithText : DrawableGameElement {

        private RopeGame _game;
        private World _world;
        private Body _body;
        private String _text;
        public bool onDisplay = false;
        public bool finished = false;
        private bool destroyed = false;
        private SpriteFont _font;
        private int margin = 300;
        Vector2 stringSize;
        private float duration = 2; //duration in seconds
        private double messageStart = 0;

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
                batch.DrawString(_font, _text, new Vector2(viewportWidth / 2f - stringSize.X / 2, margin), new(170, 54, 54), 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
        }
    }
}
