using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtorumOS {
    internal class ConfigParser {
        public CustomDictString Options { get; set; } = new();

        public void Load(string data) {
            string[] lines = data.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

            foreach(string line in lines) {
                string[] parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2) continue;

                Options.Add(parts[0], parts[1]);
            }
        }
        
        public string Save() {
            string res = "";

            foreach(CustomDictEntry<string, string> kvp in Options) {
                res += kvp.key + "=" + kvp.value + "\n";
            }

            return res.Substring(0, res.Length-1);
        }
    }

    internal class ConfigParserEntry {
        public ConfigParserEntryType t;
        public object v;

        public ConfigParserEntry(ConfigParserEntryType t, object v) {
            this.t = t;
            this.v = v;
        }
    }

    internal enum ConfigParserEntryType {
        I16,
        I32,
        I64,
        String,
        List,
        Boolean
    }
}
