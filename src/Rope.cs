using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;

namespace Meridian2;

public class Rope {
    private readonly RopeGame _game;
    private readonly World _world;

    private Body _anchor;
    private Body _endAnchor;
    private List<RopeSegment> _segments;

    public Texture2D BaseTexture;
    private const int TextureHeight = 8;
    private const int TextureWidth = 2;

    public Rope(RopeGame game, World world, Vector2 pos, int segments) {
        this._game = game;
        this._world = world;

        CreateBaseTexture();

        CreateSegments(pos, segments);
    }

    private void CreateSegments(Vector2 pos, int num) {
        Debug.Assert(num >= 0, "Cannot create less than one rope segment!");
        _segments = new List<RopeSegment>(num);

        _anchor = _world.CreateCircle(1f, 1f, pos);
        for (int i = 0; i < num; i++) {
            RopeSegment segment = new RopeSegment(this, _game, _world,
                new Vector2(pos.X, pos.Y + TextureHeight * i),
                new Vector2(TextureWidth, TextureHeight));

            _segments.Insert(i, segment);

            if (i > 0) {
                JointFactory.CreateRevoluteJoint(_world, _segments[i - 1].Body, _segments[i].Body, Vector2.Zero);
            }
            else {
                JointFactory.CreateRevoluteJoint(_world, _anchor, _segments[0].Body, Vector2.Zero);
            }
        }
        
        // Attach an anchor to the end of the rope to pull on
        _endAnchor = _world.CreateCircle(3f, 0.1f, new Vector2(pos.X, pos.Y + TextureHeight * num), BodyType.Dynamic);
        JointFactory.CreateRevoluteJoint(_world, _segments.Last().Body, _endAnchor, Vector2.Zero);
    }

    private void CreateBaseTexture() {
        BaseTexture = new Texture2D(_game.GraphicsDevice, TextureWidth, TextureHeight);
        Color[] data = new Color[BaseTexture.Width * BaseTexture.Height];
        for (int i = 0; i < data.Length; i++) {
            data[i] = Color.Red;
        }

        BaseTexture.SetData(data);
    }

    public void Update(GameTime gameTime) {
        MouseState mouse = Mouse.GetState();
        if (mouse.LeftButton == ButtonState.Pressed) {
            _endAnchor.ApplyForce((new Vector2(mouse.X, mouse.Y) - _endAnchor.Position) * 30,
                _endAnchor.GetWorldPoint(new Vector2(1, 3)));
        }
    }

    public void Draw(SpriteBatch batch) {
        foreach (RopeSegment segment in _segments) {
            segment.Draw(batch);
        }
    }
}