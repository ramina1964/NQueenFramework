using GalaSoft.MvvmLight;
using NQueen.Shared;
using NQueen.Shared.Enum;
using NQueen.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NQueen.Model
{
    public class Solver : ViewModelBase, ISolver
    {
        public Solver(sbyte boardSize, DisplayMode DisplayMode = DisplayMode.Hide) => Initialize(boardSize, DisplayMode);

        #region ISolverInterface

        public int DelayInMilliseconds { get; set; }

        public bool CancelSolver { get; set; }

        public SolutionMode SolutionMode { get; set; }

        public DisplayMode DisplayMode { get; set; }

        public HashSet<sbyte[]> Solutions = new HashSet<sbyte[]>(new SequenceEquality<sbyte>());

        public ObservableCollection<Solution> ObservableSolutions { get; set; }

        public event QueenPlacedHandler QueenPlaced;

        public event SolutionFoundHandler SolutionFound;

        public Task<ISimulationResults> GetSimulationResultsAsync(sbyte boardSize, SolutionMode solutionMode)
        {
            return Task.Factory.StartNew(() =>
            {
                Initialize(boardSize, DisplayMode);
                SolutionMode = solutionMode;
                return GetResults();
            });
        }

        #endregion ISolverInterface

        public ISimulationResults GetResults()
        {
            var solutions = MainSolve().ToList();
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Stop();
            var timeInSec = (double)stopwatch.ElapsedMilliseconds / 1000;
            var elapsedTimeInSec = Math.Round(timeInSec, 1);

            return new SimulationResults(solutions)
            {
                BoardSize = BoardSize,
                NoOfSolutions = solutions.Count,
                Solutions = solutions,
                ElapsedTimeInSec = elapsedTimeInSec
            };
        }

        public Visibility ProgressVisibility
        {
            get => _progressBarVisibility;
            set => Set(ref _progressBarVisibility, value);
        }

        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                Set(ref _progressValue, value);
                Set(ref _progressLabel, $"{value}%");
                RaisePropertyChanged(nameof(ProgressLabel));
            }
        }

        #region PublicProperties

        public ISimulationResults Results { get; set; }

        public sbyte BoardSize { get; set; }

        public string BoardSizeText { get; set; }

        public int NoOfSolutions => Solutions.Count;

        public sbyte HalfSize { get; set; }

        public sbyte[] QueenList { get; set; }

        public string ProgressLabel
        {
            get => _progressLabel;
            set => Set(ref _progressLabel, value);
        }

        #endregion PublicProperties

        protected virtual void OnQueenPlaced(object sender, QueenPlacedEventArgs e) => QueenPlaced?.Invoke(this, e);

        protected virtual void OnSolutionFound(object sender, SolutionFoundEventArgs e) => SolutionFound?.Invoke(this, e);

        #region PrivateMethods
        private void Initialize(sbyte boardSize, DisplayMode displayMode)
        {
            BoardSize = boardSize;
            DisplayMode = displayMode;
            CancelSolver = false;

            HalfSize = (sbyte)(BoardSize % 2 == 0 ?
                BoardSize / 2 :
                BoardSize / 2 + 1);
            QueenList = Enumerable.Repeat((sbyte)-1, BoardSize).ToArray();
            Solutions = new HashSet<sbyte[]>(new SequenceEquality<sbyte>());

            //ObservableSolutions = new ObservableCollection<Solution>();
            var solutionSize = Utility.FindSolutionSize(BoardSize, SolutionMode);
            ObservableSolutions = new ObservableCollection<Solution>(new List<Solution>(solutionSize));
        }

        private bool UpdateSolutions(IEnumerable<sbyte> solution)
        {
            var queens = solution.ToArray();

            // If solutionMode == SolutionMode.Single, then we are done.
            if (SolutionMode == SolutionMode.Single)
            {
                Solutions.Add(queens);
                return true;
            }

            var symmetricalSolutions = Utility.GetSymmetricalSolutions(queens).ToList();

            // If solutionMode == SolutionMode.All, add this solution and all of the symmetrical counterparts to All Solutions.
            if (SolutionMode == SolutionMode.All)
            {
                Solutions.Add(queens);
                symmetricalSolutions.ForEach(s => Solutions.Add(s));

                return true;
            }

            // One of symmetrical solutions is already in the solutions list, nothing to add.
            if (Solutions.Overlaps(symmetricalSolutions))
            { return false; }

            // None of the symmetrical solutions exists in the solutions list, add the new solution to the Unique Solutions.
            Solutions.Add(queens);
            return true;
        }

        private IEnumerable<Solution> MainSolve()
        {
            // Recursive call to start the simulation
            SolveRec();

            return Solutions
                    .Select((s, index) => new Solution(s, index + 1));
        }

        //private bool SolveRec(SolutionMode solutionMode, sbyte colNo = 0)
        private bool SolveRec(sbyte colNo = 0)
        {
            if (CancelSolver)
            { return false; }

            // All Symmetrical solutions are found and registered. Quit the recursion.
            if (QueenList[0] == HalfSize)
            { return false; }

            if (DisplayMode == DisplayMode.Visualize)
            {
                OnQueenPlaced(this, new QueenPlacedEventArgs(QueenList));
                Thread.Sleep(DelayInMilliseconds);
            }

            if (SolutionMode == SolutionMode.Single && NoOfSolutions == 1)
            { return true; }

            if (colNo == -1)
            { return false; }

            // Here a new solution is found.
            if (colNo == BoardSize)
            {
                bool isUpdated = UpdateSolutions(QueenList);

                // Activate this code in case of IsVisulaized == true.
                if (isUpdated && DisplayMode == DisplayMode.Visualize)
                { SolutionFound(this, new SolutionFoundEventArgs(QueenList)); }

                ProgressValue = Math.Round(100.0 * QueenList[0] / HalfSize, 1);
                return false;
            }

            QueenList[colNo] = LocateQueen(colNo);
            if (QueenList[colNo] == -1)
            {
                return false;
            }

            var nextCol = (sbyte)(colNo + 1);
            return SolveRec(nextCol) || SolveRec(colNo);
        }

        // Locate Queen
        private sbyte LocateQueen(sbyte colNo)
        {
            for (sbyte pos = (sbyte)(QueenList[colNo] + 1); pos < BoardSize; pos++)
            {
                var isValid = true;
                for (int j = 0; j < colNo; j++)
                {
                    int lhs = Math.Abs(pos - QueenList[j]);
                    int rhs = Math.Abs(colNo - j);
                    if (0 != lhs && lhs != rhs)
                    { continue; }

                    isValid = false;
                    break;
                }

                if (isValid)
                { return pos; }
            }

            return -1;
        }

        #endregion PrivateMethods

        private double _progressValue;
        private string _progressLabel;
        private Visibility _progressBarVisibility;
    }
}