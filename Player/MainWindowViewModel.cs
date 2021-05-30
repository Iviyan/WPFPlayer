using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Player
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool isAuthorized = false;
        private string login = null;
        private bool isPro = false;
        private ObservableCollection<Playlist> playlists = new();
        private Playlist currentPlaylist;

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

        public ObservableCollection<Playlist> Playlists
        {
            get => playlists;
            set { playlists = value; OnPropertyChanged(); }
        }


        public Playlist CurrentPlaylist
        {
            get => currentPlaylist;
            set { currentPlaylist = value; OnPropertyChanged(); }
        }

        protected ICommand deletePlaylistCommand;
        public ICommand DeletePlaylistCommand =>
            deletePlaylistCommand ??= new RelayCommand<object>(o =>
            {
                if (o is Playlist p)
                    Playlists.Remove(p);
            });

        protected ICommand addPlaylistCommand;
        public ICommand AddPlaylistCommand =>
            addPlaylistCommand ??= new RelayCommand(() =>
            {
                int i = 0;
                while (true)
                {
                    string playlistName = $"Плейлист{++i}";
                    if (!Playlists.Any(pl => pl.Name == playlistName))
                    {
                        Playlists.Add(new Playlist(playlistName));
                        break;
                    }
                }
            });

        protected ICommand deletePlaylistItemCommand;
        public ICommand DeletePlaylistItemCommand =>
            deletePlaylistItemCommand ??= new RelayCommand<object>(o =>
            {
                if (o is string i)
                    CurrentPlaylist.Items.Remove(i);
            });

        protected ICommand addPlaylistItemCommand;
        public ICommand AddPlaylistItemCommand =>
            addPlaylistItemCommand ??= new RelayCommand(() =>
            {
                OpenFileDialog openFileDialog = new();
                openFileDialog.InitialDirectory = "c:\\";
                //openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.Filter = "All media files|*.wav;*.aac;*.wma;*.wmv;*.avi;*.mpg;*.mpeg;*.m1v;*.mp2;*.mp3;*.mpa;*.mpe;*.m3u;*.mp4;*.mov;*.3g2;*.3gp2;*.3gp;*.3gpp;*.m4a;*.cda;*.aif;*.aifc;*.aiff;*.mid;*.midi;*.rmi;*.mkv";

                if (openFileDialog.ShowDialog() == true)
                { 
                    string filePath = openFileDialog.FileName;
                    if (!CurrentPlaylist.Items.Contains(filePath))
                        CurrentPlaylist.Items.Add(filePath);
                };
            });
    


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

    }
}
