using System;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Meridian2 {
    public class Player : IGameObject {
        private Texture2D _hero;
        private readonly Point _playerSize = new Point(60, 120);
        private const int PlayerVelocity = 300;

        // PlayerSpritePosition is where we draw the PNG Sprite (top-left of png)
        // PlayerPosition is where he's standing, e.g. to compute which tile he stands on
        private Vector2 _playerSpritePosition;
        private Vector2 _playerPosition;

        /* HELPER FUNCTION */
        // Given the Sprite Position (top-left of PNG, compute where his feet are
        Vector2 FeetPosition(Vector2 SpritePos) {
            return new Vector2(SpritePos.X + _playerSize.X / 2, SpritePos.Y + _playerSize.Y);
        }

        public void Initialize() {
            _playerSpritePosition = new(512 - _playerSize.X / 2, 400 - _playerSize.Y);
            _playerPosition = FeetPosition(_playerSpritePosition);
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

            Vector2 movement = input * (float)gameTime.ElapsedGameTime.TotalSeconds * PlayerVelocity;

            // If the player is within a rectangle of the center of the screen, move the player, else move the camera:

            int fraction = 3; // rectangle in the center makes up 1/3 of the width and 1/3 of the width

            Vector2 updatedPosition = _playerSpritePosition + movement;
            Vector2 updatedFeetpos = FeetPosition(updatedPosition);

            float lowerX = (fraction - 1) * (Globals.Graphics.PreferredBackBufferWidth / 2) / fraction;
            float upperX = (fraction + 1) * (Globals.Graphics.PreferredBackBufferWidth / 2) / fraction;
            float lowerY = (fraction - 1) * (Globals.Graphics.PreferredBackBufferHeight / 2) / fraction;
            float upperY = (fraction + 1) * (Globals.Graphics.PreferredBackBufferHeight / 2) / fraction;

            if (updatedPosition.X < lowerX || updatedFeetpos.X > upperX || updatedPosition.Y < lowerY ||
                updatedFeetpos.Y > upperY) {
                Vector2 updatedCamera = Globals.CameraPosition - movement; // move the camera
                Globals.UpdateCamera(updatedCamera);
            } else {
                _playerSpritePosition = updatedPosition; // move the player
                _playerPosition = FeetPosition(_playerSpritePosition);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch batch) {
            Rectangle spritePos = new((int)_playerSpritePosition.X, (int)_playerSpritePosition.Y, (int)_playerSize.X,
                (int)_playerSize.Y);
            batch.Draw(_hero, spritePos, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
        }
    }
}