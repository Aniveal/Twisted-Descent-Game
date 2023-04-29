using System;
using System.Collections.Generic;
using Meridian2.GameElements;
using Meridian2.Theseus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace Meridian2.Enemy; 

public class Enemy2 : DrawableGameElement {
    private int CrushDuration = 16;
    private int CrushThreshold = 4;
    private const float WallKillVelocity = 1f;
    private readonly Point _enemySize = new(1, 2);
    private readonly RopeGame _game;
    private readonly float _angerDistance = 6f;
    private readonly float _followDistance = 3f;
    private float _enemyForce = 0.001f;

    private Texture2D _idle;
    private Vector2 _input = Vector2.Zero;

    private bool _isWalking;
    private readonly Player _player;
    private Texture2D _runningB;
    private Texture2D _runningF;
    private Texture2D _runningL;
    private Texture2D _runningR;
    private readonly World _world;

    public Body Body;
    public int Colliding;
    public int CollidingSegments;
    public int Crushed;
    public int overCliff = 0;
    public Tile collidingCliff;
    private const int fallSpeed = 100; //pixels per second
    private float fallStart = 0;
    public bool IsAlive = true;
    public Vector2 Orientation;

    private Boolean _canChase;
    private Boolean _canKite;
    private Boolean _canShoot;
    private Boolean _isDurable;
    private Boolean _isImmuneToElectricity;
    private Boolean _isFast;


    public Enemy2(RopeGame game, World world, Player player) {
        _game = game;
        _world = world;
        _player = player;
    }

    public void Initialize(Vector2 initpos, Boolean canChase, Boolean canKite, Boolean CanShoot, Boolean isDurable, Boolean isImmuneToElectricity, Boolean isFast) {
        Body = _world.CreateEllipse((float)_enemySize.X / 4, (float)_enemySize.X / 8, 20, 0.01f,
            initpos, 0f, BodyType.Dynamic);
        Body.FixedRotation = true;
        Body.LinearDamping = 0.5f;
        Body.Tag = this;
        Body.OnCollision += OnCollision;
        Body.OnSeparation += OnSeparation;

        _canChase = canChase;
        _canKite = canKite;
        _canShoot = CanShoot;
        _isDurable = isDurable;
        _isImmuneToElectricity = isImmuneToElectricity;
        _isFast = isFast;
        
        if (_isDurable)
        {
            CrushDuration *= 2;
            CrushThreshold *= 2;
        }
        if (_isFast)
        {
            _enemyForce *= 2;
        }
    }

    public void LoadContent()
    {

        if (_isDurable)
        {
            _idle = _game.Content.Load<Texture2D>("Sprites/Enemies/Minotaur/minotaur_idle");
            _runningL = _game.Content.Load<Texture2D>("Sprites/Enemies/Minotaur/minotaur_idle");
            _runningR = _game.Content.Load<Texture2D>("Sprites/Enemies/Minotaur/minotaur_idle");
            _runningF = _game.Content.Load<Texture2D>("Sprites/Enemies/Minotaur/minotaur_idle");
            _runningB = _game.Content.Load<Texture2D>("Sprites/Enemies/Minotaur/minotaur_idle");
        }
        
        else 
        {
        _idle = _game.Content.Load<Texture2D>("Sprites/Enemies/idle_enemy");
        _runningL = _game.Content.Load<Texture2D>("Sprites/Enemies/idle_enemy");
        _runningR = _game.Content.Load<Texture2D>("Sprites/Enemies/idle_enemy");
        _runningF = _game.Content.Load<Texture2D>("Sprites/Enemies/idle_enemy");
        _runningB = _game.Content.Load<Texture2D>("Sprites/Enemies/idle_enemy");
        }
    }

    public void Electrify() {
        if (!_isImmuneToElectricity)
        {
            Kill();
        }
        //TODO: play animation (change color to yellow?), take damage
    }

    public void Kill() {
        IsAlive = false;
    }

    protected bool OnCollision(Fixture sender, Fixture other, Contact contact) {
        Body collider;
        if (sender.Body.Tag == this)
            collider = other.Body;
        else
            collider = sender.Body;
        if (collider.Tag == null) return true;
        ///player collision
        if (collider.Tag is Player)
            if (_player.IsImmune == false)
            {
                _player.IsImmune = true;
                if (_isDurable)
                {
                    _game.GameData.RemoveHealth(2);
                }
                else
                {
                    _game.GameData.RemoveHealth(1); //TODO: do stuff when health reaches 0
                }
            }

        // If colliding with rope, and rope electrified
        if (collider.Tag is RopeSegment) {
            CollidingSegments++;
            if (((RopeSegment)collider.Tag).ElecIntensity > 0) {
                _game.GameData.Score += 1000;
                Electrify();
            }
        }

        if (collider.Tag is Tile) {
            Vector2 v = Body.LinearVelocity;
            Tile tile = (Tile) collider.Tag;
            if (v.Length() > WallKillVelocity) {
                if (tile.FinalPrototype.IsCliff) {
                    collidingCliff = tile;
                    return false;
                }
                Kill();
            }
            if (tile.FinalPrototype.IsCliff && collidingCliff != null) {
                //
                //collidingCliffs.Add((Tile) collider.Tag);
                overCliff = 1;
                return false;
            }
            if (overCliff == 1) {
                return false;
            }
        }
        return true;
    }

    protected void OnSeparation(Fixture sender, Fixture other, Contact contact) {
        var collider = sender.Body.Tag == this ? other.Body : sender.Body;
        if (collider.Tag is RopeSegment) CollidingSegments--;
    }

    //DO NOT CALL DURING ONCOLLISION!!!
    public void Destroy() {
        _world.Remove(Body);
    }


    public override void Update(GameTime gameTime) {
        if (fallStart > 0) {
            if (gameTime.TotalGameTime.TotalSeconds - fallStart > 1) {
                IsAlive = false;
            }
            return;
        }
        if (CollidingSegments > CrushThreshold) {
            Crushed++;
            if (Crushed > CrushDuration) {
                Kill();
                return;
            }
        } else {
            Crushed = 0;
        }
        if (overCliff > 0) {
            fallStart = (float) gameTime.TotalGameTime.TotalSeconds;
            Body.Enabled = false;
            return;
        }

        _input = Vector2.Zero;
        _isWalking = false;

        if (_canChase) 
        {
            var currentDistance = Vector2.Distance(Body.Position, _player.Body.Position);
            if (_canKite)
            {
                if ((currentDistance > _followDistance) && (currentDistance < _angerDistance))
                {
                    if (Body.Position.X < _player.Body.Position.X)
                    {
                        _input.X += 0.1f;
                        _isWalking = true;
                    }

                    if (Body.Position.X > _player.Body.Position.X)
                    {
                        _input.X -= 0.1f;
                        _isWalking = true;
                    }

                    if (Body.Position.Y < _player.Body.Position.Y)
                    {
                        _input.Y += 0.1f;
                        _isWalking = true;
                    }

                    if (Body.Position.Y > _player.Body.Position.Y)
                    {
                        _input.Y -= 0.1f;
                        _isWalking = true;
                    }
                }
            }
            else
            {
                if (currentDistance < _angerDistance)
                {
                    if (Body.Position.X < _player.Body.Position.X)
                    {
                        _input.X += 0.1f;
                        _isWalking = true;
                    }

                    if (Body.Position.X > _player.Body.Position.X)
                    {
                        _input.X -= 0.1f;
                        _isWalking = true;
                    }

                    if (Body.Position.Y < _player.Body.Position.Y)
                    {
                        _input.Y += 0.1f;
                        _isWalking = true;
                    }

                    if (Body.Position.Y > _player.Body.Position.Y)
                    {
                        _input.Y -= 0.1f;
                        _isWalking = true;
                    }
                }
            }
                
        }


        if (_input.LengthSquared() > 1) _input.Normalize();

        var movement = _input * (float)gameTime.ElapsedGameTime.TotalMilliseconds * _enemyForce;

        Body.ApplyForce(movement);
        Orientation = _input;
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        
        var totalTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
        Rectangle spritePos = camera.getScreenRectangle(Body.Position.X - (float)_enemySize.X / 2,
            Body.Position.Y - _enemySize.Y * 2 + (float)_enemySize.X / 4, _enemySize.X, _enemySize.Y);
        int yPos = spritePos.Y;
        if (fallStart > 0) {
            spritePos.Y += (int)((float)fallSpeed * (gameTime.TotalGameTime.TotalSeconds - fallStart));
        }


        // if (isWalking)
        // {
        //     float run_duration = 200f;
        //     int run_frame_idx = (int)(totalTime / run_duration) % 4;
        //
        //     Texture2D running_sprite = running_f;
        //
        //     if (input.X > 0 && input.X >= input.Y)
        //     {
        //         running_sprite = running_r;
        //     }
        //     else if (input.X < 0 && input.X <= input.Y)
        //     {
        //         running_sprite = running_l;
        //     }
        //     else if (input.Y < 0)
        //     {
        //         running_sprite = running_b;
        //     }
        //     //running_sprite = (input.X > 0 && input.X > input.Y) ? running_r : running_l;
        //
        //     batch.Draw(
        //     running_sprite,
        //     spritePos,
        //     new Rectangle(run_frame_idx * 512, 0, 512, 768),
        //     Color.White
        // );
        // }
        // else
        //{
        var idleDuration = 400f; //ms
        var idleFrameIdx = (int)(totalTime / idleDuration) % 2;

        //Color test = overCliff > 0 ? Color.Red : Color.White;
        batch.Draw(
            _idle,
            spritePos,
            new Rectangle(idleFrameIdx * 512, 0, 512, 768),
            Color.White,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            camera.getLayerDepth(yPos + spritePos.Height)
        );
        //}
    }
}