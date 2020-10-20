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

using AForge.Video;
using AForge.Video.DirectShow;

namespace USB_Camera
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public ObservableCollection<FilterInfo> SelectableDevices { get; set;}

        public FilterInfo SelectedDevice
        {
            set { _selectedDevice = value; this.OnPropertyChanged("CurrentDevice"); }
            get { return _selectedDevice; }
        }
        private FilterInfo _selectedDevice;

        private IVideoSource vSource;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            GetSelectableDevices();
            this.Closing += MainWindow_Closing;
        }
        
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            StopCamera();
        }

        private void StopCamera()
        {
            if (vSource != null && vSource.IsRunning)
            {
                vSource.SignalToStop();
                vSource.NewFrame -= new NewFrameEventHandler(streamedFrame);
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


        private void GetSelectableDevices()
        {
            SelectableDevices = new ObservableCollection<FilterInfo>();
            foreach (FilterInfo i in new FilterInfoCollection(FilterCategory.VideoInputDevice))
            {
                SelectableDevices.Add(i);
            }
            if (SelectableDevices.Any())
            {
                SelectedDevice = SelectableDevices[0];
            }
        }
        
        

        private void streamedFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                BitmapImage bitmapImage;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    bitmapImage = bitmap.ToBitmapImage();
                }
                bitmapImage.Freeze();
                Dispatcher.BeginInvoke(new ThreadStart(delegate { VideoSource.Source = bitmapImage; }));
            }
            catch(Exception e)
            {
                MessageBox.Show("Error:\n", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StopCamera();
            }
        }

        

        private void startCamera()
        {
            if (SelectedDevice != null)
            {
                vSource = new VideoCaptureDevice(SelectedDevice.MonikerString);
                vSource.NewFrame += streamedFrame;
                vSource.Start();
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
            startCamera();
        }
    }
}
