using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Columns; 

public class ColumnTextures {
    public Texture2D Lower; //drafirst part of the texture
    public Texture2D Upper; //drawSecond texture

    public ColumnTextures(Texture2D lower, Texture2D upper) {
        Lower = lower;
        Upper = upper;
    }
}