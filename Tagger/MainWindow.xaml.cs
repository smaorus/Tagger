using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TaggingBackend;

namespace Tagger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PlayStateController playStateController;
        DispatcherTimer timer;
        bool userIsDraggingSlider = false;
        bool suppressMediaPositionUpdate = false;
        TagSaver tagSaver;

        public MainWindow()
        {
            InitializeComponent();
            playStateController = new PlayStateController();
            playStateController.DisableCommand += (s, e) => { btnPlay.IsEnabled = false; timelineSlider.IsEnabled = false; tagBox.IsEnabled = false; };
            playStateController.EnableCommand += (s, e) => { btnPlay.IsEnabled = true; timelineSlider.IsEnabled = true; timelineSlider.Value = 0; timer.Start(); tagBox.IsEnabled = true; };
            playStateController.PauseCommand += (s, e) => { btnPlay.Content = FindResource("Play"); mePlayer.Pause(); };
            playStateController.PlayCommand += (s, e) => { btnPlay.Content = FindResource("Pause"); mePlayer.Play(); RenewSliderPosition(); };


            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += new EventHandler(sliderTick);

            //tagBox.Background = new SolidColorBrush();

            tagSaver = new TagSaver();

            
        }

        private void sliderTick(object sender, EventArgs e)
        {
            RenewSliderPosition();
        }

        private void RenewSliderPosition()
        {
            if (mePlayer.NaturalDuration.HasTimeSpan && !userIsDraggingSlider)
            {
                suppressMediaPositionUpdate = true;
                timelineSlider.Value = CalcTimelineRelativePosition();
            }

        }

        private double CalcTimelineRelativePosition()
        {
            return mePlayer.Position.TotalSeconds /
                                                   mePlayer.NaturalDuration.TimeSpan.TotalSeconds;
            ;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            playStateController.Switch();
        }

        private void mePlayer_Drop(object sender, DragEventArgs e)
        {
            string filePath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            mePlayer.Source = new Uri(filePath);

            playStateController.Enable();
            tagBox.Text = "Loading..";
        }

        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            suppressMediaPositionUpdate = true;
            mePlayer.Position = TimeSpan.FromSeconds(timelineSlider.Value * mePlayer.NaturalDuration.TimeSpan.TotalSeconds);
        }

        private void SeekToMediaPosition(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Making sure the position doesn't keep updating when the timer ticks or when a user is dragging the video around
            if (userIsDraggingSlider) return;
            if (suppressMediaPositionUpdate)
            {
                suppressMediaPositionUpdate = false;
                return;
            }

            if (mePlayer.NaturalDuration.HasTimeSpan)
            {
                mePlayer.Position = TimeSpan.FromSeconds(timelineSlider.Value * mePlayer.NaturalDuration.TimeSpan.TotalSeconds);
            }
        }

        private void keyUpTagBox(object sender, KeyEventArgs a)
        {
            if (a.Key == Key.Enter)
            {
                string tag = tagBox.Text;
                tagBox.Text = string.Empty;

                ColorAnimation animation;
                animation = new ColorAnimation();
                animation.To = Colors.LightGreen;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(350));
                animation.AutoReverse = true;
                tagBox.Background = new SolidColorBrush();
                tagBox.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);

                tagSaver.Save(new Tag() { TagContent = tag, SecondsSinceStart = (int)mePlayer.Position.TotalSeconds, VideoName = mePlayer.Source.ToString() });
            }
        }

        private void VideoLoaded(object sender, EventArgs a)
        {
            tagBox.Text = string.Empty;
        }

        private void dragEnter(object sender, EventArgs a)
        {

        }
    }
}