using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using AForge;
using AForge.Video;
using AForge.Video.DirectShow;

namespace USB_Camera
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public ObservableCollection<FilterInfo> VideoDevices { get; set;}

        public FilterInfo CurrentDevice
        {
            set { _currentDevice = value; this.OnPropertyChanged("CurrentDevice"); }
            get { return _currentDevice; }
        }
        private FilterInfo _currentDevice;

        private IVideoSource _videoSource;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            GetVideoDevices();
            this.Closing += MainWindow_Closing;
        }
        
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            StopCamera();
        }

        private void StopCamera()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= new NewFrameEventHandler(video_NewFeame);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if(handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }


        private void GetVideoDevices()
        {
            VideoDevices = new ObservableCollection<FilterInfo>();
            foreach (FilterInfo i in new FilterInfoCollection(FilterCategory.VideoInputDevice))
            {
                VideoDevices.Add(i);
            }
            if (VideoDevices.Any())
            {
                CurrentDevice = VideoDevices[0];
            }
        }
        
        

        private void video_NewFeame(object sender, AForge.Video.NewFrameEventArgs arguments)
        {
            try
            {
                BitmapImage bitmapImage;
                using (var bitmap = (Bitmap)arguments.Frame.Clone())
                {
                    bitmapImage = bitmap.ToBitmapImage();
                }
                bitmapImage.Freeze();
                Dispatcher.BeginInvoke(new ThreadStart(delegate { VideoPlayer.Source = bitmapImage; }));
            }
            catch(Exception e)
            {
                MessageBox.Show("Error:\n", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StopCamera();
            }
        }

        

        private void startCamera()
        {
            if (CurrentDevice != null)
            {
                MessageBox.Show("XD");
                _videoSource = new VideoCaptureDevice(CurrentDevice.MonikerString);
                _videoSource.NewFrame += null;
                _videoSource.Start();
            }
        }

       
        private void SaturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void ContrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        
        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnRecord_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnStopRecording_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnScreenshot_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnStart_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hahahihi");
            startCamera();
        }
    }
}
