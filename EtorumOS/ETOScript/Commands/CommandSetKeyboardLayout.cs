using EtorumOS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS.ETOScript.Commands {
    internal class CommandSetKeyboardLayout : Command {
        public override void Execute(string[] args) {
            if (args.Length < 1) {
                Helpers.WriteLine(ConsoleColor.Red, "Syntax: setkblayout");
                return;
            }

            KeyboardLayoutService.Instance.ShowSelection();
        }

        public override string GetName() {
            return "setkblayout";
        }
    }
}
