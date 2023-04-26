using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Columns;

public class Amphora : DrawableGameElement {
    private const float Density = 0.1f;

    private Body _body;
    private readonly RopeGame _game;
    private readonly Vector2 _position;
    private readonly float _radius;
    private readonly World _world;

    public Amphora(RopeGame game, World world, Vector2 position, float radius) {
        _game = game;
        _world = world;
        _position = position;
        _radius = radius;

        Initialize();
    }

    public void Initialize() {
        _body = _world.CreateCircle(_radius, Density, _position, BodyType.Dynamic);
        _body.FixedRotation = true;
        _body.LinearDamping = 0.1f;
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        var dstRec = camera.getScreenRectangle(_body.Position.X - _radius, _body.Position.Y - _radius, _radius * 2,
            _radius * 2, true);
        batch.Draw(_game.ColumnTexture, dstRec, Color.Red);
    }
}