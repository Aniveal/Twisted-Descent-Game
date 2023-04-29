using System;
using Meridian2.Columns;
using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Theseus;

public class RopeSegment : DrawableGameElement {
    private const float RopeDensity = 0.2f;
    private const int ElecRange = 30; //range in segments
    private readonly Vector2 _position;
    private readonly Rope _rope;
    private readonly Vector2 _size;
    private readonly World _world;

    private bool _black;

    public Body Body;
    public bool ElecFromNext = false;
    public bool ElecFromPrev = false;
    public int ElecIntensity;
    public RopeSegment ElecSrcSegment;

    public bool IsElecSrc;
    public RopeSegment Next; //may be null if none
    public RopeSegment Previous; //may be null if none

    public RopeSegment(Rope rope, World world, Vector2 position, Vector2 size) {
        _rope = rope;
        _world = world;
        _position = position;
        _size = size;
    }

    public void Initialize() {
        Body = _world.CreateRectangle(_size.X, _size.Y, RopeDensity, _position, bodyType: BodyType.Dynamic);
        Body.LinearDamping = 0.5f;
        Body.AngularDamping = 0.5f;
        Body.Tag = this;
        _black = false;

        // Disable rope self collision
        foreach (var fixture in Body.FixtureList) fixture.CollisionGroup = -1;
    }

    public void SetPrevious(RopeSegment previous) {
        Previous = previous;
    }

    public void SetNext(RopeSegment next) {
        Next = next;
    }

    public void Electrify(RopeSegment src, int intensity, bool fromPrev) {
        if (intensity == 0) {
            if (fromPrev)
                Next?.DeElectrify(true);
            else
                Previous?.DeElectrify(false);
            return;
        }

        if (ElecSrcSegment != null && ElecSrcSegment.IsElecSrc && intensity < ElecIntensity) return;
        ElecSrcSegment = src;
        ElecIntensity = intensity;
        if (fromPrev)
            Next?.Electrify(src, intensity - 1, true);
        else
            Previous?.Electrify(src, intensity - 1, false);
    }

    public void DeElectrify(bool fromPrev) {
        if (ElecSrcSegment != null && ElecSrcSegment.IsElecSrc) {
            if (fromPrev)
                if (Previous != null && Previous.ElecSrcSegment == null) {
                    Previous?.Electrify(ElecSrcSegment, ElecIntensity - 1, false);
                }
            else
                if (Next != null && Next.ElecSrcSegment == null) {
                    Next?.Electrify(ElecSrcSegment, ElecIntensity - 1, true);
                }
            return;
        }

        ElecSrcSegment = null;
        ElecIntensity = 0;
        if (fromPrev) {
            if (Next == null || Next.ElecIntensity == 0) return;
            Next.DeElectrify(true);
        } else {
            if (Previous == null || Previous.ElecIntensity == 0) return;
            Previous.DeElectrify(false);
        }
    }

    public void LoadContent() {
        // Nothing to load
    }

    public override void Update(GameTime gameTime) {
        // Nothing to update
    }

    public void Destroy() {
        if ((Previous == null) | (Next == null)) return; //cannot destroy frist/last segment
        Previous.Next = Next;
        Next.Previous = Previous;
        _world.Remove(Body);
        if (ElecIntensity > 0) {
            if (IsElecSrc) {
                if (Next.IsElecSrc) {
                    Previous.Electrify(Next, Next.ElecIntensity, false);
                } else if (Previous.IsElecSrc) {
                    Previous.Electrify(Previous, Previous.ElecIntensity, true);
                } else {
                    Previous.DeElectrify(false);
                    Next.DeElectrify(true);
                }
            } else {
                if (Next.ElecIntensity > Previous.ElecIntensity)
                    Previous.Electrify(Next.ElecSrcSegment, Next.ElecIntensity - 1, false);
                else if (Next.ElecIntensity < Previous.ElecIntensity)
                    Next.Electrify(Previous.ElecSrcSegment, Previous.ElecIntensity - 1, true);
            }
        }
        //TODO: update electrification of neighbors
        //TODO: there probably is something else to do to destroy this object
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        // Nothing to draw
    }

    /**
     * column: The column making the callback
     * collision: True if collision, false if separation
     * unique: true if first colliding segment or last separation
     * 
     * TODO: change unique to encompass situation where column is wrapped on two separate occasions, will currently not behave as expected by player
     */
    public void ColumnCallback(ActivableColumn column, bool collision, bool unique) {
        _black = collision;
        if (column is FragileColumn) {
            if (collision & unique) _rope.Fragiles.Add((FragileColumn)column);
            if (!collision & unique)
                //TODO: change this when changing unique trigger
                _rope.Fragiles.RemoveAll(x => x == column);
        }

        if (column is ElectricColumn) {
            if (collision) {
                IsElecSrc = true;
                ElecIntensity = ElecRange;
                ElecSrcSegment = this;
                Next?.Electrify(this, ElecIntensity - 1, true);
                Previous?.Electrify(this, ElecIntensity - 1, false);
            } else {
                IsElecSrc = false;
                ElecIntensity = 0;
                ElecSrcSegment = null;
                Next?.DeElectrify(true);
                Previous?.DeElectrify(false);
            }
        }
    }
}