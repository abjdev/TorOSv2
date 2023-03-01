using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.ETOScript.Commands {
    internal class CommandLS : Command {
        public override void Execute(string[] args) {
            if (args.Length < 1) {
                Helpers.WriteLine(ConsoleColor.Red, "Syntax: ls");
                return;
            }

            foreach(string dir in Directory.GetDirectories(Kernel.Instance.CurrentPath)) {
                Helpers.Write(ConsoleColor.Magenta, "<DIR>    ");
                Helpers.WriteLine(ConsoleColor.White, dir);
            }

            foreach (string file in Directory.GetFiles(Kernel.Instance.CurrentPath)) {
                if (file.EndsWith(".acl")) continue; // don't show acl files.
                Helpers.Write(ConsoleColor.Blue, "<FILE>   ");
                Helpers.WriteLine(ConsoleColor.White, file);
            }
        }

        public override string Name => "ls";
    }
}
