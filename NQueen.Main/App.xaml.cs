using NQueen.Shared.Properties;
using NQueen.Main.ViewModel;
using NQueen.Model;
using System.Windows;

namespace NQueen.Main
{
    public partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Load();
            MainView = new MainView(MainViewModel);
            MainView.Show();
        }

        private void Load()
        {
            var boardSize = Settings.Default.DefaultBoardSize;
            var solver = new Solver(boardSize);
            MainViewModel = new MainViewModel(solver);
        }

        #region PrivateFields
        public MainView MainView { get; set; }
        public MainViewModel MainViewModel { get; set; }
        #endregion PrivateFields
    }
}