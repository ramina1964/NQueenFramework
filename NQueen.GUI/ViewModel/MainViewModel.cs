using GalaSoft.MvvmLight;
using NQueen.Model;

namespace NQueen.GUI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(ISolver solver)
        {
            SolverViewModel = new SolverViewModel(solver);
        }
        #region PublicProperties
        public SolverViewModel SolverViewModel { get; }
        #endregion PublicProperties
    }
}