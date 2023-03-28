using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2;

public class Amphora : DrawableGameComponent {
    private World _world;
    private RopeGame _game;
    private Vector2 _position;
    private float _radius;

    private Body _body;

    private const float Density = 0.1f;

    public Amphora(RopeGame game, World world, Vector2 position, float radius) : base(game) {
        _game = game;
        _world = world;
        _position = position;
        _radius = radius;
    }

    public override void Initialize() {
        _body = _world.CreateCircle(_radius, Density, _position, BodyType.Dynamic);
        _body.FixedRotation = true;
        _body.LinearDamping = 0.1f;
        
        base.Initialize();
    }
    
    public void Draw(GameTime gameTime, SpriteBatch batch) {
        batch.Draw(_game.ColumnTexture, new Rectangle((int)(_body.Position.X - _radius), (int)(_body.Position.Y - _radius), (int)_radius * 2, (int)_radius*2), Color.Red);
    }
}