using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cosmos.HAL;
using EtorumOS.Cryptography;

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
                File.WriteAllText(userDBLocation, "");
                LoadUsers();
                AddUser("root", "root", true);
                return;
            }

            //EtorumIO.RegisterReserved(userDBLocation); 
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
                final += $"{user.Name};{user.Password};{(user.IsSuperUser ? "1" : "0")}\r\n";
            }

            File.WriteAllText(userDBLocation, final);
        }

        public void StartAuthenticationProcess()
        {
            User = null;

            while (true)
            {
                EtorumConsole.Write("Name: ");
                string username = Console.ReadLine();
                EtorumConsole.Write("Password: ");
                string password = Console.ReadLine();

                if (TryAuthenticate(username, password)) break;
                else
                {
                    EtorumConsole.WriteLine("Invalid name or password. Try again");
                }
            }

            if(!Directory.Exists($@"0:\users\{User.Name}\"))
            {
                Directory.CreateDirectory($@"0:\users\{User.Name}\");
            }

            if(!File.Exists($@"0:\users\{User.Name}\autorun.etos"))
            {
                File.Create($@"0:\users\{User.Name}\autorun.etos").Close();
            }

            string c = File.ReadAllText($@"0:\users\{User.Name}\autorun.etos");
            Kernel.Instance.RunCommands(c);

            Kernel.Instance.CurrentPath = $@"0:\users\{User.Name}\";
            Kernel.Instance.EnvironmentVars["USER"] = User.Name;

            EtorumConsole.WriteLine("Welcome, " + User.Name + "!");

            if (User.Password == "root")
            {
                Helpers.WriteLine(ConsoleColor.Yellow, "Warning! It seems like you are using the default password, 'root'. This is not recommended. Use 'setpassword root <your password>'");
            }
        }

        public User GetUser(string name) {
            foreach(User user in users) {
                if(user.Name == name) {
                    return user;
                }
            }

            return null;
        }

        public void AddUser(string name, string pass, bool isSuper) {
            AddUser(new User() { Name = name, Password = MD5.Calculate(Encoding.UTF8.GetBytes(pass)), IsSuperUser = isSuper });
        }

        private void AddUser(User user) {
            users.Add(user);
            SaveUsers();
        }

        public bool TryAuthenticate(string name, string password) {
            User user = GetUser(name);

            if (user == null) return false;
            if (user.Password != MD5.Calculate(Encoding.UTF8.GetBytes(password))) return false;

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
