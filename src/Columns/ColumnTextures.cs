using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Columns {

    public class ColumnTextures {

        public Texture2D lower; //drafirst part of the texture
        public Texture2D upper; //drawSecond texture
        
        public ColumnTextures(Texture2D lower, Texture2D upper) {
            this.lower = lower;
            this.upper = upper;
        }
    }
}
