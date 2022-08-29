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
            new CommandDEL()
        };

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

            Console.WriteLine("[init] Done!");

            Console.Clear();

            Console.WriteLine("\n" + Resources.ResourceManager.Banner);

            /*PITTimer pt = new(() => {
                int origTop = Console.CursorTop;
                int origLeft = Console.CursorLeft;
                ConsoleColor origFColor = Console.ForegroundColor;
                ConsoleColor origBColor = Console.BackgroundColor;

                Console.CursorVisible = false;
                Console.CursorTop = 0;
                Console.CursorLeft = 0;
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Gray;

                string timeStr = RTC.Hour.ToString("00") + ":" + RTC.Minute.ToString("00") + ":" + RTC.Second.ToString("00") + "   " + RTC.DayOfTheMonth.ToString("00") + "/" + RTC.Month.ToString("00") + "/" + RTC.Year.ToString("00");

                Console.Write(timeStr + " ".Repeat(Console.WindowWidth - timeStr.Length));

                Console.CursorTop = Math.Min(Console.WindowHeight-1, origTop);
                Console.CursorLeft = Math.Min(Console.WindowWidth-1, origLeft);
                Console.CursorVisible = true;
                Console.ForegroundColor = origFColor;
                Console.BackgroundColor = origBColor;
            }, 1000 * 1000 * 20, true);

            Global.PIT.RegisterTimer(pt);*/

            while (true) {
                Console.Write("Name: ");
                string username = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();

                if (UserAccountService.Instance.TryAuthenticate(username, password)) break;
                else {
                    Global.PIT.WaitNS(2000000000);
                    Console.WriteLine("Invalid name or password. Try again");
                }
            }

            Console.WriteLine("Welcome, " + UserAccountService.Instance.User.Name + "!");

            if(UserAccountService.Instance.User.Password == "root") {
                Helpers.WriteLine(ConsoleColor.Yellow, "Warning! It seems like you are using the default password, 'root'. This is not recommended. Use 'setpassword root <your password>'");
            }
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
