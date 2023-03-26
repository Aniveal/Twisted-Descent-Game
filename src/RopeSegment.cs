using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Data.Common;
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

    private bool _elecSrc = false;
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

    public void ElectrifyByPrevious(int intensity)
    {
        if (_elecSrc)
        {
            return;
        }
        if (elecFromNext)
        {
            if (intensity > elecIntensity)
            {
                elecIntensity = intensity;
                elecFromNext = false;
                elecFromPrev = true;
                next.ElectrifyByPrevious(intensity - 1);
            }
            return;
        }
        if (intensity > elecIntensity)
        {
            elecIntensity = intensity;
            elecFromPrev = true;
            next?.ElectrifyByPrevious(intensity - 1);
            return;
        }
        if (elecFromPrev && intensity < elecIntensity)
        {
            elecIntensity = intensity;
            if (next != null)
            {
                if (next.elecFromNext && next.elecIntensity - 1 > intensity) 
                {
                    elecIntensity = next.elecIntensity - 1;
                    elecFromNext = true;
                    elecFromPrev = false;
                    previous.ElectrifyByNext(elecIntensity - 1);
                    return;
                }
                if (intensity > 0)
                {
                    next.ElectrifyByPrevious(intensity - 1);
                }
                else
                {
                    elecFromPrev = false;
                    next.ElectrifyByPrevious(0);
                }
            }
        }
    }

    public void ElectrifyByNext(int intensity)
    {
        if (_elecSrc)
        {
            return;
        }
        if (elecFromPrev)
        {
            if (intensity > elecIntensity)
            {
                elecIntensity = intensity;
                elecFromPrev = false;
                elecFromNext = true;
                previous.ElectrifyByNext(intensity - 1);
            }
            return;
        }
        if (intensity > elecIntensity)
        {
            elecIntensity = intensity;
            elecFromNext = true;
            previous?.ElectrifyByNext(intensity - 1);
            return;
        }
        if (elecFromNext && intensity < elecIntensity)
        {
            elecIntensity = intensity;
            if (previous != null)
            {
                if (previous.elecFromPrev && previous.elecIntensity - 1 > intensity)
                {
                    elecIntensity = previous.elecIntensity - 1;
                    elecFromPrev = true;
                    elecFromNext = false;
                    next.ElectrifyByNext(elecIntensity - 1);
                    return;
                }
                if (intensity > 0)
                {
                    previous.ElectrifyByPrevious(intensity - 1);
                }
                else
                {
                    elecFromNext = false;
                    previous.ElectrifyByPrevious(0);
                }
            }
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
            color: Color.Yellow, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 0f);
            return;
        }
        if (_black)
            {
            batch.Draw(_rope.BaseTexture, sourceRectangle: null, position: Body.Position, scale: 1f, rotation: Body.Rotation,
            color: Color.Black, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 0f);
            return;
            }
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
                _elecSrc = true;
                elecFromNext = false;
                elecFromPrev = false;
                elecIntensity = _elecRange;
                next?.ElectrifyByPrevious(elecIntensity - 1);
                previous?.ElectrifyByNext(elecIntensity - 1);
            }
            else
            {
                _elecSrc = false;
                if (next != null && (next._elecSrc | next.elecFromNext))
                {
                    elecIntensity = next.elecIntensity - 1;
                    previous?.ElectrifyByNext(next.elecIntensity);
                } else if (previous != null && (previous._elecSrc | previous.elecFromPrev))
                {
                    elecIntensity = previous.elecIntensity - 1;
                    next?.ElectrifyByPrevious(previous.elecIntensity);
                } else
                {
                    elecIntensity = 0;
                    next?.ElectrifyByPrevious(0);
                    previous?.ElectrifyByNext(0);
                }
            }            
        }
    }
}