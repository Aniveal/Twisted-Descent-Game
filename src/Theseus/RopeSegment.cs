using Meridian2.Columns;
using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Data.Common;
using System.Reflection.Metadata.Ecma335;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Theseus;

public class RopeSegment : DrawableGameElement
{
    private readonly Rope _rope;
    private readonly World _world;
    private readonly Vector2 _position;
    private readonly Vector2 _size;
    public RopeSegment previous = null; //may be null if none
    public RopeSegment next = null; //may be null if none

    public Body Body;

    private const float RopeDensity = 0.1f;

    private bool _black;

    public bool isElecSrc = false;
    public RopeSegment elecSrcSegment = null;
    private const int _elecRange = 10; //range in segments
    public int elecIntensity = 0;
    public bool elecFromPrev = false;
    public bool elecFromNext = false;

    public RopeSegment(Rope rope, World world, Vector2 position, Vector2 size)
    {
        _rope = rope;
        _world = world;
        _position = position;
        _size = size;
    }

    public void Initialize()
    {
        Body = _world.CreateRectangle(_size.X, _size.Y, RopeDensity, _position, bodyType: BodyType.Dynamic);
        Body.LinearDamping = 0.5f;
        Body.AngularDamping = 0.5f;
        Body.Tag = this;
        _black = false;

        // Disable rope self collision
        foreach (Fixture fixture in Body.FixtureList)
        {
            fixture.CollisionGroup = -1;
        }
    }

    public void SetPrevious(RopeSegment previous)
    {
        this.previous = previous;
    }

    public void SetNext(RopeSegment next)
    {
        this.next = next;
    }

    public void Electrify(RopeSegment src, int intensity, bool fromPrev)
    {
        if (intensity == 0) { return; }

        if (elecSrcSegment != null && elecSrcSegment.isElecSrc && intensity < elecIntensity)
        {
            return;
        }
        elecSrcSegment = src;
        elecIntensity = intensity;
        if (fromPrev)
        {
            next?.Electrify(src, intensity - 1, true);
        }
        else
        {
            previous?.Electrify(src, intensity - 1, false);
        }

    }

    public void DeElectrify(bool fromPrev)
    {
        if (elecSrcSegment != null && elecSrcSegment.isElecSrc)
        {
            if (fromPrev)
            {
                previous.Electrify(elecSrcSegment, elecIntensity - 1, false);
            }
            else
            {
                next.Electrify(elecSrcSegment, elecIntensity - 1, true);
            }
            return;
        }
        elecSrcSegment = null;
        elecIntensity = 0;
        if (fromPrev)
        {
            next?.DeElectrify(true);
        }
        else
        {
            previous?.DeElectrify(false);
        }
    }

    public void LoadContent()
    {
        // Nothing to load
    }

    public override void Update(GameTime gameTime)
    {
        // Nothing to update
    }

    public void Destroy() {
        _world.Remove(Body);
        //TODO: update electrification of neighbors
        //TODO: there probably is something else to do to destroy this object
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        Color ropeColor = Color.White;
        if (elecIntensity > 0) {
            ropeColor = Color.Black;
        }

        Rectangle dstRectangle = camera.getScreenRectangle(Body.Position.X, Body.Position.Y, _size.X, _size.Y);
        batch.Draw(_rope.BaseTexture, dstRectangle, sourceRectangle: null, color: ropeColor, rotation: Body.Rotation,
            origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 0f);
    }

    /**
     * column: The column making the callback
     * collision: True if collision, false if separation
     * unique: true if first colliding segment or last separation
     * 
     * TODO: change unique to encompass situation where column is wrapped on two separate occasions, will currently not behave as expected by player
     */
    public void ColumnCallback(ActivableColumn column, bool collision, bool unique)
    {
        _black = collision;
        if (column is FragileColumn)
        {
            if (collision & unique)
            {
                _rope._fragiles.Add((FragileColumn)column);
            }
            if (!collision & unique)
            {
                //TODO: change this when changing unique trigger
                _rope._fragiles.RemoveAll(x => x == column);
            }
        }
        if (column is ElectricColumn)
        {
            if (collision)
            {
                isElecSrc = true;
                elecIntensity = _elecRange;
                elecSrcSegment = this;
                next?.Electrify(this, elecIntensity - 1, true);
                previous?.Electrify(this, elecIntensity - 1, false);
            }
            else
            {
                isElecSrc = false;
                elecIntensity = 0;
                elecSrcSegment = null;
                next?.DeElectrify(true);
                previous?.DeElectrify(false);
            }
        }
    }
}