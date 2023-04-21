using Microsoft.Xna.Framework;

namespace Meridian2;

public abstract class Screen {
    private RopeGame game;

    public Screen(RopeGame game) {
        this.game = game;
    }

    virtual public void Initialize() {
    }

    virtual public void Update(GameTime gameTime) {
    }

    virtual public void Draw(GameTime gameTime) {
    }

    protected RopeGame getGame() {
        return game;
    }
}