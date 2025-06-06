using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CoverArtManager.Views
{
    public partial class CropWindow : Window
    {
        private Point? startPoint;
        public BitmapSource CroppedBitmap { get; private set; }

        public CropWindow(BitmapSource source)
        {
            InitializeComponent();
            Image.Source = source;
            CroppedBitmap = source;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(Canvas);
            Canvas.CaptureMouse();
            CropRect.Width = CropRect.Height = 0;
            Canvas.Children.Remove(CropRect);
            Canvas.Children.Add(CropRect);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (startPoint.HasValue && e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(Canvas);
                var x = Math.Min(pos.X, startPoint.Value.X);
                var y = Math.Min(pos.Y, startPoint.Value.Y);
                var w = Math.Abs(pos.X - startPoint.Value.X);
                var h = Math.Abs(pos.Y - startPoint.Value.Y);
                Canvas.SetLeft(CropRect, x);
                Canvas.SetTop(CropRect, y);
                CropRect.Width = w;
                CropRect.Height = h;
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            startPoint = null;
            Canvas.ReleaseMouseCapture();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (Image.Source is BitmapSource bmp)
            {
                var rect = new Int32Rect(
                    (int)Canvas.GetLeft(CropRect),
                    (int)Canvas.GetTop(CropRect),
                    (int)CropRect.Width,
                    (int)CropRect.Height);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    var cropped = new CroppedBitmap(bmp, rect);
                    CroppedBitmap = cropped;
                }
            }
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
