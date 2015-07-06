using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DetoxAPI;
using DetoxAPI.EventArgs;
using Detox.Classes;
using CC = DetoxAPI.ConsoleCommands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Console = DetoxAPI.Console;

namespace ConsoleCommands
{
    [DetoxApiVersion(1, 0)]
    public class Main : DetoxPlugin
    {
        public override string Author { get { return "999eagle"; } }
        public override string Name { get { return "ConsoleCommands"; } }
        public override string Description { get { return "This plugin adds some console commands."; } }
        public override Version Version { get { return new Version(1, 0); } }

        private bool godmode, buildgrid;
        private Texture2D gridTexture;
        private Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.1f);

        private string[] allItemNames = null;

        public override bool Initialize()
        {
            godmode = false;
            Events.XnaEvents.PostLoadContent.Register(this, OnPostLoadContent, 1);
            Events.XnaEvents.PostUpdate.Register(this, OnPostUpdate, 1);
            Events.TerrariaMainEvents.PreDrawInterface.Register(this, OnPreDrawInterface, 1);
            CC.AddCommnd(new List<string>() { "tgm", "godmode" }, "Toggles godmode for the player.", OnGodmodeCommand);
            CC.AddCommnd(new List<string> { "grid" }, "Toggles building grid.", OnGridCommand);
            CC.AddCommnd(new List<string> { "give" }, "Gives the player an item.", OnGiveCommand);
            CC.AddCommnd(new List<string> { "time" }, "Sets the current time.", OnTimeCommand);
            return true;
        }

        void OnPostLoadContent(EventArgs args)
        {
            gridTexture = new Texture2D(Terraria.MainGame.GraphicsDevice, 16, 16);
            gridTexture.SetData(new[]
                {
                    Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White,
                    Color.White, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White,
                    Color.White, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White,
                    Color.White, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White,
                    Color.White, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White,
                    Color.White, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White,
                    Color.White, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White,
                    Color.White, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White,
                    Color.White, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White,
                    Color.White, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White,
                    Color.White, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White,
                    Color.White, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White,
                    Color.White, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White,
                    Color.White, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White,
                    Color.White, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White,
                    Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White
                });
        }

        void OnPreDrawInterface(EventArgs args)
        {
            if (!buildgrid || gridTexture == null)
                return;

            // Calculate the grid position..
            var screenPosition = Terraria.GetMainField<Vector2>("screenPosition");
            var posX = (int)((int)(screenPosition.X / 16f) * 16 - screenPosition.X);
            var posY = (int)((int)(screenPosition.Y / 16f) * 16 - screenPosition.Y);
            var width = Terraria.GetMainField<int>("screenWidth") / gridTexture.Width;
            var height = Terraria.GetMainField<int>("screenHeight") / gridTexture.Height;

            // Draw the grid..
            for (var w = 0; w <= width + 1; w++)
            {
                for (var j = 0; j <= height + 1; j++)
                    Terraria.MainSpriteBatch.Draw(gridTexture, new Vector2(w * gridTexture.Width + posX, j * gridTexture.Height + posY), new Rectangle(0, 0, gridTexture.Width, gridTexture.Height), gridColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            }
        }

        void OnPostUpdate(XnaUpdateEventArgs args)
        {
            var player = DetoxPlayers.LocalPlayer;
            if (godmode)
            {
                player["statLife"] = player["statLifeMax"];
                player["statMana"] = player["statManaMax"];
                player["breath"] = player["breathMax"];
                player["dead"] = false;
                player["pvpDeath"] = false;
                player["noFallDmg"] = true;
                player["noKnockback"] = true;
            }
        }

        void OnGodmodeCommand(ConsoleCommandArgs args)
        {
            godmode = !godmode;
            Console.PrintConsole("Godmode toggled", ConsoleMessageType.About);
            args.Handled = true;
        }

        void OnGridCommand(ConsoleCommandArgs args)
        {
            buildgrid = !buildgrid;
            Console.PrintConsole("Building grid toggled", ConsoleMessageType.About);
            args.Handled = true;
        }

        int GetItemId(string itemNameOrId)
        {
            if (allItemNames == null)
            {
                allItemNames = Terraria.GetMainField<string[]>("itemName");
            }
            var itemId = -1;
            if (Int32.TryParse(itemNameOrId, out itemId))
            {
                return itemId;
            }
            else
            {
                for (int j = 0; j < allItemNames.Length; j++)
                {
                    if (allItemNames[j].ToLower() == itemNameOrId.ToLower())
                    {
                        return j;
                    }
                }
            }
            return -1;
        }

        void OnGiveCommand(ConsoleCommandArgs args)
        {
            if (args.Arguments.Count > 1)
            {
                var allItemNames = Terraria.GetMainField<string[]>("itemName");
                var itemIds = new List<int>();
                var player = DetoxPlayers.LocalPlayer;
                int successes = 0;
                for (int i = 1; i < args.Arguments.Count; i += 2)
                {
                    var itemId = GetItemId(args.Arguments[i]);
                    var count = -1;
                    if (itemId != -1)
                    {
                        if (Int32.TryParse(args.Arguments[i + 1], out count))
                        {
                            for (int j = 0; j < count; j++)
                            {
                                player.Invoke("PutItemInInventory", itemId, -1);
                            }
                            successes++;
                        }
                        else
                        {
                            Console.PrintConsole(args.Arguments[i + 1] + " is not a valid integer!", ConsoleMessageType.Warning);
                        }
                    }
                    else
                    {
                        Console.PrintConsole("Item \"" + args.Arguments[i] + "\" not found!", ConsoleMessageType.Warning);
                    }
                }
                if (successes > 0)
                {
                    Console.PrintConsole("Put " + successes + " item" + (successes > 1 ? "s" : "") + "into your inventory!", ConsoleMessageType.About);
                }
                else
                {
                    Console.PrintConsole("Couldn't put any item into your inventory.", ConsoleMessageType.Warning);
                }
            }
            else
            {
                Console.PrintConsole("No item specified!", ConsoleMessageType.Error);
                Console.PrintConsole("Usage: /give <item1> <count1> (<item2> <count2> ...)", ConsoleMessageType.Normal);
            }
            args.Handled = true;
        }

        void OnTimeCommand(ConsoleCommandArgs args)
        {
            args.Handled = true;
            if (args.Arguments.Count == 2)
            {
                double time;
                if (Double.TryParse(args.Arguments[1], out time))
                {
                    Terraria.SetMainField("time", time);
                    Console.PrintConsole("Time set!", ConsoleMessageType.About);
                }
                else
                {
                    Console.PrintConsole("Invalid time specified!", ConsoleMessageType.Error);
                    Console.PrintConsole("Usage: /time <time>", ConsoleMessageType.Normal);
                }
            }
            else
            {
                Console.PrintConsole("No time specified!", ConsoleMessageType.Error);
                Console.PrintConsole("Usage: /time <time>", ConsoleMessageType.Normal);
            }
        }
    }
}
