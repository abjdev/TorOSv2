using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cosmos.System;
using Console = System.Console;

namespace EtorumOS.Services {
    internal class KeyboardLayoutService : Service {
        public static KeyboardLayoutService Instance;

        private const string cfgLoc = @"0:\os\keyboard.cfg";
        private ConfigParser cfg;

        public KeyboardLayoutService() {
            Instance = this;
            this.Init();
        }

        public override void Init() {
            Kernel.Instance.mDebugger.Send("1");
            if (!File.Exists(cfgLoc)) {
                cfg = new();
                Kernel.Instance.mDebugger.Send(SetLayout("en_US") ? "init t" : "init f");
                File.WriteAllText(cfgLoc, cfg.Save());

                Console.WriteLine("== FIRST SETUP ==");
                ShowSelection();
            }else {
                Kernel.Instance.mDebugger.Send("a1");
                cfg = new();
                Kernel.Instance.mDebugger.Send("a2");
                cfg.Load(File.ReadAllText(cfgLoc));

                Kernel.Instance.mDebugger.Send("a3");
                Kernel.Instance.mDebugger.Send(cfg == null ? "t1" : "f1");
                Kernel.Instance.mDebugger.Send(cfg.Options == null ? "t2" : "f2");
                Kernel.Instance.mDebugger.Send(cfg.Options.IsEntriesNull() ? "t3" : "f3");

                if (!cfg.Options.Contains("layout")) {
                    Kernel.Instance.mDebugger.Send("a4");
                    SetLayout("en_US");
                }else {
                    Kernel.Instance.mDebugger.Send("a5");
                    SetLayout((string)cfg.Options["layout"]);
                }

                Kernel.Instance.mDebugger.Send("a6");
            }

            EtorumIO.RegisterReserved(cfgLoc);
        }

        public void ShowSelection() {
            while (true) {
                Console.WriteLine("Select a keyboard layout to use:");
                Console.WriteLine(@"1. US/English Layout (en_US)
2. German Layout (de_DE)
3. French Layout (fr_FR)

Your Selection: ");

                string res = Console.ReadLine();
                bool success = true;

                switch (res) {
                    case "1":
                        success = SetLayout("en_US");
                        break;
                    case "2":
                        success = SetLayout("de_DE");
                        Kernel.Instance.mDebugger.Send(success ? "de_DE t" : "de_DE f");
                        break;
                    case "3":
                        success = SetLayout("fr_FR");
                        break;
                    default:
                        success = false;
                        break;
                }

                if (success) break;
                if (!success) Console.WriteLine("Invalid selection. Try again.");
            }

            File.WriteAllText(cfgLoc, cfg.Save());
        }

        public bool SetLayout(string layoutName) {
            if (layoutName == "en_US" || layoutName == "de_DE" || layoutName == "fr_FR") return false;

            cfg.Options["layout"] = layoutName;

            switch(layoutName) {
                case "en_US":
                    KeyboardManager.SetKeyLayout(new Cosmos.System.ScanMaps.US_Standard());
                    break;
                case "de_DE":
                    KeyboardManager.SetKeyLayout(new Cosmos.System.ScanMaps.DE_Standard());
                    break;
                case "fr_FR":
                    KeyboardManager.SetKeyLayout(new Cosmos.System.ScanMaps.FR_Standard());
                    break;

            }
            return true;
        }

        public override void Update() {
            
        }
    }
}
