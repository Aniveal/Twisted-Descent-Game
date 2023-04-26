using Microsoft.Xna.Framework;

namespace Meridian2; 

public class Globals {
    //TODO: remove those once when making the camera
    public static Vector2 CameraPosition { get; set; }

    public static void UpdateCamera(Vector2 newPosition) {
        CameraPosition = newPosition;
    }
}