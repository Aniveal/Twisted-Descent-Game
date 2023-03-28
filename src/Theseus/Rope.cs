using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Meridian2.Columns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;

namespace Meridian2.Theseus;

public class Rope : IGameObject
{
    private readonly GameScreen _gameScreen;
    private readonly Vector2 _pos;
    private readonly int _segmentCount;

    private Body _anchor;
    private List<RopeSegment> _segments;

    public Texture2D BaseTexture;
    private const int TextureHeight = 4;
    private const int TextureWidth = 2;

    public List<FragileColumn> _fragiles = new List<FragileColumn>();
    public TimeSpan lastBreak = TimeSpan.Zero;
    // 1 second cooldown between column breaks
    public TimeSpan breakCoolDown = new TimeSpan(0, 0, 1);

    public Rope(GameScreen gameScreen, Vector2 pos, int segmentCount)
    {
        _gameScreen = gameScreen;
        _pos = pos;
        _segmentCount = segmentCount;
    }

    public Vector2 GetEndPosition()
    {
        return _segments.Last().Body.GetWorldPoint(new Vector2((float)TextureWidth / 2, TextureHeight));
    }

    public RopeSegment LastSegment()
    {
        return _segments.Last();
    }

    private void CreateSegments(Vector2 pos, int num)
    {
        Debug.Assert(num >= 0, "Cannot create less than one rope segment!");
        _segments = new List<RopeSegment>(num);

        _anchor = _gameScreen.World.CreateCircle(1f, 1f, pos);

        // Disable rope collision of anchor
        foreach (Fixture fixture in _anchor.FixtureList)
        {
            fixture.CollisionGroup = -1;
        }

        for (int i = 0; i < num; i++)
        {
            RopeSegment segment = new RopeSegment(this, _gameScreen,
                new Vector2(pos.X, pos.Y + TextureHeight * i),
                new Vector2(TextureWidth, TextureHeight));
            segment.Initialize();

            _segments.Insert(i, segment);

            if (i > 0)
            {
                JointFactory.CreateRevoluteJoint(_gameScreen.World, _segments[i - 1].Body, _segments[i].Body,
                    Vector2.Zero);
                segment.SetPrevious(_segments[i - 1]);
                _segments[i - 1].SetNext(segment);
            }
            else
            {
                JointFactory.CreateRevoluteJoint(_gameScreen.World, _anchor, _segments[0].Body, Vector2.Zero);
            }
        }
    }

    public void Pull(GameTime gameTime)
    {

        if (_fragiles.Count > 0)
        {
            if (gameTime.TotalGameTime - lastBreak > breakCoolDown)
            {
                FragileColumn col = _fragiles.Last();
                _fragiles.RemoveAll(c => c == col);
                col.Break();
                lastBreak = gameTime.TotalGameTime;
            }
        }

    }

    private void CreateBaseTexture()
    {
        BaseTexture = new Texture2D(_gameScreen.Game.GraphicsDevice, TextureWidth, TextureHeight);
        Color[] data = new Color[BaseTexture.Width * BaseTexture.Height];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = Color.Red;
        }

        BaseTexture.SetData(data);
    }

    public void Initialize()
    {
        CreateBaseTexture();

        CreateSegments(_pos, _segmentCount);
    }

    public void LoadContent()
    {
        foreach (RopeSegment segment in _segments)
        {
            segment.LoadContent();
        }
    }

    public void Update(GameTime gameTime)
    {
        // Diagnostics.Instance.SetForce(_endAnchor.LinearVelocity.LengthSquared());
        //
        // MouseState mouse = Mouse.GetState();
        // if (mouse.LeftButton == ButtonState.Pressed) {
        //     Vector2 mouseDirection = (new Vector2(mouse.X, mouse.Y) - _endAnchor.Position) * 3;
        //     if (mouseDirection.LengthSquared() > 3000) {
        //         mouseDirection.Normalize();
        //         mouseDirection *= 3000;
        //     }
        //
        //     _endAnchor.ApplyForce(mouseDirection, _endAnchor.GetWorldPoint(new Vector2(1, 3)));
        // }
    }

    public void Draw(GameTime gameTime, SpriteBatch batch)
    {
        foreach (RopeSegment segment in _segments)
        {
            segment.Draw(gameTime, batch);
        }
    }
}