using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SDSK.QuiqCompose.WinDesktop.Classes.Helpers {
    public sealed class ProcessStartHelper {
        public static void StartProcess(string fileName, Action actionIfFail = null) {
            try {
                Process.Start(fileName);
            } catch(FileNotFoundException) {
                MessageBox.Show("File to be started is not found.");
                actionIfFail?.Invoke();
            } catch {
                MessageBox.Show("A process can't be started.");
                actionIfFail?.Invoke();
            }
        }
    }
}
