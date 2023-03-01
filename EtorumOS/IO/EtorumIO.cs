using EtorumOS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EtorumOS.IO
{
    internal static class EtorumIO
    {
        public static List<string> reservedFiles = new() { };

        public static void RegisterReserved(string path)
        {
            reservedFiles.Add(SanitizePermissionPath(path));
        }

        public static bool UserCan(User user, PermissionType perm, string path)
        {
            if (path.EndsWith(".acl")) return false; // user should never be able to directly access an acl file - no matter what

            bool isReserved = reservedFiles.ContainsString(SanitizePermissionPath(path));
            if (isReserved) return false;

            if (user.IsSuperUser) return true;

            ACL acl = new ACL(path);

            return acl.Can(perm, user.Name);
        }

        public static string CreatePath(string pathPart) {
            string tempPath = Path.GetFullPath(Path.Combine(Kernel.Instance.CurrentPath, pathPart));

            if (tempPath.EndsWith("..")) {
                string[] parts = tempPath.Split('\\');
                tempPath = parts.Take(parts.Length - 2).Join("\\", true);
            }

            return tempPath;
        }

        public static bool CurrentUserCan(PermissionType perm, string path)
        {
            return UserCan(UserAccountService.Instance.User, perm, path);
        }

        internal static string SanitizePermissionPath(string path)
        {
            string output = "";

            foreach (char chr in path)
            {
                if (char.IsLetterOrDigit(chr)) output += chr;
            }

            Kernel.Instance.mDebugger.Send("Sanitized (FOR PERM) Path: " + output);
            return output.Trim();
        }
    }

    public enum PermissionType
    {
        READ_FILE,
        WRITE_FILE
    }
}
