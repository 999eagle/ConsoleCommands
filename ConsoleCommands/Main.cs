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

        private bool godmode, range, buildgrid;
        private Texture2D gridTexture;

        public override bool Initialize()
        {
            godmode = false;
            Events.XnaEvents.PostLoadContent.Register(this, OnPostLoadContent, 1);
            Events.XnaEvents.PostUpdate.Register(this, OnPostUpdate, 1);
            Events.TerrariaMainEvents.PreDrawInterface.Register(this, OnPreDrawInterface, 1);
            CC.AddCommnd(new List<string>() { "tgm", "godmode" }, "Toggles godmode for the player.", OnGodmodeCommand);
            CC.AddCommnd(new List<string> { "grid" }, "Toggles building grid.", OnGridCommand);
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
                    Terraria.MainSpriteBatch.Draw(gridTexture, new Vector2(w * gridTexture.Width + posX, j * gridTexture.Height + posY), new Rectangle(0, 0, gridTexture.Width, gridTexture.Height), new Color(100, 100, 100, 15), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
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
            Console.PrintConsole("Godmode toggled", ConsoleMessageType.Normal);
            args.Handled = true;
        }

        void OnGridCommand(ConsoleCommandArgs args)
        {
            buildgrid = !buildgrid;
            Console.PrintConsole("Building grid toggled", ConsoleMessageType.Normal);
            args.Handled = true;
        }
    }
}
