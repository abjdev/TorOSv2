using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cosmos.System;
using Console = System.Console;
using EtorumOS.KeyboardLayouts;
using EtorumOS.IO;

namespace EtorumOS.Services
{
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
            if (!File.Exists(cfgLoc)) { // Config doesnt exist, save an empty one first and then allow the user to select a layout
                Kernel.Instance.mDebugger.Send("1.1");
                cfg = new();
                Kernel.Instance.mDebugger.Send("1.2");
                File.WriteAllText(cfgLoc, cfg.Save());
                Kernel.Instance.mDebugger.Send("1.3");
                EtorumConsole.WriteLine("== FIRST SETUP ==");
                Kernel.Instance.mDebugger.Send("1.4");
                ShowSelection();
            }else { // Config exists, load it
                cfg = new();
                cfg.Load(File.ReadAllText(cfgLoc));

                if (!cfg.Options.ContainsKey("layout")) { // The config is unfinished and doesn't contain a layout, default to en_US
                    if(!SetLayout("en_US")) ShowSelection(); // Show selection if en_US can not be used for some reason
                }
                else {
                    if (!SetLayout((string)cfg.Options["layout"])) ShowSelection(); // layout key was found in config, set the layout to it or if the layout is
                                                                                    // is invalid, show the selection
                }
            }

            EtorumIO.RegisterReserved(cfgLoc);
        }

        public void ShowSelection() {
            while (true) {
                EtorumConsole.WriteLine("Select a keyboard layout to use:");
                EtorumConsole.WriteLine(@"1. US/English Layout (en_US)
2. German Layout (de_DE)
3. French Layout (fr_FR)

Your Selection: ");

                string res = EtorumConsole.ReadLine();
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
                if (!success) EtorumConsole.WriteLine("Invalid selection. Try again.");
            }

            File.WriteAllText(cfgLoc, cfg.Save());
        }

        public bool SetLayout(string layoutName) {
            Kernel.Instance.mDebugger.Send("layout selected: " + layoutName);
            if (!(layoutName == "en_US" || layoutName == "de_DE" || layoutName == "fr_FR")) return false;

            Kernel.Instance.mDebugger.Send("sl1 " + (cfg == null ? "t" : "f"));
            if(cfg != null) Kernel.Instance.mDebugger.Send((cfg.Options == null ? "t" : "f"));
            cfg.Options["layout"] = layoutName;

            Kernel.Instance.mDebugger.Send("sl2");
            if (layoutName == "en_US")
            {
                KeyboardManager.SetKeyLayout(new Cosmos.System.ScanMaps.US_Standard());
            }else if(layoutName == "de_DE")
            {
                Kernel.Instance.mDebugger.Send("sl3");
                KeyboardManager.SetKeyLayout(new DE_Fixed());
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
