using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLab
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Camera _camera;
        private Player player;
        private Enemy enemy;

        public static int ScreenHeight;
        public static int ScreenWidth;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            ScreenHeight = graphics.PreferredBackBufferHeight;
            ScreenWidth = graphics.PreferredBackBufferWidth;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var playerTexture = Content.Load<Texture2D>("Sprite");
            var enemyTexture = Content.Load<Texture2D>("Enemy");

            _camera = new Camera();

            player = new Player(playerTexture)
            {
                Position = new Vector2(100, 100),
                Origin = new Vector2(20, 20),
            };

            enemy = new Enemy(enemyTexture)
            {
                Position = new Vector2(400, 200),
                Origin = new Vector2(15, 15),
            };
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            player.Update(gameTime);
            enemy.Update(0, player);
            _camera.Follow(player);

            base.Update(gameTime);
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(transformMatrix: _camera.Transform);

            player.Draw(spriteBatch);
            enemy.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}