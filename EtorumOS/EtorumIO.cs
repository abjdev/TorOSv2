using EtorumOS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS {
    internal static class EtorumIO {
        public static List<string> reservedFiles = new() { };

        public static void RegisterReserved(string path) {
            reservedFiles.Add(path);
        }

        public static bool UserCanWrite(User user, string path) {
            return !reservedFiles.Contains(path);
        }

        public static bool CurrentUserCanWrite(string path)
        {
            return !reservedFiles.Contains(path);
        }
    }
}
