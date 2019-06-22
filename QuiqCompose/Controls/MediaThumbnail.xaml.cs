using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace SDSK.QuiqCompose.WinDesktop.Controls {
    [ContentProperty()]
    public partial class MediaThumbnail : UserControl {
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                nameof(ImageSource),
                typeof(ImageSource),
                typeof(MediaThumbnail));

        public static readonly DependencyProperty ZoomCommandProperty =
            DependencyProperty.Register(
                nameof(ZoomCommand),
                typeof(ICommand),
                typeof(MediaThumbnail));

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register(
                nameof(RemoveCommand),
                typeof(ICommand),
                typeof(MediaThumbnail));

        public ImageSource ImageSource {
            get => (ImageSource) GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public ICommand RemoveCommand {
            get => (ICommand) GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public ICommand ZoomCommand {
            get => (ICommand) GetValue(ZoomCommandProperty);
            set => SetValue(ZoomCommandProperty, value);
        }

        public MediaThumbnail() => InitializeComponent();
    }
}
