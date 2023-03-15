using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2;

public class Rope {
    private readonly RopeGame _game;
    private readonly World _world;

    private List<RopeSegment> _segments;

    public Texture2D BaseTexture;
    private const int TextureHeight = 10;
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

        for (int i = 0; i < num; i++) {
            RopeSegment segment = new RopeSegment(this, _game, _world,
                new Vector2(pos.X, pos.Y + TextureHeight * i),
                new Vector2(TextureWidth, TextureHeight));

            _segments.Insert(i, segment);
        }
        
        _segments.Last().Body.ApplyLinearImpulse(new Vector2(-50, 10));
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
        updateForce(gameTime);
        updatePhysics(gameTime);
    }

    private void updatePhysics(GameTime gameTime) {
    }

    private void updateForce(GameTime gameTime) {
    }

    public void Draw(SpriteBatch batch) {
        foreach (RopeSegment segment in _segments) {
            segment.Draw(batch);
        }
    }
}