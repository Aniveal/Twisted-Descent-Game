using System;
using Meridian2.Theseus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Meridian2.Columns; 

public class SpearsController {
    private const int SpearCooldown = 2000;
    private const int PlacementDisantce = 1;
    private const float SpearWidth = 0.2f;

    private Texture2D _baseSpearTexture;
    private ColumnTextures normalSpearTexture;
    private readonly ColumnsManager _columnsManager;
    private readonly GameData _data;
    private readonly RopeGame _game;
    private bool _placing;
    private readonly Player _player;
    public bool LeftDown;
    public bool PlaceDown;
    public bool RightDown;

    //id of the selected spear that will be placed on button press
    public int Selected;

    public double SpearTimer;


    public SpearsController(RopeGame game, ColumnsManager columnsManager, Player player) {
        _game = game;
        _data = game.GameData;
        _player = player;
        _columnsManager = columnsManager;
        SpearTimer = 5000;
    }

    public void LoadContent() {
        _baseSpearTexture = _game.Content.Load<Texture2D>("circle");
        normalSpearTexture = new ColumnTextures(_game.Content.Load<Texture2D>("Sprites/Spear/spear_lower2"),
            _game.Content.Load<Texture2D>("Sprites/Spear/spear_upper2"));

    }

    public void Update(GameTime gameTime) {
        SpearTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
        SpearTimer = Math.Min(SpearTimer, SpearCooldown);

        if (SpearTimer < SpearCooldown) {
            //cannot place a new spear yet
            _placing = false;
            return;
        }

        var keyboard = Keyboard.GetState();
        //TODO: remove placing with 1,2,3. Left in for easier testing
        //basic column
        if (keyboard.IsKeyDown(Keys.D1)) {
            if (_placing) return;
            var pPos = _player.Body.Position;
            var pOr = _player.Orientation;
            var sPos = pPos + pOr * 2;

            _columnsManager.Add(new Column(_game, _game._gameScreen.World, sPos, SpearWidth, normalSpearTexture, true));
            SpearTimer = 0;
            _placing = true;
            return;
        }

        //electric column
        if (keyboard.IsKeyDown(Keys.D2)) {
            if (_placing) return;
            var pPos = _player.Body.Position;
            var pOr = _player.Orientation;
            var sPos = pPos + pOr;

            _columnsManager.Add(new ElectricColumn(_game, _game._gameScreen.World, sPos, SpearWidth, _baseSpearTexture));
            SpearTimer = 0;
            _placing = true;
            return;
        }

        //fragile column
        if (keyboard.IsKeyDown(Keys.D3)) {
            if (_placing) return;
            var pPos = _player.Body.Position;
            var pOr = _player.Orientation;
            var sPos = pPos + pOr;

            _columnsManager.Add(new FragileColumn(_game, _game._gameScreen.World, sPos, SpearWidth, _baseSpearTexture));
            SpearTimer = 0;
            _placing = true;
            return;
        }

        //move selection left
        if (keyboard.IsKeyDown(Keys.Q)) {
            //TODO: adapt to controler
            if (!LeftDown) {
                Selected = Selected == 0 ? 2 : Selected - 1;
                LeftDown = true;
            }
        } else {
            LeftDown = false;
        }

        //move selection right
        if (keyboard.IsKeyDown(Keys.E)) {
            //TODO adapt to controler
            if (!RightDown) {
                Selected = Selected == 2 ? 0 : Selected + 1;
                RightDown = true;
            }
        } else {
            RightDown = false;
        }

        //place selected spear
        if (keyboard.IsKeyDown(Keys.R)) {
            if (!PlaceDown) {
                if (_data.Spears[Selected] < 1) {
                    //TODO play animation to highlight you cannot place spear
                } else {
                    PlaceDown = true;
                    var pPos = _player.Body.Position;
                    var pOr = _player.Orientation;
                    var sPos = pPos + pOr * 2;
                    _data.Spears[Selected]--;
                    switch (Selected) {
                        case 0:
                            _columnsManager.Add(new Column(_game, _game._gameScreen.World, sPos, SpearWidth,
                                normalSpearTexture, true));
                            break;
                        case 1:
                            _columnsManager.Add(new ElectricColumn(_game, _game._gameScreen.World, sPos, SpearWidth,
                                _baseSpearTexture));    
                            break;
                        case 2:
                            _columnsManager.Add(new FragileColumn(_game, _game._gameScreen.World, sPos, SpearWidth,
                                _baseSpearTexture));
                            break;
                    }
                }
            }
        } else {
            PlaceDown = false;
        }
    }
}