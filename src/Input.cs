﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Meridian2; 

public class Input
{
    private static KeyboardState _currentKeyState;
    private static KeyboardState _previousKeyState;

    private static GamePadState _currentGamePadState;
    private static GamePadState _previousGamePadState;

    public static void GetState()
    {
        _previousKeyState = _currentKeyState;
        _currentKeyState = Keyboard.GetState();

        _previousGamePadState = _currentGamePadState;
        _currentGamePadState = GamePad.GetState(PlayerIndex.One);
    }

    public static bool IsKeyPressed(Keys key, bool once)
    {
        if(!once) return _currentKeyState.IsKeyDown(key);
        return _currentKeyState.IsKeyDown(key) && !_previousKeyState.IsKeyDown(key);
    }
    
    public static bool IsButtonPressed(Buttons button, bool once) {
        var type = _currentGamePadState.Buttons.GetType();
        var prevState = (ButtonState)(type.GetMethod("get_" + button).Invoke(_previousGamePadState.Buttons, null));
        var state = (ButtonState)(type.GetMethod("get_" + button).Invoke(_currentGamePadState.Buttons, null));
        
        if(!once) return state == ButtonState.Pressed;
        return state == ButtonState.Pressed && prevState != ButtonState.Pressed;
    }
}