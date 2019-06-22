using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SDSK.QuiqCompose.WinDesktop.Classes.Helpers;

namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.DataTypes {
    public sealed class ComposeSubWindowMediaItem {
        private Uri _mediaImageSource;
        public Uri MediaImageSourceUri {
            get => _mediaImageSource;
            set {
                _mediaImageSource = value;

                if(value != null && File.Exists(value.LocalPath)) {
                    BitmapImage image = new BitmapImage();

                    using(FileStream stream = new FileStream(value.LocalPath, FileMode.Open, FileAccess.Read)) {
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();
                    }

                    float ratio = Math.Max(image.PixelWidth, image.PixelHeight) / Math.Min(image.PixelWidth, image.PixelHeight);
                    image.DecodePixelWidth = image.PixelWidth > image.PixelHeight ? 100 : (int) (100 * ratio);
                    image.DecodePixelHeight = image.PixelHeight > image.PixelWidth ? 100 : (int) (100 * ratio);

                    if(image.CanFreeze && !image.IsFrozen) image.Freeze();
                    ProcessedMediaImage = image;
                }
            }
        }

        public BitmapImage ProcessedMediaImage { get; private set; }

        private Delegate _itemRemoveAction;
        public Delegate ItemRemoveAction {
            get => _itemRemoveAction;
            set {
                _itemRemoveAction = value;
                ItemRemoveCommand = new CommandHelper.BasicActionCommand(() => {
                    value?.DynamicInvoke(this);
                });
            }
        }

        private Delegate _itemZoomAction;
        public Delegate ItemZoomAction {
            get => _itemZoomAction;
            set {
                _itemZoomAction = value;
                ItemZoomCommand = new CommandHelper.BasicActionCommand(() => {
                    value?.DynamicInvoke(MediaImageSourceUri);
                });
            }
        }

        public ICommand ItemRemoveCommand { get; private set; } = null;
        public ICommand ItemZoomCommand { get; private set; } = null;
    }
}
