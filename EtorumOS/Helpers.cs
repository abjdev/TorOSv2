using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS {
    internal static class Helpers {
        public static void WriteLine(this ConsoleColor color, string text) {
            Console.ForegroundColor = color;
            EtorumConsole.WriteLine(text);
            Console.ResetColor();
        }

        public static void Write(this ConsoleColor color, string text) {
            Console.ForegroundColor = color;
            EtorumConsole.Write(text);
            Console.ResetColor();
        }

        public static string Join<T>(this IEnumerable<T> values, string sep, bool trailing = false) {
            string output = "";

            foreach (object val in values) {
                output += val.ToString() + sep;
            }

            return trailing ? output : output.Substring(0, Math.Max(0, output.Length - sep.Length)); 
        }

        /// <summary>
        /// because for some reason List<string>.Contains does not work
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainsString(this List<string> list, string value) {
            foreach(string str in list) {
                if (str == value) return true;
            }

            return false;
        }

        public static string Repeat(this string originalString, int amount) {
            string output = "";

            for (int i = 0; i < amount; i++) output += originalString;

            return output;
        }
    }
}
