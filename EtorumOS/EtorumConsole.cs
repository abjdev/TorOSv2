using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS {
    /// <summary>
    /// Helper class that also puts all the output in the debugger
    /// Will probably be switched to handling all console stuff if I will
    /// ever decide to make the console rendered using canvas
    /// </summary>
    internal class EtorumConsole {
        public static void WriteLine(string msg) {
            Kernel.Instance.mDebugger.Send(msg);
            Console.WriteLine(msg);
        }

        public static void Write(string msg) {
            Kernel.Instance.mDebugger.Send(msg);
            Console.Write(msg);
        }
    }
}
