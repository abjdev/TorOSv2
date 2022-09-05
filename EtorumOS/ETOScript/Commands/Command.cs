using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.ETOScript.Commands {
    public abstract class Command {
        public abstract string Name { get; }

        public abstract void Execute(string[] args);
    }
}
