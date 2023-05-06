using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2
{
    internal class AmphoraRoom : Room
    {
        public AmphoraRoom(MapGenerator mg, List<Prototype> protList, int x, int y, int sizeX, int sizeY, int index, int diff) : base(mg, x, y, sizeX, sizeY, index, protList, diff)
        {

            nTreasures = 0;
            amphoraDensity = 0.4f;
        }
    }
}
