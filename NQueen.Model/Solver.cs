﻿using GalaSoft.MvvmLight;
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

        public event QueenPlacedDelegate QueenPlaced;

        public event SolutionFoundDelegate SolutionFound;

        public Task<ISimulationResults> GetSimulationResultsAsync(sbyte boardSize, SolutionMode solutionMode)
        {
            return Task.Factory.StartNew(() =>
            {
                Initialize(boardSize, DisplayMode);
                return GetResults(solutionMode);
            });
        }

        public Task<ISimulationResults> GetSimulationResultsAsync(sbyte boardSize) =>
            GetSimulationResultsAsync(boardSize, SolutionMode);

        #endregion ISolverInterface

        public ISimulationResults GetResults(SolutionMode solutionMode)
        {
            var stopwatch = Stopwatch.StartNew();
            var solutions = MainSolve(solutionMode).ToList();
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

        //public int NoOfSolutions => ObservableSolutions.Count;

        public int NoOfSolutions => Solutions.Count;

        public sbyte HalfSize { get; set; }

        public sbyte[] QueenList { get; set; }

        public string ProgressLabel
        {
            get => _progressLabel;
            set => Set(ref _progressLabel, value);
        }

        #endregion PublicProperties

        protected virtual void OnQueenPlaced(sbyte[] e)
        {
            QueenPlaced?.Invoke(this, e);
        }

        protected virtual void OnSolutionFound(sbyte[] e)
        {
            SolutionFound?.Invoke(this, e);
        }

        #region PrivateMethods
        private void Initialize(sbyte boardSize, DisplayMode displayMode)
        {
            BoardSize = boardSize;
            HalfSize = (sbyte)(BoardSize % 2 == 0 ?
                BoardSize / 2 :
                BoardSize / 2 + 1);

            QueenList = Enumerable.Repeat((sbyte)-1, BoardSize).ToArray();
            DisplayMode = displayMode;

            Solutions = new HashSet<sbyte[]>(new SequenceEquality<sbyte>());
            var solutionSize = Utility.FindSolutionSize(BoardSize, SolutionMode);
            ObservableSolutions = new ObservableCollection<Solution>(new List<Solution>(solutionSize));
            CancelSolver = false;
        }

        private bool UpdateSols(IEnumerable<sbyte> solution, SolutionMode solutionMode)
        {
            var queens = solution.ToArray();

            // If solutionMode == SolutionMode.Single, then we are done.
            if (solutionMode == SolutionMode.Single)
            {
                Solutions.Add(queens);
                return true;
            }

            List<sbyte[]> symmSols = Utility.GetSymmSols(queens).ToList();

            // If solutionMode == SolutionMode.All, add this solution and all of the symmetrical counterparts to All Solutions.
            if (solutionMode == SolutionMode.All)
            {
                Solutions.Add(queens);
                symmSols.ForEach(s => Solutions.Add(s));

                return true;
            }

            // One of symmetrical solutions is already in the solutions list, nothing to add.
            if (Solutions.Overlaps(symmSols))
            { return false; }

            // None of the symmetrical solutions exists in the solutions list, add the new solution to the Unique Solutions.
            Solutions.Add(queens);
            return true;
        }

        private IEnumerable<Solution> MainSolve(SolutionMode mode)
        {
            // Recursive call to start the simulation
            SolveRec(0, mode);

            return Solutions
                    .Select((s, index) => new Solution(s, index + 1));
        }

        private bool SolveRec(sbyte colNo, SolutionMode mode)
        {
            if (CancelSolver)
            { return false; }

            if (DisplayMode == DisplayMode.Visualize)
            {
                OnQueenPlaced(QueenList);
                Thread.Sleep(DelayInMilliseconds);
            }

            if (mode == SolutionMode.Single && Solutions.Count == 1)
            { return true; }

            if (colNo == -1)
            { return false; }

            // Here a new solution is found.
            if (colNo == BoardSize)
            {
                bool isUpdated = UpdateSols(QueenList, mode);

                // Activate this code in case of IsVisulaized == true.
                if (isUpdated && DisplayMode == DisplayMode.Visualize)
                { SolutionFound(this, QueenList); }

                ProgressValue = Math.Round(100.0 * QueenList[0] / BoardSize);
                return false;
            }

            QueenList[colNo] = LocateQueen(colNo, mode);
            if (QueenList[colNo] == -1)
            {
                return false;
            }

            return SolveRec((sbyte)(colNo + 1), mode) || SolveRec(colNo, mode);
        }

        // Locate Queen
        private sbyte LocateQueen(sbyte colNo, SolutionMode mode)
        {
            bool isHalfSizeReachedMultSol = colNo == HalfSize && Solutions.Count > 0 &&
                Array.IndexOf<sbyte>(QueenList, 0, 0, HalfSize) == -1 && mode != SolutionMode.Single;

            if (isHalfSizeReachedMultSol)
            { return -1; }

            for (sbyte pos = (sbyte)(QueenList[colNo] + 1); pos < BoardSize; pos++)
            {
                bool isValid = true;
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