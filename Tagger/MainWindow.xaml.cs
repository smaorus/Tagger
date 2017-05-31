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

namespace Tagger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PlayStateController playStateController;
        public MainWindow()
        {
            InitializeComponent();
            playStateController = new PlayStateController();
            playStateController.DisableCommand += (s, e) => btnPlay.IsEnabled = false;
            playStateController.EnableCommand += (s, e) => btnPlay.IsEnabled = true;
            playStateController.PauseCommand += (s, e) => { btnPlay.Content = FindResource("Play"); mePlayer.Pause(); };
            playStateController.PlayCommand += (s, e) => { btnPlay.Content = FindResource("Pause"); mePlayer.Play(); };           
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
