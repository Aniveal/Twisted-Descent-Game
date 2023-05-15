using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace TwistedDescent.Screens; 

public abstract class Screen {
    protected readonly RopeGame _game;
    public Camera Camera;

    public World World;

    public Screen(RopeGame game) {
        _game = game;
    }

    public virtual void Initialize() { }

    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch = null) { }

    protected RopeGame getGame() {
        return _game;
    }
}