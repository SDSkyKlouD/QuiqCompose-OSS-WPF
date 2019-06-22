using System.Windows.Input;

namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.Utilities {
    public sealed class MouseWheelGesture : MouseGesture {
        public WheelDirection TargetGestureDirection { get; private set; }

        public static MouseWheelGesture ControlDown {
            get => new MouseWheelGesture(ModifierKeys.Control) { TargetGestureDirection = WheelDirection.Down };
        }

        public static MouseWheelGesture ControlUp {
            get => new MouseWheelGesture(ModifierKeys.Control) { TargetGestureDirection = WheelDirection.Up };
        }

        public MouseWheelGesture() : base(MouseAction.WheelClick) { }

        public MouseWheelGesture(ModifierKeys modifier) : base(MouseAction.WheelClick, modifier) { }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs) {
            if(!base.Matches(targetElement, inputEventArgs)) {
                return false;
            }

            if(!(inputEventArgs is MouseWheelEventArgs)) {
                return false;
            }

            var args = inputEventArgs as MouseWheelEventArgs;

            switch(TargetGestureDirection) {
                case WheelDirection.None:
                    return args.Delta == 0;
                case WheelDirection.Up:
                    return args.Delta > 0;
                case WheelDirection.Down:
                    return args.Delta < 0;
                default:
                    return false;
            }
        }
    }

    public enum WheelDirection {
        None,
        Up,
        Down
    }
}
