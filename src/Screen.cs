using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2; 

public abstract class State {
    #region Fields

    protected ContentManager Content;

    protected GraphicsDevice GraphicsDevice;

    protected RopeGame Game;

    #endregion

    #region Methods

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    public abstract void PostUpdate(GameTime gameTime);

    public State(RopeGame game, GraphicsDevice graphicsDevice, ContentManager content) {
        Game = game;

        GraphicsDevice = graphicsDevice;

        Content = content;
    }

    public abstract void Update(GameTime gameTime);

    #endregion
}