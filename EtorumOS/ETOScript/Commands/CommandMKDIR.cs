using EtorumOS.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.ETOScript.Commands {
    internal class CommandMKDIR : Command {
        public override void Execute(string[] args) {
            if (args.Length < 2) {
                Helpers.WriteLine(ConsoleColor.Red, "Syntax: mkdir <name/path>");
                return;
            }

            string tempPath = EtorumIO.CreatePath(args[1]);
            Directory.CreateDirectory(tempPath);
        }

        public override string Name => "mkdir";
    }
}
