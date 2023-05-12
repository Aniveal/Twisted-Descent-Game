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
    public class BodyWithText : IDrawableObject {

        private World _world;
        private Body _body;
        private String _text;
        public bool onDisplay = false;
        public bool finished = false;

        public BodyWithText(Vector2 position, float radius, World world, String text) {
            _world = world;
            _text = text;
            _body = _world.CreateCircle(radius, 0, position);
            _body.OnCollision += OnCollision;
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
            if (onDisplay) {
                //TODO draw text
            }
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
    }
}
