using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.ETOScript.Commands {
    internal class CommandREAD : Command {
        public override void Execute(string[] args) {
            if (args.Length < 2) {
                Helpers.WriteLine(ConsoleColor.Red, "Syntax: read <path>");
                return;
            }

            string tempPath = Path.GetFullPath(Path.Combine(Kernel.Instance.CurrentPath, args[1]));

            if (!File.Exists(tempPath)) {
                Helpers.WriteLine(ConsoleColor.Red, "File was not found. (was looking for " + tempPath + ")");
                return;
            }

            if(!EtorumIO.CurrentUserCan(PermissionType.READ_FILE, tempPath)) {
                Helpers.WriteLine(ConsoleColor.Red, "Access to file is denied.");
                return;
            }

            EtorumConsole.WriteLine(File.ReadAllText(tempPath));
        }

        public override string Name => "read";
    }
}
