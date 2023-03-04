using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EtorumOS.IO;
using libLowSpagVM;

namespace EtorumOS.ETOScript.Commands
{
    internal class CommandLVM : Command {
        public override void Execute(string[] args) {
            if (args.Length < 2) {
                Helpers.WriteLine(ConsoleColor.Red, "Syntax: lvm <path>");
                return;
            }

            string tempPath = EtorumIO.CreatePath(args[1]);

            if (!File.Exists(tempPath)) {
                Helpers.WriteLine(ConsoleColor.Red, "File was not found. (was looking for " + tempPath + ")");
                return;
            }

            if(!EtorumIO.CurrentUserCan(PermissionType.READ_FILE, tempPath)) {
                Helpers.WriteLine(ConsoleColor.Red, "Access to file is denied.");
                return;
            }

            CPU cpu = CPU.Load(File.ReadAllBytes(tempPath));
            Instructions.Write = (string text) => EtorumConsole.WriteLine(text);

            cpu.Run();
        }

        public override string Name => "lvm";
    }
}
