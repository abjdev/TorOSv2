using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.ETOScript.Commands {
    internal class CommandCEDIT : Command {
        private List<CEdit_Command> Commands = new()
        {
            new CEdit_Show(), new CEdit_ShowPast(), new CEdit_SetLine(), new CEdit_Append(), new CEdit_Prepend(),
            new CEdit_Quit(), new CEdit_Save(), new CEdit_AddAfter(), new CEdit_AddLine(), new CEdit_Help()
        };

        public List<string> lines = new();
        public string filePath = "";
        public bool exit = false;
        public bool changed = false;

        public override void Execute(string[] args) {
            if (args.Length < 2) {
                Helpers.WriteLine(ConsoleColor.Red, "Syntax: cedit <path>");
                return;
            }

            string tempPath = Path.GetFullPath(Path.Combine(Kernel.Instance.CurrentPath, args[1]));

            if (!File.Exists(tempPath)) {
                Helpers.WriteLine(ConsoleColor.Red, "File was not found. (was looking for " + tempPath + ")");
                return;
            }

            Console.WriteLine("Welcome to CEdit, your command based editor! \nCEdit uses ETOScript syntax.");
            Console.WriteLine("To get started, do 'show' to show all lines.");
            filePath = tempPath;

            lines = File.ReadAllLines(filePath).ToList();

            while (true)
            {
                Helpers.Write(ConsoleColor.Green, $"[CEdit] ");
                Helpers.Write(ConsoleColor.DarkGray, filePath + " > ");

                string cliIn = Console.ReadLine();
                RunCommands(cliIn);

                if (exit) break;
            }
        }

        public void RunCommands(string code)
        {
            string[] cmdList = Parser.GetCommands(code);

            foreach (string cmd in cmdList)
            {
                string[] args = Parser.ParseCommand(cmd);

                if (args.Length < 1) return;

                bool foundCmd = false;
                foreach (CEdit_Command icmd in Commands)
                {
                    if (icmd.Name == args[0])
                    {
                        try
                        {
                            icmd.Execute(args, this);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Executing " + args[0] + " failed: " + ex.ToString() + "\n" + ex.Message);
                        }
                        foundCmd = true;
                        break;
                    }
                }

                if (foundCmd) continue;

                if (args[0].StartsWith("./") && !foundCmd)
                {
                    if (File.Exists(args[0]))
                    {
                        RunCommands(File.ReadAllText(args[0].Replace("./", "")));
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Can not run " + args[0] + ": File not found");
                        return;
                    }
                }

                Console.WriteLine("Command \"" + args[0] + "\" not found.");
            }
        }

        public override string Name => "cedit";

        private abstract class CEdit_Command : Command
        {
            public CommandCEDIT cedit;

            public void Execute(string[] args, CommandCEDIT cedit)
            {
                this.cedit = cedit;
                this.Execute(args);
            }

            public abstract string Desc { get; }
            public abstract string Syntax { get; }
        }

        private class CEdit_ShowPast : CEdit_Command
        {
            
            public override void Execute(string[] args)
            {
                int lineIdx, lineCount;

                if(!Int32.TryParse(args[1], out lineIdx) || lineIdx < 0 || lineIdx > cedit.lines.Count)
                {
                    Helpers.WriteLine(ConsoleColor.Red, "Argument line is invalid."); return;
                }

                if(!Int32.TryParse(args[2], out lineCount))
                {
                    Helpers.WriteLine(ConsoleColor.Red, "Argument count is invalid."); return;
                }

                for(int i = lineIdx; i < lineIdx + lineCount; i++)
                {
                    if (i >= cedit.lines.Count) break;
                    Console.WriteLine(i + " > " + cedit.lines[i]);
                }
            }

            public override string Name => "showpast";
            public override string Desc => "Shows <count> lines past line <line>";
            public override string Syntax => "showpast <line> <count>";
        }

        private class CEdit_Show : CEdit_Command
        {

            public override void Execute(string[] args)
            {
                for (int i = 0; i < cedit.lines.Count; i++)
                {
                    Console.WriteLine(i + " > " + cedit.lines[i]);
                }
            }

            public override string Name => "show";
            public override string Desc => "Shows all lines";
            public override string Syntax => "show";
        }

        private class CEdit_SetLine : CEdit_Command
        {

            public override void Execute(string[] args)
            {
                int lineIdx;

                if (!Int32.TryParse(args[1], out lineIdx) || lineIdx < 0 || lineIdx >= cedit.lines.Count)
                {
                    Helpers.WriteLine(ConsoleColor.Red, "Argument line is invalid."); return;
                }

                cedit.lines[lineIdx] = args[2];
                Console.WriteLine(lineIdx + " > " + cedit.lines[lineIdx]);
                cedit.changed = true;
            }

            public override string Name => "setline";
            public override string Desc => "Sets the content in a line";
            public override string Syntax => "setline <line> <content>";
        }

        private class CEdit_Append : CEdit_Command
        {
            public override void Execute(string[] args)
            {
                int lineIdx;

                if (!Int32.TryParse(args[1], out lineIdx) || lineIdx < 0 || lineIdx >= cedit.lines.Count)
                {
                    Helpers.WriteLine(ConsoleColor.Red, "Argument line is invalid."); return;
                }

                cedit.lines[lineIdx] += args[2];
                Console.WriteLine(lineIdx + " > " + cedit.lines[lineIdx]);
                cedit.changed = true;
            }

            public override string Name => "append";
            public override string Desc => "Appends content to a line";
            public override string Syntax => "append <line> <content>";
        }

        private class CEdit_AddAfter : CEdit_Command
        {

            public override void Execute(string[] args)
            {
                int lineIdx;

                if (!Int32.TryParse(args[1], out lineIdx) || lineIdx < 0 || lineIdx >= cedit.lines.Count)
                {
                    Helpers.WriteLine(ConsoleColor.Red, "Argument line is invalid."); return;
                }

                cedit.lines.Insert(lineIdx+1, args[2]);
                Console.WriteLine(lineIdx+1 + " > " + cedit.lines[lineIdx+1]);
                cedit.changed = true;
            }

            public override string Name => "addafter";
            public override string Desc => "Adds a new line after <line>";
            public override string Syntax => "addafter <line>";
        }

        private class CEdit_AddLine : CEdit_Command
        {

            public override void Execute(string[] args)
            {
                cedit.lines.Add(args[1]);
                Console.WriteLine(cedit.lines.Count-1 + " > " + cedit.lines.Last());
                cedit.changed = true;
            }

            public override string Name => "addline";
            public override string Desc => "Adds a new line at the end";
            public override string Syntax => "addline <content>";
        }

        private class CEdit_Prepend : CEdit_Command
        {

            public override void Execute(string[] args)
            {
                int lineIdx;

                if (!Int32.TryParse(args[1], out lineIdx) || lineIdx < 0 || lineIdx >= cedit.lines.Count)
                {
                    Helpers.WriteLine(ConsoleColor.Red, "Argument line is invalid."); return;
                }

                cedit.lines[lineIdx] = args[2] + cedit.lines[lineIdx];
                Console.WriteLine(lineIdx + " > " + cedit.lines[lineIdx]);
                cedit.changed = true;
            }

            public override string Name => "prepend";
            public override string Desc => "Prepends content to a line";
            public override string Syntax => "prepend <line> <content>";
        }

        private class CEdit_Quit : CEdit_Command
        {
            public override void Execute(string[] args)
            {
                if(!cedit.changed)
                {
                    cedit.exit = true;
                    return;
                }

                Helpers.WriteLine(ConsoleColor.DarkGreen, "You have unsaved changes. Are you sure that you want to exit?\n  (Y) Yes, exit\n  (N) No, don't exit");

                while (true)
                {
                    string c = Console.ReadKey().KeyChar.ToString().ToLower();

                    if (c == "y")
                    {
                        cedit.exit = true;
                        return;
                    }else
                    {
                        return;
                    }
                }
            }

            public override string Name => "quit";
            public override string Desc => "Quits CEdit";
            public override string Syntax => "quit";
        }

        private class CEdit_Save : CEdit_Command
        {

            public override void Execute(string[] args)
            {
                File.WriteAllText(cedit.filePath, cedit.lines.Join("\n"));
                Console.WriteLine("Saved " + cedit.lines.Count + " lines to " + cedit.filePath);
            }

            public override string Name => "save";
            public override string Desc => "Save all your changes to the file";
            public override string Syntax => "save";
        }

        private class CEdit_Help : CEdit_Command
        {
            public override void Execute(string[] args)
            {
                foreach(CEdit_Command cmd in cedit.Commands)
                {
                    Console.WriteLine(cmd.Syntax + " | " + cmd.Desc);
                }
            }

            public override string Name => "help";
            public override string Desc => "Shows this message";
            public override string Syntax => "help";
        }
    }
}
