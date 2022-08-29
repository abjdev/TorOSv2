using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.ETOScript {
    internal class Parser {
        public static string[] GetCommands(string text) {
            string[] seperatedCmds = text.Split(new string[] { "\r\n", "\r", "\n", ";" }, StringSplitOptions.RemoveEmptyEntries);
            return seperatedCmds;
        }

        public static string[] ParseCommand(string cmd) {
            List<string> parts = new();
            int idx = 0;
            string temp = "";

            while(idx < cmd.Length) {
                char c = cmd[idx];

                if(c == ' ') {
                    if (temp != "") parts.Add(temp);
                    temp = "";
                }else if(c == '"') {
                    if(temp != "") parts.Add(temp);
                    temp = "";

                    idx++;

                    while (idx < cmd.Length && cmd[idx] != '"') {
                        temp += cmd[idx];
                        idx++;
                    }

                    parts.Add(temp);
                    temp = "";
                }else {
                    temp += c;
                }

                idx++;
            }

            if (temp != "") parts.Add(temp);

            return parts.ToArray();
        }
    }
}
