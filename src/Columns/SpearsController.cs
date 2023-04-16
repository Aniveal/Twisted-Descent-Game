using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Meridian2.Theseus;

namespace Meridian2.Columns {

    public class SpearsController {
        private const int SpearCooldown = 2000;
        private const int placementDisantce = 1;
        private const float SpearWidth = 0.2f;
        private RopeGame _game;
        private ColumnsManager _columnsManager;
        private Player _player;
        private GameData _data;
        
        public double spearTimer;

        private Texture2D baseSpearTexture;
        private bool placing = false;


        public SpearsController(RopeGame game, ColumnsManager columnsManager, Player player) {
            _game = game;
            _data = game.gameData;
            _player = player;
            _columnsManager = columnsManager;
            spearTimer = 5000;
        }

        public void LoadContent() {
            baseSpearTexture = _game.Content.Load<Texture2D>("circle");
        }

        public void Update(GameTime gameTime) {
            spearTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            spearTimer = Math.Min(spearTimer, SpearCooldown);
            
            if (spearTimer < SpearCooldown) {
                //cannot place a new spear yet
                return;
            }

            KeyboardState keyboard = Keyboard.GetState();
            //basic column
            if (keyboard.IsKeyDown(Keys.D1)) {
                if (placing) {
                    return;
                }
                Vector2 pPos = _player.Body.Position;
                Vector2 pOr = _player.orientation;
                Vector2 sPos = pPos + pOr * 2;

                _columnsManager.Add(new Column(_game, _game._gameScreen.World, sPos, SpearWidth, baseSpearTexture));
                spearTimer = 0;
                placing = true;
                return;
            }
            //electric column
            if (keyboard.IsKeyDown(Keys.D2)) {
                if (placing) {
                    return;
                }
                Vector2 pPos = _player.Body.Position;
                Vector2 pOr = _player.orientation;
                Vector2 sPos = pPos + pOr;

                _columnsManager.Add(new ElectricColumn(_game, _game._gameScreen.World, sPos, SpearWidth, baseSpearTexture));
                spearTimer = 0;
                placing = true;
                return;
            }
            //fragile column
            if (keyboard.IsKeyDown(Keys.D3)) {
                if (placing) {
                    return;
                }
                Vector2 pPos = _player.Body.Position;
                Vector2 pOr = _player.orientation;
                Vector2 sPos = pPos + pOr;

                _columnsManager.Add(new FragileColumn(_game, _game._gameScreen.World, sPos, SpearWidth, baseSpearTexture));
                spearTimer = 0;
                placing = true;
                return;
            }
            placing = false;
        }
    }
}