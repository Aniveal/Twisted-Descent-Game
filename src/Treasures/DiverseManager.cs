using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Meridian2.Columns; //TODO: move amphoras into treasures and refactor it to diverse?

namespace Meridian2.Treasures {

    public class DiverseManager {

        private List<Amphora> amphoras;
        private List<Chest> treasures;
        private List<BodyWithText> texts;

        public DiverseManager() {
            amphoras = new List<Amphora>();
            treasures = new List<Chest>();
            texts = new List<BodyWithText>();
        }

        public void Add(Amphora amphora) {
            amphoras.Add(amphora);
        }

        public void Add(Chest chest) {
            treasures.Add(chest);
        }

        public void Add(BodyWithText body) {
            texts.Add(body);
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
            foreach (BodyWithText t in texts.ToList()) {
                t.Update(gameTime);
                if (t.onDisplay) {
                    t.Destroy();
                }
                if (t.finished) { 
                    texts.Remove(t);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
            foreach(Amphora a in amphoras) {
                if (camera.IsVisible(a._body.Position)) {
                    a.Draw(gameTime, batch, camera);
                }
            }
            foreach(Chest c in treasures) {
                if (camera.IsVisible(c.Pos)) {
                    c.Draw(gameTime, batch, camera);
                }
            }
            foreach (BodyWithText t in texts) {
                if (t.onDisplay) {
                    t.Draw(gameTime, batch, camera);
                }
            }
        }
    }
}