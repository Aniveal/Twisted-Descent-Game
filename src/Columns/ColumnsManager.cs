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
        private List<DrawableGameElement> columns;

        public ColumnsManager()
        {
            columns = new List<DrawableGameElement>();
            
        }

        public void Add(DrawableGameElement column)
        {
            columns.Add(column);
        }

        public void Remove(DrawableGameElement column)
        {
            columns.Remove(column);
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
            foreach (DrawableGameElement element in columns)
            {
                element.Draw(gameTime, batch, camera);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (DrawableGameElement element in columns)
            {
                element.Update(gameTime);
            }
        }
    }
}
