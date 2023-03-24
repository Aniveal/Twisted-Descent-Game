using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;


namespace Meridian2
{
    public class SoundEngine 
    {
        //A list of all the possible gravel Footstep sounds to chose from
        private List<SoundEffect> gravelFootsteps;
        private int nGravelFootsteps = 20;

        public SoundEngine() {

            gravelFootsteps = new List<SoundEffect>();
            LoadContent();
        }
        
        // this is NOT an override from game object
        private void LoadContent()
        {
            Debug.WriteLine("Loading Content from SoundEngine...");

            
            for(int i = 0; i < nGravelFootsteps; i++)
            {
                gravelFootsteps.Add(Globals.Content.Load<SoundEffect>("Sound/GravelFootsteps/gravel" + i));
            }

            Debug.WriteLine("Done!");
        }

        public void playGravelFootstep()
        {
            Random r = new Random();
            gravelFootsteps[r.Next(20)].Play();
        }

    }
    
}
