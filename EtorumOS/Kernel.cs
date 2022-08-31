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

namespace EtorumOS {
    public class Kernel : Sys.Kernel {
        CosmosVFS vfs = new();

        public static Kernel Instance { get; private set; }
        public string CurrentPath { get; set; } = @"0:\";
        public List<Command> Commands { get; private set; } = new() {
            new CommandCD(), new CommandMKDIR(), new CommandLS(), new CommandREAD(), new CommandSetKeyboardLayout(),
            new CommandDEL(), new CommandSetPassword(), new CommandEDIT()
        };

        public CustomDictString EnvironmentVars = new();

        protected override void BeforeRun() {
            Instance = this;

            Console.Clear();
            Console.WriteLine("[init] Loading VFS...");
            VFSManager.RegisterVFS(vfs);
            Console.WriteLine("[init] Check OS files...");

            if(!Directory.Exists(@"0:\os")) {
                Directory.CreateDirectory(@"0:\os");
            }

            Console.WriteLine("[init] Initiliaze services...");

            new UserAccountService();
            new KeyboardLayoutService();
            new PITService();

            Console.WriteLine("[init] Done!");

            Console.Clear();

            Console.WriteLine("\n" + Resources.ResourceManager.Banner);

            UserAccountService.Instance.StartAuthenticationProcess();
        }

        protected override void Run() {
            try {
                Helpers.Write(ConsoleColor.Green, $"[{UserAccountService.Instance.User.Name}] ");
                Helpers.Write(ConsoleColor.DarkGray, CurrentPath + " > ");

                string cliIn = Console.ReadLine();
                RunCommands(cliIn);
            }catch(Exception ex) {
                Panic(ex);
                while (true) ;
            }
        }

        private void Panic(Exception ex) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            
            for(int y = 0; y < Console.WindowHeight; y++) {
                Console.Write(" ".Repeat(Console.WindowWidth));
            }

            Console.CursorTop = 0;
            Console.CursorLeft = 0;
            Console.WriteLine("** ETORUM PANIC **");
            Console.WriteLine(ex.ToString());
        }

        public void RunCommands(string code) {
            string[] cmdList = Parser.GetCommands(code);

            foreach (string cmd in cmdList) {
                string[] args = Parser.ParseCommand(cmd);

                if (args.Length < 1) return;

                bool foundCmd = false;
                foreach (Command icmd in Commands) {
                    if (icmd.GetName() == args[0]) {
                        try {
                            icmd.Execute(args);
                            mDebugger.Send("Running CMD");
                        }catch(Exception ex) {
                            Console.WriteLine("Executing " + args[0] + " failed: " + ex.ToString());
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
                        Console.WriteLine("Can not run " + args[0] + ": File not found");
                        return;
                    }
                }

                Console.WriteLine("Command \"" + args[0] + "\" not found.");
            }
        }
    }
}
