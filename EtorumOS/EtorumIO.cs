using EtorumOS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EtorumOS {
    internal static class EtorumIO {
        public static List<string> reservedFiles = new() { };

        public static void RegisterReserved(string path) {
            reservedFiles.Add(Sanitize(path));
        }

        public static bool UserCan(User user, PermissionType perm, string path) {
            return !reservedFiles.Contains(Sanitize(path));
        }

        public static bool CurrentUserCan(PermissionType perm, string path)
        {
            return UserCan(UserAccountService.Instance.User, perm, path);
        }

        internal static string Sanitize(string path) {
            string output = "";

            foreach(char chr in path) {
                if (char.IsLetterOrDigit(chr)) output += chr;
            }

            return output;
        }
    }

    public enum PermissionType {
        READ_FILE,
        WRITE_FILE
    }
}
