using Meridian2.GameElements;
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
    //flag indicating that the item should be considered destroyed in the game
    public bool isDestroyed = false;
    //flag indicating the explosion animation has finished and the item can be cleaned up
    public bool hasExploded = false;

    private const float ExplosionDuration = 500f; //explosion in milliseconds
    private float explosionStart = 0;
    private bool exploding = false;

    private Texture2D _amphoraTexture;
    private Texture2D _explosionTexture;

    public Amphora(RopeGame game, World world, Vector2 position, float radius, Texture2D amphoraTexture, Texture2D explosionTexture) {
        _game = game;
        _world = world;
        _position = position;
        _radius = radius;
        slinged = false;
        _amphoraTexture = amphoraTexture;
        _explosionTexture = explosionTexture;
        Initialize();
    }

    public void Initialize() {
        _body = _world.CreateCircle(_radius, Density, _position, BodyType.Dynamic);
        _body.FixedRotation = true;
        _body.LinearDamping = 0.1f;
        _body.Tag = this;
        _body.OnCollision += OnCollision;
    }

    // DO NOT CALL FROM A PHYSICS CALLBACK
    public void DestroyBody() {
        _world.Remove(_body);
        isDestroyed = true;
        hasExploded = true; //TODO: do this once the explosion animation is finished
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
        if (isDestroyed) return;
        Vector2 aa = new Vector2(_position.X-ExplosionSize, _position.Y - ExplosionSize);
        Vector2 bb = new Vector2(_position.X + ExplosionSize, _position.Y + ExplosionSize);
        AABB aabb = new AABB(aa, bb);
        exploding = true;
        currentExplosionSize = ExplosionSize;
        _world.QueryAABB(ExplodeObject, aabb);
        isDestroyed = true;
        //TODO graphics
    }

    public void BiggerExplode() {
        if (isDestroyed) return;
        Vector2 aa = new Vector2(_position.X-ExplosionSize, _position.Y - ExplosionSize);
        Vector2 bb = new Vector2(_position.X + ExplosionSize, _position.Y + ExplosionSize);
        AABB aabb = new AABB(aa, bb);
        currentExplosionSize = 1.5f*ExplosionSize;
        _world.QueryAABB(ExplodeObject, aabb);
        isDestroyed = true;
    }

    protected bool OnCollision(Fixture sender, Fixture other, Contact contact) {
        Body collider = sender.Body.Tag == this ? other.Body : sender.Body;
        if (collider.Tag == null) {
            return true;
        }
        if (collider.Tag is RopeSegment) {
            slinged = true;
        }
        if (collider.Tag is Player) {
            slinged = false; //We want amphoras to be slung, not thrown
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
            ((Amphora)collider.Tag).isDestroyed = true;
            ((Amphora)collider.Tag).hasExploded = true;
            BiggerExplode();
        }

        return true;
    }

    public override void Update(GameTime gameTime)
    {
        if (exploding && explosionStart == 0) {
            explosionStart = gameTime.TotalGameTime.Milliseconds;
        }
        if (exploding && explosionStart + ExplosionDuration < gameTime.TotalGameTime.Milliseconds) {
            hasExploded = true;
        }
        if (slinged && _body.LinearVelocity.Length() < VelocityDangerThreshold) {
            slinged = false;
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        if (!isDestroyed) {
            Rectangle dstRec = camera.getScreenRectangle(_body.Position.X - _radius, _body.Position.Y - _radius, _radius * 2,
            _radius * 2, true);
            batch.Draw(_amphoraTexture, dstRec, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, camera.getLayerDepth(dstRec.Y));
        }
        if (exploding) {
            //TODO: improve once we have an animation
            Rectangle dstRec = camera.getScreenRectangle(_position.X - currentExplosionSize, _position.Y - currentExplosionSize, 
            2*currentExplosionSize, currentExplosionSize);
            batch.Draw(_explosionTexture, dstRec, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0.20f);
        }
    }
}