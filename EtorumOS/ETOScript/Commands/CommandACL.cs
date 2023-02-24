using EtorumOS.IO;
using EtorumOS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.ETOScript.Commands {
    internal class CommandACL : Command {
        public override void Execute(string[] args) {
            if (args.Length < 2) {
                Helpers.WriteLine(ConsoleColor.Red, "Syntax: acl <path> [<user> <-+r|-+w>]");
                return;
            }

            string tempPath = EtorumIO.CreatePath(args[1]);

            if (!File.Exists(tempPath)) {
                Helpers.WriteLine(ConsoleColor.Red, "File was not found. (was looking for " + tempPath + ")");
                return;
            }

            // only superusers can change ACLs
            if (!UserAccountService.Instance.User.IsSuperUser) {
                Helpers.WriteLine(ConsoleColor.Red, "You are not permitted to use this command!");
                return;
            }

            ACL acl = new ACL(tempPath);

            if (args.Length < 3) {
                Helpers.WriteLine(ConsoleColor.Green, "ACL:");
                Helpers.WriteLine(ConsoleColor.Green, acl.ToString());
                return;
            }
            
            string user = args[2];
            string permission = args[3];

            if (permission == "-r") {
                acl.SetPermission(user, false, acl.Can(PermissionType.WRITE_FILE, user));
            } else if (permission == "+r") {
                acl.SetPermission(user, true, acl.Can(PermissionType.WRITE_FILE, user));
            } else if (permission == "-w") {
                acl.SetPermission(user, acl.Can(PermissionType.READ_FILE, user), false);
            } else if (permission == "+w") {
                acl.SetPermission(user, acl.Can(PermissionType.READ_FILE, user), true);
            } else {
                Helpers.WriteLine(ConsoleColor.Red, "Syntax: acl <path> <user> [-+r|-+w]");
                return;
            }

            Helpers.WriteLine(ConsoleColor.Green, "ACL updated successfully!");
            Helpers.WriteLine(ConsoleColor.Green, "New ACL:");
            Helpers.WriteLine(ConsoleColor.Green, acl.ToString());
        }

        public override string Name => "acl";
    }
}
