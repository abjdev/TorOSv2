using EtorumOS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.ETOScript.Commands {
    internal class CommandUM : Command {
        public override void Execute(string[] args) {
            if (args.Length < 2) {
                Helpers.WriteLine(ConsoleColor.Red, "Syntax: um <new/delete/logout> [user]");
                return;
            }

            if (args[1] == "new" && args.Length == 3) {
                Helpers.WriteLine(ConsoleColor.White, "Enter password: ");
                string password = null;
                while (true) {
                    var key = System.Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                        break;
                    password += key.KeyChar;
                }
                
                UserAccountService.Instance.AddUser(args[2], password, false);
                Helpers.WriteLine(ConsoleColor.Green, "Added user.");
            }else if (args[1] == "delete" && args.Length == 3) {
                if (UserAccountService.Instance.RemoveUser(args[2])) {
                    Helpers.WriteLine(ConsoleColor.Green, "User deleted.");
                }else {
                    Helpers.WriteLine(ConsoleColor.Red, "User not found or currently in a session.");
                }
            }else if (args[1] == "logout") {
                Console.Clear();
                UserAccountService.Instance.StartAuthenticationProcess();
            }
            else {
                Helpers.WriteLine(ConsoleColor.Red, "Syntax: um <new/delete/logout> [user]");
            }
        }

        public override string Name => "um";
    }
}
