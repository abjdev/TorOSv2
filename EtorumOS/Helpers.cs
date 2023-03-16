using System;
using System.Collections.Generic;
using System.Drawing;
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

        /// <summary>
        /// required to prevent stackcorruption
        /// </summary>
        public static void SafeSet<T>(T[] arr, int index, T value)
        {
            if (index >= arr.Length || index < 0) throw new ArgumentOutOfRangeException("SafeSet failed: " + index + " >= " + arr.Length + " or < 0");

            arr[index] = value;
        }

        /// <summary>
        /// required to prevent stackcorruption
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static T SafeGet<T>(T[] arr, int index)
        {
            if (index >= arr.Length || index < 0) throw new ArgumentOutOfRangeException("SafeGet failed: " + index + " >= " + arr.Length + " or < 0");

            return arr[index];
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

        public static Color ToColor(this ConsoleColor color) => color switch
        {
            ConsoleColor.Black => Color.Black,
            ConsoleColor.Blue => Color.Blue,
            ConsoleColor.Cyan => Color.Cyan,
            ConsoleColor.DarkBlue => Color.DarkBlue,
            ConsoleColor.DarkCyan => Color.DarkCyan,
            ConsoleColor.DarkGray => Color.DarkGray,
            ConsoleColor.DarkGreen => Color.DarkGreen,
            ConsoleColor.DarkMagenta => Color.DarkMagenta,
            ConsoleColor.DarkRed => Color.DarkRed,
            ConsoleColor.DarkYellow => Color.GreenYellow,
            ConsoleColor.Gray => Color.Gray,
            ConsoleColor.Green => Color.Green,
            ConsoleColor.Magenta => Color.Magenta,
            ConsoleColor.Red => Color.Red,
            ConsoleColor.White => Color.White,
            ConsoleColor.Yellow => Color.Yellow,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
