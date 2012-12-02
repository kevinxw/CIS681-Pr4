/*
 * Show a dialog ask for a name
 */

//////
/// Nothing to be TESTED here!
//////

using System.Windows;

namespace CIS681.Fall2012.VDS.UI.Promot {
    /// <summary>
    /// Interaction logic for NamingDialog.xaml
    /// </summary>
    public partial class QuestionDialog {
        public string Question { get; set; }

        public QuestionDialog() {
            InitializeComponent();
            Question = "Are you sure to do this?";
            Owner = App.Current.MainWindow;
        }

        public QuestionDialog(string question)
            : this() {
            Question = question;
        }

        public bool Answer { get; private set; }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e) {
            Answer = true;
            this.Close();
        }
    }
}
