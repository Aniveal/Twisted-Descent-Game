using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using tainicom.Aether.Physics2D.Collision;
using TwistedDescent.Theseus;
using TwistedDescent.Enemy;
using System;
using TwistedDescent.GameElements;

namespace TwistedDescent.Columns;

public class Amphora : DrawableGameElement {
    private const float Density = 0.1f;

    public Body _body;
    private readonly RopeGame _game;
    private readonly float _radius;
    private readonly World _world;
    private bool slinged;
    private const float VelocityDangerThreshold = 0.5f;
    private const float ExplosionSize = 3f;
    private float currentExplosionSize;
    //flag indicating that the item should be considered destroyed in the game
    public bool isDestroyed = false;
    public bool bodyIsDestroyed = false;
    //flag indicating the explosion animation has finished and the item can be cleaned up
    public bool hasExploded = false;

    private const float explosionSpeedThreshold = 2.5f;
    private const double ExplosionDuration = 0.5; //explosion in milliseconds
    private double explosionStart = 0;
    private bool exploding = false;

    private List<FragileColumn> toBreak = new List<FragileColumn>();

    private static Texture2D _amphoraTexture;
    private static Texture2D _explosionTexture;

    public int overCliff = 0;
    public Tile collidingCliff;
    protected const int fallSpeed = 100; //pixels per second
    protected float fallStart = 0;

    public Amphora(RopeGame game, World world, Vector2 position, float radius) {
        _game = game;
        _world = world;
        _radius = radius;
        slinged = false;
        Initialize(position);
    }

    public void Initialize(Vector2 pos) {
        _body = _world.CreateCircle(_radius, Density, pos, BodyType.Dynamic);
        _body.FixedRotation = true;
        _body.LinearDamping = 0.1f;
        _body.Tag = this;
        _body.OnCollision += OnCollision;
    }

    public void LoadContent() {
        if (_amphoraTexture == null) {
            _amphoraTexture = _game.Content.Load<Texture2D>("Sprites/amphora");
        }

        if (_explosionTexture == null) {
            _explosionTexture = _game.Content.Load<Texture2D>("Sprites/Effects/explosion");
        }
    }

    // DO NOT CALL FROM A PHYSICS CALLBACK
    public void DestroyBody() {
        if (bodyIsDestroyed) return;
        _world.Remove(_body);
        bodyIsDestroyed = true;
        isDestroyed = true;
    }

    private bool ExplodeObject(Fixture f) {
        if (f.Body.Tag == null || f.Body.Tag == this) {
            return true;
        }
        if (f.Body.Tag is Enemy.Enemy) {
            Enemy.Enemy e = (Enemy.Enemy)f.Body.Tag;
            if ((_body.Position-e.Body.Position).Length() < currentExplosionSize) {
                e.Kill(2);
            }
            return true;
        }
        if (f.Body.Tag is FragileColumn) {
            FragileColumn c = (FragileColumn)f.Body.Tag;
            if ((_body.Position-c.Position).Length() < currentExplosionSize) {
                if (!toBreak.Contains(c)) {
                    toBreak.Add(c);
                }
            }
        }

        if (f.Body.Tag is Amphora) {
            Amphora a = (Amphora)f.Body.Tag;
            if ((_body.Position-a._body.Position).Length() < currentExplosionSize) {
                a.Explode();
            }
        }
        return true;
    }

    public void Explode() {
        if (isDestroyed) return;
        Vector2 aa = new Vector2(_body.Position.X-ExplosionSize, _body.Position.Y - ExplosionSize);
        Vector2 bb = new Vector2(_body.Position.X + ExplosionSize, _body.Position.Y + ExplosionSize);
        AABB aabb = new AABB(aa, bb);
        exploding = true;
        isDestroyed = true;
        currentExplosionSize = ExplosionSize;
        _world.QueryAABB(ExplodeObject, aabb);
        SoundEngine.Instance.Amphora(_body.Position);
    }

    public void BiggerExplode() {
        if (isDestroyed) return;
        Vector2 aa = new Vector2(_body.Position.X-ExplosionSize, _body.Position.Y - ExplosionSize);
        Vector2 bb = new Vector2(_body.Position.X + ExplosionSize, _body.Position.Y + ExplosionSize);
        AABB aabb = new AABB(aa, bb);
        exploding = true;
        isDestroyed = true;
        currentExplosionSize = 1.5f*ExplosionSize;
        _world.QueryAABB(ExplodeObject, aabb);
        SoundEngine.Instance.Amphora(_body.Position);
    }

    protected bool OnCollision(Fixture sender, Fixture other, Contact contact) {
        Body collider = sender.Body.Tag == this ? other.Body : sender.Body;
        if (collider.Tag == null) {
            return true;
        }
        // if (collider.Tag is RopeSegment) {
        //     slinged = true;
        // }
        
        //Explode on collision if slinged
        if (collider.Tag is Tile) {
            if (_body.LinearVelocity.Length() > explosionSpeedThreshold) {
                if (!((Tile)collider.Tag).FinalPrototype.IsCliff) {
                    Explode();
                }
            }
            if (((Tile)collider.Tag).FinalPrototype.IsCliff) {
                if (collidingCliff == null) {
                    collidingCliff = (Tile)collider.Tag;
                } else if (_body.LinearVelocity.Length() > explosionSpeedThreshold) {
                    overCliff = 1;
                }
                return false;
            }
            if (overCliff == 1) {
                return false;
            }
            return true;
        }
        //Explode at this speed
        //Also applies for tiles, but need special case there due to cliffs...
        if (_body.LinearVelocity.Length() < explosionSpeedThreshold) {
            return true;
        }

        if (collider.Tag is Column) {
            Explode();
            return true;
        }
        if (collider.Tag is Enemy.Enemy) {
            Explode();
            return true;
        }
        if (collider.Tag is Amphora) {
            if (isDestroyed) {
                return true;
            }
            ((Amphora)collider.Tag).isDestroyed = true;
            ((Amphora)collider.Tag).hasExploded = true;
            BiggerExplode();
        }

        return true;
    }

    public override void Update(GameTime gameTime)
    {
        if (toBreak.Count > 0) {
            foreach (FragileColumn c in toBreak) {
                c.Break();
            }
            toBreak.Clear();
        }
        if (fallStart > 0) {
            if (gameTime.TotalGameTime.TotalSeconds - fallStart > 1) {
                DestroyBody();
                hasExploded = true;
            }
            return;
        }
        if (exploding && explosionStart == 0) {
            explosionStart = gameTime.TotalGameTime.TotalSeconds;
        }
        if (exploding && explosionStart + ExplosionDuration < gameTime.TotalGameTime.TotalSeconds) {
            hasExploded = true;
            exploding = false;
        }
        if (overCliff > 0) {
            fallStart = (float)gameTime.TotalGameTime.TotalSeconds;
            _body.Enabled = true;
            foreach (Fixture f in _body.FixtureList) {
                f.Friction = 5.0f;
            }
            return;
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        if (!isDestroyed) {
            Rectangle dstRec = camera.getSpriteRectangle(_body.Position.X - _radius, _body.Position.Y + _radius, _radius*2, _radius*3.2f);
            Color color = Color.White;
            if (fallStart > 0) {
                dstRec.Y += (int)((float)fallSpeed * (gameTime.TotalGameTime.TotalSeconds - fallStart));
                color = Color.White * (float)(1 - (gameTime.TotalGameTime.TotalSeconds - fallStart));
            }
            batch.Draw(_amphoraTexture, dstRec, null, color, 0, Vector2.Zero, SpriteEffects.None, camera.getLayerDepth(dstRec.Y + dstRec.Height));
        }
        if (exploding && !hasExploded) {
            var explosionFrame = 6 - (int) Math.Round((explosionStart + ExplosionDuration - gameTime.TotalGameTime.TotalSeconds) /
                ExplosionDuration * 6);

            Rectangle dstRec = camera.getScreenRectangle(_body.Position.X - currentExplosionSize, _body.Position.Y - currentExplosionSize*2,
            currentExplosionSize*2, currentExplosionSize*2);

            var frameWidth = (int)(_explosionTexture.Width / 7);

            batch.Draw(_explosionTexture, dstRec, new Rectangle(explosionFrame * frameWidth, 0, frameWidth, _explosionTexture.Height), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.95f);
        }
    }
}