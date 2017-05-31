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

namespace Tagger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PlayStateController playStateController;
        DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
            playStateController = new PlayStateController();
            playStateController.DisableCommand += (s, e) => { btnPlay.IsEnabled = false; timelineSlider.IsEnabled = false;};
            playStateController.EnableCommand += (s, e) => { btnPlay.IsEnabled = true; timelineSlider.IsEnabled = true; timelineSlider.Value = 0; timer.Start(); };
            playStateController.PauseCommand += (s, e) => { btnPlay.Content = FindResource("Play"); mePlayer.Pause(); };
            playStateController.PlayCommand += (s, e) => { btnPlay.Content = FindResource("Pause"); mePlayer.Play(); };


            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += new EventHandler(sliderTick);     
             
        }

        private void sliderTick(object sender, EventArgs e)
        {
            if (mePlayer.NaturalDuration.HasTimeSpan)
            {
                timelineSlider.Value = mePlayer.Position.TotalSeconds /
                                           mePlayer.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            playStateController.Switch();
        }

        private void mePlayer_Drop(object sender, DragEventArgs e)
        {
            string filePath = ((string[]) e.Data.GetData(DataFormats.FileDrop))[0];
            mePlayer.Source = new Uri(filePath);

            playStateController.Enable();
        }
    }
}
