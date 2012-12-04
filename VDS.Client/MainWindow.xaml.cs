/*
 * Main Window
 */

//#define CONSOLE_DEBUG_ON

using System;
using System.IO;
using System.Windows.Input;
using CIS681.Fall2012.VDS.Data.Client;
using CIS681.Fall2012.VDS.Data.IO;
using CIS681.Fall2012.VDS.UI.Promot;
using System.Windows.Media;
using CIS681.Fall2012.VDS.Data;

namespace CIS681.Fall2012.VDS.UI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        // dirty definition, will use data binding in the future
        private static readonly Brush Green = (Brush)(new BrushConverter()).ConvertFrom("#3c8f01");
        private static readonly Brush Red = (Brush)(new BrushConverter()).ConvertFrom("#e9503e");

        public MainWindow() {
            InitializeComponent();

            // create default project at start-up
            CreateNewPorject();
            // bind commands
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, New_Executed));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Open_Executed));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save_Executed));

            Closing += MainWindow_Closing;
#if CONSOLE_DEBUG_ON
            // display console to help debug
            CMDConsole.ConsoleManager.Show();
#endif
        }

        /// <summary>
        /// Create new project
        /// </summary>
        private void CreateNewPorject(bool askForTitle=false) {
            Project.CurrentProject = new Project();
            if (askForTitle) RenameProject();
            ProjectTabsGrid.Children.Clear();
            ProjectTabsGrid.Children.Add(Project.CurrentProject.Tabs);
            // create a diagram by default
            CreateNewDiagram();
        }
        /// <summary>
        /// Create new blank diagram
        /// </summary>
        private void CreateNewDiagram() {
            Diagram diagram = new Diagram();
            if (Project.CurrentProject == null)
                CreateNewPorject();
            Project.CurrentProject.Children.Add(diagram);
        }
        /// <summary>
        /// Save current project
        /// </summary>
        private void SaveProject() {
            if (Project.CurrentProject == null) {
                new ErrorHandler("Please create a project first!").Show();
                return;
            }
            CurrentStatus.Text = "Saving...";
            WinStatus.Background = Red;
            using (Stream s = FileManager.Save()) {
                WinStatus.Background = Green;
                CurrentStatus.Text = "Offline";
                if (s == null)  // user cancel the operation
                    return;
                DataSerializer<Project>.Save(Project.CurrentProject, s);
            }
        }
        /// <summary>
        /// Open a new project
        /// </summary>
        private void OpenProject() {
            try {
                CurrentStatus.Text = "Opening a data file...";
                WinStatus.Background = Red;
                using (Stream s = FileManager.Open()) {
                    WinStatus.Background = Green;
                    CurrentStatus.Text = "Offline";
                    if (s == null)  // user cancel the operation
                        return;
                    Project.CurrentProject = DataSerializer<Project>.Load(s);
                }
                // reload tabs
                ProjectTabsGrid.Children.Clear();
                ProjectTabsGrid.Children.Add(Project.CurrentProject.Tabs);
                // update title
                ProjectNameBlock.Text = Project.CurrentProject.Title;
            }
            catch (Exception e) {
                CurrentStatus.Text = "Error!";
                Project.CurrentProject = null;
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
            Project.CurrentProject.Title = dialog.Answer;
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
            QuestionDialog dialog = new QuestionDialog("Are you sure to close this project?\nUnsaved progress could be LOST!!!");
            dialog.ShowDialog();
            e.Cancel = !dialog.Answer;
        } 
    }
}
