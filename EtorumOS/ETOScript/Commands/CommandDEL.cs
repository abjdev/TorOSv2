using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.ETOScript.Commands {
    internal class CommandDEL : Command {
        public override void Execute(string[] args) {
            if (args.Length < 2) {
                Helpers.WriteLine(ConsoleColor.Red, "Syntax: del <path>");
                return;
            }

            string tempPath = Path.GetFullPath(Path.Combine(Kernel.Instance.CurrentPath, args[1]));

            if (!File.Exists(tempPath)) {
                Helpers.WriteLine(ConsoleColor.Red, "File was not found. (was looking for " + tempPath + ")");
                return;
            }

            if(!EtorumIO.CurrentUserCan(PermissionType.WRITE_FILE, tempPath))
            {
                Helpers.WriteLine(ConsoleColor.Red, "Permission denied.");
                return;
            }

            File.Delete(tempPath);
        }

        public override string Name => "del";
    }
}
