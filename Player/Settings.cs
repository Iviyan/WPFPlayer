using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    public class Settings
    {
        public string FolderName { get; }
        public string AccountsFilePath { get; }
        public string PlaylistsFilePath { get; }
        public string SettingsFilePath { get; }

        public Settings(string folderName = "Settings")
        {
            FolderName = folderName;
            AccountsFilePath = $"{FolderName}\\Accounts.json";
            PlaylistsFilePath = $"{FolderName}\\Playlists.json";
            SettingsFilePath = $"{FolderName}\\Settings.json";

            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            if (!File.Exists(AccountsFilePath))
                File.WriteAllText(AccountsFilePath, "[]");
            
            if (!File.Exists(PlaylistsFilePath))
                File.WriteAllText(PlaylistsFilePath, "{}");
            
            if (!File.Exists(SettingsFilePath))
                File.WriteAllText(SettingsFilePath, "{}");
        }

        public bool AddAccount(Account account)
        {
            string text = File.ReadAllText(AccountsFilePath);
            var accounts = JsonConvert.DeserializeObject<List<Account>>(text);

            if (accounts.Any(a => a.Login == account.Login))
                return false;
            accounts.Add(account);

            JsonSerializer js = new();
            using var fw = new StreamWriter(AccountsFilePath);
            js.Serialize(fw, accounts);
            return true;
        }
        
        public Account GetAccount(string login)
        {
            string text = File.ReadAllText(AccountsFilePath);
            var accounts = JsonConvert.DeserializeObject<List<Account>>(text);

            return accounts.Find(a => a.Login == login);
        }

        /// <summary>
        /// null - без изменений
        /// </summary>
        public bool UpdateAccount(string login, string pass = null, bool? pro = null)
        {
            string text = File.ReadAllText(AccountsFilePath);
            var accounts = JsonConvert.DeserializeObject<List<Account>>(text);

            var acc = accounts.Find(a => a.Login == login);
            if (acc == null) return false;

            if (pass != null) acc.Password = pass;
            if (pro != null) acc.Pro = pro.Value;

            JsonSerializer js = new();
            using var fw = new StreamWriter(AccountsFilePath);
            js.Serialize(fw, accounts);
            return true;
        }

        public Account Auth(string login, string pass)
        {
            string text = File.ReadAllText(AccountsFilePath);
            var accounts = JsonConvert.DeserializeObject<List<Account>>(text);

            var acc = accounts.Find(a => a.Login == login);
            if (acc != null && acc.Password == pass)
                return acc;
            return null;
        }
        
        public Dictionary<string, List<string>> GetPlaylists()
        {
            string text = File.ReadAllText(PlaylistsFilePath);
            return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(text);
        }

        public void UpdatePlaylists(Dictionary<string, List<string>> playlists)
        {
            JsonSerializer js = new();
            using var fw = new StreamWriter(PlaylistsFilePath);
            js.Serialize(fw, playlists);
        }
        
        public void UpdateLocalSettings(LocalSettings localSettings)
        {
            JsonSerializer js = new();
            using var fw = new StreamWriter(SettingsFilePath);
            js.Serialize(fw, localSettings);
        }

        public LocalSettings GetLocalSettings()
        {
            string text = File.ReadAllText(SettingsFilePath);
            return JsonConvert.DeserializeObject<LocalSettings>(text);
        }
    }
}
