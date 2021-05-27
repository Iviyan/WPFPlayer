using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    class MainWindowModel : INotifyPropertyChanged
    {
        private bool isAuthorized;
        private string login;
        private bool isPro;

        public bool IsAuthorized
        {
            get => isAuthorized;
            set { isAuthorized = value; OnPropertyChanged(); }
        }

        public string Login
        {
            get => login;
            set {
                login = value; OnPropertyChanged();
                IsAuthorized = login != null;
                if (login == null) IsPro = false;
            }
        }

        public bool IsPro
        {
            get => isPro;
            set { isPro = value; OnPropertyChanged(); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

    }
}
