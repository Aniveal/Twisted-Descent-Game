using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GameLab
{
    public class Player
    {
        private Texture2D _texture;
        private float angle;
        public Vector2 Position;
        public Vector2 Origin;
        public float LinearVelocity = 4f;
        public float DashMultiplier = 20f;
        bool isDashActive = false;
        public double CoolDownTime = 0;



        public Player(Texture2D texture)
        {
            _texture = texture;
        }

        public void Update(GameTime gameTime)
        {
            CoolDownTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Space) & CoolDownTime >= 5000)
            {
                isDashActive = true;
                CoolDownTime = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Position.X -= LinearVelocity;
                if (isDashActive)
                {
                    isDashActive = false;
                    Position.X -= DashMultiplier * LinearVelocity;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Position.X += LinearVelocity;
                if (isDashActive) 
                {
                    isDashActive = false;
                    Position.X += DashMultiplier * LinearVelocity;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Position.Y -= LinearVelocity;
                if (isDashActive)
                {
                    isDashActive = false;
                    Position.Y -= DashMultiplier * LinearVelocity;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Position.Y += LinearVelocity;
                if (isDashActive)
                {
                    isDashActive = false;
                    Position.Y += DashMultiplier * LinearVelocity;
                }
            }
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, null, Color.White, angle, Origin, 1, SpriteEffects.None, 0f);
        }
    }
}