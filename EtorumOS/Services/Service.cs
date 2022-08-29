using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.Services {
    abstract internal class Service {
        public abstract void Init();
        public abstract void Update();
    }
}
