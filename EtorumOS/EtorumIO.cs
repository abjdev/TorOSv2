using EtorumOS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS {
    internal static class EtorumIO {
        public static List<string> reservedFiles = new() { };

        public static void RegisterReserved(string path) {
            reservedFiles.Add(path);
        }

        public static bool UserCanAccess(User user, string path) {
            return !reservedFiles.Contains(path);
        }
    }
}
