using Cosmos.System;
using Cosmos.System.FileSystem;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = System.Console;

namespace EtorumOS {
    /// <summary>
    /// Helper class that also puts all the output in the debugger
    /// Will probably be switched to handling all console stuff if I will
    /// ever decide to make the console rendered using canvas
    /// </summary>
    internal class EtorumConsole {
        public static Canvas Canvas { get; set; }

        private static List<string> lines = new() { "" };
        private static List<Color> charColorBuffer = new();

        public static (int x, int y) Position { get; set; } = (0, 0);
        
        public static bool SwitchToGraphicsMode()
        {
            try {
                Canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(1280, 720, ColorDepth.ColorDepth32));

                return true;
            }catch(Exception ex) {
                EtorumConsole.WriteLine("Enabled Graphics Mode Failed: " + ex.Message);
                return false;
            }
        }
        
        public static void WriteLine(string msg) {
            Kernel.Instance.mDebugger.Send(msg);

            if (Canvas == null) {
                Console.WriteLine(msg);
            }else {
                msg += "\n";

                Write(msg);
            }
        }

        public static Exception? RedrawWhole()
        {
            if(Canvas == null) {
                return new InvalidOperationException("Can not redraw non-canvas console!");
            }

            Canvas.Clear(Color.Black);

            int bufferOffset = 0;
            int x = 0, y = 0;

            foreach(string line in lines) {
                foreach(char c in line) {
                    Canvas.DrawString(c.ToString(), PCScreenFont.Default, charColorBuffer[bufferOffset], x*8, y*16);
                    x++;
                    bufferOffset++;
                }

                y++;
            }

            Canvas.Display();
            return null;
        }

        private static int AllLinesSum(int lineOffset = 0)
        {
            int sum = 0;
            for(int i = 0; i < lines.Count - 1 - lineOffset; i++) {
                sum += lines[i].Length;
            }
            return sum;
        }

        public static void Write(string msg) {
            Kernel.Instance.mDebugger.Send(msg);

            int offset = AllLinesSum(1);
            if (Canvas == null) {
                Console.Write(msg);
            }else {
                foreach (char c in msg) {
                    if (c == '\n') {
                        // TODO: If position.x != end, break up the lines accordingly
                        lines.Add("");
                        Position = (0, Position.y + 1);
                        offset = AllLinesSum(1);
                    } else if (c == '\t') {
                        lines[Position.y] = lines[Position.y] + "    ";

                        // not using for loops here as it might reduce performance without any good benefit
                        charColorBuffer.Add(Color.White);
                        charColorBuffer.Add(Color.White);
                        charColorBuffer.Add(Color.White);
                        charColorBuffer.Add(Color.White);
                    } else if (!char.IsControl(c)) {
                        Canvas.DrawString(c.ToString(), PCScreenFont.Default, Console.ForegroundColor.ToColor(), Position.x * 8, Position.y * 16);

                        if (Position.x == lines[Position.y].Length) {
                            lines[Position.y] = lines[Position.y] + c;
                            charColorBuffer.Add(Console.ForegroundColor.ToColor());
                        } else {
                            var currentLine = lines[lines.Count - 1].ToCharArray();

                            Helpers.SafeSet(currentLine, Position.x, c);
                            charColorBuffer[offset + Position.x] = Console.ForegroundColor.ToColor();

                            lines[Position.y] = new string(currentLine);
                        }

                        Position = (Position.x + 1, Position.y);
                    }
                }

                Canvas.Display();
            }
        }

        public static string ReadLine()
        {
            if (Canvas != null) {
                string buffer = "";
                int inputPosition = 0;

                while (true) {
                    var key = Console.ReadKey();

                    if (key.Key == ConsoleKey.LeftArrow) {
                        inputPosition--;
                        if (inputPosition < 0) inputPosition = 0;
                    } else if (key.Key == ConsoleKey.RightArrow) {
                        inputPosition++;
                        if (inputPosition >= buffer.Length) inputPosition = buffer.Length - 1;
                    } else if (key.Key == ConsoleKey.Enter) {
                        WriteLine("");
                        return buffer;
                    }

                    if (!char.IsControl(key.KeyChar)) {
                        if (inputPosition == buffer.Length) {
                            buffer += key.KeyChar;
                        }else {
                            buffer = buffer.Insert(inputPosition, key.KeyChar.ToString());
                        }

                        inputPosition++;

                        var oldPos = Position;
                        Write(buffer);
                        Position = oldPos;
                    }
                }
            }else {
                return Console.ReadLine();
            }
        }

        public static ConsoleKeyInfo ReadKey()
        {
            var key = Console.ReadKey();
            Write(key.KeyChar.ToString());
            return key;
        }
    }
}
