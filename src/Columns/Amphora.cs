﻿using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using tainicom.Aether.Physics2D.Collision;
using Meridian2.Theseus;
using Meridian2.Enemy;
using System;

namespace Meridian2.Columns;

public class Amphora : DrawableGameElement {
    private const float Density = 0.1f;

    private Body _body;
    private readonly RopeGame _game;
    private readonly Vector2 _position;
    private readonly float _radius;
    private readonly World _world;
    private bool slinged;
    private const float VelocityDangerThreshold = 0.2f;
    private const float ExplosionSize = 2f;
    private float currentExplosionSize;

    public Amphora(RopeGame game, World world, Vector2 position, float radius) {
        _game = game;
        _world = world;
        _position = position;
        _radius = radius;
        slinged = false;
        Initialize();
    }

    public void Initialize() {
        _body = _world.CreateCircle(_radius, Density, _position, BodyType.Dynamic);
        _body.FixedRotation = true;
        _body.LinearDamping = 0.1f;
        _body.Tag = this;
        _body.OnCollision += OnCollision;
    }

    public void Destroy() {
        //TODO
        //careful, cannot destroy body when called by any method in the OnCollision Callback chain
    }

    private bool ExplodeObject(Fixture f) {
        if (f.Body.Tag == null) {
            return true;
        }
        if (f.Body.Tag is Enemy.Enemy) {
            Enemy.Enemy e = (Enemy.Enemy)f.Body.Tag;
            if ((_position-e.Body.Position).Length() < currentExplosionSize) {
                e.Kill();
            }
            return true;
        }
        if (f.Body.Tag is FragileColumn) {
            FragileColumn c = (FragileColumn)f.Body.Tag;
            if ((_position-c.Position).Length() < currentExplosionSize) {
                c.Break();
            }
        }

        if (f.Body.Tag is Amphora) {
            Amphora a = (Amphora)f.Body.Tag;
            if ((_position-a._position).Length() < currentExplosionSize) {
                a.Explode();
            }
        }
        return true;
    }

    public void Explode() {
        Vector2 aa = new Vector2(_position.X-ExplosionSize, _position.Y - ExplosionSize);
        Vector2 bb = new Vector2(_position.X + ExplosionSize, _position.Y + ExplosionSize);
        AABB aabb = new AABB(aa, bb);
        currentExplosionSize = ExplosionSize;
        _world.QueryAABB(ExplodeObject, aabb);
        Destroy();
        //TODO graphics
    }

    public void BiggerExplode() {
        Vector2 aa = new Vector2(_position.X-ExplosionSize, _position.Y - ExplosionSize);
        Vector2 bb = new Vector2(_position.X + ExplosionSize, _position.Y + ExplosionSize);
        AABB aabb = new AABB(aa, bb);
        currentExplosionSize = 1.5f*ExplosionSize;
        _world.QueryAABB(ExplodeObject, aabb);
        Destroy();
    }

    protected bool OnCollision(Fixture sender, Fixture other, Contact contact) {
        Body collider = sender.Body.Tag == this ? other.Body : sender.Body;
        if (collider.Tag == null) {
            return true;
        }
        if (collider.Tag is RopeSegment) {
            slinged = true;
        }
        if (!slinged) return true;
        //Explode on collision if slinged
        if (collider.Tag is Tile) {
            if (!((Tile)collider.Tag).FinalPrototype.IsCliff) {
                Explode();
            }
            return true;
        }
        if (collider.Tag is Column) {
            Explode();
            if (collider.Tag is FragileColumn) {
                ((FragileColumn)collider.Tag).Break();
            }
            return true;
        }
        if (collider.Tag is Enemy2 || collider.Tag is Enemy.Enemy) {
            Explode();
            return true;
        }
        if (collider.Tag is Amphora) {
            ((Amphora)collider.Tag).Destroy();
            BiggerExplode();
        }

        return true;
    }

    public override void Update(GameTime gameTime)
    {
        if (!slinged) {
            return;
        }
        if (_body.LinearVelocity.Length() < VelocityDangerThreshold) {
            slinged = false;
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        var dstRec = camera.getScreenRectangle(_body.Position.X - _radius, _body.Position.Y - _radius, _radius * 2,
            _radius * 2, true);
        batch.Draw(_game.ColumnTexture, dstRec, Color.Red);
    }
}