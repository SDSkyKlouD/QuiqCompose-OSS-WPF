using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SDSK.QuiqCompose.WinDesktop.Controls {
    [ContentProperty()]
    public partial class MetroWindowBottomControlBar : UserControl {
        public static readonly DependencyPropertyKey LeftControlsProperty =
            DependencyProperty.RegisterReadOnly(
                nameof(LeftControls),
                typeof(UIElementCollection),
                typeof(MetroWindowBottomControlBar),
                new PropertyMetadata());
        public static readonly DependencyPropertyKey RightControlsProperty =
            DependencyProperty.RegisterReadOnly(
                nameof(RightControls),
                typeof(UIElementCollection),
                typeof(MetroWindowBottomControlBar),
                new PropertyMetadata());

        public UIElementCollection LeftControls {
            get => (UIElementCollection) GetValue(LeftControlsProperty.DependencyProperty);
            set => SetValue(LeftControlsProperty, value);
        }

        public UIElementCollection RightControls {
            get => (UIElementCollection) GetValue(RightControlsProperty.DependencyProperty);
            set => SetValue(RightControlsProperty, value);
        }

        public MetroWindowBottomControlBar() {
            InitializeComponent();

            LeftControls = PART_LeftControlsHost.Children;
            RightControls = PART_RightControlsHost.Children;
        }

        private void WindowDragAreaMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if(e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                Window.GetWindow(this).DragMove();
            }
        }
    }
}
