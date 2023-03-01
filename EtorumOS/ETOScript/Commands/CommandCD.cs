using EtorumOS.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.ETOScript.Commands {
    internal class CommandCD : Command {
        public override void Execute(string[] args) {
            if (args.Length < 2) {
                Helpers.WriteLine(ConsoleColor.Red, "Syntax: cd <path>");
                return;
            }

            string tempPath = EtorumIO.CreatePath(args[1]);

            if(!Directory.Exists(tempPath)) {
                Helpers.WriteLine(ConsoleColor.Red, "Path was not found. (was looking for " + tempPath + ")");
                return;
            }

            Kernel.Instance.CurrentPath = tempPath;
        }

        public override string Name => "cd";
    }
}
