using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using Meridian2.Theseus;

namespace Meridian2.Treasures {

    public class Chest : IDrawableObject
    {
        public Body body;
        protected RopeGame _game;
        protected World _world;
        protected Texture2D textureClosed;
        protected Texture2D textureOpen;
        protected int width = 2; //default width, sprite size hardcoded based on this
        protected int height = 1; //default height, sprite size hardcoded based on this
        public Vector2 pos;

        public bool open = false;


        //TODO: modify usage of width and height in Body and draw to fit to proportions of future sprite
        public Chest(RopeGame game, World world, Vector2 position) {
            _game = game;
            _world = world;
            pos = position;
            body = _world.CreateRectangle(width, height, 1, pos, -MathHelper.Pi /4);
        }

        public void LoadContent() {
            textureClosed = _game.Content.Load<Texture2D>("rectangle");
            textureOpen = _game.Content.Load<Texture2D>("rectangle");
        }
        
        public virtual void Loot() {

        }

        protected bool OnCollision(Fixture sender, Fixture other, Contact contact) {
            Body collider;
            if (sender.Body.Tag == this) {
                collider = other.Body;
            } else {
                collider = sender.Body;
            }
            if (collider.Tag == null) {
                return true;
            }
            ///player collision
            if (collider.Tag is Player) {
                //TODO: play opening animation?
                this.Loot();
            }
            return true;
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera)
        {
            Rectangle dstRec = new Rectangle();
            dstRec = camera.getScreenRectangle(pos.X - 0.7071f, pos.Y - 1.4142f, 2.1213f, 1.0607f);
            if (open) {
                batch.Draw(textureOpen, dstRec, Color.White);
            } else {
                batch.Draw(textureClosed, dstRec, Color.Red);
            }
        }
    }
}