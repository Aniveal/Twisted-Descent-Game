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
    public class Column : TwoPhaseGameElement
    {
        protected RopeGame _game;
        protected World _world;
        protected Vector2 _center;
        protected float _radius;

        public Body Body;
        protected Texture2D _columnTexture;
        protected SpriteBatch _spriteBatch;

        public Column(RopeGame game, World world, Vector2 center, float radius, Texture2D texture)
        {
            _game = game;
            _world = world;
            _center = center;
            _radius = radius;
            _columnTexture = texture;


            Body = _world.CreateCircle(_radius, 0, _center, BodyType.Static);
        }

        public override void DrawFirst(GameTime gameTime, SpriteBatch batch, Camera camera)
        {
            Rectangle dstRec = camera.getScreenRectangle(_center.X - _radius, _center.Y - _radius, _radius * 2, _radius*2);
            batch.Draw(_columnTexture, dstRec, Color.Gray);
        }

        public override void DrawSecond(GameTime gameTime, SpriteBatch batch, Camera camera)
        {
            //TODO: update once sprites are available
        }
    }
}
