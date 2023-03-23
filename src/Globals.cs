using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2
{
	public class Globals
	{
        public static float TotalSeconds { get; set; }
        public static ContentManager Content { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static GraphicsDeviceManager graphics { get; set; }

        public static Point SelectedTile { get; set; }

        public static void UpdateTime(GameTime gt)
        {
            TotalSeconds = (float)gt.ElapsedGameTime.TotalSeconds;
        }

        public static void UpdateTile(Point update)
        {
            int x = SelectedTile.X + update.X;
            int y = SelectedTile.Y + update.Y;
            SelectedTile = new((x < 0) ? 0 : x, (y < 0) ? 0 : y);

        }
    }
}

