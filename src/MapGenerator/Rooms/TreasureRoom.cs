using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwistedDescent
{
    internal class TreasureRoom : Room
    {
        public TreasureRoom(MapGenerator mg, List<Prototype> protList, int x, int y, int index) : base(mg, x, y, 7, 7, index, protList, 0)
        {

            for (int xx = 2; xx < 5; xx++)
                for (int yy = 2; yy < 5; yy++)
                    setWalkable(xx, yy);
            innerOpening(3, 3);

            columnDensity = 0f;
            roomDifficulty = 0;
            nTreasures = 0;
            amphoraDensity = 0f;

            //how many treasures
            int t = RnGsus.Instance.Next(3);
            
            
            for(int i = 0; i < t; i++)
            {
                float tX = RnGsus.Instance.Next(3) + 2.5f;
                float tY = RnGsus.Instance.Next(3) + 1.5f;
                TreasurePositions.Add(new Microsoft.Xna.Framework.Vector2(tX, tY));
            }
        }
    }
}
