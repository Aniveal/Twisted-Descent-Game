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
    public class Column
    {
        protected RopeGame _game;
        protected World _world;
        protected Vector2 _center;
        protected float _radius;

        public Body Body;
        protected Texture2D _columnTexture;

        public Column(RopeGame game, World world, Vector2 center, float radius, Texture2D texture)
        {
            _game = game;
            _world = world;
            _center = center;
            _radius = radius;
            _columnTexture = texture;

            Body = _world.CreateCircle(_radius, 0, _center, BodyType.Static);
        }

        public virtual void Draw(SpriteBatch batch)
        {
            batch.Draw(_columnTexture, new Rectangle((int)(_center.X - _radius), (int)(_center.Y - _radius), (int)_radius * 2, (int)_radius * 2), Color.Gray);
        }
    }
}
