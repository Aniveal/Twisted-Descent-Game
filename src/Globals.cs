using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2 {
    public class Globals {
        public static ContentManager Content { get; set; }
        public static Vector2 CameraPosition { get; set; }
        public static GraphicsDeviceManager Graphics { get; set; }
        public static SoundEngine SoundEngine { get; set; }

        public static void UpdateCamera(Vector2 NewPosition) {
            CameraPosition = NewPosition;
        }
    }
}