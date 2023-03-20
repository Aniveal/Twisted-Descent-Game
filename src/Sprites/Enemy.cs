using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLab
{
    public class Enemy
    {
        private Texture2D _texture;

        private float _rotation;
        public Vector2 Position;
        public Vector2 Origin;
        public float RotationVelocity = 3f;
        public float LinearVelocity = 3f;
        public float FollowDistance = 100;
        public float AngerDistance = 200;

        public Enemy(Texture2D texture)
        {
            _texture = texture;
        }

        public void Update(int difficulty, Player player)
        {
            if(difficulty == 0)
            {
                var distance = player.Position - this.Position;
                var angle = (float)Math.Atan2(distance.Y, distance.X);

                var direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                var currentDistance = Vector2.Distance(this.Position, player.Position);
                if (currentDistance > AngerDistance)
                {
                    return;
                }
                if (currentDistance > FollowDistance)
                {
                    _rotation = angle;
                    var t = MathHelper.Min((float)Math.Abs(currentDistance - FollowDistance), LinearVelocity);
                    var velocity = direction * t;

                    Position += velocity;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, new Rectangle(0, 0, 30, 30), Color.White, _rotation, Origin, 1, SpriteEffects.None, 0f);
        }
    }
}