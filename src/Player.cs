using System;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;

namespace Meridian2 {
    public class Player : IGameObject {
        private readonly GameScreen _gameScreen;

        private Texture2D _hero;
        private readonly Point _playerSize = new(60, 120);
        private const int PlayerForce = 5000;

        public Body Body;

        public Player(GameScreen gameScreen) {
            _gameScreen = gameScreen;
        }

        public void Initialize() {
            Body = _gameScreen.World.CreateEllipse((float)_playerSize.X / 2, (float)_playerSize.X / 4, 20, 0.1f,
                _gameScreen.Rope.GetEndPosition(), 0f, BodyType.Dynamic);
            Body.FixedRotation = true;
            Body.LinearDamping = 1f;

            // Disable rope collision
            foreach (Fixture fixture in Body.FixtureList) {
                fixture.CollisionGroup = -1;
            }

            JointFactory.CreateRevoluteJoint(_gameScreen.World, _gameScreen.Rope.LastSegment().Body, Body,
                Vector2.Zero);
        }

        public void LoadContent() {
            _hero = Globals.Content.Load<Texture2D>("hero");
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

        public void Update(GameTime gameTime) {
            Vector2 input = Vector2.Zero;

            GamePadCapabilities gamePadCapabilities = GamePad.GetCapabilities(PlayerIndex.One);
            if (gamePadCapabilities.IsConnected) {
                input = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
                input.Y *= -1;
            }

            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Right)) {
                input.X += 1;
            }

            if (keyboard.IsKeyDown(Keys.Left)) {
                input.X -= 1;
            }

            if (keyboard.IsKeyDown(Keys.Down)) {
                input.Y += 1;
            }

            if (keyboard.IsKeyDown(Keys.Up)) {
                input.Y -= 1;
            }

            input = ScreenToIsometric(input);

            if (input.LengthSquared() > 1) {
                input.Normalize();
            }

            Vector2 movement = input * (float)gameTime.ElapsedGameTime.TotalMilliseconds * PlayerForce;

            Body.ApplyForce(movement);

            // TODO: Camera movement was temporarily disabled and should be handled in a separate class
            // If the player is within a rectangle of the center of the screen, move the player, else move the camera:
            // int fraction = 3; // rectangle in the center makes up 1/3 of the width and 1/3 of the width
            //
            // Vector2 updatedPosition = _playerSpritePosition + movement;
            // Vector2 updatedFeetpos = FeetPosition(updatedPosition);
            //
            // float lowerX = (fraction - 1) * (Globals.Graphics.PreferredBackBufferWidth / 2) / fraction;
            // float upperX = (fraction + 1) * (Globals.Graphics.PreferredBackBufferWidth / 2) / fraction;
            // float lowerY = (fraction - 1) * (Globals.Graphics.PreferredBackBufferHeight / 2) / fraction;
            // float upperY = (fraction + 1) * (Globals.Graphics.PreferredBackBufferHeight / 2) / fraction;
            //
            // if (updatedPosition.X < lowerX || updatedFeetpos.X > upperX || updatedPosition.Y < lowerY ||
            //     updatedFeetpos.Y > upperY) {
            //     Vector2 updatedCamera = Globals.CameraPosition - movement; // move the camera
            //     Globals.UpdateCamera(updatedCamera);
            // } else {
            //     _playerSpritePosition = updatedPosition; // move the player
            //     _playerPosition = FeetPosition(_playerSpritePosition);
            // }
        }

        public void Draw(GameTime gameTime, SpriteBatch batch) {
            var playerSpriteX = Body.Position.X - (float)_playerSize.X / 2;
            var playerSpriteY = Body.Position.Y - _playerSize.Y;

            Rectangle spritePos = new Rectangle((int)playerSpriteX, (int)playerSpriteY, _playerSize.X, _playerSize.Y);
            batch.Draw(_hero, spritePos, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
        }
    }
}