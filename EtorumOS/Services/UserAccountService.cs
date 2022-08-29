using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EtorumOS.Services {
    internal class UserAccountService : Service {
        public static UserAccountService Instance;
        public User User { get; private set; }

        private List<User> users = new();
        private const string userDBLocation = @"0:\os\users.db";

        public UserAccountService() {
            Instance = this;
            this.Init();
        }

        public override void Init() {
            if(!File.Exists(userDBLocation)) {
                File.WriteAllText(userDBLocation, "root;root;1");
            }

            EtorumIO.RegisterReserved(userDBLocation);
            LoadUsers();
        }

        public void LoadUsers() {
            string raw = File.ReadAllText(userDBLocation);
            string[] rows = raw.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach(string row in rows) {
                string[] columns = row.Split(';', StringSplitOptions.RemoveEmptyEntries);

                if (columns.Length != 3) continue;

                users.Add(new() { Name = columns[0], Password = columns[1], IsSuperUser = columns[2] == "1" ? true : false });
            }
        }

        public void SaveUsers() {
            string final = "";

            foreach(User user in users) {
                final += $"{user.Name};{user.Password};{(user.IsSuperUser ? "1" : "0")}";
            }

            File.WriteAllText(userDBLocation, final);
        }

        public User GetUser(string name) {
            foreach(User user in users) {
                if(user.Name == name) {
                    return user;
                }
            }

            return null;
        }

        public bool TryAuthenticate(string name, string password) {
            User user = GetUser(name);

            if (user == null) return false;
            if (user.Password != password) return false;

            User = user;
            return true;
        }

        public override void Update() {
            
        }
    }

    internal class User {
        public string Name;
        public string Password;
        public bool IsSuperUser;
    }
}
