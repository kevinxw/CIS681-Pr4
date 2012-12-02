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
    public partial class TextDialog {
        public string Question { get; set; }

        public TextDialog() {
            InitializeComponent();
            Question = "Please input some text.";
            Owner = App.Current.MainWindow;
        }

        public TextDialog(string question)
            : this() {
            Question = question;
        }

        public string Answer { get; private set; }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Answer = AnswerTextBox.Text;
            this.Close();
        }
    }
}
