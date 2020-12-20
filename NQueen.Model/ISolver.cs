using NQueen.Shared;
using NQueen.Shared.Enum;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace NQueen.Model
{
    public delegate void SolutionFoundDelegate(object sender, sbyte[] e);
    public delegate void QueenPlacedDelegate(object sender, sbyte[] e);

    public interface ISolver
    {
        int DelayInMilliseconds { get; set; }

        bool CancelSolver { get; set; }

        SolutionMode SolutionMode { get; set; }

        DisplayMode DisplayMode { get; set; }

        Visibility ProgressVisibility { get; set; }

        double ProgressValue { get; set; }

        ObservableCollection<Solution> ObservableSolutions { get; set; }

        event QueenPlacedDelegate QueenPlaced;

        event SolutionFoundDelegate SolutionFound;

        Task<ISimulationResults> GetSimulationResultsAsync(sbyte boardSize, SolutionMode solutionMode);

        Task<ISimulationResults> GetSimulationResultsAsync(sbyte boardSize);
    }
}