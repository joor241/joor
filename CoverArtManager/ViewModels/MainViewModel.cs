using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Media.Imaging;
using TagLib;

namespace CoverArtManager.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? audioFilePath;
        [ObservableProperty]
        private string? imageFilePath;
        [ObservableProperty]
        private BitmapSource? audioImage;
        [ObservableProperty]
        private BitmapSource? imagePreview;

        public string? AudioFileName => Path.GetFileName(AudioFilePath);
        public string? ImageFileName => Path.GetFileName(ImageFilePath);

        partial void OnAudioFilePathChanged(string? value)
        {
            OnPropertyChanged(nameof(AudioFileName));
        }

        partial void OnImageFilePathChanged(string? value)
        {
            OnPropertyChanged(nameof(ImageFileName));
        }

        public void LoadAudio(string path)
        {
            AudioFilePath = path;
            AudioImage = GetAudioCoverArt(path) ?? GetFileIcon(path);
        }

        public void LoadImage(string path)
        {
            ImageFilePath = path;
            ImagePreview = LoadBitmap(path);
        }

        private BitmapSource? GetAudioCoverArt(string file)
        {
            try
            {
                var tfile = TagLib.File.Create(file);
                if (tfile.Tag.Pictures.Length > 0)
                {
                    using var ms = new MemoryStream(tfile.Tag.Pictures[0].Data.Data);
                    return LoadBitmap(ms);
                }
            }
            catch { }
            return null;
        }

        private BitmapSource GetFileIcon(string path)
        {
            var icon = System.Drawing.Icon.ExtractAssociatedIcon(path);
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                icon!.Handle,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private BitmapSource LoadBitmap(string path)
        {
            using var fs = File.OpenRead(path);
            return LoadBitmap(fs);
        }

        private BitmapSource LoadBitmap(Stream stream)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.StreamSource = stream;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }

        [RelayCommand]
        private void Embed()
        {
            if (AudioFilePath == null || ImagePreview == null) return;
            try
            {
                var tfile = TagLib.File.Create(AudioFilePath);
                using var ms = new MemoryStream();
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(ImagePreview));
                encoder.Save(ms);
                ms.Position = 0;
                var pic = new TagLib.Picture(ms.ToArray())
                {
                    Type = TagLib.PictureType.FrontCover
                };
                tfile.Tag.Pictures = new TagLib.IPicture[] { pic };
                tfile.Save();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to embed cover art: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Remove()
        {
            if (AudioFilePath == null) return;
            try
            {
                var tfile = TagLib.File.Create(AudioFilePath);
                tfile.Tag.Pictures = Array.Empty<IPicture>();
                tfile.Save();
                AudioImage = GetFileIcon(AudioFilePath);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to remove cover art: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Edit()
        {
            if (ImagePreview == null) return;
            var dlg = new Views.CropWindow(ImagePreview);
            if (dlg.ShowDialog() == true)
            {
                ImagePreview = dlg.CroppedBitmap;
            }
        }

        [RelayCommand]
        private void Clear()
        {
            AudioFilePath = null;
            ImageFilePath = null;
            AudioImage = null;
            ImagePreview = null;
        }
    }
}
