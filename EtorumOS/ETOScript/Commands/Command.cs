using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.ETOScript.Commands {
    public abstract class Command {
        public abstract string GetName();
        public abstract void Execute(string[] args);
    }
}
