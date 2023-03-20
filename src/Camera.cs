using Microsoft.Xna.Framework;
namespace GameLab
{
    public class Camera
    {
        public Matrix Transform { get; private set; }

        public void Follow(Player player)
        {
            var position = Matrix.CreateTranslation(
              -player.Position.X - (100),
              -player.Position.Y - (100),
              0);

            var offset = Matrix.CreateTranslation(
                Game1.ScreenWidth / 2,
                Game1.ScreenHeight / 2,
                0);

            Transform = position * offset;
        }
    }
}