using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EtorumOS.IO;

namespace EtorumOS.ETOScript.Commands
{
    internal class CommandDebug : Command {
        public override void Execute(string[] args) {
            if (args.Length < 2) {
                Helpers.WriteLine(ConsoleColor.Red, "Syntax: debug [... args]");
                return;
            }

            if (args[1] == "dumpsysfiles") {
                foreach(string file in EtorumIO.reservedFiles) {
                    Helpers.Write(ConsoleColor.Blue, "  <SYS> ");
                    Helpers.Write(ConsoleColor.White, file + "\n");
                }
            }else if (args[1] == "sanitize") {
                Helpers.WriteLine(ConsoleColor.White, EtorumIO.SanitizePermissionPath(args[2]));
            }else if (args[1] == "permcheck") {
                bool result = false;

                switch(args[2]) {
                    case "read":
                        result = EtorumIO.CurrentUserCan(PermissionType.READ_FILE, args[3]);
                        break;
                    case "write":
                        result = EtorumIO.CurrentUserCan(PermissionType.WRITE_FILE, args[3]);
                        break;
                    default:
                        Helpers.WriteLine(ConsoleColor.Red, "Invalid permcheck (read/write)");
                        return;
                }

                Helpers.Write(ConsoleColor.Yellow, "Result: ");

                if(result) {
                    Helpers.WriteLine(ConsoleColor.Green, "Yes");
                }else {
                    Helpers.WriteLine(ConsoleColor.Red, "No");
                }
                
            }else if (args[1] == "tryparse") {
                Helpers.WriteLine(ConsoleColor.Yellow, "Enter input to parse: ");
                var input = EtorumConsole.ReadLine();

                string[] parsed = Parser.ParseCommand(input);
                Helpers.WriteLine(ConsoleColor.Gray, string.Join(", ", parsed));
                
            }
        }

        public override string Name => "debug";
    }
}
