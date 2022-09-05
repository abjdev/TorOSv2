using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.ETOScript {
    internal class Parser {
        public static string[] GetCommands(string text) {
            List<string> seperatedCmds_ = new();

            int i = 0;
            string temp = "";
            bool isInStr = false;
            while (i < text.Length)
            {
                if (text[i] == '"') isInStr = !isInStr;
                else if(text[i] == ';' || text[i] == '\n' || text[i] == '\r')
                {
                    if (!isInStr)
                    {
                        seperatedCmds_.Add(temp);
                        temp = "";
                        isInStr = false;
                    }else
                    {
                        temp += text[i];
                    }
                }else
                {
                    temp += text[i];
                }
                i++;
            }

            if(temp != "") seperatedCmds_.Add(temp);

            string[] seperatedCmds = text.Split(new string[] { "\r\n", "\r", "\n", ";" }, StringSplitOptions.RemoveEmptyEntries);
            return seperatedCmds_.ToArray();
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
