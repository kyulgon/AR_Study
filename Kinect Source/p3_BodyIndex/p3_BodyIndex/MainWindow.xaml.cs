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

using Microsoft.Kinect;

namespace p3_BodyIndex
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Size of the RGB pixel in the bitmap
        /// </summary>
        private const int BytesPerPixel = 4;

        /// <summary>
        /// Collection of colors to be used to display the BodyIndexFrame data.
        /// </summary>
        //private static readonly uint[] BodyColor =
        //{
        //    0x0000FF00,
        //    0x00FF0000,
        //    0xFFFF4000,
        //    0x40FFFF00,
        //    0xFF40FF00,
        //    0xFF808000,
        //};

        private KinectSensor _sensor = null;
        private BodyIndexFrameReader _bodyIndexFrameReader = null;
        private FrameDescription _description = null;
        private WriteableBitmap _bitmap = null;

        public ImageSource ImageSource
        {
            get
            {
                return _bitmap;
            }
        }

        private byte[] _bodyIndexBufffer = null;
        private int _stride = 0;
        private byte[] _bodyIndexColorBuffer = null;
        private Color[] _bodyColors = null;

        public MainWindow()
        {
            _sensor = KinectSensor.GetDefault();
            _bodyIndexFrameReader = _sensor.BodyIndexFrameSource.OpenReader();
            _bodyIndexFrameReader.FrameArrived += _bodyIndexFrameReader_FrameArrived;

            _description = _sensor.BodyIndexFrameSource.FrameDescription;
            _bodyIndexBufffer = new byte[_description.LengthInPixels];
            _bitmap = new WriteableBitmap(_description.Width, _description.Height, 96, 96, PixelFormats.Bgra32, null);
            _stride = (int)(_description.Width * BytesPerPixel);
            _bodyIndexColorBuffer = new byte[_description.LengthInPixels * BytesPerPixel];

            _bodyColors = new Color[] { Colors.Red, Colors.Green, Colors.Blue, Colors.Purple, Colors.Yellow, Colors.Brown };
            _sensor.Open();

            this.DataContext = this;

            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_bodyIndexFrameReader != null)
            {
                //_bodyIndexFrameReader.FrameArrived -= _bodyIndexFrameReader_FrameArrived;

                _bodyIndexFrameReader.Dispose();
                _bodyIndexFrameReader = null;
            }

            if (_sensor != null)
            {
                _sensor.Close();
                _sensor = null;
            }
        }

        private void _bodyIndexFrameReader_FrameArrived(object sender, BodyIndexFrameArrivedEventArgs e)
        {
            using (var bodyIndexFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyIndexFrame != null)
                {
                    bodyIndexFrame.CopyFrameDataToArray(_bodyIndexBufffer);


                    for (int i = 0; i < _bodyIndexBufffer.Length; i++)
                    {
                        var index = _bodyIndexBufffer[i];
                        var colorIndex = i * 4;

                        if (index != 255)
                        {
                            var color = _bodyColors[index];
                            _bodyIndexColorBuffer[colorIndex + 0] = color.B;
                            _bodyIndexColorBuffer[colorIndex + 1] = color.G;
                            _bodyIndexColorBuffer[colorIndex + 2] = color.R;
                            _bodyIndexColorBuffer[colorIndex + 3] = 255;
                        }
                        else
                        {
                            _bodyIndexColorBuffer[colorIndex + 0] = 255;
                            _bodyIndexColorBuffer[colorIndex + 1] = 255;
                            _bodyIndexColorBuffer[colorIndex + 2] = 255;
                            _bodyIndexColorBuffer[colorIndex + 3] = 255;
                        }
                    }

                    _bitmap.WritePixels(new Int32Rect(0,0,_description.Width,_description.Height), _bodyIndexColorBuffer, _stride, 0);
                }
            }

        }

    }
}
