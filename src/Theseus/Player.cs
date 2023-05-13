using System;
using System.Diagnostics;
using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;

namespace Meridian2.Theseus; 

public class Player : DrawableGameElement {
    private const int DashCoolDown = 5000;
    private const int DashUsageTime = 400;
    private readonly RopeGame _game;
    private readonly Point _playerSize = new(1, 2);

    private bool _dash;

    //Current value of delay
    private float _footstepSoundDelayCurrent;

    //How many milliseconds between footsteps
    private readonly float _footstepSoundDelayMax = 400f;
    private readonly float _footstepSoundDelayDashMax = 200f;

    private Texture2D _idle;
    private readonly double _immuneCooldown = 1500;
    private double _immuneTimer;
    private Vector2 _input = Vector2.Zero;
    private bool _isPulling;
    private bool _isWalking;
    private float _playerForce = 15f;
    private readonly Rope _rope;
    private DistanceJoint _ropeConnection;
    private Texture2D _runningB;
    private Texture2D _runningF;
    private Texture2D _runningL;
    private Texture2D _runningR;
    private readonly World _world;

    private Texture2D currentSprite;

    public Body Body;

    public double DashTimer = 4000;

    public bool IsImmune;

    public Vector2 Orientation;

    private readonly Color _immunityColor = new(255, 150, 150);

    public Player(RopeGame game, World world, Rope rope) {
        _rope = rope;
        _game = game;
        _world = world;
    }

    public void Initialize() {
        Body = _world.CreateEllipse((float)_playerSize.X / 2, (float)_playerSize.X / 4, 20, 0.01f,
            _rope.GetEndPosition(), 0f, BodyType.Dynamic);
        Body.FixedRotation = true;
        Body.LinearDamping = 1f;
        Body.Tag = this;

        currentSprite = _idle;

        Body.Mass = 10;
        // Disable rope collision
        foreach (var fixture in Body.FixtureList) fixture.CollisionGroup = -1;

        LinkToRope();
    }

    public void LoadContent() {
        _idle = _game.Content.Load<Texture2D>("Sprites/Theseus/idle");
        _runningL = _game.Content.Load<Texture2D>("Sprites/Theseus/running_l");
        _runningR = _game.Content.Load<Texture2D>("Sprites/Theseus/running_r");
        _runningF = _game.Content.Load<Texture2D>("Sprites/Theseus/running_f");
        _runningB = _game.Content.Load<Texture2D>("Sprites/Theseus/running_b");
    }

    private Vector2 ScreenToIsometric(Vector2 vector) {
        var rotSin = Math.Sin(-Math.PI / 4);
        var rotCos = Math.Cos(-Math.PI / 4);

        // Rotate by 45 degrees
        var isoX = (float)(rotCos * vector.X - rotSin * vector.Y);
        var isoY = (float)(rotSin * vector.X + rotCos * vector.Y);

        // Stretch to 2:1 ratio
        return new Vector2(isoX - isoY, (isoX + isoY) / 2);
    }


    public override void Update(GameTime gameTime) {
        if (_game.GameData.GameOver) return;
        _input = Vector2.Zero;
        DashTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
        DashTimer = Math.Min(DashTimer, 5000);

        if (IsImmune) {
            _immuneTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_immuneTimer > _immuneCooldown) {
                IsImmune = false;
                _immuneTimer = 0;
            }
        }

        _isWalking = false;

        var gamePadCapabilities = GamePad.GetCapabilities(PlayerIndex.One);
        if (gamePadCapabilities.IsConnected) {
            _input = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
            if (_input.Length() > 0.05) {
                _input.Y *= -1;
                _isWalking = true;
            }
        }

        var keyboard = Keyboard.GetState();
        if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D)) {
            _input.X += 1;
            _isWalking = true;
        }

        if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A)) {
            _input.X -= 1;
            _isWalking = true;
        }

        if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S)) {
            _input.Y += 1;
            _isWalking = true;
        }

        if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W)) {
            _input.Y -= 1;
            _isWalking = true;
        }
        
        if (_isWalking && (keyboard.IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) &&
            DashTimer >= DashCoolDown) {
            _dash = true;
            DashTimer = 0;
            _playerForce = 50f;
            _footstepSoundDelayCurrent = 0;
        }

        if (_dash & (DashTimer >= DashUsageTime)) {
            _dash = false;
            _playerForce = 15f;
            DashTimer = 0;
        }


        //Footstep Sound:
        if (_footstepSoundDelayCurrent >= 0)
            _footstepSoundDelayCurrent -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

        if (_isWalking && _footstepSoundDelayCurrent < 0) {
            SoundEngine.Instance.playGravelFootstep();
            _footstepSoundDelayCurrent = _dash ? _footstepSoundDelayDashMax : _footstepSoundDelayMax;
        }

        if (_input.LengthSquared() > 1) _input.Normalize();

        var movement = _input * (float)gameTime.ElapsedGameTime.TotalMilliseconds * _playerForce;

        Body.ApplyForce(movement);
        Orientation = _input;

        var ropeJointDistance = (_ropeConnection.WorldAnchorB - _ropeConnection.WorldAnchorA).Length();
        if (_ropeConnection != null) {
            // Extend rope if force on joint is too strong
            if (ropeJointDistance > Rope.TextureHeight * 1.5) {
                // Remove player joint
                _world.Remove(_ropeConnection);
                _ropeConnection = null;

                _rope.AppendSegment();
                LinkToRope();
            } else if (movement == Vector2.Zero) {
                // _world.Remove(_ropeConnection);
                // _ropeConnection = null;
                // _rope.RemoveSegment();
                // LinkToRope();
            }
        }

        ropeJointDistance = (_ropeConnection.WorldAnchorB - _ropeConnection.WorldAnchorA).Length();
        Diagnostics.Instance.SetForce(ropeJointDistance);
        if (keyboard.IsKeyDown(Keys.P) || GamePad.GetState(PlayerIndex.One).Triggers.Right > 0.5f) {
            _isPulling = true;
            if (ropeJointDistance < Rope.TextureHeight * 1.5) {
                _rope.RemoveLastSegment();
            }

            _rope.Pull(gameTime);
        } else {
            _isPulling = false;
        }
    }

    private void LinkToRope() {
        _ropeConnection = JointFactory.CreateDistanceJoint(_world, _rope.LastSegment().Body, Body,
            new Vector2(Rope.TextureWidth / 2, Rope.TextureHeight),
            new Vector2(0, (float)_playerSize.X / 4));
        _ropeConnection.Length = Rope.RopeJointLength;
        _ropeConnection.Frequency = 15;
        _ropeConnection.DampingRatio = Rope.RopeJointDampingRatio;
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        var spritePos = camera.getScreenRectangle(Body.Position.X - (float)_playerSize.X / 2,
            Body.Position.Y - _playerSize.Y * 2 + (float)_playerSize.X / 4, _playerSize.X, _playerSize.Y);

        var totalTime = (float)gameTime.TotalGameTime.TotalMilliseconds;

        var playerColor = Color.White;
        if (IsImmune) {
            var shift = Math.Sin(totalTime / 70) + 1;
            playerColor = Color.Lerp(playerColor, _immunityColor, (float)shift);
        }
        
        if (_isWalking) {
            var runDuration = 200f;
            var runFrameIdx = (int)(totalTime / runDuration) % 4;

            //Get the direction of the input
            Vector2 dir = Body.LinearVelocity;
            dir.Normalize();

            var runningSprite = _runningR;

            if (dir.X < 0 && dir.Y > 0)
                runningSprite = _runningF;
            else if (dir.X >= 0 && dir.Y > 0)
                runningSprite = _runningL;
            else if (dir.X > 0 && dir.Y < 0)
                runningSprite = _runningB;
            //running_sprite = (input.X > 0 && input.X > input.Y) ? running_r : running_l;

            batch.Draw(
                runningSprite,
                spritePos,
                new Rectangle(runFrameIdx * 512, 0, 512, 768),
                playerColor,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                camera.getLayerDepth(spritePos.Y + spritePos.Height)
            );

        } else {

            currentSprite = _idle;

            var idleDuration = 400f; //ms
            var idleFrameIdx = (int)(totalTime / idleDuration) % 2;

            batch.Draw(
                _idle,
                spritePos,
                new Rectangle(idleFrameIdx * 512, 0, 512, 768),
                playerColor,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                camera.getLayerDepth(spritePos.Y + spritePos.Height)
            );
        }
        
        _rope.DrawPlayerConnection(gameTime, batch, camera, Body.Position + new Vector2(0.02f, -0.09f));
    }
}