using Meridian2.Columns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Data.Common;
using System.Reflection.Metadata.Ecma335;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2;

public class RopeSegment : IGameObject {
    private readonly Rope _rope;
    private readonly GameScreen _gameScreen;
    private readonly Vector2 _position;
    private readonly Vector2 _size;
    public RopeSegment previous = null; //may be null if none
    public RopeSegment next = null; //may be null if none

    public Body Body;

    private const float RopeDensity = 0.01f;

    private bool _black;

    public bool isElecSrc = false;
    public RopeSegment elecSrcSegment = null;
    private const int _elecRange = 10; //range in segments
    public int elecIntensity = 0;
    public bool elecFromPrev = false;
    public bool elecFromNext = false;

    public RopeSegment(Rope rope, GameScreen gameScreen, Vector2 position, Vector2 size) {
        _rope = rope;
        _gameScreen = gameScreen;
        _position = position;
        _size = size;
    }

    public void Initialize() {
        Body = _gameScreen.World.CreateRectangle(_size.X, _size.Y, RopeDensity, _position, bodyType: BodyType.Dynamic);
        Body.LinearDamping = 1f;
        Body.AngularDamping = 2f;
        Body.Tag = this;
        _black = false;

        // Disable rope self collision
        foreach (Fixture fixture in Body.FixtureList) {
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
        } else
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
            } else
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
        } else
        {
            previous?.DeElectrify(false);
        }
    }

    public void LoadContent() {
        // Nothing to load
    }

    public void Update(GameTime gameTime) {
        // Nothing to update
    }

    public void Draw(GameTime gameTime, SpriteBatch batch) {
        if (elecIntensity > 0)
        {
           
            batch.Draw(_rope.BaseTexture, sourceRectangle: null, position: Body.Position, scale: 1f, rotation: Body.Rotation,
            color: Color.Black, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 0f);
            return;
        }
        //if (_black)
        //    {
        //    batch.Draw(_rope.BaseTexture, sourceRectangle: null, position: Body.Position, scale: 1f, rotation: Body.Rotation,
        //    color: Color.Black, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 0f);
        //    return;
        //    }
        batch.Draw(_rope.BaseTexture, sourceRectangle: null, position: Body.Position, scale: 1f, rotation: Body.Rotation,
            color: Color.White, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 0f);
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