using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    public class Account
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool Pro { get; set; } = false;
        public IList<Playlist> Playlists { get; set; }

        public Account(string login, string pass) =>
            (Login, Password) = (login, pass);
    }
}
