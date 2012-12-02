/*
 * Handle errors occoured during application running
 * Will be extended and used for logging or debugging in the future
 */

//////
/// Nothing to be TESTED here!
//////

using System;
using System.Windows;

namespace CIS681.Fall2012.VDS.UI {
    public class ErrorHandler {
        public string Messsage { get; set; }
        public Exception Exception { get; private set; }

        public ErrorHandler(Exception ex) {
            Messsage = ex.Message;
            this.Exception = ex;
        }
        public ErrorHandler(string msg) {
            Messsage = msg;
        }
        public ErrorHandler(string msg, Exception ex) {
            Messsage = msg;
            this.Exception = ex;
        }

        public void Show() {
            MessageBox.Show(Messsage, "Oops!",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
