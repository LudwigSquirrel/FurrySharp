using System;
using System.Collections.Generic;
using System.Linq;
using FurrySharp.Multiplatform;
using FurrySharp.Resources;
using FurrySharp.States;
using FurrySharp.UI;
using FurrySharp.UI.Font;

namespace FurrySharp.Parsing;

public static class TerminalCommandParser
{
    public static void DoCommand(string input, Terminal output)
    {
        if (input == "" || input == null)
        {
            return;
        }
        var split = SplitInput(input);
        var methods = typeof(TerminalCommands).GetMethods();
        foreach (var method in methods)
        {
            // Find the method with the same name as the first word of the input.
            if (method.Name == split[0])
            {
                // Check if the signature is correct.
                var parameters = method.GetParameters();
                if (parameters[0].ParameterType != typeof(Terminal))
                {
                    output.PrintError($"First argument for method {method.Name} must be of type {nameof(Terminal)}.");
                    return;
                }
                if (split.Count != parameters.Length) // this works because the first parameter is always Terminal, and the first word of the input is the method name.
                {
                    output.PrintError($"Invalid number of arguments for method {method.Name}. Expected {parameters.Length}, got {split.Count}.");
                    return;
                }
                object[] parametersToPass = new object[split.Count];
                parametersToPass[0] = output;
                for (var i = 1; i < split.Count; i++)
                {
                    if (parameters[i].ParameterType == typeof(int))
                    {
                        if (!int.TryParse(split[i], out int result))
                        {
                            output.PrintError($"Invalid argument {split[i]} for method {method.Name}. Expected int.");
                            return;
                        }
                        parametersToPass[i] = result;
                    }
                    else if (parameters[i].ParameterType == typeof(float))
                    {
                        if (!float.TryParse(split[i], out float result))
                        {
                            output.PrintError($"Invalid argument {split[i]} for method {method.Name}. Expected float.");
                            return;
                        }
                        parametersToPass[i] = result;
                    }
                    else if (parameters[i].ParameterType == typeof(double))
                    {
                        if (!double.TryParse(split[i], out double result))
                        {
                            output.PrintError($"Invalid argument {split[i]} for method {method.Name}. Expected double.");
                            return;
                        }
                        parametersToPass[i] = result;
                    }
                    else if (parameters[i].ParameterType == typeof(string))
                    {
                        parametersToPass[i] = split[i];
                    }
                    else
                    {
                        output.PrintError($"Invalid argument type for method {method.Name}. Expected int, float, double, or string.");
                        return;
                    }
                }
                method.Invoke(null, parametersToPass);
                return;
            }
        }
    }

    private static List<string> SplitInput(string input)
    {
        return input.Split('"')
            .Select((element, index) => index % 2 == 0  // If even index
                ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
                : new string[] { element })  // Keep the entire item
            .SelectMany(element => element).ToList();
    }
}

public static class TerminalCommands
{
    public static void test(Terminal output)
    {
        output.PrintInfo("Test!");
    }
    
    public static void rltex(Terminal output, string texturePath)
    {
        output.PrintInfo($"Reloading texture {texturePath}");
    }

    public static void clr(Terminal output)
    {
        output.ClearOutput();
    }
    
    public static void crash(Terminal output)
    {
        throw new Exception("OwO user made me crashy washy >w<");
    }
    
    public static void spawn(Terminal output, string entityName)
    {
        if (Program.FurryGame.CurrentState is SandboxState sandboxState)
        {
            var result = sandboxState.EntityManager.Spawn(entityName);
            if (result)
            {
                output.PrintInfo($"Spawned entity {entityName}.");
            }
            else
            {
                output.PrintError($"Failed to spawn entity {entityName}.");
            }
        }
    }
    
    public static void kill(Terminal output, int entityId)
    {
        if (Program.FurryGame.CurrentState is SandboxState sandboxState)
        {
            var entity = sandboxState.EntityManager.GetEntity(entityId);
            var result = sandboxState.EntityManager.RemoveEntity(entity);
            if (result)
            {
                output.PrintInfo($"Killed entity {entityId}.");
            }
            else
            {
                output.PrintError($"Failed to kill {entityId}.");
            }
        }
    }
}