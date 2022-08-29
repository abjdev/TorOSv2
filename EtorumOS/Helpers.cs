using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS {
    internal static class Helpers {
        public static void WriteLine(this ConsoleColor color, string text) {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void Write(this ConsoleColor color, string text) {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        public static string Join<T>(this IEnumerable<T> values, string sep) {
            string output = "";

            foreach (object val in values) {
                output += val.ToString() + sep;
            }

            return output.Substring(0, Math.Max(0, output.Length - sep.Length)); 
        }

        public static string Repeat(this string originalString, int amount) {
            string output = "";

            for (int i = 0; i < amount; i++) output += originalString;

            return output;
        }
    }
}
