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
                    if(!SetLayout("en_US")) ShowSelection();
                }
                else {
                    Kernel.Instance.mDebugger.Send("a5");
                    if (!SetLayout((string)cfg.Options["layout"])) ShowSelection();
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
                byte[] bytes = Encoding.Unicode.GetBytes(res);
                Kernel.Instance.mDebugger.Send(bytes.Join(", "));
                bool success = true;

                if (res == "1") {
                    success = SetLayout("en_US");
                } else if (res == "2") {
                    success = SetLayout("de_DE");
                    Kernel.Instance.mDebugger.Send(success ? "de_DE t" : "de_DE f");
                } else if (res == "3") {
                    success = SetLayout("fr_FR");
                } else {
                    success = false;
                }

                if (success) break;
                if (!success) Console.WriteLine("Invalid selection. Try again.");
            }

            File.WriteAllText(cfgLoc, cfg.Save());
        }

        public bool SetLayout(string layoutName) {
            Kernel.Instance.mDebugger.Send("layout selected: " + layoutName);
            if (!(layoutName == "en_US" || layoutName == "de_DE" || layoutName == "fr_FR")) return false;

            Kernel.Instance.mDebugger.Send("sl1 " + (cfg == null ? "t" : "f"));
            if(cfg != null) Kernel.Instance.mDebugger.Send((cfg.Options == null ? "t" : "f"));
            cfg.Options.Set("layout", layoutName);

            Kernel.Instance.mDebugger.Send("sl2");
            if (layoutName == "en_US")
            {
                KeyboardManager.SetKeyLayout(new Cosmos.System.ScanMaps.US_Standard());
            }else if(layoutName == "de_DE")
            {
                Kernel.Instance.mDebugger.Send("sl3");
                KeyboardManager.SetKeyLayout(new Cosmos.System.ScanMaps.DE_Standard());
            }else if(layoutName == "fr_FR")
            {
                KeyboardManager.SetKeyLayout(new Cosmos.System.ScanMaps.FR_Standard());
            }

            Kernel.Instance.mDebugger.Send("sl4");

            return true;
        }

        public override void Update() {
            
        }
    }
}
