using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Player
{
    public class Playlist : INotifyPropertyChanged
    {
        private string name;

        public string Name
        {
            get => name;
            set { name = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> items;

        public ObservableCollection<string> Items
        {
            get => items;
            set { items = value; OnPropertyChanged(); }
        }

        public Playlist(string name)
        {
            Name = name;
            Items = new();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
