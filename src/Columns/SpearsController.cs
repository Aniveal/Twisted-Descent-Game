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
        public bool leftDown;
        public bool rightDown;
        public bool placeDown;

        private Texture2D baseSpearTexture;
        private bool placing = false;

        //id of the selected spear that will be placed on button press
        public int selected = 0;


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
                placing = false;
                return;
            }

            KeyboardState keyboard = Keyboard.GetState();
            //TODO: remove placing with 1,2,3. Left in for easier testing
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
            //move selection left
            if (keyboard.IsKeyDown(Keys.Q)) { //TODO: adapt to controler
                if (!leftDown) {
                    selected = selected == 0 ? 2 : selected-1;
                    leftDown = true;
                }
                
            } else {
                leftDown = false;
            }
            //move selection right
            if (keyboard.IsKeyDown(Keys.E)) { //TODO adapt to controler
                if (!rightDown) {
                    selected = selected == 2 ? 0 : selected+1;
                    rightDown = true;
                }
            } else {
                rightDown = false;
            }
            //place selected spear
            if (keyboard.IsKeyDown(Keys.R)) {
                if (!placeDown) {
                    if (_data.spears[selected] < 1) {
                        //TODO play animation to highlight you cannot place spear
                    } else {
                        placeDown = true;
                        Vector2 pPos = _player.Body.Position;
                        Vector2 pOr = _player.orientation;
                        Vector2 sPos = pPos + pOr * 2;
                        _data.spears[selected]--;
                        switch(selected) {
                            case 0:
                                _columnsManager.Add(new Column(_game, _game._gameScreen.World, sPos, SpearWidth, baseSpearTexture));
                                break;
                            case 1:
                                _columnsManager.Add(new ElectricColumn(_game, _game._gameScreen.World, sPos, SpearWidth, baseSpearTexture));
                                break;
                            case 2:
                                _columnsManager.Add(new FragileColumn(_game, _game._gameScreen.World, sPos, SpearWidth, baseSpearTexture));
                                break;
                            default:
                                break;
                    }
                    }
                }
            } else {
                placeDown = false;
            }
        }
    }
}