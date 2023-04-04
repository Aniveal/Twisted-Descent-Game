using Meridian2.GameElements;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2.Columns
{
    /**
     * Propagates Update(), DrawFirst(), DrawSecond() to all columns
     * 
     * TODO: add a system to only work on columns on or close to the screen once we have a bigger map
     * This will also bring up the question of when to initialize the columns, currently done by the columns in thei constructor
     */
    public class ColumnsManager
    {
        private List<TwoPhaseGameElement> columns;

        public ColumnsManager()
        {
            columns = new List<TwoPhaseGameElement>();
            
        }

        public void Add(TwoPhaseGameElement column)
        {
            columns.Add(column);
        }

        public void Remove(TwoPhaseGameElement column)
        {
            columns.Remove(column);
        }

        public void DrawFirst(GameTime gameTime, SpriteBatch batch, Camera camera)
        {
            foreach (TwoPhaseGameElement element in columns)
            {
                element.DrawFirst(gameTime, batch, camera);
            }
        }

        public void DrawSecond(GameTime gameTime, SpriteBatch batch, Camera camera)
        {
            foreach (TwoPhaseGameElement element in columns)
            {
                element.DrawSecond(gameTime, batch, camera);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (TwoPhaseGameElement element in columns)
            {
                element.Update(gameTime);
            }
        }
    }
}
