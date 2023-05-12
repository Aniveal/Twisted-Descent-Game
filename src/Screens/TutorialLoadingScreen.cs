using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Content;

namespace Meridian2.Screens;

public class TutorialLoadingScreen : LoadingScreen
{
    public TutorialLoadingScreen(RopeGame game, ContentManager content) : base(game, content)
    {
        
    }

    public override void Update(GameTime gameTime) {
        timer += gameTime.ElapsedGameTime.TotalMilliseconds;

        if (!gameLoaded && timer > 100)
        {
            gameLoaded = true;
            base.getGame().GameData = new GameData(base.getGame());
            base.getGame()._gameScreen = new GameScreen(base.getGame(), true);
            base.getGame()._mapScreen = new MapScreen(base.getGame());
            base.getGame()._gameScreen.Initialize();
            base.getGame()._mapScreen.Initialize();
            base.getGame().ChangeState(RopeGame.State.Running);
        }
    }
}