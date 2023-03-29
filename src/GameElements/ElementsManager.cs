using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meridian2.Interfaces
{
    /**
     * Propagates Update and Draw calls to a list of Game Elements
     * 
     * 
     */
    public class ElementsManager
    {
        private List<DrawableGameElement> gameElements;
        private SpriteBatch _batch;

        public ElementsManager(SpriteBatch batch)
        {
            gameElements = new List<DrawableGameElement>();
            _batch = batch;
        }

        public void AddColumn(DrawableGameElement column)
        {
            gameElements.Add(column);
        }

        public void RemoveColumn(DrawableGameElement column)
        {
            gameElements.Remove(column);
        }

        public void Draw(GameTime gameTime)
        {
            _batch.Begin();
            foreach (DrawableGameElement element in gameElements)
            {
                element.Draw(gameTime, _batch);
            }
            _batch.End();
        }

        public void Update(GameTime gameTime)
        {
            foreach (DrawableGameElement element in gameElements)
            {
                element.Update(gameTime);
            }
        }
    }
}
