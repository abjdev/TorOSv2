using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EtorumOS.IO {
    public class ACL {
        public string BaseFilePath { get; set; }
        // (bool, bool) = (CanRead, CanWrite)
        public Dictionary<string, (bool CanRead, bool CanWrite)> Permissions { get; set; }

        public ACL(string path) {
            BaseFilePath = path;
            Permissions = new Dictionary<string, (bool, bool)>();

            Parse();
        }

        public void Parse() {
            if (File.Exists(BaseFilePath + ".acl")) {
                string[] lines = File.ReadAllLines(BaseFilePath + ".acl");
                foreach (string line in lines) {
                    string[] parts = line.Split(' ');
                    if (parts.Length == 3) {
                        Permissions.Add(parts[0], (parts[1] == "1", parts[2] == "1"));
                    }
                }
            }
        }

        public void SetPermission(string user, bool canRead, bool canWrite) {
            if(Permissions.ContainsKey(user)) {
                Permissions[user] = (canRead, canWrite);
            }else {
                Permissions.Add(user, (canRead, canWrite));
            }
            
            Save();
        }

        public void RemovePermission(string user) {
            Permissions.Remove(user);
            Save();
        }

        public bool CanRead(string user) {
            if (Permissions.ContainsKey(user)) {
                return Permissions[user].Item1;
            }
            
            return false;
        }

        public bool CanWrite(string user) {
            if (Permissions.ContainsKey(user)) {
                return Permissions[user].Item2;
            }
            return false;
        }

        public bool Can(PermissionType type, string user) {
            switch(type) {
                case PermissionType.READ_FILE: return CanRead(user);
                case PermissionType.WRITE_FILE: return CanWrite(user);
            }

            return false;
        }

        public void Save() {
            string o = "";

            foreach(var kvp in Permissions) {
                o += (kvp.Key + " " + (kvp.Value.CanRead ? '1' : '0') + ' ' + (kvp.Value.CanWrite ? '1' : '0')) + "\n";
            }

            File.WriteAllText(BaseFilePath + ".acl", o);
        }

        public override string ToString() {
            string o = "";

            foreach (var kvp in Permissions) {
                o += $"{kvp.Key}: R({kvp.Value.CanRead}) W({kvp.Value.CanWrite})\r\n";
            }

            return o;
        }
    }
}
