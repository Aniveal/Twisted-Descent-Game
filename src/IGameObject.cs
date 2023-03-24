using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2; 

public interface IGameObject : IGameComponent {
    void LoadContent();
    void Update(GameTime gameTime);
    void Draw(GameTime gameTime, SpriteBatch batch);
}