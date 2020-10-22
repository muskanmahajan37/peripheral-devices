using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Accord.Video.FFMPEG;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;
using Microsoft.Win32;



namespace USB_Camera
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public ObservableCollection<FilterInfo> SelectableDevices { get; set;}

        public ObservableCollection<System.Drawing.Size> VideoResolutions { get; set; }

        public FilterInfo SelectedDevice
        {
            set { _selectedDevice = value; this.OnPropertyChanged("CurrentDevice"); }
            get { return _selectedDevice; }
        }
        private FilterInfo _selectedDevice;

        private VideoCaptureDevice _vSource;


        public System.Drawing.Size CurrentResolution
        {
            get { return _currentResolution; }
            set { _currentResolution = value; this.OnPropertyChanged("CurrentResolition "); }
        }   
        private System.Drawing.Size _currentResolution;

        private MotionDetector detector;
        private VideoFileWriter _writer;
        private bool _isRecording;
        private DateTime? _firstFrameTime = null;
        BitmapImage bitmapImage;



        public MainWindow()
        {
            InitializeComponent();
            detector = new MotionDetector(new SimpleBackgroundModelingDetector(), new MotionAreaHighlighting());
            this.DataContext = this;
            GetSelectableDevices();
            GetVideoResolution();
            this.Closing += MainWindow_Closing;
        }
        
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            StopCamera();
        }
        private void GetVideoResolution()
        {
            _vSource = new VideoCaptureDevice(SelectedDevice.MonikerString);
            VideoResolutions = new ObservableCollection<System.Drawing.Size>();
            for (int i = 0; i < _vSource.VideoCapabilities.Length; i++)
            {
                VideoResolutions.Add(_vSource.VideoCapabilities[i].FrameSize);
            }
            if (VideoResolutions.Any())
            {
                _currentResolution = VideoResolutions[0];
            }
            else
            {
                MessageBox.Show("No video sources were found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void setVideoResolution()
        {
            for (int i = 0; i < _vSource.VideoCapabilities.Length; i++)
            {
                if (_currentResolution.Equals(_vSource.VideoCapabilities[i].FrameSize))
                {
                    _vSource.VideoResolution = _vSource.VideoCapabilities[i];
                }
            }

        }


        private void motionDetect(NewFrameEventArgs eventArgs)
        {
            try
            {
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    if (detector.ProcessFrame(bitmap) > 0.02)
                    {

                        Dispatcher.BeginInvoke(new ThreadStart(delegate { motionTextBox.Text = "Motion detected"; }));

                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new ThreadStart(delegate { motionTextBox.Text = "Motion not detected"; }));
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //StopCamera();
            }
        }
        private void recordVideo(NewFrameEventArgs eventArgs)
        {
            using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
            {
                try
                {
                    if (_firstFrameTime != null)
                    {
                        _writer.WriteVideoFrame(bitmap, DateTime.Now - _firstFrameTime.Value);
                    }
                    else
                    {
                        _writer.WriteVideoFrame(bitmap);
                        _firstFrameTime = DateTime.Now;
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                   // StopCamera();
                }
            }
        }

        private void StopCamera()
        {
            if (_vSource != null && _vSource.IsRunning)
            {
                _vSource.SignalToStop();
                _vSource.NewFrame -= new NewFrameEventHandler(streamedFrame);
            }
            VideoSource.Source = null;
            Dispatcher.BeginInvoke(new ThreadStart(delegate { motionTextBox.Text = ""; }));
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
                if (_isRecording)
                {
                    recordVideo(eventArgs);
                }

                
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    bitmapImage = bitmap.ToBitmapImage();
                }
                bitmapImage.Freeze();
                Dispatcher.BeginInvoke(new ThreadStart(delegate { VideoSource.Source = bitmapImage; }));
                motionDetect(eventArgs);
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
                _vSource = new VideoCaptureDevice(SelectedDevice.MonikerString);
                _vSource.NewFrame += streamedFrame;
                _vSource.Start();
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
            StopCamera();
        }

        private void BtnRecord_Click(object sender, RoutedEventArgs e)
        {
                var dialog = new SaveFileDialog();
                 dialog.FileName = "Video1";
                 dialog.DefaultExt = ".avi";
                 dialog.AddExtension = true;
                 var dialogresult = dialog.ShowDialog();
                 if (dialogresult != true)
                 {
                     return;
                 }
                 _firstFrameTime = null;
                 _writer = new VideoFileWriter();
                 _writer.Open(dialog.FileName, (int)Math.Round(bitmapImage.Width, 0), (int)Math.Round(bitmapImage.Height, 0),30,VideoCodec.Default,500000);

                 _isRecording = true;
            }


            private void BtnStopRecording_Click(object sender, RoutedEventArgs e)
        {
            _isRecording = false;
            _writer.Close();
            _writer.Dispose();
        }

        private void BtnScreenshot_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = "Snapshot1";
            dialog.DefaultExt = ".png";
            var dialogresult = dialog.ShowDialog();
            if (dialogresult != true)
            {
                return;
            }
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            using (var filestream = new FileStream(dialog.FileName, FileMode.Create))
            {
                encoder.Save(filestream);
            }
        }

        private void BtnStart_Click_1(object sender, RoutedEventArgs e)
        {
            startCamera();
        }
    }
}
