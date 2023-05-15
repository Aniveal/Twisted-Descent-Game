using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2.Theseus; 

public class TheseusManager {
    private readonly RopeGame _game;
    private readonly World _world;
    public Player Player;
    public Rope Rope;

    public TheseusManager(RopeGame game, World world) {
        _game = game;
        _world = world;
        Rope = new Rope(_game, _world, new Vector2(0, 0), 10);
        Player = new Player(_game, _world, Rope);
    }

    public void Initialize() {
        Rope.SetStartPosition(_game._gameScreen._map.getRopeStartPosition());
        
        Rope.Initialize();
        Player.Initialize();
    }

    public void LoadContent() {
        Rope.LoadContent();
        Player.LoadContent();
    }

    public void Update(GameTime gameTime) {
        Player.Update(gameTime);
        Rope.Update(gameTime);
    }

    public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        Rope.Draw(gameTime, batch, camera);
        Player.Draw(gameTime, batch, camera);
    }
}