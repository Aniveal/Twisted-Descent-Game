using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using TwistedDescent.GameElements;
using TwistedDescent.Theseus;

namespace TwistedDescent.Enemy; 

public class Enemy : DrawableGameElement {
    protected const int CrushDuration = 16;
    protected const int CrushThreshold = 4;
    protected const float WallKillVelocity = 1.5f;
    protected Point _enemySize = new(1, 2);
    protected readonly RopeGame _game;
    protected readonly float _angerDistance = 8f;
    protected int _difficultyLevel;
    protected float _enemyForce = 0.004f;
    private int randomDir = new Random().Next(4);

    private const int DashCoolDown = 4000;
    private const int DashUsageTime = 500;
    private bool _dash;
    public double DashTimer = 3000;

    private int _reducedHealth = 1;

    protected Texture2D _idle;

    protected Vector2 _input = Vector2.Zero;

    protected bool _isWalking;
    protected readonly Player _player;
    protected Texture2D _runningB;
    protected Texture2D _runningF;
    protected Texture2D _runningL;
    protected Texture2D _runningR;
    protected Texture2D _deathAnimation;
    private Texture2D _angryL;
    private Texture2D _angryR;
    private Texture2D _angryF;
    private Texture2D _angryB;
    private Texture2D _skeleton;
    protected readonly World _world;

    private int _enemySpriteWidth = 512;
    private int _enemySpriteHeight = 768;

    public Body Body;
    public int Colliding;
    public int CollidingSegments;
    public int Crushed;
    public int overCliff = 0;
    public Tile collidingCliff;
    protected const int fallSpeed = 100; //pixels per second
    protected float fallStart = 0;
    protected float deathStart = 0;
    public bool IsAlive = true;
    public bool drawDeathAnimation = false;
    public bool drawElectrify = false;
    public Vector2 Orientation;

    private Boolean _canShoot = false;
    private Boolean _hasImmunity = false;
    private Boolean _isImmuneToElectricity = false;
    private Boolean _isImmuneToAmphoras = false;
    private bool _chasing = false;
    private bool _DashEnemies = false;
    public bool tutorialEnemy = false;
    private bool _isAngry = false;

    public Enemy(RopeGame game, World world, Player player) {
        _game = game;
        _world = world;
        _player = player;
    }
    public void generateRandomAbilities(int diff)
    {
        Random rng = new Random();
        if (rng.Next(100) < 50) // 50 percent dash enemies, 50 percent normal
        {
            _DashEnemies = true;
            _enemyForce = 0.09f;
            _enemySize = new Point(2, 2);

            if (rng.Next(100) < diff * 2 * 7 / 10) // dash enemies has a chance to have one immunitiybut they are immune to squish
            {
                _hasImmunity = true;
                if (rng.Next(100) > 50)
                    _isImmuneToElectricity = true;
                else
                    _isImmuneToAmphoras = true;
            }
        }
        else // normal enemies
        {
            if (rng.Next(100) < diff * 2)
            {
                _hasImmunity = true;
                _isImmuneToAmphoras = true;
                _isImmuneToElectricity = true;
            }
            else
            {
                if (rng.Next(100) < diff * 2 * 7 / 10)
                {
                    _hasImmunity = true;
                    if (rng.Next(100) > 50)
                        _isImmuneToElectricity = true;
                    else
                        _isImmuneToAmphoras = true;
                }
            }
        }


    }

    public void SetTutorialMode() {
        tutorialEnemy = true;
        _isImmuneToAmphoras = false;
        _isImmuneToElectricity = false;
        _DashEnemies = false;
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
        if (_DashEnemies)
        {
            _enemySpriteWidth = 2000;
            _enemySpriteHeight = 1700;
            _idle = _game.Content.Load<Texture2D>("Sprites/Enemies/minotaur_idle");  // 2 frames, each one is 2000 pixels wide
            _runningL = _game.Content.Load<Texture2D>("Sprites/Enemies/minotaur_attack_l");  // 4 frames, each one is 2000 pixels wide
            _runningR = _game.Content.Load<Texture2D>("Sprites/Enemies/minotaur_attack_r");  // 4 frames, each one is 2000 pixels wide
            _runningF = _game.Content.Load<Texture2D>("Sprites/Enemies/minotaur_attack_f");  // 4 frames, each one is 2000 pixels wide
            _runningB = _game.Content.Load<Texture2D>("Sprites/Enemies/minotaur_attack_b");  // 4 frames, each one is 2000 pixels wide

            _deathAnimation = _game.Content.Load<Texture2D>("Sprites/Enemies/minotaur_dying");  // 5 frames, each one is 2000 pixels wide

            _angryL = _game.Content.Load<Texture2D>("Sprites/Enemies/minotaur_angry_l");  // 8 frames, each one is 2000 pixels wide
            _angryR = _game.Content.Load<Texture2D>("Sprites/Enemies/minotaur_angry_r");  // 8 frames, each one is 2000 pixels wide
            _angryF = _game.Content.Load<Texture2D>("Sprites/Enemies/minotaur_angry_f");  // 8 frames, each one is 2000 pixels wide
            _angryB = _game.Content.Load<Texture2D>("Sprites/Enemies/minotaur_angry_b");  // 8 frames, each one is 2000 pixels wide
            _skeleton = _game.Content.Load<Texture2D>("Sprites/Enemies/minotaur_skeleton");  
        }
        else
        {
            _idle = _game.Content.Load<Texture2D>("Sprites/Enemies/cyclop_idle_f");  // 2 frames, each one is 512 pixels wide
            _runningL = _game.Content.Load<Texture2D>("Sprites/Enemies/cyclop_walk_l");  // 4 frames, each one is 512 pixels wide
            _runningR = _game.Content.Load<Texture2D>("Sprites/Enemies/cyclop_walk_r");  // 4 frames, each one is 512 pixels wide
            _runningF = _game.Content.Load<Texture2D>("Sprites/Enemies/cyclop_walk_f");  // 4 frames, each one is 512 pixels wide
            _runningB = _game.Content.Load<Texture2D>("Sprites/Enemies/cyclop_walk_b");  // 4 frames, each one is 512 pixels wide
            _deathAnimation = _game.Content.Load<Texture2D>("Sprites/Enemies/cyclop_dying");  // 5 frames, each one is 512 pixels wide
            _skeleton = _game.Content.Load<Texture2D>("Sprites/Enemies/cyclop_skeleton");
        }
    }

    public virtual void Electrify() {
        Kill(1);
        //TODO: play animation (change color to yellow?), take damage
    }

    public void Kill(int cause) {
        if (cause == 0 && !_DashEnemies) // normal
        {
            SoundEngine.Instance.Squish(this.Body.Position);
            _game.GameData.Kills += 1;
            _game.GameData.AddTime(10f);
            drawDeathAnimation = true;
        }
        if (cause == 1 && !_isImmuneToElectricity) // electricity
        {
            SoundEngine.Instance.ElectroShock(this.Body.Position);
            _game.GameData.Kills += 1;
            _game.GameData.AddTime(10f);
            //IsAlive = false;
            drawDeathAnimation = true;
            drawElectrify = true;
        }
        if (cause == 2 && !_isImmuneToAmphoras) // apmohras
        {
            SoundEngine.Instance.Squish(this.Body.Position);
            _game.GameData.Kills += 1;
            _game.GameData.AddTime(10f);
            //IsAlive = false;
            drawDeathAnimation = true;
        }
        if (cause == 3) // cliff
        {
            _game.GameData.Kills += 1;
            _game.GameData.AddTime(10f);
            //IsAlive = false;
            drawDeathAnimation = true;
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

        if (drawDeathAnimation)
        {
            Body.Enabled = false;
            drawDeathAnimation = false;
            deathStart = (float)gameTime.TotalGameTime.TotalMilliseconds;
            return;
        }

        if (deathStart > 0)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - deathStart > 400)
            {
                IsAlive = false;
            }
            return;
        }

        if (fallStart > 0) {
            if (gameTime.TotalGameTime.TotalSeconds - fallStart > 1) {
                Kill(3);
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
            fallStart = (float)gameTime.TotalGameTime.TotalSeconds;
            Body.Enabled = false;
            SoundEngine.Instance.WilhelmScream(this.Body.Position);
            return;
        }

        if (tutorialEnemy)
        {
            //tutorial enemies do not move by themselves
            return;
        }

        _input = Vector2.Zero;
        _isWalking = false;

        var currentDistance = Vector2.Distance(Body.Position, _player.Body.Position);

        if (_DashEnemies)
        {
            if (currentDistance < _angerDistance || _chasing)
            {
                _chasing = true;
                DashTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                DashTimer = Math.Min(DashTimer, 4000);
                _isAngry = !_dash;
                if (DashTimer >= DashCoolDown)
                {
                    _dash = true;
                    _isAngry = !_dash;
                    DashTimer = 0;
                }
                if (_dash)
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
                if (DashTimer >= DashUsageTime && _dash)
                {
                    DashTimer = 0;
                    _dash = false;
                    _isAngry = !_dash;
                }
            }
        }
        else
        {
            if (currentDistance < _angerDistance || _chasing) {

                _chasing = true;
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
            else
            {
                _enemyForce = 0.001f;
                int dir = (randomDir + (int)(gameTime.TotalGameTime.TotalSeconds / 3)) % 4;
                if (dir % 4 == 0)
                {
                    _input.X += 0.1f;
                }
                if (dir % 4 == 1)
                {
                    _input.Y += 0.1f;
                }
                if (dir % 4 == 2)
                {
                    _input.X -= 0.1f;
                }
                if (dir % 4 == 3)
                {
                    _input.Y -= 0.1f;
                }
                _isWalking = true;
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
        Color color = Color.White;

        if (fallStart > 0) {
            spritePos.Y += (int)((float)fallSpeed * (gameTime.TotalGameTime.TotalSeconds - fallStart));
            color = Color.White * (float)(1 - (gameTime.TotalGameTime.TotalSeconds - fallStart));
        }

        if (deathStart > 0)
        {
            var deathDuration = 400f; //ms
            var TimeSinceDeath = gameTime.TotalGameTime.TotalMilliseconds - deathStart;
            var deathFrameIdx = (int)((5 * TimeSinceDeath / deathDuration) % 5);

            if (drawElectrify)
            {
                var oppacity = (int)((TimeSinceDeath < deathDuration - 100) ? 255 : 255 * (deathDuration - TimeSinceDeath) / 100);
                batch.Draw(
                    _skeleton,
                    spritePos,
                    new Rectangle(deathFrameIdx * _enemySpriteWidth, 0, _enemySpriteWidth, _enemySpriteHeight),
                    new Color(color, oppacity),
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    camera.getLayerDepth(spritePos.Y + spritePos.Height)
                );
            }
            else
            {
                batch.Draw(
                    _deathAnimation,
                    spritePos,
                    new Rectangle(deathFrameIdx * _enemySpriteWidth, 0, _enemySpriteWidth, _enemySpriteHeight),
                    color,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    camera.getLayerDepth(spritePos.Y + spritePos.Height)
                );
            }
            return;
        }

        if (_isWalking) {

            var runningDuration = 400f; //ms
            var runningFrameIdx = (int)(totalTime / runningDuration) % 4;

            Vector2 dir = Body.LinearVelocity;

            dir.Normalize();

            var runningSprite = _runningR;

            if (dir.X < 0 && dir.Y > 0)
                runningSprite = _runningF;
            else if (dir.X >= 0 && dir.Y > 0)
                runningSprite = _runningL;
            else if (dir.X > 0 && dir.Y < 0)
                runningSprite = _runningB;

            batch.Draw(
                runningSprite,
                spritePos,
                new Rectangle(runningFrameIdx * _enemySpriteWidth, 0, _enemySpriteWidth, _enemySpriteHeight),
                color,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                camera.getLayerDepth(spritePos.Y + spritePos.Height)
            );

            if (_isImmuneToAmphoras)
            {
                Color[] data = new Color[10 * 20];
                Texture2D rectTexture = new Texture2D(_game.GraphicsDevice, 10, 20);
                for (int i = 0; i < data.Length; ++i)
                    data[i] = Color.Red;
                rectTexture.SetData(data);
                var position = new Vector2(spritePos.X, spritePos.Y - 5);
                batch.Draw(rectTexture, position, null, Color.Red, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, camera.getLayerDepth(spritePos.Y + spritePos.Height));
            }
            if (_isImmuneToElectricity)
            {
                Color[] data = new Color[10 * 20];
                Texture2D rectTexture = new Texture2D(_game.GraphicsDevice, 10, 20);
                for (int i = 0; i < data.Length; ++i)
                    data[i] = Color.Yellow;
                rectTexture.SetData(data);
                var position = new Vector2(spritePos.X + spritePos.Width, spritePos.Y - 5);
                batch.Draw(rectTexture, position, null, Color.Yellow, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, camera.getLayerDepth(spritePos.Y + spritePos.Height));
            }


        }
        else {

            if (_isAngry)
            {
                var angryDuration = 100f; //ms
                var angryFrameIdx = (int)(totalTime / angryDuration) % 8;

                Vector2 dir = Body.LinearVelocity;

                dir.Normalize();

                var angrySprite = _angryR;
                if (Body.Position.X > _player.Body.Position.X && Body.Position.Y < _player.Body.Position.Y)
                    angrySprite = _angryF;
                if (Body.Position.X <= _player.Body.Position.X && Body.Position.Y < _player.Body.Position.Y)
                    angrySprite = _angryL;
                if (Body.Position.X <= _player.Body.Position.X && Body.Position.Y > _player.Body.Position.Y)
                    angrySprite = _angryB;

                batch.Draw(
                    angrySprite,
                    spritePos,
                    new Rectangle(angryFrameIdx * _enemySpriteWidth, 0, _enemySpriteWidth, _enemySpriteHeight),
                    color,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    camera.getLayerDepth(spritePos.Y + spritePos.Height)
                );
            }
            else
            {
                var idleDuration = 400f; //ms
                var idleFrameIdx = (int)(totalTime / idleDuration) % 2;
                //Color test = overCliff > 0 ? Color.Red : Color.White;

                batch.Draw(
                    _idle,
                    spritePos,
                    new Rectangle(idleFrameIdx * _enemySpriteWidth, 0, _enemySpriteWidth, _enemySpriteHeight),
                    color,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    camera.getLayerDepth(yPos + spritePos.Height)
                );
            }
            

            if (_isImmuneToAmphoras)
            {
                Color[] data = new Color[10 * 20];
                Texture2D rectTexture = new Texture2D(_game.GraphicsDevice, 10, 20);
                for (int i = 0; i < data.Length; ++i)
                    data[i] = Color.Red;
                rectTexture.SetData(data);
                var position = new Vector2(spritePos.X, spritePos.Y - 5);
                batch.Draw(rectTexture, position, null, Color.Red, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, camera.getLayerDepth(spritePos.Y + spritePos.Height));
            }
            if (_isImmuneToElectricity)
            {
                Color[] data = new Color[10 * 20];
                Texture2D rectTexture = new Texture2D(_game.GraphicsDevice, 10, 20);
                for (int i = 0; i < data.Length; ++i)
                    data[i] = Color.Yellow;
                rectTexture.SetData(data);
                var position = new Vector2(spritePos.X + spritePos.Width, spritePos.Y - 5);
                batch.Draw(rectTexture, position, null, Color.Yellow, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, camera.getLayerDepth(spritePos.Y + spritePos.Height));
            }
        }
    }
}