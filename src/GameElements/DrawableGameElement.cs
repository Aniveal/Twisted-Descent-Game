using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwistedDescent.GameElements; 

public class DrawableGameElement : IDrawableObject, IUpdatableObject {
    public virtual void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) { }

    public virtual void Update(GameTime gameTime) { }
}