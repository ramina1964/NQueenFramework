using NQueen.Shared.Enum;
using NQueen.Shared.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NQueen.Shared.Utility
{
    public static class Utility
    {
        public static IEnumerable<sbyte[]> GetSymmSols(IReadOnlyList<sbyte> solution)
        {
            sbyte boardSize = (sbyte)solution.Count;
            sbyte[] symmToMidHorizontal = new sbyte[boardSize];
            sbyte[] symmToMidVertical = new sbyte[boardSize];
            sbyte[] symmToMainDiag = new sbyte[boardSize];
            sbyte[] symmToBiDiag = new sbyte[boardSize];
            sbyte[] rotCounter90 = new sbyte[boardSize];
            sbyte[] rotCounter180 = new sbyte[boardSize];
            sbyte[] rotCounter270 = new sbyte[boardSize];

            for (sbyte j = 0; j < boardSize; j++)
            {
                sbyte index1 = (sbyte)(boardSize - j - 1);
                sbyte index2 = (sbyte)(boardSize - solution[j] - 1);

                symmToMidHorizontal[index1] = solution[j];
                rotCounter90[index2] = symmToMainDiag[solution[j]] = j;
                rotCounter180[index1] = symmToMidVertical[j] = index2;
                rotCounter270[solution[j]] = symmToBiDiag[index2] = index1;
            }

            return new HashSet<sbyte[]>(new SequenceEquality<sbyte>())
            {
                symmToMidVertical,
                symmToMidHorizontal,
                symmToMainDiag,
                symmToBiDiag,
                rotCounter90,
                rotCounter180,
                rotCounter270,
            };
        }

        public static List<sbyte[]> GetSymmSols(List<sbyte[]> solution)
        {
            var list = solution
                       .SelectMany(s => GetSymmSols(s)).ToList();

            return list;
        }

        public static int FindSolutionSize(sbyte boardSize, SolutionMode solutionMode) =>
            (solutionMode == SolutionMode.Single)
                ? 1
                : (solutionMode == SolutionMode.Unique)
                ? FindSolutionSizeUnique(boardSize)
                : FindSolutionSizeAll(boardSize);

        public static string SolutionTitle(SolutionMode solutionMode)
        {
            switch (solutionMode)
            {
                case SolutionMode.Single:
                    return "No. of Solutions";

                case SolutionMode.Unique:
                    return $"No. of Unique Solutions";

                case SolutionMode.All:
                    return $"No. of All Solutions";

                default:
                    throw new MissingFieldException("Non-Existent Enum Value!");
            }
        }

        public static string SolutionTitle(SolutionMode solutionMode, int noOfSolutions)
        {
            if (solutionMode == SolutionMode.Single)
            { return "Solution:"; }

            if (noOfSolutions <= MaxNoOfSolutionsInOutput)
            {
                return (solutionMode == SolutionMode.All)
                 ? $"List of All Solution(s), Included Symmetrical Ones:"
                 : $"List of Unique Solution(s), Excluded Symmetrical Ones:";
            }

            // Here is: NoOfSolutions > MaxNoOfSolutionsInOutput
            return (solutionMode == SolutionMode.All)
                ? $"List of First {MaxNoOfSolutionsInOutput} Solution(s), May Include Symmetrical Ones:"
                : $"List of First {MaxNoOfSolutionsInOutput} Unique Solution(s), Excluded Symmetrical Ones:";
        }

        public static int MaxNoOfSolutionsInOutput = Settings.Default.MaxNoOfSolutionsInOutput;

        private static int FindSolutionSizeUnique(sbyte boardSize)
        {
            switch (boardSize)
            {
                case 1:
                    return 1;
                case 2:
                    return 1;
                case 3:
                    return 1;
                case 4:
                    return 1;
                case 5:
                    return 2;
                case 6:
                    return 1;
                case 7:
                    return 6;
                case 8:
                    return 12;
                case 9:
                    return 46;
                case 10:
                    return 92;
                case 11:
                    return 341;
                case 12:
                    return 1787;
                case 13:
                    return 9233;
                case 14:
                    return 45752;
                case 15:
                    return 285053;
                case 16:
                    return 1846955;
                case 17:
                    return 11977939;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static int FindSolutionSizeAll(sbyte boardSize)
        {
            switch (boardSize)
            {
                case 1:
                    return 1;
                case 2:
                    return 1;
                case 3:
                    return 1;
                case 4:
                    return 2;
                case 5:
                    return 10;
                case 6:
                    return 4;
                case 7:
                    return 40;
                case 8:
                    return 92;
                case 9:
                    return 352;
                case 10:
                    return 724;
                case 11:
                    return 2680;
                case 12:
                    return 14200;
                case 13:
                    return 73712;
                case 14:
                    return 365596;
                case 15:
                    return 2279184;
                case 16:
                    return 14772512;
                case 17:
                    return 95815104;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}