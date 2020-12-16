﻿using FluentValidation;
using FluentValidation.Results;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NQueen.Shared;
using NQueen.Shared.Enum;
using NQueen.Shared.Properties;
using NQueen.Model;
using NQueen.Presentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using NQueen.Shared.Utility;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace NQueen.Main.ViewModel
{
    public sealed class SolverViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Constructor
        public SolverViewModel(ISolver solver)
        {
            Initialize(solver);

            // Add event handlers to the invocation lists of the corresponding delegates
            Solver.QueenPlaced += Queens_QueenPlaced;
            Solver.SolutionFound += Queens_SolutionFound;
        }
        #endregion Constructor

        #region IDataErrorInfo
        public string this[string columnName]
        {
            get
            {
                var firstOrDefault = _validation.Validate(this).Errors.FirstOrDefault(lol => lol.PropertyName == columnName);
                if (firstOrDefault != null)
                { return _validation != null ? firstOrDefault.ErrorMessage : ""; }

                return string.Empty;
            }
        }

        public string Error
        {
            get
            {
                var results = _validation?.Validate(this);
                if (results == null || !results.Errors.Any())
                    return string.Empty;

                string errors = string.Join(Environment.NewLine, results.Errors.Select(x => x.ErrorMessage).ToArray());
                return errors;
            }
        }
        #endregion IDataErrorInfo

        #region PublicProperties
        public RelayCommand SimulateCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public RelayCommand SaveCommand { get; set; }

        public int MaxNoOfSolutionsInOutput => Settings.Default.MaxNoOfSolutionsInOutput;

        public IEnumerable<SolutionMode> EnumSolutionToItem
        {
            get => Enum.GetValues(typeof(SolutionMode)).Cast<SolutionMode>();
            set => Set(ref _enumSolutionToItem, value);
        }

        public IEnumerable<DisplayMode> EnumDisplayToItem
        {
            get => Enum.GetValues(typeof(DisplayMode)).Cast<DisplayMode>();
            set => Set(ref _enumDisplayToItem, value);
        }

        public bool IsVisualized
        {
            get => _isVisualized;
            set => Set(ref _isVisualized, value);
        }

        public int DelayInMilliseconds
        {
            get => _delayInMilliseconds;
            set
            {
                Set(ref _delayInMilliseconds, value);
                Solver.DelayInMilliseconds = value;
            }
        }

        public ISimulationResults SimulationResults
        {
            get => _simulationResults;
            set => Set(ref _simulationResults, value);
        }

        public ObservableCollection<Solution> Solutions { get; set; }

        public Solution SelectedSolution
        {
            get => _selectedSolution;
            set
            {
                Set(ref _selectedSolution, value);
                if (value != null)
                { Chessboard.PlaceQueens(_selectedSolution.Positions); }
            }
        }

        public SolutionMode SolutionMode
        {
            get => _solutionMode;
            set
            {
                bool isChanged = Set(ref _solutionMode, value);
                if (!isChanged)
                { return; }

                Solver.SolutionMode = value;
                SolutionTitle =
                    (SolutionMode == SolutionMode.Single) ? $"Solution" :
                    $"Solutions (Max: {MaxNoOfSolutionsInOutput})";

                RaisePropertyChanged(nameof(BoardSizeText));
                RaisePropertyChanged(nameof(SolutionTitle));
                ValidationResult = _validation.Validate(this);
                IsValid = ValidationResult.IsValid;

                if (IsValid)
                {
                    IsResultsReady = false;
                    SimulateCommand?.RaiseCanExecuteChanged();
                    SaveCommand?.RaiseCanExecuteChanged();
                    UpdateGui();
                }
            }
        }

        public DisplayMode DisplayMode
        {
            get => _displayMode;
            set
            {
                bool isChanged = Set(ref _displayMode, value);
                if (!isChanged)
                { return; }

                if (Solver != null)
                { Solver.DisplayMode = value; }

                ValidationResult = _validation.Validate(this);
                IsValid = ValidationResult.IsValid;

                if (IsValid)
                {
                    IsResultsReady = false;
                    IsVisualized = DisplayMode.Visualize == value;
                    RaisePropertyChanged(nameof(BoardSizeText));
                    SimulateCommand?.RaiseCanExecuteChanged();
                    SaveCommand?.RaiseCanExecuteChanged();
                    UpdateGui();
                }
            }
        }

        public string BoardSizeText
        {
            get => _boardSizeText;
            set
            {
                bool isChanged = Set(ref _boardSizeText, value);
                if (!isChanged)
                { return; }

                ValidationResult = _validation.Validate(this);
                IsValid = ValidationResult.IsValid;

                if (IsValid)
                {
                    IsResultsReady = false;
                    Set(ref _boardSize, sbyte.Parse(_boardSizeText));
                    RaisePropertyChanged(nameof(BoardSize));
                    SaveCommand?.RaiseCanExecuteChanged();
                    SimulateCommand?.RaiseCanExecuteChanged();
                    UpdateGui();
                }
            }
        }

        public sbyte BoardSize
        {
            get => _boardSize;
            set => Set(ref _boardSize, value);
        }

        public ValidationResult ValidationResult { get; set; }

        public string ResultTitle => Utility.SolutionTitle(SolutionMode);

        public bool IsValid
        {
            get => _isValid;
            set => Set(ref _isValid, value);
        }

        public ISolver Solver
        {
            get => _solver;
            set => Set(ref _solver, value);
        }

        public string SolutionTitle
        {
            get => _solutionTitle;
            set => Set(ref _solutionTitle, value);
        }

        public string NoOfSolutions
        {
            get => _noOfSoltions;
            set
            {
                if (Set(ref _noOfSoltions, value))
                { RaisePropertyChanged(nameof(ResultTitle)); }
            }
        }

        public Chessboard Chessboard { get; set; }

        public void SetChessboard(double boardDimension)
        {
            BoardSizeText = "8";
            Chessboard = new Chessboard { WindowWidth = boardDimension, WindowHeight = boardDimension };
            Chessboard.CreateSquares(BoardSize, new List<SquareViewModel>());

            CanEditBoardSize = true;
            CanEditSolutionMode = true;
            IsSingleRunning = false;
            IsMultipleRunning = false;
            SolutionMode = SolutionMode.Unique;
            DisplayMode = DisplayMode.Hide;
        }

        public string ElapsedTimeInSec
        {
            get => _elapsedTime;
            set => Set(ref _elapsedTime, value);
        }

        public bool IsSingleRunning
        {
            get => _isSingleRunning;
            set
            {
                bool isChanged = Set(ref _isSingleRunning, value);
                if (isChanged)
                {
                    CancelCommand.RaiseCanExecuteChanged();
                    SimulateCommand.RaiseCanExecuteChanged();
                    SaveCommand.RaiseCanExecuteChanged();
                }

                // Also set value of IsFinished Property as well as notify all listeners.
                Set(nameof(IsSimFinished), ref _isFinished, !value, true);
            }
        }

        public bool IsMultipleRunning
        {
            get => _isMultipleRunning;
            set
            {
                bool isChanged = Set(ref _isMultipleRunning, value);
                if (isChanged)
                {
                    CancelCommand.RaiseCanExecuteChanged();
                    SimulateCommand.RaiseCanExecuteChanged();
                    SaveCommand.RaiseCanExecuteChanged();
                }

                // Also set value of IsFinished Property as well as notify all listeners.
                Set(nameof(IsSimFinished), ref _isFinished, !value, true);
            }
        }

        public bool IsSimFinished
        {
            get => _isFinished;
            set => Set(ref _isFinished, value);
        }

        public bool CanEditBoardSize
        {
            get => _canEditBoardSize;
            set => Set(ref _canEditBoardSize, value);
        }

        public bool CanEditSolutionMode
        {
            get => _canEditSolutionMode;
            set => Set(ref _canEditSolutionMode, value);
        }

        public bool IsResultsReady
        {
            get => _isCalculated;
            set => Set(ref _isCalculated, value);
        }
        #endregion PublicProperties

        #region PrivateMethods
        private void Initialize(ISolver solver)
        {
            _validation = new InputViewModel { CascadeMode = CascadeMode.Stop };

            SimulateCommand = new RelayCommand(SimulateAsync, CanSimulate);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
            SaveCommand = new RelayCommand(Save, CanSave);

            Solver = solver;
            IsSingleRunning = false;
            IsMultipleRunning = false;
            Solver.ProgressVisibility = Visibility.Collapsed;
            Solver.ProgressValue = 0;
            Solutions = Solver.Solutions;
            NoOfSolutions = $"{Solutions.Count,0:N0}";
            DelayInMilliseconds = Settings.Default.DefaultDelayInMilliseconds;
        }

        private void UpdateGui()
        {
            Solver.Solutions.Clear();
            BoardSize = sbyte.Parse(BoardSizeText);
            NoOfSolutions = "0";
            ElapsedTimeInSec = $"{0,0:N1}";
            RaisePropertyChanged(nameof(ResultTitle));

            Solutions?.Clear();
            Chessboard?.Squares.Clear();
            Chessboard?.CreateSquares(BoardSize, new List<SquareViewModel>());
        }

        private void Queens_SolutionFound(object sender, sbyte[] e)
        {
            var id = Solutions.Count + 1;
            var sol = new Solution(e, id);

            Application
                .Current
                .Dispatcher
                .BeginInvoke(DispatcherPriority.Background, new Action(() => Solutions.Add(sol)));

            SelectedSolution = sol;
        }

        private void Queens_QueenPlaced(object sender, sbyte[] e)
        {
            var sol = new Solution(e, 1);
            var positions = sol
                            .QueenList.Where(q => q > -1)
                            .Select((item, index) => new Position((sbyte)index, item)).ToList();

            Chessboard.PlaceQueens(positions);
        }

        private async void SimulateAsync()
        {
            UpdateSummary();

            UpdateGui();
            SimulationResults = await Solver
                                .GetSimulationResultsAsync(BoardSize);

            ExtractCorrectNoOfSols();
            UpdateSummary();

            IsResultsReady = true;
            SaveCommand.RaiseCanExecuteChanged();

            NoOfSolutions = $"{SimulationResults.NoOfSolutions,0:N0}";
            ElapsedTimeInSec = $"{SimulationResults.ElapsedTimeInSec,0:N1}";
            SelectedSolution = Solutions.FirstOrDefault();
            Solver.ProgressVisibility = Visibility.Collapsed;
        }

        private void UpdateSummary()
        {
            // Before simulation
            if (!IsSingleRunning && !IsMultipleRunning)
            {
                if (SolutionMode == SolutionMode.Single)
                { IsSingleRunning = true; }

                else
                {
                    IsMultipleRunning = true;
                    Solver.ProgressValue = 0;
                    Solver.ProgressVisibility = Visibility.Visible;
                }

                CanEditBoardSize = false;
                CanEditSolutionMode = false;
                Solver.CancelSolver = false;

                return;
            }

            // After Simulation
            if (SolutionMode == SolutionMode.Single)
            { IsSingleRunning = false; }

            else
            {
                IsMultipleRunning = false;
                Solver.ProgressValue = 0;
                Solver.ProgressVisibility = Visibility.Collapsed;
            }

            CanEditBoardSize = true;
            CanEditSolutionMode = true;
        }

        private void ExtractCorrectNoOfSols()
        {
            var sols = SimulationResults
                        .Solutions
                        .Take(MaxNoOfSolutionsInOutput)
                        .ToList();

            // In case of activated visualization, clear all solutions before adding a no. of MaxNoOfSolutionsInOutput to the solutions.
            if (DisplayMode == DisplayMode.Visualize)
            {
                Solutions.Clear();

                sols 
                .ForEach(sol => Solutions.Add(sol));
            }

            // In case of activated visualization, just add a no. of MaxNoOfSolutionsInOutput to the solutions.
            else
            {
                sols.ForEach(sol => Solutions.Add(sol));
            }
        }

        private bool CanSimulate => IsValid && !IsSingleRunning && !IsMultipleRunning;

        private void Cancel() => Solver.CancelSolver = true;

        private bool CanCancel() => IsSingleRunning || IsMultipleRunning;

        private void Save()
        {
            TextFilePresentation results = new TextFilePresentation(SimulationResults);
            string filePath = results.Write2File(SolutionMode);

            string caption = Resources.TitleSaveResultsMessage;
            string msg = string.Format(Resources.SaveResultsMessage, filePath);
            const MessageBoxButton button = MessageBoxButton.OK;
            const MessageBoxImage icon = MessageBoxImage.Information;
            MessageBox.Show(msg, caption, button, icon);

            IsResultsReady = false;
            SaveCommand.RaiseCanExecuteChanged();
        }

        private bool CanSave() =>
            !IsSingleRunning && !IsMultipleRunning && IsResultsReady && SimulationResults?.NoOfSolutions > 0;

        #endregion PrivateMethods

        #region PrivateFields
        private IEnumerable<SolutionMode> _enumSolutionToItem;
        private IEnumerable<DisplayMode> _enumDisplayToItem;
        private InputViewModel _validation;
        private static ISimulationResults _simulationResults;
        private int _delayInMilliseconds;
        private string _noOfSoltions;
        private string _elapsedTime;
        private SolutionMode _solutionMode;
        private DisplayMode _displayMode;
        private string _boardSizeText;
        private sbyte _boardSize;
        private bool _isVisualized;
        private bool _isValid;
        private bool _isFinished;
        private bool _isSingleRunning;
        private bool _isMultipleRunning;
        private bool _isCalculated;
        private ISolver _solver;
        private Solution _selectedSolution;
        private bool _canEditBoardSize;
        private bool _canEditSolutionMode;
        private string _solutionTitle;
        #endregion PrivateFields
    }
}