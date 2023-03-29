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

        public void DrawFirst(GameTime gameTime, SpriteBatch batch)
        {
            foreach (TwoPhaseGameElement element in columns)
            {
                element.DrawFirst(gameTime, batch);
            }
        }

        public void DrawSecond(GameTime gameTime, SpriteBatch batch)
        {
            foreach (TwoPhaseGameElement element in columns)
            {
                element.DrawSecond(gameTime, batch);
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
