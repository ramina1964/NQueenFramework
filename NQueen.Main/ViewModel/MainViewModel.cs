﻿using GalaSoft.MvvmLight;
using NQueen.Model;

namespace NQueen.Main.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(ISolver solver) => SolverViewModel = new SolverViewModel(solver);

        public SolverViewModel SolverViewModel { get; }
    }
}