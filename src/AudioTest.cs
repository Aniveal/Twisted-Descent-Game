using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Meridian2
{
    public class AudioTest : IGameObject
    {

        private float cooldown = 0;

        public AudioTest()
        {
            
        }

        public void Initialize() { }

        public void LoadContent() { }

        public void Update(GameTime gameTime)
        {
            if (cooldown > 0)
            {
                cooldown = cooldown - (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                return;
            }

            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.F))
            {
                Globals.SoundEngine.playGravelFootstep();
                cooldown = 350; //0.5 seconds
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch batch) { }
    }
}
