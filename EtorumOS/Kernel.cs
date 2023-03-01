using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using System.IO;
using EtorumOS.Services;
using EtorumOS.ETOScript;
using static Cosmos.HAL.PIT;
using Cosmos.HAL;
using EtorumOS.ETOScript.Commands;
using Cosmos.Core;

namespace EtorumOS {
    public class Kernel : Sys.Kernel {
        CosmosVFS vfs = new();

        public static Kernel Instance { get; private set; }
        public string CurrentPath { get; set; } = @"0:\";
        public List<Command> Commands { get; private set; } = new() {
            new CommandCD(), new CommandMKDIR(), new CommandLS(), new CommandREAD(), new CommandSetKeyboardLayout(),
            new CommandDEL(), new CommandSetPassword(), new CommandCEDIT(), new CommandDebug(), new CommandACL(),
            new CommandUM()
        };

        public Dictionary<string, string> EnvironmentVars = new();

        protected override void BeforeRun() {
            Instance = this;

            try {

                Console.Clear();
                EtorumConsole.WriteLine("[init] Loading VFS...");
                VFSManager.RegisterVFS(vfs, true, true);
                EtorumConsole.WriteLine("[init] Check OS files...");

                if (!Directory.Exists(@"0:\os")) {
                    Directory.CreateDirectory(@"0:\os");
                }

                EtorumConsole.WriteLine("[init] Initiliaze services...");

                EtorumConsole.WriteLine("[init] Initiliaze UserAccountService...");
                new UserAccountService();
                EtorumConsole.WriteLine("[init] Initiliaze KeyboardLayoutService...");
                new KeyboardLayoutService();
                EtorumConsole.WriteLine("[init] Initiliaze PITService...");
                new PITService();

                EtorumConsole.WriteLine("[init] Done!");

                Console.Clear();

                EtorumConsole.WriteLine("\n" + Resources.ResourceManager.Banner);

                UserAccountService.Instance.StartAuthenticationProcess();
            }catch(Exception ex) {
                mDebugger.Send("Exception occurred during init");
                //mDebugger.Send(ex.Message);
                Panic(ex, "INITILIZATION");
                while (true) { }
            }
        }

        protected override void Run() {
            try {
                Helpers.Write(ConsoleColor.Green, $"[{UserAccountService.Instance.User.Name}] ");
                Helpers.Write(ConsoleColor.DarkGray, CurrentPath + " > ");

                string cliIn = Console.ReadLine();

                foreach (KeyValuePair<string, string> kvp in EnvironmentVars) {
                    cliIn = cliIn.Replace("%" + kvp.Key, kvp.Value);
                }
                
                RunCommands(cliIn);
            }catch(Exception ex) {
                mDebugger.Send("Exception occurred during main loop");
                //mDebugger.Send(ex.Message);
                Panic(ex, "MAIN LOOP");
                while (true) { }
            }
        }

        private void Panic(Exception ex, string task = "UNSPECIFIED") {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            
            /*for(int y = 0; y < Console.WindowHeight; y++) {
                EtorumConsole.Write(" ".Repeat(Console.WindowWidth));
            }*/

            //Console.CursorTop = 0;
            //Console.CursorLeft = 0;
            EtorumConsole.WriteLine("** ETORUM PANIC during " + task + "**");
            EtorumConsole.WriteLine(ex.ToString());
        }

        private void IntCrash(string intCrashName) {
            Panic(new Exception(intCrashName), "UNKNOWN");
            while (true) ;
        }

        public void RunCommands(string code) {
            string[] cmdList = Parser.GetCommands(code);

            foreach (string cmd in cmdList) {
                string[] args = Parser.ParseCommand(cmd);

                if (args.Length < 1) return;

                mDebugger.Send(string.Join(", ", args));

                bool foundCmd = false;
                foreach (Command icmd in Commands) {
                    if (icmd.Name == args[0]) {
                        try {
                            icmd.Execute(args);
                            mDebugger.Send("Running CMD");
                        }catch(Exception ex) {
                            EtorumConsole.WriteLine("Executing " + args[0] + " failed: " + ex.ToString() + "\n" + ex.Message);
                        }
                        foundCmd = true;
                        break;
                    }
                }

                if (foundCmd) continue;

                if (args[0].StartsWith("./") && !foundCmd) {
                    if (File.Exists(args[0])) {
                        RunCommands(File.ReadAllText(args[0].Replace("./", "")));
                        continue;
                    }else {
                        EtorumConsole.WriteLine("Can not run " + args[0] + ": File not found");
                        return;
                    }
                }

                EtorumConsole.WriteLine("Command \"" + args[0] + "\" not found.");
            }
        }
    }
}
