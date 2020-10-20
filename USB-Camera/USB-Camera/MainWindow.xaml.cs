using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            LoadAvailableDevices();

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler propertyChangedHandler = this.PropertyChanged;
            if(propertyChangedHandler != null)
            {
                var propertyEvent = new PropertyChangedEventArgs(name);
                propertyChangedHandler(this, propertyEvent);
            }
        }
        
        private FilterInfo selectedDev;

        public FilterInfo selectedDevice
        {
            set { selectedDev = value; this.OnPropertyChanged("SelectedCamera"); }
            get { return selectedDev; }
        }
        private void LoadAvailableDevices()
        {
            SelectableDevices = new ObservableCollection<FilterInfo>();
            foreach (FilterInfo i in new FilterInfoCollection(FilterCategory.VideoInputDevice)) SelectableDevices.Add(i);
            if (SelectableDevices.Any()) MessageBox.Show("Urządzenie odnaleziono", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private IVideoSource videoSource;
        
        public ObservableCollection<FilterInfo> SelectableDevices
        {
            set;
            get;
        }

        private void capture(object o, AForge.Video.NewFrameEventArgs arguments)
        {

        }

        private void GetCameraSource()
        {
            if (selectedDevice != null)
            {
                videoSource = new VideoCaptureDevice(selectedDevice.MonikerString);
                videoSource.NewFrame += null;
            }
        }

        private void ComboBoxCameraSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SaturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void ContrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
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
    }
}
