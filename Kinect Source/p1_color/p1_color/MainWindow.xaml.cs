using System;
using System.Collections.Generic;
using System.Globalization;
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

using Microsoft.Kinect;

namespace p1_color
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor _sensor = null;                    // 키넥트 센서 변수
        private ColorFrameReader _colorFrameReader = null;      // 키넥트에서 받아오는 Color Frame 변수
        private WriteableBitmap _bitmap = null;                 // 영상을 출력하기 위한 C#의 Bitmap 이미지 변수
        
        public ImageSource ImageSource
        {
            get
            {
                return _bitmap;
            }
        }

        public MainWindow()
        {
            _sensor = KinectSensor.GetDefault();                                // 키넥트 정보를 받아옴(2버전은 1개의 키넥트만 연결 가능)
            _colorFrameReader = _sensor.ColorFrameSource.OpenReader();          // 컬러프레임 연결
            _colorFrameReader.FrameArrived += _colorFrameReader_FrameArrived;   // 매 프레임마다 들어가는 함수 연결

            FrameDescription colorFrameDescription = _sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);
            _bitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null); // 키넥트에서 들어오는 Color영상의 정보값

            _sensor.Open();     // 센서 시작

            this.DataContext = this;
            InitializeComponent();
        }

        private void _colorFrameReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                    FrameDescription des = colorFrame.FrameDescription;

                    using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                    {
                        _bitmap.Lock();

                        if ((des.Width == _bitmap.PixelWidth) && (des.Height == _bitmap.PixelHeight))
                        {
                            colorFrame.CopyConvertedFrameDataToIntPtr(
                                _bitmap.BackBuffer,
                                (uint)(des.Width * des.Height * 4),
                                ColorImageFormat.Bgra);

                            _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
                        }

                        _bitmap.Unlock();
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_colorFrameReader != null)
            {
                _colorFrameReader.Dispose();
                _colorFrameReader = null;
            }

            if (_sensor != null)
            {
                _sensor.Close();
                _sensor = null;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_bitmap != null)
            {
                BitmapEncoder encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(_bitmap));

                string time = System.DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

                string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                string path = Path.Combine(myPhotos, "KinectScreenshot-Color-" + time + ".png");

                try
                {
                    // FileStream is IDisposable
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        encoder.Save(fs);
                    }
                }
                catch (IOException)
                {
                    
                }
            }
        }
    }
}
