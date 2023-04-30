using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Meridian2.Columns; //TODO: move amphoras into treasures and refactor it to diverse?

namespace Meridian2.Treasures {

    public class DiverseManager {

        private List<Amphora> amphoras;
        private List<Chest> treasures;

        public DiverseManager() {
            amphoras = new List<Amphora>();
            treasures = new List<Chest>();
        }

        public void LoadContent() {
            foreach(Chest c in treasures) {
                c.LoadContent();
            }
        }


        public void Update(GameTime gameTime) {
            foreach(Amphora a in amphoras) {
                a.Update(gameTime);
                if (a.isDestroyed) {
                    a.DestroyBody(); //destroy the body
                }
                if (a.hasExploded) {
                    amphoras.Remove(a);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
            foreach(Amphora a in amphoras) {
                a.Draw(gameTime, batch, camera);
            }
            foreach(Chest c in treasures) {
                c.Draw(gameTime, batch, camera);
            }
        }
    }
}