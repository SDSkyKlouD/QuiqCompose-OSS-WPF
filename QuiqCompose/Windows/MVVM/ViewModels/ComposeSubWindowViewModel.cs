using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Win32;
using SDSK.QuiqCompose.WinDesktop.Classes;
using SDSK.QuiqCompose.WinDesktop.Classes.Helpers;
using SDSK.QuiqCompose.WinDesktop.Windows.MVVM.DataTypes;

namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.ViewModels {
    public sealed class ComposeSubWindowViewModel : INotifyPropertyChanged {
        #region INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        #region ViewModel instance
        internal static ComposeSubWindowViewModel Instance { get; private set; } = null;
        #endregion

        #region Bindings
        private float _windowOpacity
            = ConfigurationManager.IsConfigurationReady()
                ? ConfigurationManager.ConfigurationInstance.UIConfigurations.ComposeWindowOpacity : 1;
        public float WindowOpacity {
            get => _windowOpacity;
            set {
                _windowOpacity = value;
                OnPropertyChanged(nameof(WindowOpacity));
            }
        }

        private bool _mediaPanelEnabled = true;
        public bool MediaPanelEnabled {
            get => _mediaPanelEnabled;
            set {
                _mediaPanelEnabled = value;
                OnPropertyChanged(nameof(MediaPanelEnabled));
            }
        }

        private bool _mediaAddButtonVisibility = true;
        public bool MediaAddButtonVisibility {
            get => _mediaAddButtonVisibility;
            set {
                _mediaAddButtonVisibility = value;
                OnPropertyChanged(nameof(MediaAddButtonVisibility));
            }
        }

        private ObservableCollection<ComposeSubWindowMediaItem> _mediaList;
        public ObservableCollection<ComposeSubWindowMediaItem> MediaList {
            get => _mediaList;
            set {
                var list = value;
                list.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => {
                    MediaAddButtonVisibility = (sender as ICollection<ComposeSubWindowMediaItem>).Count < 4;
                };
                _mediaList = list;

                OnPropertyChanged(nameof(MediaList));
            }
        }

        public ICommand MediaAddCommandBinding { get; } = new CommandHelper.BasicActionCommand(() => {
            OpenFileDialog fileDialog = new OpenFileDialog {
                Title = LocalizeHelper.GetLocalizedString("ComposeSubWindow_AddMediaDialog_Title"),
                Filter = $"{LocalizeHelper.GetLocalizedString("ComposeSubWindow_AddMediaDialog_Filter_StillImage")} (*.jpg, *.jpeg, *.png, *.webp)|*.jpg;*.jpeg;*.png;*.webp",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = true
            };

            if(fileDialog.ShowDialog(System.Windows.Application.Current.MainWindow) == true) {
                for(int i = 0, fileCount = fileDialog.FileNames.Length, existingMediaCount = Instance.MediaList.Count; i < fileCount; i++) {
                    if(i < 4 - existingMediaCount) {
                        RemoveMediaItemDelegate removeDelegate = Instance.RemoveMediaItem;
                        ZoomMediaItemDelegate zoomDelegate = Instance.ZoomMediaItem;

                        Instance.MediaList.Add(new ComposeSubWindowMediaItem() {
                            MediaImageSourceUri = new Uri(fileDialog.FileNames[i], UriKind.Absolute),
                            ItemRemoveAction = removeDelegate,
                            ItemZoomAction = zoomDelegate
                        });
                    } else {
                        break;
                    }
                }
            }
        });
        #endregion

        #region Constructor
        public ComposeSubWindowViewModel() {
            Instance = this;
            MediaList = new ObservableCollection<ComposeSubWindowMediaItem>();
        }
        #endregion
        
        #region Private functions
        internal delegate void RemoveMediaItemDelegate(ComposeSubWindowMediaItem item);
        private void RemoveMediaItem(ComposeSubWindowMediaItem item) => MediaList.Remove(item);

        internal delegate void ZoomMediaItemDelegate(Uri mediaUri);
        private void ZoomMediaItem(Uri mediaUri) => ProcessStartHelper.StartProcess(mediaUri.AbsoluteUri);
        #endregion
    }
}
