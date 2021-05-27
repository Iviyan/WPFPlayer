using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace Player
{
    /// <summary>
    /// Логика взаимодействия для ControlBar.xaml
    /// </summary>
    public partial class ControlBar : UserControl
    {
        private MediaElement media;
        private IList<string> originalPlaylist;
        private IList<string> playlist;
        private int currentTrack = 0;

        private DispatcherTimer timerVideoTime;

        public static readonly DependencyProperty MediaProperty = DependencyProperty.Register(
            "Media", typeof(MediaElement), typeof(ControlBar),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnMediaChanged)));

        private static void OnMediaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ControlBar cb = d as ControlBar;

            var _media = e.OldValue as MediaElement;
            var media = e.NewValue as MediaElement;

            if (_media != null)
            {
                _media.MediaOpened -= cb.Media_MediaOpened;
                _media.MediaEnded -= cb.Media_MediaEnded;
                _media.MediaFailed -= cb.Media_MediaFailed;

                cb.VolumeSl.ClearValue(Slider.ValueProperty);
            }

            if (media != null)
            {
                media.MediaOpened += cb.Media_MediaOpened;
                media.MediaEnded += cb.Media_MediaEnded;
                media.MediaFailed += cb.Media_MediaFailed;

                Binding bind = new();
                bind.Source = media;
                bind.Path = new PropertyPath("Volume");
                bind.Mode = BindingMode.TwoWay;
                cb.VolumeSl.SetBinding(Slider.ValueProperty, bind);
            }

            cb.media = media;

            cb.SetControlEnable();
        }

        public MediaElement Media
        {
            get => (MediaElement)GetValue(MediaProperty);
            set { SetValue(MediaProperty, value); }
        }

        public IList<string> Playlist
        {
            get => playlist;
            set {
                originalPlaylist = value;
                if (ShuffleTbtn.IsChecked == true)
                    playlist = originalPlaylist.Shuffle();
                else
                    playlist = originalPlaylist;

                CurrentTrack = 0;
                SetControlEnable();
            }
        }

        public int CurrentTrack
        {
            get => currentTrack;
            set {
                currentTrack = value;
                if (playlist != null)
                    SetTrack();
            }
        }

        public static readonly DependencyProperty RewindButtonsVisibilityProperty = DependencyProperty.Register(
            "RewindButtonsVisibility", typeof(bool), typeof(ControlBar),
            new FrameworkPropertyMetadata(true));

        public bool RewindButtonsVisibility
        {
            get { return (bool)GetValue(RewindButtonsVisibilityProperty); }
            set { SetValue(RewindButtonsVisibilityProperty, value); }
        }


        public ControlBar()
        {
            InitializeComponent();

            SetControlEnable();

            timerVideoTime = new DispatcherTimer();
            timerVideoTime.Interval = TimeSpan.FromSeconds(0.2);
            timerVideoTime.Tick += TimerVideoTime_Tick;
        }


        void SetControlEnable()
        {
            bool b = Media != null;

            VolumeSl.IsEnabled = b;
            if (playlist != null || b == false)
            {
                TimeSl.IsEnabled = b;
                PlayPauseTBtn.IsEnabled = b;
                BackBtn.IsEnabled = b;
                NextBtn.IsEnabled = b;
                ShuffleTbtn.IsEnabled = b;
                RepeatTBtn.IsEnabled = b;
            }
        }

        void SetTrack()
        {
            string path = Playlist[CurrentTrack];

            media.Source = new Uri(path, UriKind.RelativeOrAbsolute);
            TitleTBl.Text = System.IO.Path.GetFileName(path);
            if (PlayPauseTBtn.IsChecked != true)
            {
                media.Play();
                media.Pause();
            }
        }

        void Next()
        {
            if (RepeatTBtn.IsChecked == false)
            {
                media.Position = TimeSpan.Zero;
                return;
            }

            if (CurrentTrack + 1 == Playlist.Count)
            {
                CurrentTrack = 0;

                if (RepeatTBtn.IsChecked != true)
                    PlayPauseTBtn.IsChecked = false;
            } else
                CurrentTrack++;
        }

        void Prev()
        {
            if (media.Position.TotalSeconds > 5)
            {
                media.Position = TimeSpan.Zero;
                return;
            }

            if (CurrentTrack - 1 < 0)
            {
                CurrentTrack = Playlist.Count - 1;
            } else
                CurrentTrack--;
        }

        private void Media_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Media_MediaOpened(object sender, RoutedEventArgs e)
        {
            TimeSl.Maximum = media.NaturalDuration.TimeSpan.TotalSeconds;

            if (PlayPauseTBtn.IsChecked == true)
                timerVideoTime.Start();
        }

        private void TimerVideoTime_Tick(object sender, EventArgs e)
        {
            TimeSl.Value = media.Position.TotalSeconds;
        }

        private void Media_MediaEnded(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void PlayPauseTBtn_Checked(object sender, RoutedEventArgs e)
        {
            media.Play();
            timerVideoTime.Start();
        }
        private void PlayPauseTBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            media.Pause();
            timerVideoTime.Stop();
        }

        private void TimeSl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            timerVideoTime.Stop();
        }

        private void TimeSl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            timerVideoTime.Start();
            media.Position = TimeSpan.FromSeconds(TimeSl.Value);
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Prev();
        }

        private void ShuffleTbtn_Checked(object sender, RoutedEventArgs e)
        {
            playlist = originalPlaylist.ShuffleWithoutFirstElement(CurrentTrack);
            currentTrack = 0;
        }

        private void ShuffleTbtn_Unchecked(object sender, RoutedEventArgs e)
        {
            currentTrack = originalPlaylist.IndexOf(playlist[CurrentTrack]);
            playlist = originalPlaylist;
        }

        private void Back10sBtn_Click(object sender, RoutedEventArgs e)
        {
            var time_ = media.Position - TimeSpan.FromSeconds(10);
            if (time_ >= TimeSpan.Zero)
                media.Position = time_;
            else
                media.Position = TimeSpan.Zero;
        }

        private void Forward10sBtn_Click(object sender, RoutedEventArgs e)
        {
            var time_ = media.Position + TimeSpan.FromSeconds(10);
            if (time_ <= media.NaturalDuration)
                media.Position = time_;
            else
                Next();
        }
    }
}
