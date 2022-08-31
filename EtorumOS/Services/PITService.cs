using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cosmos.System;
using Console = System.Console;
using EtorumOS.KeyboardLayouts;
using Cosmos.HAL;

namespace EtorumOS.Services {
    internal class PITService : Service {
        public static PITService Instance;
        public static PIT Pit { get; private set; }

        public PITService() {
            Instance = this;
            this.Init();
        }

        public override void Init() {
            Pit = new PIT();    
        }

        public void GenerateNewPIT()
        {
            Pit = new PIT();
        }

        public override void Update() {
            
        }
    }
}
