using System;
using Meridian2.Theseus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Meridian2.Columns;

public class SpearsController
{
    private const int SpearCooldown = 2000;
    private const int PlacementDisantce = 1;
    private const float SpearWidth = 1.2f;

    private Texture2D _baseSpearTexture;
    private Texture2D _normalSpearTexture;

    private Texture2D _woodenSpear;
    private Texture2D _woodenSpearAnimation;
    private Texture2D _woodenSpearBroken;

    private Texture2D _metalSpear;
    private Texture2D _electricSpear;
    private Texture2D _electricSpearAnimation;

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


    public SpearsController(RopeGame game, ColumnsManager columnsManager, Player player)
    {
        _game = game;
        _data = game.GameData;
        _player = player;
        _columnsManager = columnsManager;
        SpearTimer = 5000;
    }

    public void LoadContent()
    {
        _baseSpearTexture = _game.Content.Load<Texture2D>("circle");
        _normalSpearTexture = _game.Content.Load<Texture2D>("Sprites/Spear/spear");

        _woodenSpear = _game.Content.Load<Texture2D>("Sprites/Spear/wooden_spear");
        _woodenSpearAnimation = _game.Content.Load<Texture2D>("Sprites/Spear/wooden_spear_animation");
        _woodenSpearBroken = _game.Content.Load<Texture2D>("Sprites/Spear/wooden_spear_lower");

        _metalSpear = _game.Content.Load<Texture2D>("Sprites/Spear/metal_spear");

        _electricSpear = _game.Content.Load<Texture2D>("Sprites/Spear/lightning_spear");
        _electricSpearAnimation = _game.Content.Load<Texture2D>("Sprites/Spear/lightning_spear_animation");
    }

    public void Update(GameTime gameTime)
    {
        SpearTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
        SpearTimer = Math.Min(SpearTimer, SpearCooldown);

        if (SpearTimer < SpearCooldown)
        {
            //cannot place a new spear yet
            _placing = false;
            return;
        }

        var keyboard = Keyboard.GetState();
        //move selection left
        if (keyboard.IsKeyDown(Keys.Q) || Input.IsButtonPressed(Buttons.LeftShoulder, true))
        {
            //TODO: adapt to controler
            if (!LeftDown)
            {
                Selected = Selected == 0 ? 2 : Selected - 1;
                LeftDown = true;
            }
        }
        else
        {
            LeftDown = false;
        }

        //move selection right
        if (keyboard.IsKeyDown(Keys.E) || Input.IsButtonPressed(Buttons.RightShoulder, true))
        {
            //TODO adapt to controler
            if (!RightDown)
            {
                Selected = Selected == 2 ? 0 : Selected + 1;
                RightDown = true;
            }
        }
        else
        {
            RightDown = false;
        }

        //place selected spear
        if (keyboard.IsKeyDown(Keys.R) || Input.IsButtonPressed(Buttons.X, true))
        {
            if (!PlaceDown)
            {
                if (_data.Spears[Selected] < 1)
                {
                    //TODO play animation to highlight you cannot place spear
                }
                else
                {
                    PlaceDown = true;
                    var pPos = _player.Body.Position;
                    var pOr = _player.Orientation;
                    var sPos = pPos + pOr * 2;
                    _data.Spears[Selected]--;
                    switch (Selected)
                    {
                        case 0:
                            _columnsManager.Add(new Column(_game._gameScreen.World, sPos, SpearWidth,
                                _metalSpear, true));
                            break;
                        case 1:
                            _columnsManager.Add(new ElectricColumn(_game._gameScreen.World, sPos, SpearWidth,
                                _electricSpear, _electricSpearAnimation, true));
                            break;
                        case 2:
                            _columnsManager.Add(new FragileColumn(_game._gameScreen.World, sPos, SpearWidth,
                                _woodenSpear, _woodenSpearBroken, _woodenSpearAnimation, true));
                            break;
                    }
                }
            }
        }
        else
        {
            PlaceDown = false;
        }

    }

    public void PlaceSpear(float x, float y, int type)
    {
        switch (type)
        {
            case 0:
                _columnsManager.Add(new Column(_game._gameScreen.World, new Vector2(x, y), SpearWidth,
                    _normalSpearTexture, true));
                break;
            case 1:
                _columnsManager.Add(new ElectricColumn(_game._gameScreen.World, new Vector2(x, y), SpearWidth,
                    _normalSpearTexture, true));
                break;
            case 2:
                _columnsManager.Add(new FragileColumn(_game._gameScreen.World, new Vector2(x, y), SpearWidth,
                    _normalSpearTexture, _normalSpearTexture, true));
                break;
        }
    }
}