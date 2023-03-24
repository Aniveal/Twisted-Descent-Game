using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2
{
    public class SoundEngine
    {
        private static SoundEngine instance = new SoundEngine();

        private SoundEngine() {
            
        }

        //Use this method to get a ref to the SoundEngine object
        public static SoundEngine Instance() { 
            return instance; 
        }
    }
}
