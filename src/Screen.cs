using Microsoft.Xna.Framework;

namespace Meridian2;

public abstract class Screen {
    private RopeGame game;

    public Screen(RopeGame game) {
        this.game = game;
    }

    virtual public void Init() {
    }

    virtual public void Update(GameTime gameTime) {
    }

    virtual public void Draw() {
    }

    protected RopeGame getGame() {
        return game;
    }
}