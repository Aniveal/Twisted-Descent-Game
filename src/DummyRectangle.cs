using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2
{
    public class DummyRectangle
    {
        private RopeGame _game;
        private World _world;
        private Vector2 _pos;
        private int _w;
        private int _h;
        private Texture2D _texture;

        public Body body;

        public DummyRectangle(RopeGame game, World world, Vector2 pos, int width, int height, Texture2D texture)
        {
            _game = game;
            _world = world;
            _pos = pos;
            _w = width;
            _h = height;
            _texture = texture;

            body = _world.CreateRectangle(_w, _h, 0, _pos, 0, BodyType.Static);
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(_texture, new Rectangle((int)_pos.X, (int)_pos.Y, _w, _h), Color.Black);
        }
    }
}
