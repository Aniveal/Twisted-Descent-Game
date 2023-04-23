using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2; 

public class DrawJob {
    public Texture2D Texture;
    public Rectangle Source;
    public Rectangle Destination;
    public Color Color;
    public float Rotation;
}