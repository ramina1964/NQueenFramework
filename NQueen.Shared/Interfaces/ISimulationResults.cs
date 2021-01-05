using NQueen.Shared;
using System.Collections.Generic;

namespace NQueen.Shared.Interfaces
{
    public interface ISimulationResults
    {
        sbyte BoardSize { get; set; }

        IEnumerable<Solution> Solutions { get; set; }

        int NoOfSolutions { get; set; }

        double ElapsedTimeInSec { get; set; }
    }
}