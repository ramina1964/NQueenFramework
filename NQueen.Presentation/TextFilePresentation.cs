using NQueen.Shared;
using NQueen.Shared.Enum;
using NQueen.Shared.Interfaces;
using NQueen.Shared.Properties;
using NQueen.Shared.Utility;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace NQueen.Presentation
{
    public class TextFilePresentation
    {
        #region Constructor
        public TextFilePresentation(ISimulationResults results)
        {
            MaxNoOfSolutionsInOutput = Settings.Default.MaxNoOfSolutionsInOutput;
            BoardSize = results.BoardSize;
            Solutions = new ObservableCollection<Solution>(results.Solutions);
            NoOfSolutions = Solutions.Count;
            ElapsedTimeInSec = results.ElapsedTimeInSec;
            NoOfSolutionsInOutput = Math.Min(NoOfSolutions, MaxNoOfSolutionsInOutput);
        }
        #endregion Constructor

        #region PublicMembers
        public ObservableCollection<Solution> Solutions { get; }

        public double ElapsedTimeInSec { get; set; }

        public int NoOfSolutions { get; }

        public int MaxNoOfSolutionsInOutput { get; }

        public sbyte BoardSize { get; set; }

        public int NoOfSolutionsInOutput { get; }

        public StringBuilder FormatSingleSolution(Solution solution) => new StringBuilder().Append($"{solution.Details}");

        public string Write2File(SolutionMode solutionMode)
        {
            string topFolder = Environment.CurrentDirectory;
            string subFolder = Path.Combine(topFolder, "Results");
            string fileName = $"Board Size - {BoardSize}" + ".txt";

            DirectoryInfo dirInfo = Directory.Exists(subFolder)
                ? new DirectoryInfo(subFolder)
                : Directory.CreateDirectory(subFolder);

            string filePath = Path.Combine(dirInfo.Name, fileName);
            File.WriteAllText(filePath, PrintFinalResults(solutionMode));
            return filePath;
        }

        public StringBuilder FormatSolutionTitle(SolutionMode solutionMode)
        {
            const string prefix = "Sol. No.";
            const int prefixLength = 8;
            const int counterPlaces = prefixLength - 28;

            string title = Utility.SolutionTitle(solutionMode, NoOfSolutions);
            StringBuilder sb = new StringBuilder().AppendLine(title);

            for (int index = 0; index < NoOfSolutionsInOutput; index++)
            {
                string solTitle = $"{prefix,0} {index + 1,counterPlaces}";
                sb.Append(solTitle).Append(FormatSingleSolution(Solutions[index]) + "\n");
            }

            return sb.AppendLine();
        }
        #endregion PublicMembers

        #region PrivateMembers
        private StringBuilder GetSummary(SolutionMode solutionMode)
        {
            const int placeHolderText = -28;
            const int decimalPlaces = 2;
            const int placeHolderInt = 20;

            StringBuilder sb = new StringBuilder().AppendLine()
                .AppendLine("Summary of Results:")
                .AppendLine($"{"Date and Time",placeHolderText}{DateTime.Now,placeHolderInt}");

            sb.AppendLine($"{"BoardSize",placeHolderText}{BoardSize,placeHolderInt}");
            if (NoOfSolutions > MaxNoOfSolutionsInOutput)
            { sb.AppendLine($"No. of Solutions Included{MaxNoOfSolutionsInOutput,placeHolderInt}"); }

            // Here is: NoOfSolutions <= MaxNoOfSolutionsInOutput
            else
            {
                sb.AppendLine(solutionMode == SolutionMode.All
                      ? $"{"No. of All Solutions",placeHolderText}{NoOfSolutions,placeHolderInt}"
                      : $"{"No. of Unique Solutions",placeHolderText}{NoOfSolutions,placeHolderInt}");
            }

            double timeInSec = Math.Round(ElapsedTimeInSec, decimalPlaces);
            sb.AppendLine($"{"Elapsed Time (s)",placeHolderText}{timeInSec,placeHolderInt}");

            return sb;
        }

        private string PrintFinalResults(SolutionMode solutionMode)
        {
            StringBuilder result = new StringBuilder()
                        .Append(FormatSolutionTitle(solutionMode))
                        .Append(GetSummary(solutionMode))
                        .AppendLine();

            return result.ToString();
        }
        #endregion PrivateMembers
    }
}