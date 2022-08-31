using EtorumOS.External.MIV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.ETOScript.Commands {
    // Edit command is using MIV
    internal class CommandEDIT : Command {

        public override void Execute(string[] args) {
            if (args.Length < 2) {
                Helpers.WriteLine(ConsoleColor.Red, "Syntax: edit <path>");
                return;
            }

            string tempPath = Path.GetFullPath(Path.Combine(Kernel.Instance.CurrentPath, args[1]));

            if (!File.Exists(tempPath)) {
                Helpers.WriteLine(ConsoleColor.Red, "File was not found. (was looking for " + tempPath + ")");
                return;
            }

            string res = MIV.Miv(File.ReadAllText(tempPath));

            if (res == null)
            {
                Console.WriteLine("Not saved.");
                return;
            }

            File.WriteAllText(tempPath, res);
            Console.WriteLine("Saved successfully.");
        }

        public override string GetName() {
            return "edit";
        }
    }
}
