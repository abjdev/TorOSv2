using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.ETOScript.Commands {
    internal class CommandUI : Command {
        public override void Execute(string[] args) {
            if (args.Length < 2) {
                Helpers.WriteLine(ConsoleColor.Red, "Are you sure you want to switch to graphics mode? This mode is currently unstable, glitchy and slow. (y/n)");
                var res = Console.ReadKey();
                if(res.KeyChar.ToString().ToLowerInvariant() == "y") EtorumConsole.SwitchToGraphicsMode();
                return;
            }
            
            if (args[1].ToLower() == "redraw") {
                var res = EtorumConsole.RedrawWhole();

                if(res != null) {
                    Helpers.WriteLine(ConsoleColor.Red, res.Message);
                }
            }
            
        }

        public override string Name => "ui";
    }
}
