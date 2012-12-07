/*
 * Main Window
 */

//#define DEBUG_ON

using System;
using System.IO;
using System.ServiceModel;
using System.Windows.Input;
using System.Windows.Media;
using CIS681.Fall2012.VDS.Communication;
using CIS681.Fall2012.VDS.Data;
using CIS681.Fall2012.VDS.Data.IO;
using CIS681.Fall2012.VDS.UI.Promot;
using System.Windows.Controls;

namespace CIS681.Fall2012.VDS.UI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        // dirty definition, will use data binding in the future
        private static readonly Brush Green = (Brush)(new BrushConverter()).ConvertFrom("#3c8f01");
        private static readonly Brush Red = (Brush)(new BrushConverter()).ConvertFrom("#e9503e");
        private static readonly Brush Yellow = (Brush)(new BrushConverter()).ConvertFrom("#f5b400");
        private static readonly Brush Blue = (Brush)(new BrushConverter()).ConvertFrom("#4a8af4");

        private SyncServiceClient client;

        // is project synchronizing enabled
        private bool _isProjSyncEnabled = false;
        public bool IsProjectSynchronizationEnabled {
            get { return _isProjSyncEnabled; }
            set {
#if DEBUG_ON
                Console.WriteLine("IsProjectSynchronizationEnabled value changed.");
#endif
                if (_isProjSyncEnabled == value || Project.Current == null) return;
                if (_isProjSyncEnabled = value) {
                    Project.Current.DataUpdated += OnProjectDataUpdated;
                    client.Connect();
                }
                else
                    Project.Current.DataUpdated -= OnProjectDataUpdated;
                IsSynchronizationEnabledCheckBox.IsChecked = value;
            }
        }

        /// <summary>
        /// When hte project data is updated, send it to the server
        /// </summary>
        /// <param name="project"></param>
        private void OnProjectDataUpdated(BaseData project) {
            client.Start();
            switch (client.State) {
                case ConnectionState.Connecting:
                    WinStatus.Background = Blue;
                    CurrentStatus.Text = "Connecting";
                    break;
                case ConnectionState.Connected:
                    WinStatus.Background = Green;
                    CurrentStatus.Text = "Online (Synchronizing)";
                    break;
                case ConnectionState.Disconnected:
                    WinStatus.Background = Red;
                    CurrentStatus.Text = "Offline (Connection lost)";
                    IsProjectSynchronizationEnabled = false;
                    break;
                case ConnectionState.Uninitialized:
                default:
                    WinStatus.Background = Yellow;
                    CurrentStatus.Text = "Offline (Connection uninitialized)";
                    break;
            }
        }

        public MainWindow() {
            InitializeComponent();

            // initialize client
            client = new SyncServiceClient(this);

            // create default project at start-up
            CreateNewPorject();

            // bind commands
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, New_Executed));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Open_Executed));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save_Executed));

            Closing += MainWindow_Closing;

#if DEBUG_ON
            // display console to help debug
            CMDConsole.ConsoleManager.Show();
#endif
        }

        public void ReloadProject(Project proj) {
            bool enabledOldValue = IsProjectSynchronizationEnabled;
            IsProjectSynchronizationEnabled = false;
            Project.Current = proj;
            // reload tabs
            ProjectTabsGrid.Children.Clear();
            if (Project.Current == null) return;
            IsProjectSynchronizationEnabled = enabledOldValue;
            ProjectTabsGrid.Children.Add(Project.Current.Tabs);
            // update title
            ProjectNameBlock.Text = Project.Current.Title;
            //#if DEBUG_ON
            Console.WriteLine("Project updated.");
            //#endif
        }

        /// <summary>
        /// Create new project
        /// </summary>
        private void CreateNewPorject(bool askForTitle = false) {
            // reset synchronization, user need to enable synchronization after create a new project
            ReloadProject(new Project());
            if (askForTitle) RenameProject();
            // create a diagram by default
            CreateNewDiagram();
        }
        /// <summary>
        /// Create new blank diagram
        /// </summary>
        private void CreateNewDiagram() {
            Diagram diagram = new Diagram();
            if (Project.Current == null)
                CreateNewPorject();
            Project.Current.Children.Add(diagram);
        }
        /// <summary>
        /// Save current project
        /// </summary>
        private void SaveProject() {
            if (Project.Current == null) {
                new ErrorHandler("Please create a project first!").Show();
                return;
            }
            CurrentStatus.Text = "Saving...";
            WinStatus.Background = Red;
            using (Stream s = FileManager.Save()) {
                WinStatus.Background = Green;
                CurrentStatus.Text = "Saved";
                if (s == null)  // user cancel the operation
                    return;
                DataSerializer<Project>.Save(Project.Current, s);
            }
        }
        /// <summary>
        /// Open a new project
        /// </summary>
        private void OpenProject() {
            QuestionDialog dialog = new QuestionDialog("Are you sure to open another project?\nUnsaved progress could be LOST!!!");
            dialog.ShowDialog();
            CurrentStatus.Text = "Opening a data file...";
            WinStatus.Background = Red;
            try {
                using (Stream s = FileManager.Open()) {
                    WinStatus.Background = Green;
                    CurrentStatus.Text = "Project opened";
                    if (s == null)  // user cancel the operation
                        return;
                    ReloadProject(DataSerializer<Project>.Load(s));
                }
            }
            catch (Exception e) {
                CurrentStatus.Text = "Error!";
                ReloadProject(null);
                new ErrorHandler("Project cannot be opened!", e).Show();
            }
        }

        /// <summary>
        /// Rename current project
        /// </summary>
        private void RenameProject() {
            // name the project
            TextDialog dialog = new TextDialog("Please enter a new name for the project");
            dialog.ShowDialog();
            Project.Current.Title = dialog.Answer;
        }
        private void New_Executed(object sender, ExecutedRoutedEventArgs e) {
            if ((string)e.Parameter == "Project")
                CreateNewPorject(true);
            else
                CreateNewDiagram();
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e) {
            SaveProject();
        }
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e) {
            OpenProject();
        }

        /// <summary>
        /// Rename the project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectNameBlock_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount < 2) return;   // need double click
            RenameProject();
        }

        /// <summary>
        /// Confirm before closing window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            QuestionDialog dialog = new QuestionDialog("Are you sure to close this project?\nUnsaved offline project progress could be LOST!!!");
            dialog.ShowDialog();
            e.Cancel = !dialog.Answer;
        }

        /// <summary>
        /// Enable synchronization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsSynchronizationEnabledCheckBox_Click(object sender, System.Windows.RoutedEventArgs e) {
            IsProjectSynchronizationEnabled = (sender as CheckBox).IsChecked.GetValueOrDefault();
        }
    }
}
