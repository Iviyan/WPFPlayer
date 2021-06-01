using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel vm = new();
        Settings settings = new();
        LocalSettings localSettings;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = vm;

            AccountMenu = this.Resources["AccountCMenu"] as ContextMenu;

            localSettings = settings.GetLocalSettings();
            Account acc = null;
            if (localSettings.Login != null)
            {
                acc = settings.Auth(localSettings.Login, localSettings.Password);
                if (acc != null)
                {
                    vm.Login = acc.Login;
                    vm.IsPro = acc.Pro;

                }
            }

            vm.Playlists = vm.IsAuthorized ? new(acc.Playlists) : new(localSettings.Playlists ?? new Playlist[] { });

            MediaControlBar.MediaOpened += MediaControlBar_MediaOpened;
            MediaControlBar.BeforeStart += MediaControlBar_BeforeStart;
        }

        private void MediaControlBar_MediaOpened()
        {
            if (vm.CurrentPlaylist.Items == MediaControlBar.OriginalPlaylist)
            {
                int ind = vm.CurrentPlaylist.Items.IndexOf(MediaControlBar.Track);
                if (ind >= 0) PlaylistLB.SelectedIndex = ind;
            }

            if (media.HasVideo)
            {
                VideoView.Visibility = Visibility.Visible;
                ExpandVideo.Visibility = Visibility.Visible;
            } else
            {
                VideoView.Visibility = Visibility.Collapsed;
                ExpandVideo.Visibility = Visibility.Hidden;
            }
        }

        static Random rnd = new();

        static string[] advertisingFiles = { /*"Advertising\\1xbet.mp4"*/ };
        const int advertisingInterval = 5;
        int currentAdvertisingInterval = advertisingInterval;
        private void MediaControlBar_BeforeStart()
        {
            if (!vm.IsPro || advertisingFiles.Length > 0)
            {
                currentAdvertisingInterval--;
                if (currentAdvertisingInterval <= 0)
                {
                    MediaControlBar.InsertMedia(new Uri(advertisingFiles[rnd.Next(0, advertisingFiles.Length - 1)], UriKind.RelativeOrAbsolute), "Реклама", true);
                    currentAdvertisingInterval = advertisingInterval;
                }
            }
        }

        ContextMenu AccountMenu;
        private void AccountBtn_Click(object sender, RoutedEventArgs e)
        {
            AccountMenu.PlacementTarget = sender as UIElement;
            AccountMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            AccountMenu.IsOpen = true;
        }

        private void AuthMI_Click(object sender, RoutedEventArgs e)
        {
            AuthWindow authWindow = new(settings);
            authWindow.ShowDialog();
            Account acc = authWindow.Account;
            if (acc != null)
            {
                localSettings.Login = vm.Login = acc.Login;
                vm.IsPro = acc.Pro;
                localSettings.Password = acc.Password;

                if (acc.Playlists?.Count > 0)
                {
                    if (vm.Playlists?.Count == 0)
                        vm.Playlists = new(acc.Playlists);
                    else
                    {
                        var res = MessageBox.Show("Плейлисты есть как на аккаунте, так и в локальном хранилище.\nДа - выполнить слияние\nНет - оставить версию, что на аккаунте\nОставитьв локальную версию", "Конфликт", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                        if (res == MessageBoxResult.Yes) vm.Playlists = new(acc.Playlists.Concat(localSettings.Playlists));
                        else if (res == MessageBoxResult.No) vm.Playlists = new(acc.Playlists);
                    }
                }
            }
        }

        private void RegMI_Click(object sender, RoutedEventArgs e)
        {
            RegWindow regWindow = new(settings);
            bool? res = regWindow.ShowDialog();
            if (res == true)
            {
                Account acc = regWindow.Account;
                localSettings.Login = vm.Login = acc.Login;
                vm.IsPro = acc.Pro;
                localSettings.Password = acc.Password;
                localSettings.Playlists = null;
            }
        }

        private void LogoutMI_Click(object sender, RoutedEventArgs e)
        {
            Account acc = new(localSettings.Login, localSettings.Password) { Pro = vm.IsPro, Playlists = vm.Playlists };
            settings.UpdateAccount(acc);

            vm.Login = null;
            localSettings.Login = localSettings.Password = null;
            vm.Playlists = null;
            MediaControlBar.Playlist = null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (vm.IsAuthorized)
            {
                Account acc = new(localSettings.Login, localSettings.Password) { Pro = vm.IsPro, Playlists = vm.Playlists };
                settings.UpdateAccount(acc);
            } else
            {
                localSettings.Playlists = vm.Playlists;
            }
            settings.UpdateLocalSettings(localSettings);
        }

        private void BuyProMI_Click(object sender, RoutedEventArgs e)
        {
            bool r = settings.UpdateAccount(vm.Login, pro: true);
            if (r) vm.IsPro = true;
        }

        private void PlaylistLB_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        if (vm.IsPro)
                        {
                            if (!MainWindowViewModel.mediaFileExtensions
                                    .Contains(System.IO.Path.GetExtension(file).Substring(1).ToLower())) continue;
                        } else
                        {
                            if (!MainWindowViewModel.audioFileExtensions
                                    .Contains(System.IO.Path.GetExtension(file).Substring(1).ToLower()))
                            {
                                if (MainWindowViewModel.mediaFileExtensions
                                    .Contains(System.IO.Path.GetExtension(file).Substring(1).ToLower()))
                                    MessageBox.Show("Добавление видеофайлов доступно только в про версии.");
                                continue;
                            }
                        }
                        vm.CurrentPlaylist.Items.Add(file);
                    }
                }
            }
        }

        private void PlaylistLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlaylistLB.SelectedIndex >= 0)
            {
                if (vm.CurrentPlaylist.Items == MediaControlBar.OriginalPlaylist && MediaControlBar.Track == PlaylistLB.SelectedItem as string)
                    return;
                MediaControlBar.SetPlaylist(vm.CurrentPlaylist.Items, PlaylistLB.SelectedIndex);
            }
        }

        private void CloseVideoViewBtn_Click(object sender, RoutedEventArgs e)
        {
            VideoView.Visibility = Visibility.Collapsed;
        }

        private void ExpandVideo_Click(object sender, RoutedEventArgs e)
        {
            VideoView.Visibility = Visibility.Visible;
        }

        private void ChangePassMI_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordWindow changePasswordWindow = new(settings, localSettings.Login);
            bool? res = changePasswordWindow.ShowDialog();
            if (res == true)
            {
                localSettings.Password = changePasswordWindow.Password;
            }
        }
    }
}
