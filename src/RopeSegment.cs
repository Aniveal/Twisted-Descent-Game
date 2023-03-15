using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2;

public class RopeSegment {
    private Rope _rope;
    private RopeGame _game;
    private World _world;
    private Vector2 _position;
    private Vector2 _size;

    public Body Body;

    private const float RopeDensity = 1f;

    public RopeSegment(Rope rope, RopeGame game, World world, Vector2 position, Vector2 size) {
        _rope = rope;
        _game = game;
        _world = world;
        _position = position;
        _size = size;
        
        Body = _world.CreateRectangle(size.X, size.Y, RopeDensity, position, bodyType: BodyType.Dynamic);
        Body.LinearDamping = 0.1f;
        Body.AngularDamping = 0.1f;
    }

    public void Draw(SpriteBatch batch) {
        batch.Draw(_rope.BaseTexture, sourceRectangle: null, position: Body.Position, scale: 1f, rotation: Body.Rotation,
            color: Color.White, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 0f);
    }
}