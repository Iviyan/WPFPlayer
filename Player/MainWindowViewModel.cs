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

        public static string[] videoFileExtensions = { "webm", "mpg", "mp2", "mpeg", "mpe", "mpv", "ogg", "mp4", "m4p", "m4v", "avi", "wmv", "mov", "flv", "swf" };
        public static string[] audioFileExtensions = { "wav", "mp3", "ogg", "gsm", "dct", "flac", "au", "aiff", "raw", "m4a", "aac" };
        public static string[] mediaFileExtensions = videoFileExtensions.Union(audioFileExtensions).ToArray();

        public static string audioFileExtensionsFilter = $"All audio files|{audioFileExtensions.Aggregate("", (res, cur) => res += $"*.{cur};")}".TrimEnd(';');
        public static string mediaFileExtensionsFilter = $"All media files|{mediaFileExtensions.Aggregate("", (res, cur) => res += $"*.{cur};")}".TrimEnd(';');

        protected ICommand addPlaylistItemCommand;
        public ICommand AddPlaylistItemCommand =>
            addPlaylistItemCommand ??= new RelayCommand(() =>
            {
                OpenFileDialog openFileDialog = new();
                openFileDialog.InitialDirectory = "c:\\";
                //openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.Filter = IsPro ? mediaFileExtensionsFilter : audioFileExtensionsFilter;
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
