using System;
using System.Collections.Generic;
using System.Linq;
using FurrySharp.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static FurrySharp.Registry.GameConstants;

namespace FurrySharp.Input;

public enum KeyFunctions
{
    Up = 1,
    Down,
    Left,
    Right,
    Attack1,
}

public enum MouseButts
{
    Left,
    Right,
    Middle,
    Mouse4,
    Mouse5,
}

/// <summary>
/// An easy handler to keep track of pressed keys and mouse buttons and has some methods to check if the are pressed or not.
/// </summary>
public static class GameInput
{
    private enum InputState
    {
        None,
        Held,
        Pressed,
    };

    private static InputState UpdateInput(InputState current, bool pressed)
    {
        if (!pressed)
        {
            return InputState.None;
        }

        if (current == InputState.None)
        {
            return InputState.Pressed;
        }

        return InputState.Held;
    }

    private static InputState GetRebindableKeyState(KeyFunctions name)
    {
        InputState state = InputState.None;
        if (RebindableKeys.TryGetValue(name, out RebindableKey key))
        {
            if (key.Keys.Count > 0)
            {
                // Get the highest state of all keys bound to this function.
                state = key.Keys.Select(k => KeyState[k]).Max();    
            }
            
            if (key.MouseButts.Count > 0)
            {
                // Get the highest state of all the little rodent butts bound to this function.
                var state1 = key.MouseButts.Select(b => MouseState[b]).Max();
                state = state1 > state ? state1 : state;
            }
        }

        return state;
    }

    private static Dictionary<Keys, InputState> KeyState;
    private static Dictionary<MouseButts, InputState> MouseState;

    private static Dictionary<Keys, bool> MockKeyDown;
    private static Dictionary<MouseButts, bool> MockMouseButtDown;

    public static Dictionary<KeyFunctions, RebindableKey> RebindableKeys;
    public static Point PointerScreenPosition;

    static GameInput()
    {
        // Initialize the dictionary with all keys to None
        // ReSharper disable once SuspiciousTypeConversion.Global
        var keysEnumerable = (Enum.GetValues(typeof(Keys)) as IEnumerable<Keys>);
        var enumerable = keysEnumerable as Keys[] ?? keysEnumerable!.ToArray();
        KeyState = enumerable!.ToDictionary(k => k, _ => InputState.None);
        MockKeyDown = enumerable!.ToDictionary(k => k, _ => false);

        var mouseButtsEnumerable = (Enum.GetValues(typeof(MouseButts)) as IEnumerable<MouseButts>);
        var enumerable1 = mouseButtsEnumerable as MouseButts[] ?? mouseButtsEnumerable!.ToArray();
        MouseState = enumerable1!.ToDictionary(k => k, _ => InputState.None);
        MockMouseButtDown = enumerable1!.ToDictionary(k => k, _ => false);

        RebindableKeys = new Dictionary<KeyFunctions, RebindableKey>();
    }

    /// <summary>
    /// Updates the internal key states, and mouse states.
    /// </summary>
    public static void UpdateInputs()
    {
        MouseState m = Mouse.GetState();
        KeyboardState s = Keyboard.GetState();

        foreach (Keys key in Enum.GetValues(typeof(Keys)))
        {
            KeyState[key] = UpdateInput(KeyState[key], s.IsKeyDown(key) || MockKeyDown[key]);
        }

        MouseState[MouseButts.Left] = UpdateInput(MouseState[MouseButts.Left], m.LeftButton == ButtonState.Pressed || MockMouseButtDown[MouseButts.Left]);
        MouseState[MouseButts.Right] = UpdateInput(MouseState[MouseButts.Right], m.RightButton == ButtonState.Pressed || MockMouseButtDown[MouseButts.Right]);
        MouseState[MouseButts.Middle] = UpdateInput(MouseState[MouseButts.Middle], m.MiddleButton == ButtonState.Pressed || MockMouseButtDown[MouseButts.Middle]);
        MouseState[MouseButts.Mouse4] = UpdateInput(MouseState[MouseButts.Mouse4], m.XButton1 == ButtonState.Pressed || MockMouseButtDown[MouseButts.Mouse4]);
        MouseState[MouseButts.Mouse5] = UpdateInput(MouseState[MouseButts.Mouse5], m.XButton2 == ButtonState.Pressed || MockMouseButtDown[MouseButts.Mouse5]);

        Point p = m.Position;

        int x = -(SpriteDrawer.RenderDestination.X - p.X);
        int y = -(SpriteDrawer.RenderDestination.Y - p.Y);

        x = x * GAME_WIDTH_IN_PIXELS / SpriteDrawer.RenderDestination.Width;
        y = y * GAME_HEIGHT_IN_PIXELS / SpriteDrawer.RenderDestination.Height;

        PointerScreenPosition = new Point(x, y);
    }

    /// <summary>
    /// Checks if any key is down, returns true and outputs the pressed key.
    /// </summary>
    /// <param name="pressed"></param>
    /// <returns></returns>
    public static bool IsAnyKeyPressed(out Keys? pressed)
    {
        pressed = null;

        if (!KeyState.Any(k => k.Value == InputState.Pressed))
        {
            return false;
        }

        pressed = KeyState.FirstOrDefault(k => k.Value == InputState.Pressed).Key;

        return true;
    }

    /// <summary>
    /// Simply checks if a key is down.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool IsKeyPressed(Keys key)
    {
        return KeyState[key] != InputState.None;
    }

    /// <summary>
    /// Returns if a key was just pressed this frame.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool JustPressedKey(Keys key)
    {
        return KeyState[key] == InputState.Pressed;
    }

    public static bool IsFunctionPressed(KeyFunctions function)
    {
        return GetRebindableKeyState(function) != InputState.None;
    }

    public static bool JustPressedFunction(KeyFunctions function)
    {
        return GetRebindableKeyState(function) == InputState.Pressed;
    }

    public static void MockHeld(Keys key)
    {
        MockKeyDown[key] = true;
    }

    public static void MockHeld(MouseButts butt)
    {
        MockMouseButtDown[butt] = true;
    }
}