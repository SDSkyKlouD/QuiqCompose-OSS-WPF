#pragma warning disable CS0067

using System;
using System.Windows;
using System.Windows.Input;

namespace SDSK.QuiqCompose.WinDesktop.Classes.Helpers {
    /// <summary>
    /// A bunch of ICommand implementations for easier usage.
    /// </summary>
    public sealed class CommandHelper {
        /// <summary>
        /// Easy-to-use ICommand implementation for control boolean-type UI configurations.
        /// 
        /// REQUIRES CommandParameter passed as boolean type.
        /// </summary>
        public sealed class UiConfigurationBoolCommand : ICommand {
            public event EventHandler CanExecuteChanged;
            public bool CanBeExecuted { get; set; } = false;
            private readonly string _configurationName;

            /// <summary>
            /// Construct UiConfigurationOnOffCommand
            /// </summary>
            /// <param name="configurationName">Configuration name in UIConfigurations.</param>
            /// <param name="canBeExecuted">Set or unset this command can be executed.</param>
            public UiConfigurationBoolCommand(string configurationName, bool canBeExecuted = true) {
                _configurationName = configurationName;
                CanBeExecuted = canBeExecuted;
            }

            public bool CanExecute(object parameter) => CanBeExecuted;

            public void Execute(object parameter) {
                if(ConfigurationManager.IsConfigurationReady()
                    && !string.IsNullOrWhiteSpace(_configurationName)
                    && parameter is bool paramBool) {
                    ConfigurationManager.ConfigurationInstance.UIConfigurations[_configurationName] = paramBool;
                    ConfigurationManager.Save();
                }
            }
        }

        /// <summary>
        /// Easy-to-use ICommand implementation for start process.
        /// 
        /// REQUIRES either CommandParameter as absolute path string or absolute path that passed via constructor.
        /// </summary>
        public sealed class ProcessStartCommand : ICommand {
            public event EventHandler CanExecuteChanged;
            public bool CanBeExecuted { get; set; } = false;
            private readonly Action _actionIfStartFailed = null;
            private readonly string _pathToBeExecuted = null;

            /// <summary>
            /// Construct ProcessStartCommand.
            /// </summary>
            /// <param name="pathToBeExecuted">Absolute path to be started. If you leave this parameter null, CommandParameter will be used as the path.</param>
            /// <param name="actionIfFail">An action if starting a process fails.</param>
            /// <param name="canBeExecuted">Set or unset this command can be executed.</param>
            public ProcessStartCommand(string pathToBeExecuted = null, Action actionIfFail = null, bool canBeExecuted = true) {
                _pathToBeExecuted = pathToBeExecuted;
                _actionIfStartFailed = actionIfFail;
                CanBeExecuted = canBeExecuted;
            }

            public bool CanExecute(object parameter) => CanBeExecuted;

            public void Execute(object parameter) {
                string path = null;

                if(!string.IsNullOrWhiteSpace(_pathToBeExecuted)) {
                    path = _pathToBeExecuted;
                } else if(parameter is string paramPath && !string.IsNullOrWhiteSpace(paramPath)) {
                    path = paramPath;
                }

                if(!string.IsNullOrWhiteSpace(path) && Uri.TryCreate(path, UriKind.Absolute, out _)) {
                    ProcessStartHelper.StartProcess(path, _actionIfStartFailed);
                }
            }
        }

        /// <summary>
        /// Easy-to-use ICommand implementation to show a window.
        /// 
        /// REQUIRES a type of window that passed via constructor.
        /// Owner window will be set to application's current main window, if not specified.
        /// </summary>
        public sealed class ShowWindowCommand : ICommand {
            public event EventHandler CanExecuteChanged;
            public bool CanBeExecuted { get; set; } = false;
            private readonly Type _windowType = null;
            private readonly Window _ownerWindow = null;
            private readonly bool _showWindowAsDialog = false;

            /// <summary>
            /// Construct ShowWindowCommand.
            /// </summary>
            /// <param name="windowType">A type of window to be shown.</param>
            /// <param name="ownerWindow">Specify the owner window of parameter `window`. If leave this null, the application's current main window will own it.</param>
            /// <param name="showWindowAsDialog">Whether to show the window as dialog.</param>
            /// <param name="canBeExecuted">Set or unset this command can be executed.</param>
            public ShowWindowCommand(Type windowType, Window ownerWindow = null, bool showWindowAsDialog = false, bool canBeExecuted = true) {
                _windowType = windowType;
                _ownerWindow = ownerWindow;
                _showWindowAsDialog = showWindowAsDialog;
                CanBeExecuted = canBeExecuted;
            }

            public bool CanExecute(object parameter) => CanBeExecuted;

            public void Execute(object parameter) {
                Window window = (Window) Activator.CreateInstance(_windowType);

                if(window != null) {
                    window.Owner = _ownerWindow ?? Application.Current.MainWindow;

                    if(_showWindowAsDialog) {
                        window.ShowDialog();
                    } else {
                        window.Show();
                    }
                }
            }
        }

        /// <summary>
        /// Easy-to-use ICommand implementation to close a window.
        /// 
        /// REQUIRES either an instance of Window that passed via constructor or an Window that passed via CommandParameter.
        /// </summary>
        public sealed class CloseWindowCommand : ICommand {
            public event EventHandler CanExecuteChanged;
            public bool CanBeExecuted { get; set; } = false;
            private readonly Window _windowToBeClosed = null;

            /// <summary>
            /// Construct CloseWindowCommand.
            /// </summary>
            /// <param name="windowToBeClosed">An instance of Window. If leave this null, CommandParameter will be used instead.</param>
            /// <param name="canBeExecuted">Set or unset this command can be executed.</param>
            public CloseWindowCommand(Window windowToBeClosed = null, bool canBeExecuted = true) {
                _windowToBeClosed = windowToBeClosed;
                CanBeExecuted = canBeExecuted;
            }

            public bool CanExecute(object parameter) => CanBeExecuted;

            public void Execute(object parameter) {
                if(_windowToBeClosed != null) {
                    _windowToBeClosed.Close();
                } else if(parameter != null && parameter is Window paramWindow) {
                    paramWindow.Close();
                }
            }
        }

        /// <summary>
        /// Easy-to-use ICommand implementation to simply execute an Action
        /// </summary>
        public sealed class BasicActionCommand : ICommand {
            public event EventHandler CanExecuteChanged;
            public bool CanBeExecuted { get; set; } = false;
            private readonly Action _action = null;

            /// <summary>
            /// Construct BasicActionCommand.
            /// </summary>
            /// <param name="action">An action to be executed.</param>
            /// <param name="canBeExecuted">Set or unset this command can be executed.</param>
            public BasicActionCommand(Action action, bool canBeExecuted = true) {
                _action = action;
                CanBeExecuted = canBeExecuted;
            }

            public bool CanExecute(object parameter) => CanBeExecuted;

            public void Execute(object parameter) => _action?.Invoke();
        }
    }
}
