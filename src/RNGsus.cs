using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2
{
    //Use this class only for random numbers!!!
    public sealed class RNGsus
    {
        //Set seed to 0 to get a random one
        private const int seed = 0;
        private static readonly RNGsus instance = new RNGsus();
        private Random random;

        

        static RNGsus()
        {
            
        }

        private RNGsus()
        {
            if(seed == 0)
            {
                random = new Random();
            }
            else random = new Random(seed);
        }

        public static RNGsus Instance
        {
            get
            {
                return instance;
            }
        }

        //Returns a random number between 0 and 1 (Same as Random.NextDouble)
        public double NextDouble()
        {
            return random.NextDouble();
        }

        //Same as Random.Next()
        public int Next()
        {
            return random.Next();
        }
        public int Next(int i)
        {
            return random.Next(i);
        }

    }
}
