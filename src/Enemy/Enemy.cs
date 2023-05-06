﻿using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Meridian2.GameElements;
using Meridian2.Theseus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace Meridian2.Enemy; 

public class Enemy : DrawableGameElement {
    protected const int CrushDuration = 16;
    protected const int CrushThreshold = 4;
    protected const float WallKillVelocity = 1.5f;
    protected readonly Point _enemySize = new(1, 2);
    protected readonly RopeGame _game;
    protected readonly float _angerDistance = 7f;
    protected int _difficultyLevel;
    protected float _enemyForce = 0.003f;
    protected readonly float _followDistance = 3f;
    
    private int _reducedHealth = 1;

    protected Texture2D _idle;

    protected Vector2 _input = Vector2.Zero;

    protected bool _isWalking;
    protected readonly Player _player;
    protected Texture2D _runningB;
    protected Texture2D _runningF;
    protected Texture2D _runningL;
    protected Texture2D _runningR;
    protected readonly World _world;

    public Body Body;
    public int Colliding;
    public int CollidingSegments;
    public int Crushed;
    public int overCliff = 0;
    public Tile collidingCliff;
    protected const int fallSpeed = 100; //pixels per second
    protected float fallStart = 0;
    public bool IsAlive = true;
    public Vector2 Orientation;

    private Boolean _canShoot = false;
    private Boolean _hasImmunity = false;
    private Boolean _isImmuneToElectricity = false;
    private Boolean _isImmuneToAmphoras = false;
    private Boolean _canKite = false;
    

    public Enemy(RopeGame game, World world, Player player) {
        _game = game;
        _world = world;
        _player = player;
    }
    public void generateRandomAbilities(int diff)
    {
        Random rng = new Random();
        // dif is between 1 to 50
        if(rng.Next(100) < diff * 2)
        {
            _enemyForce /= 4;
            _reducedHealth *= 2;
            _hasImmunity = true;
            _isImmuneToAmphoras = true;
            _isImmuneToElectricity = true;
            _canKite = true;
        }
        else
        {
            if (rng.Next(100) < diff * 2  * 7 / 10)
            {
                _hasImmunity = true;
                if (rng.Next(100) > 50)
                    _isImmuneToElectricity = true;
                else
                    _isImmuneToAmphoras = true;
            }
            if (rng.Next(100) < diff * 2 * 7 / 10)
                _canKite = true;
        }

        //if (rng.Next(100) > 50)
        //    _canShoot = true;

    }

    public void Initialize(Vector2 initpos, int difficultyLevel) {
        Body = _world.CreateEllipse((float)_enemySize.X / 4, (float)_enemySize.X / 8, 20, 0.01f,
            initpos, 0f, BodyType.Dynamic);
        Body.FixedRotation = true;
        Body.LinearDamping = 0.5f;
        Body.Tag = this;
        Body.OnCollision += OnCollision;
        Body.OnSeparation += OnSeparation;
        _difficultyLevel = difficultyLevel;
    }

    public virtual void LoadContent() {
        _idle = _game.Content.Load<Texture2D>("Sprites/Enemies/idle_enemy");
        _runningL = _game.Content.Load<Texture2D>("Sprites/Enemies/idle_enemy");
        _runningR = _game.Content.Load<Texture2D>("Sprites/Enemies/idle_enemy");
        _runningF = _game.Content.Load<Texture2D>("Sprites/Enemies/idle_enemy");
        _runningB = _game.Content.Load<Texture2D>("Sprites/Enemies/idle_enemy");
    }

    public virtual void Electrify() {
        Kill(1);
        //TODO: play animation (change color to yellow?), take damage
    }

    public void Kill(int cause) {
        if (cause == 0) // normal
        {
            SoundEngine.Instance.Squish();
            IsAlive = false;
        }
        if (cause == 1 && !_isImmuneToElectricity) // electricity
        {
            SoundEngine.Instance.Squish();
            IsAlive = false;
        }
        if (cause == 2 && !_isImmuneToAmphoras) // apmohras
        {
            SoundEngine.Instance.Squish();
            IsAlive = false;
        }
    }

    protected virtual bool OnCollision(Fixture sender, Fixture other, Contact contact) {
        Body collider;
        if (sender.Body.Tag == this)
            collider = other.Body;
        else
            collider = sender.Body;
        if (collider.Tag == null) return true;
        ///player collision
        if (collider.Tag is Player)
            if (_player.IsImmune == false) {
                _player.IsImmune = true;
                _game.GameData.RemoveHealth(_reducedHealth); //TODO: do stuff when health reaches 0
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
                Kill(0);
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
                Kill(0);
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
        if (!_canKite) // enemies chase you
        {
            var currentDistance = Vector2.Distance(Body.Position, _player.Body.Position);

            if (currentDistance < _angerDistance) {
                if (Body.Position.X < _player.Body.Position.X) {
                    _input.X += 0.1f;
                    _isWalking = true;
                }

                if (Body.Position.X > _player.Body.Position.X) {
                    _input.X -= 0.1f;
                    _isWalking = true;
                }

                if (Body.Position.Y < _player.Body.Position.Y) {
                    _input.Y += 0.1f;
                    _isWalking = true;
                }

                if (Body.Position.Y > _player.Body.Position.Y) {
                    _input.Y -= 0.1f;
                    _isWalking = true;
                }
            }
        }

        if (_canKite) // enemies chase you for a while
        {
            var currentDistance = Vector2.Distance(Body.Position, _player.Body.Position);
            if ((currentDistance > _followDistance) & (currentDistance < _angerDistance)) {
                if (Body.Position.X < _player.Body.Position.X) {
                    _input.X += 0.1f;
                    _isWalking = true;
                }

                if (Body.Position.X > _player.Body.Position.X) {
                    _input.X -= 0.1f;
                    _isWalking = true;
                }

                if (Body.Position.Y < _player.Body.Position.Y) {
                    _input.Y += 0.1f;
                    _isWalking = true;
                }

                if (Body.Position.Y > _player.Body.Position.Y) {
                    _input.Y -= 0.1f;
                    _isWalking = true;
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

        Rectangle AbilityPos = camera.getScreenRectangle(Body.Position.X - (float)_enemySize.X / 2,
            Body.Position.Y - _enemySize.Y * 2 + (float)_enemySize.X / 4, 0.2f, 0.1f);

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