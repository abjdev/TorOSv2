using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EtorumOS.IO;
using libLowSpagAssembler;
using libLowSpagVM;

namespace EtorumOS.ETOScript.Commands
{
    internal class CommandLASM : Command {
        public override void Execute(string[] args) {
            if (args.Length < 3) {
                Helpers.WriteLine(ConsoleColor.Red, "Syntax: lasm <input path> <output path>");
                return;
            }

            string inputPath = EtorumIO.CreatePath(args[1]);
            string outputPath = EtorumIO.CreatePath(args[2]);

            if (!File.Exists(inputPath)) {
                Helpers.WriteLine(ConsoleColor.Red, "File was not found. (was looking for " + inputPath + ")");
                return;
            }

            if(!EtorumIO.CurrentUserCan(PermissionType.READ_FILE, inputPath)) {
                Helpers.WriteLine(ConsoleColor.Red, "Access to file is denied.");
                return;
            }

            if (!EtorumIO.CurrentUserCan(PermissionType.WRITE_FILE, outputPath)) {
                Helpers.WriteLine(ConsoleColor.Red, "Access to file is denied.");
                return;
            }

            Assembler asm = new Assembler(File.ReadAllText(inputPath));
            File.WriteAllBytes(outputPath, asm.Assemble());
        }

        public override string Name => "lasm";
    }
}
