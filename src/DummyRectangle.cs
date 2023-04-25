using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2; 

public class DummyRectangle {
    private RopeGame _game;
    private readonly int _h;
    private readonly Vector2 _pos;
    private readonly Texture2D _texture;
    private readonly int _w;
    private readonly World _world;

    public Body Body;

    //position = middle of the rectangle
    public DummyRectangle(RopeGame game, World world, Vector2 pos, int width, int height, Texture2D texture) {
        _game = game;
        _world = world;
        _pos = pos;
        _w = width;
        _h = height;
        _texture = texture;

        Body = _world.CreateRectangle(_w, _h, 0, _pos);
    }

    public void Draw(SpriteBatch batch) {
        batch.Draw(_texture, new Rectangle((int)_pos.X - _w / 2, (int)_pos.Y - _h / 2, _w, _h), Color.Black);
    }
}