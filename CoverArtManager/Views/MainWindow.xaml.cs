using System.Windows;
using System.Windows.Controls;

namespace CoverArtManager.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AudioDrop_Drop(object sender, DragEventArgs e)
        {
            if (DataContext is ViewModels.MainViewModel vm && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                    vm.LoadAudio(files[0]);
            }
        }

        private void ImageDrop_Drop(object sender, DragEventArgs e)
        {
            if (DataContext is ViewModels.MainViewModel vm && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                    vm.LoadImage(files[0]);
            }
        }
    }
}
