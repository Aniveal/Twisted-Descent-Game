using System.Collections.Generic;
using System.Linq;
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

        public void Add(Amphora amphora) {
            amphoras.Add(amphora);
        }

        public void Add(Chest chest) {
            treasures.Add(chest);
        }

        public void LoadContent() {
            foreach(Chest c in treasures) {
                c.LoadContent();
            }
            foreach(Amphora a in amphoras) {
                a.LoadContent();
            }
        }

        public void Update(GameTime gameTime) {
            foreach(Amphora a in amphoras.ToList()) {
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