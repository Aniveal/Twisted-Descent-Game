using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Meridian2.Columns;
using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;

namespace Meridian2.Theseus;

public class Rope : DrawableGameElement
{
    private readonly RopeGame _game;
    private readonly World _world;
    private readonly Vector2 _pos;
    private readonly int _segmentCount;

    private Body _anchor;
    private List<RopeSegment> _segments;

    public Texture2D BaseTexture;
    public const float TextureHeight = 0.1f;
    public const float TextureWidth = 0.05f;

    private const int decayRate = 10; //a segment decays every decayRate ticks.
    private const int decayRange = 200; //the last decayRange segments can decay
    private int decayCount = 0;

    private Random decayRNG;
    public List<FragileColumn> _fragiles = new List<FragileColumn>();
    public TimeSpan lastBreak = TimeSpan.Zero;
    // 1 second cooldown between column breaks
    public TimeSpan breakCoolDown = new TimeSpan(0, 0, 1);

    public const float RopeJointLength = 0.001f;
    public const float RopeJointFrequency = 20;
    public const float RopeJointDampingRatio = 0.5f;

    private const int KeepActive = 400;

    public Rope(RopeGame game, World world, Vector2 pos, int segmentCount) {
        _game = game;
        _world = world;
        _pos = pos;
        _segmentCount = segmentCount;
        decayRNG = new Random();
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

        _anchor = _world.CreateCircle(1f, 1f, pos);

        // Disable rope collision of anchor
        foreach (Fixture fixture in _anchor.FixtureList)
        {
            fixture.CollisionGroup = -1;
        }

        for (int i = 0; i < num; i++)
        {
            RopeSegment segment = new RopeSegment(this, _world,
                new Vector2(pos.X, pos.Y + TextureHeight * i),
                new Vector2(TextureWidth, TextureHeight));
            segment.Initialize();

            _segments.Insert(i, segment);

            if (i > 0)
            {
                var joint = JointFactory.CreateDistanceJoint(_world, _segments[i - 1].Body, _segments[i].Body, new Vector2(TextureWidth / 2, TextureHeight),
                    new Vector2(TextureWidth / 2, 0));
                joint.Length = RopeJointLength;
                joint.Frequency = RopeJointFrequency;
                joint.DampingRatio = RopeJointDampingRatio;

                segment.SetPrevious(_segments[i - 1]);
                _segments[i - 1].SetNext(segment);
            }
            else
            {
                var joint = JointFactory.CreateDistanceJoint(_world, _anchor, _segments[0].Body, Vector2.Zero,
                    new Vector2(TextureWidth / 2, 0));
                joint.Length = RopeJointLength;
                joint.Frequency = RopeJointFrequency;
                joint.DampingRatio = RopeJointDampingRatio;
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
        BaseTexture = new Texture2D(_game.GraphicsDevice, 1, 2);
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

    public override void Update(GameTime gameTime)
    {
        //natural shortening
        if (decayCount >= decayRate) {
            int r = decayRNG.Next(decayRange);
            int c = _segments.Count;
            if (c - 2 - r > 0) { //only decay if chosen segment exists; - 2 because we never remove last segment
                RemoveSegment(c - 2 -r);
            }
        
            decayCount = 0;
        }
        decayCount ++;
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera)
    {
        foreach (RopeSegment segment in _segments)
        {
            segment.Draw(gameTime, batch, camera);
        }
    }

    public RopeSegment AppendSegment() {
        RopeSegment last = _segments.Last();
        
        RopeSegment nextLast = new RopeSegment(this, _world,
            new Vector2(_segments.Last().Body.Position.X, _segments.Last().Body.Position.Y),
            new Vector2(TextureWidth, TextureHeight));
        nextLast.Initialize();

        var joint = JointFactory.CreateDistanceJoint(_world, last.Body, nextLast.Body, new Vector2(TextureWidth / 2, TextureHeight),
            new Vector2(TextureWidth / 2, 0));
        joint.Length = RopeJointLength;
        joint.Frequency = RopeJointFrequency;
        joint.DampingRatio = RopeJointDampingRatio;

        _segments.Add(nextLast);
        
        nextLast.SetPrevious(last);
        last.SetNext(nextLast);

        int disableIndex = _segments.Count - KeepActive - 1;
        if (disableIndex >= 0) {
            _segments[disableIndex].Body.BodyType = BodyType.Static;
        }

        return nextLast;
    }

    //Remove last segment
    // !!! WARNING !!!
    // Does not update the player rope connection, needs to be done separately
    public bool RemoveSegment() {
        if (_segments.Count< 5) {
            return false;
        }
        RopeSegment last = _segments.Last();
        RopeSegment penultimate = last.previous;

        penultimate.SetNext(null);
        _segments.Remove(last);
        last.Destroy();
        
        int enableIndex = _segments.Count - KeepActive;
        if (enableIndex >= 0) {
            _segments[enableIndex].Body.BodyType = BodyType.Dynamic;
        }
        
        return true;
    }

    //remove any rope segment
    public bool RemoveSegment(RopeSegment segment) {
        if (segment == _segments.Last()) {
            return false; //removing last segment has to be called axplicitely ba player in order to adjust player joint
        }
        if (segment == _segments.First()) {
            return false; //cannot remove first segment
        }
        
        RopeSegment prev = segment.previous;
        RopeSegment next = segment.next;
        prev.next = next;
        next.previous = prev;
        _segments.Remove(segment);
        segment.Destroy();
        var joint = JointFactory.CreateDistanceJoint(_world, prev.Body, next.Body, new Vector2(TextureWidth / 2, TextureHeight),
            new Vector2(TextureWidth / 2, 0));
        joint.Length = RopeJointLength;
        joint.Frequency = RopeJointFrequency;
        joint.DampingRatio = RopeJointDampingRatio;
        return true;
    }

    //remove rope segment at given index, not first or last
    //If possible, use this instead of above
    public bool RemoveSegment(int index) {
        if (index < 1 || index >= _segments.Count - 1) {
            return false;
        }
        RopeSegment segment = _segments[index];

        RopeSegment prev = segment.previous;
        RopeSegment next = segment.next;
        prev.next = next;
        next.previous = prev;
        _segments.RemoveAt(index);
        segment.Destroy();
        var joint = JointFactory.CreateDistanceJoint(_world, prev.Body, next.Body, new Vector2(TextureWidth / 2, TextureHeight),
            new Vector2(TextureWidth / 2, 0));
        joint.Length = RopeJointLength;
        joint.Frequency = RopeJointFrequency;
        joint.DampingRatio = RopeJointDampingRatio;
        
        int enableIndex = _segments.Count - KeepActive;
        if (enableIndex >= 0) {
            _segments[enableIndex].Body.BodyType = BodyType.Dynamic;
        }
        
        return true;
    }
}