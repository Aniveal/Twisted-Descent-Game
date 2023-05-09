using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Meridian2.Columns;
using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;

namespace Meridian2.Theseus;

public class Rope : DrawableGameElement {
    public const float TextureHeight = 0.1f;
    public const float TextureWidth = 0.05f;

    private const int DecayRate = 10; //a segment decays every decayRate ticks.
    private const int DecayRange = 200; //the last decayRange segments can decay

    public const float RopeJointLength = 0.001f;
    public const float RopeJointFrequency = 20;
    public const float RopeJointDampingRatio = 0.5f;

    private const int MinFps = 60;
    private int KeepActive = 400;
    private readonly RopeGame _game;
    private readonly Vector2 _pos;
    private readonly int _segmentCount;
    private readonly World _world;

    private Body _anchor;
    private int _decayCount;

    private readonly Random _decayRng;
    private List<RopeSegment> _segments;
    private List<RopeSegment> _suspendedSegments;

    private Texture2D _pixel;
    private readonly Color _ropeColor = new(170, 54, 54);
    
    // 1 second cooldown between column breaks
    public TimeSpan BreakCoolDown = new(0, 0, 1);
    public List<FragileColumn> Fragiles = new();
    public TimeSpan LastBreak = TimeSpan.Zero;
    private FragileColumn _breakingColumn;

    public Rope(RopeGame game, World world, Vector2 pos, int segmentCount) {
        _game = game;
        _world = world;
        _pos = pos;
        _segmentCount = segmentCount;
        _decayRng = new Random();
        _suspendedSegments = new List<RopeSegment>();
    }

    public Vector2 GetEndPosition() {
        return _segments.Last().Body.GetWorldPoint(new Vector2(TextureWidth / 2, TextureHeight));
    }

    public RopeSegment LastSegment() {
        return _segments.Last();
    }

    private void CreateSegments(Vector2 pos, int num) {
        Debug.Assert(num >= 0, "Cannot create less than one rope segment!");
        _segments = new List<RopeSegment>(num);

        _anchor = _world.CreateCircle(1f, 1f, pos);

        // Disable rope collision of anchor
        foreach (var fixture in _anchor.FixtureList) fixture.CollisionGroup = -1;

        for (var i = 0; i < num; i++) {
            var segment = new RopeSegment(this, _world,
                new Vector2(pos.X, pos.Y + TextureHeight * i),
                new Vector2(TextureWidth, TextureHeight));
            segment.Initialize();

            _segments.Insert(i, segment);

            if (i > 0) {
                var joint = JointFactory.CreateDistanceJoint(_world, _segments[i - 1].Body, _segments[i].Body,
                    new Vector2(TextureWidth / 2, TextureHeight),
                    new Vector2(TextureWidth / 2, 0));
                joint.Length = RopeJointLength;
                joint.Frequency = RopeJointFrequency;
                joint.DampingRatio = RopeJointDampingRatio;

                segment.SetPrevious(_segments[i - 1]);
                _segments[i - 1].SetNext(segment);
            } else {
                var joint = JointFactory.CreateDistanceJoint(_world, _anchor, _segments[0].Body, Vector2.Zero,
                    new Vector2(TextureWidth / 2, 0));
                joint.Length = RopeJointLength;
                joint.Frequency = RopeJointFrequency;
                joint.DampingRatio = RopeJointDampingRatio;
            }
        }
    }

    private void UpdateBreakingColumn(GameTime gameTime) {
        if (gameTime.TotalGameTime - LastBreak > BreakCoolDown)
        {
            //Traverse linked list and break last column
            RopeSegment currentSegment = _segments.Last();
            while (true)
            {
                if (currentSegment == null)
                    break;

                else if (currentSegment.touchingColumn == null)
                {
                    currentSegment = currentSegment.Previous;
                    continue;
                }
                else if (currentSegment.touchingColumn.GetType() == typeof(FragileColumn))
                {
                    break;
                }
                else currentSegment = currentSegment.Previous;

            }

            if (currentSegment != null)
            {
                FragileColumn col = currentSegment.touchingColumn as FragileColumn;
                _breakingColumn = col;
            } else {
                _breakingColumn = null;
            }
            
            foreach (var fragileColumn in _game._gameScreen.ColumnsManager.Columns.OfType<FragileColumn>()) {
                fragileColumn.hideTooltip();
            }

            _breakingColumn?.showTooltip();
        }
    }
    
    public void Pull(GameTime gameTime) {
        if (gameTime.TotalGameTime - LastBreak > BreakCoolDown)
        {
            if (_breakingColumn != null) {
                _breakingColumn.Break();
                LastBreak = gameTime.TotalGameTime;
                _breakingColumn = null;
            }
        }
    }

    private void CreateBaseTexture() {
        _pixel = new Texture2D(_game.GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void Initialize() {
        CreateBaseTexture();

        CreateSegments(_pos, _segmentCount);
    }

    public void LoadContent() {
        foreach (var segment in _segments) segment.LoadContent();
    }

    private void IncreaseSuspension(int amount) {
        for (int i = 0; i < amount; i++) {
            var disableIndex = _segments.Count - KeepActive - 1 - i;
            if (disableIndex >= 0) _segments[disableIndex].Body.BodyType = BodyType.Static;
        }

        KeepActive -= amount;
    }
    
    private void DecreaseSuspension(int amount) {
        for (int i = 0; i < amount; i++) {
            var enableIndex = _segments.Count - KeepActive + i;
            if (enableIndex >= 0 && enableIndex < _segments.Count) _segments[enableIndex].Body.BodyType = BodyType.Dynamic;
        }

        KeepActive += amount;
    }
    
    private void DynamicSegmentSuspension() {
        if (Diagnostics.Instance.Fps <= MinFps) {
            // IncreaseSuspension(5);
        } else {
            // DecreaseSuspension(1);
        }
    }
    
    public override void Update(GameTime gameTime) {
        UpdateBreakingColumn(gameTime);
        DynamicSegmentSuspension();

        //natural shortening
        if (_decayCount >= DecayRate) {
            var r = _decayRng.Next(DecayRange);
            var c = _segments.Count;
            if (c - 2 - r > 0) //only decay if chosen segment exists; - 2 because we never remove last segment
                RemoveSegment(c - 2 - r);

            _decayCount = 0;
        }

        _decayCount++;
    }

    public void DrawPlayerConnection(GameTime gameTime, SpriteBatch batch, Camera camera, Vector2 connection) {
        var pos1 = camera.getScreenPoint(_segments.Last().Body.Position);
        var pos2 = camera.getScreenPoint(connection);

        float distance = Vector2.Distance(pos2, pos1);
        float angle = (float)Math.Atan2((double)pos1.Y - (double)pos2.Y, (double)pos1.X - (double)pos2.X);

        batch.Draw(_pixel, pos2, null, _ropeColor, angle, Vector2.Zero, new Vector2(distance, 2),
            SpriteEffects.None, camera.getLayerDepth(pos2.Y));
    }
    
    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        // Draw each segment
        foreach (var segment in _segments) {
            if (segment.Next == null) {
                continue;
            }

            var pos1 = camera.getScreenPoint(segment.Body.Position);
            var pos2 = camera.getScreenPoint(segment.Next.Body.Position);

            float distance = Vector2.Distance(pos2, pos1);
            float angle = (float)Math.Atan2((double)pos1.Y - (double)pos2.Y, (double)pos1.X - (double)pos2.X);

            Color color = _ropeColor;
            if (segment.ElecIntensity > 0) {
                color = Color.Yellow;
            }
            
            batch.Draw(_pixel, pos2, null, color, angle, Vector2.Zero, new Vector2(distance, 2),
                SpriteEffects.None, camera.getLayerDepth(pos2.Y));
        }
    }

    public RopeSegment AppendSegment() {
        var last = _segments.Last();

        var nextLast = new RopeSegment(this, _world,
            new Vector2(_segments.Last().Body.Position.X, _segments.Last().Body.Position.Y),
            new Vector2(TextureWidth, TextureHeight));
        nextLast.Initialize();

        var joint = JointFactory.CreateDistanceJoint(_world, last.Body, nextLast.Body,
            new Vector2(TextureWidth / 2, TextureHeight),
            new Vector2(TextureWidth / 2, 0));
        joint.Length = RopeJointLength;
        joint.Frequency = RopeJointFrequency;
        joint.DampingRatio = RopeJointDampingRatio;

        _segments.Add(nextLast);

        nextLast.SetPrevious(last);
        last.SetNext(nextLast);

        var disableIndex = _segments.Count - KeepActive - 1;
        if (disableIndex >= 0) _segments[disableIndex].Body.BodyType = BodyType.Static;

        return nextLast;
    }

    public bool RemoveLastSegment() {
        var removeIndex = _segments.Count - 2;

        if (removeIndex > 0) {
            return RemoveSegment(removeIndex);
        }

        return false;
    }

    //remove rope segment at given index, not first or last
    //If possible, use this instead of above
    public bool RemoveSegment(int index) {
        if (index < 1 || index >= _segments.Count - 1) return false;
        var segment = _segments[index];

        var prev = segment.Previous;
        var next = segment.Next;
        _segments.RemoveAt(index);
        segment.Destroy();
        var joint = JointFactory.CreateDistanceJoint(_world, prev.Body, next.Body,
            new Vector2(TextureWidth / 2, TextureHeight),
            new Vector2(TextureWidth / 2, 0));
        joint.Length = RopeJointLength;
        joint.Frequency = RopeJointFrequency;
        joint.DampingRatio = RopeJointDampingRatio;

        var enableIndex = _segments.Count - KeepActive;
        if (enableIndex >= 0) _segments[enableIndex].Body.BodyType = BodyType.Dynamic;

        return true;
    }
}