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
        public static Vector2 CameraPosition { get; set; }

        public static void UpdateTime(GameTime gt)
        {
            TotalSeconds = (float)gt.ElapsedGameTime.TotalSeconds;
        }

        public static void UpdateCamera(Vector2 NewPosition)
        {
            CameraPosition = NewPosition;
        }
    }
}

