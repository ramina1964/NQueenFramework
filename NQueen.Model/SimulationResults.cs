﻿using NQueen.Shared;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NQueen.Model
{
    public class SimulationResults : ISimulationResults
    {
        public SimulationResults(IEnumerable<Solution> allSolutions)
        {
            Debug.Assert(allSolutions != null, "allSolutions != null");
            IList<Solution> enumerable = allSolutions as IList<Solution> ?? allSolutions.ToList();
            Solution sol = enumerable.FirstOrDefault();
            if (sol == null)
            {
                NoOfSolutions = 0;
                Solutions = new List<Solution>();
            }
            else
            {
                BoardSize = (sbyte)sol.Positions.Count;
                NoOfSolutions = enumerable.Count();
                Solutions = new List<Solution>(enumerable);
            }
        }

        #region ISimulationResult

        public sbyte BoardSize { get; set; }

        public IEnumerable<Solution> Solutions { get; set; }

        public int NoOfSolutions { get; set; }

        public double ElapsedTimeInSec { get; set; }
        
        #endregion ISimulationResult
    }
}