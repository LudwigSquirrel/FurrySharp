using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace FurrySharp.Input;

public enum KeyFunctions
{
    Up = 1,
    Down,
    Left,
    Right,
}

/// <summary>
/// An easy handler to keep track of pressed keys and mouse buttons and has some methods to check if the are pressed or not.
/// </summary>
public static class KeyInput
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
            // Get the highest state of all keys bound to this function.
            state = key.Keys.Select(k => KeyState[k]).Max();
        }

        return state;
    }

    private static Dictionary<Keys, InputState> KeyState;
    public static Dictionary<KeyFunctions, RebindableKey> RebindableKeys;

    static KeyInput()
    {
        // Initialize the dictionary with all keys to None
        // ReSharper disable once SuspiciousTypeConversion.Global
        var keysEnumerable = (Enum.GetValues(typeof(Keys)) as IEnumerable<Keys>);
        KeyState = keysEnumerable!.ToDictionary(k => k, k => InputState.None);
        RebindableKeys = new Dictionary<KeyFunctions, RebindableKey>();
    }

    /// <summary>
    /// Updates the internal key states.
    /// </summary>
    public static void UpdateInputs()
    {
        KeyboardState s = Keyboard.GetState();

        foreach (Keys key in Enum.GetValues(typeof(Keys)))
        {
            KeyState[key] = UpdateInput(KeyState[key], s.IsKeyDown(key));
        }
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
}