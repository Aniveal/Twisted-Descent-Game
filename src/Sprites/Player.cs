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
        public float RotationVelocity = 3f;
        public float LinearVelocity = 4f;

        public Player(Texture2D texture)
        {
            _texture = texture;
        }

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                angle -= MathHelper.ToRadians(RotationVelocity);
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                angle += MathHelper.ToRadians(RotationVelocity);

            var direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(90) - angle), -(float)Math.Sin(MathHelper.ToRadians(90) - angle));

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                Position += direction * LinearVelocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, null, Color.White, angle, Origin, 1, SpriteEffects.None, 0f);
        }
    }
}