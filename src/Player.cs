using System;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Meridian2
{
	public class Player
	{
        Texture2D hero;
        Point PLAYER_SIZE = new Point(60, 120);
        const int PLAYER_VELOCITY = 200;

        // PlayerSpritePosition is where we draw the PNG Sprite (top-left of png)
        // PlayerPosition is where he's standing, e.g. to compute which tile he stands on
        Vector2 PlayerSpritePosition = new Vector2(100, 100);
        Vector2 PlayerPosition = new Vector2(100,100);

        /* Initialization */
        public Player()
		{
            hero = Globals.Content.Load<Texture2D>("hero");
        }

        /* Update */
        public void Update(Vector2 input)
        {
            float deltaTime = (float)Globals.TotalSeconds;
            Vector2 updatedPos = PlayerSpritePosition + input * deltaTime * PLAYER_VELOCITY;

            PlayerSpritePosition = updatedPos;
            PlayerPosition = FeetPosition(PlayerSpritePosition); // where the player would stand after updating
        }

        /* Draw */
        public void Draw()
		{
            Rectangle SpritePos = new((int)PlayerSpritePosition.X, (int)PlayerSpritePosition.Y, (int)PLAYER_SIZE.X, (int)PLAYER_SIZE.Y);
            Globals.SpriteBatch.Draw(hero, SpritePos, Color.White);


            Rectangle FeetPos = new((int)PlayerPosition.X, (int)PlayerPosition.Y, 10, 10);
            Globals.SpriteBatch.Draw(hero, FeetPos, Color.Black);
        }

        /* HELPER FUNCTION */
        // Given the Sprite Position (top-left of PNG, compute where his feet are
         Vector2 FeetPosition(Vector2 SpritePos)
        {
            return new Vector2(SpritePos.X + PLAYER_SIZE.X/2, SpritePos.Y + PLAYER_SIZE.Y);
        }
	}
}

