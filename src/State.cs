using Microsoft.Xna.Framework;

namespace Meridian2;

public abstract class Screen {
    private readonly RopeGame _game;
    public Camera Camera;

    public Screen(RopeGame game) {
        _game = game;
    }

    public virtual void Initialize() { }

    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(GameTime gameTime) { }

    protected RopeGame getGame() {
        return _game;
    }
}