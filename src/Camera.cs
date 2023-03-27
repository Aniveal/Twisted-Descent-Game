using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2
{
    public class Camera
    {
        public Matrix Transform { get; private set; }

        public void Follow(Rope rope)
        {
            var position = Matrix.CreateTranslation(
               - (100),
               - (100),
              0);

            var offset = Matrix.CreateTranslation(
                4,
                2,
                0);

            Transform = position * offset;
        }
    }
}