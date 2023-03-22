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
        const int PLAYER_VELOCITY = 300;

        // PlayerSpritePosition is where we draw the PNG Sprite (top-left of png)
        // PlayerPosition is where he's standing, e.g. to compute which tile he stands on
        Vector2 PlayerSpritePosition;
        Vector2 PlayerPosition;
        

        /* Initialization */
        public Player()
		{
            hero = Globals.Content.Load<Texture2D>("hero");
            PlayerSpritePosition = new(512 - PLAYER_SIZE.X/2, 400 - PLAYER_SIZE.Y);
            PlayerPosition = FeetPosition(PlayerSpritePosition);
        }

        /* Update */
        public void Update(Vector2 input)
        {
            float deltaTime = (float)Globals.TotalSeconds;
            Vector2 movement =  input * deltaTime * PLAYER_VELOCITY;
            
            // If the player is within a rectangle of the center of the screen, move the player, else move the camera:

            int fraction = 3; // rectangle in the center makes up 1/3 of the width and 1/3 of the width

            Vector2 updated_position = PlayerSpritePosition + movement;
            Vector2 updated_feetpos = FeetPosition(updated_position);

            float lowerX = (fraction - 1) * (Globals.graphics.PreferredBackBufferWidth / 2) / fraction;
            float upperX = (fraction + 1) * (Globals.graphics.PreferredBackBufferWidth / 2) / fraction;
            float lowerY = (fraction - 1) * (Globals.graphics.PreferredBackBufferHeight / 2) / fraction;
            float upperY = (fraction + 1) * (Globals.graphics.PreferredBackBufferHeight / 2) / fraction;

            if (updated_position.X < lowerX || updated_feetpos.X > upperX || updated_position.Y < lowerY || updated_feetpos.Y > upperY)
            {
                Vector2 updatedCamera = Globals.CameraPosition - movement; // move the camera
                Globals.UpdateCamera(updatedCamera);
            }
            else
            {
                PlayerSpritePosition = updated_position;   // move the player
                PlayerPosition = FeetPosition(PlayerSpritePosition);
            }
        }

        /* Draw */
        public void Draw()
		{
            Rectangle SpritePos = new((int)PlayerSpritePosition.X, (int)PlayerSpritePosition.Y, (int)PLAYER_SIZE.X, (int)PLAYER_SIZE.Y);
            //Globals.SpriteBatch.Draw(hero, SpritePos, Color.White);
            Globals.SpriteBatch.Draw(hero, SpritePos, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
        }

        /* HELPER FUNCTION */
        // Given the Sprite Position (top-left of PNG, compute where his feet are
         Vector2 FeetPosition(Vector2 SpritePos)
        {
            return new Vector2(SpritePos.X + PLAYER_SIZE.X/2, SpritePos.Y + PLAYER_SIZE.Y);
        }
	}
}

