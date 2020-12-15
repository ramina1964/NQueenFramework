using NQueen.Main.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NQueen.Main
{
    public partial class MainView
    {
        public MainView(MainViewModel mainViewModel)
        {
            InitializeComponent();
            Loaded += MainView_Loaded;
            MainViewModel = mainViewModel;
        }

        public MainViewModel MainViewModel { get; set; }

        private void MainView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var view = e.OriginalSource as MainView;
            var board = chessboard;
            var size = (int)System.Math.Min(board.ActualWidth, board.ActualHeight);
            board.Width = size;
            board.Height = size;
            MainViewModel.SolverViewModel.SetChessboard(size);
            DataContext = MainViewModel.SolverViewModel;
        }
    }
}
