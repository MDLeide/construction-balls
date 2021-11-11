using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cashew.Utility.Async;
using CommandTerminal;
using UnityEngine;
using Object = UnityEngine.Object;


static class TerminalCommands
{
    public static TerminalCommandsComponent Component;

    [RegisterCommand(Help = "Completes all research", MinArgCount = 0, MaxArgCount = 0)]
    static void ResearchAll(CommandArg[] args)
    {
        Component.Research.ResearchAll();
    }

    [RegisterCommand(Help = "Completes research on an item")]
    static void Research(CommandArg[] args)
    {
        if (int.TryParse(args[0].String, out var id))
        {
            Component.Research.ResearchItem(id);
        }
        else
        {
            var name = string.Concat(args.Select(p => p.String));
            Component.Research.ResearchItem(name);
        }
    }

    [RegisterCommand(Help = "Spawns a ball in front of the player")]
    static void Ball(CommandArg[] args)
    {
        Ball ball;

        if (!args.Any())
            ball = Component.BlueBall;
        else
        {
            switch (args[0].String.ToLower())
            {
                case "blue":
                    ball = Component.BlueBall;
                    break;
                case "red":
                    ball = Component.RedBall;
                    break;
                case "yellow":
                    ball = Component.YellowBall;
                    break;
                default:
                    Debug.Log($"Could not spawn a ball of the color: {args[0].String}");
                    return;
            }
        }

        int qty = 1;
        if (args.Length > 0)
        {
            qty = args[1].Int;

            if (Terminal.IssuedError)
                return;
        }

        DoSpawn(ball.gameObject, qty);
    }

    [RegisterCommand(Help = "Spawns an item in front of the player", MinArgCount = 1, MaxArgCount = 2)]
    static void Spawn(CommandArg[] args)
    {
        CraftingRecipe recipe;

        if (int.TryParse(args[0].String, out var id))
        {
            recipe = Component.Crafting.AllRecipes.FirstOrDefault(p => p.ID == id);
            if (recipe == null)
                Debug.Log($"Could not find a recipe with the ID: {id}");
        }
        else
        {
            var itemName = args[0].String;
            recipe = Component.Crafting.AllRecipes.FirstOrDefault(p => NamesMatch(p.CraftPrototype.name, itemName));
            if (recipe == null)
                Debug.Log($"Could not find a recipe with the name: {itemName}");
        }
        
        var qty = 1;
        if (args.Length > 1)
            qty = args[1].Int;

        if (Terminal.IssuedError)
            return;


        DoSpawn(recipe.CraftPrototype, qty);

        bool NamesMatch(string a, string b)
        {
            return string.Equals(a.Replace(" ", ""), b.Replace(" ", ""), StringComparison.InvariantCultureIgnoreCase);
        }
    }

    static void DoSpawn(GameObject obj, int qty)
    {
        var o = Object.Instantiate(obj);
        o.transform.position = GetSpawnPoint();
        if (qty > 1)
            _ = Execute.Later(.1f, () => DoSpawn(obj, qty - 1));
    }

    static Vector3 GetSpawnPoint()
    {
        return Component.Player.Look.position + Component.Player.Look.forward * 3;
    }
}