using EtorumOS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.ETOScript.Commands {
    internal class CommandSetPassword : Command {
        public override void Execute(string[] args) {
            if (!UserAccountService.Instance.User.IsSuperUser)
            {
                Helpers.WriteLine(ConsoleColor.Red, "Permission denied.");
                return;
            }

            if (args.Length < 3) {
                Helpers.WriteLine(ConsoleColor.Red, "Syntax: setpassword <user> <pass>");
                return;
            }

            User target = UserAccountService.Instance.GetUser(args[1]);

            if (target == null)
            {
                Helpers.WriteLine(ConsoleColor.Red, "User not found!");
                return;
            }

            target.Password = args[2];
            UserAccountService.Instance.SaveUsers();
            Console.WriteLine("Password was changed.");
        
            if(target.Name == UserAccountService.Instance.User.Name)
            {
                Helpers.WriteLine(ConsoleColor.Yellow, "You were logged out because your password was changed. Please enter your credentials again.");
                UserAccountService.Instance.StartAuthenticationProcess();
            }
        }

        public override string Name => "setpassword";
    }
}
