using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Columns
{
    public class Column : DrawableGameElement
    {
        protected RopeGame _game;
        protected World _world;
        protected Vector2 _center;
        protected float _radius;

        public Body Body;
        protected Texture2D _columnTexture;
        protected Texture2D _lowerTexture;
        protected Texture2D _upperTexture;
        protected SpriteBatch _spriteBatch;
        protected bool multiTexture;

        public Column(RopeGame game, World world, Vector2 center, float radius, Texture2D texture)
        {
            _game = game;
            _world = world;
            _center = center;
            _radius = radius;
            _columnTexture = texture;
            multiTexture = false;
            Body = _world.CreateCircle(_radius, 0, _center, BodyType.Static);
        }

        public Column(RopeGame game, World world, Vector2 center, float radius, ColumnTextures texture) {
            _game = game;
            _world = world;
            _center = center;
            _radius = radius;
            _lowerTexture = texture.lower;
            _upperTexture = texture.upper;
            multiTexture = true;
            Body = _world.CreateCircle(_radius, 0, _center, BodyType.Static);
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera)
        {
            if (multiTexture) {
                Rectangle dstRec = camera.getSpriteRectangle(_center.X - 2*_radius, _center.Y + _radius, _radius * 4, _radius * 8);
                //batch.Draw(_lowerTexture, dstRec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, camera.getLayerDepth(dstRec.Y + dstRec.Height));
                batch.Draw(_upperTexture, dstRec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, camera.getLayerDepth(dstRec.Y + dstRec.Height));
            } else {
                Rectangle dstRec = camera.getScreenRectangle(_center.X - _radius, _center.Y - _radius, _radius * 2, _radius*2, true);
                batch.Draw(_columnTexture, dstRec, null, Color.Gray, 0f, Vector2.Zero, SpriteEffects.None, camera.getLayerDepth(dstRec.Y + dstRec.Height));
            }
        }
    }
}
