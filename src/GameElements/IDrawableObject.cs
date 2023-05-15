using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwistedDescent.GameElements; 

internal interface IDrawableObject {
    //TODO: we will probably want to add the camera tranform as an argument to the draw method
    public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera);
}