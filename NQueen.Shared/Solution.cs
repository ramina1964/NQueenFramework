﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NQueen.Shared
{
    public class Solution : ISolution
    {
        #region Constructor
        public Solution(sbyte[] queenList, int? id = null)
        {
            BoardSize = queenList.Length;
            Id = id;
            Name = ToString();
            QueenList = queenList.ToArray();
            Positions = SetPositions(QueenList);
            Details = GetDetails();
        }
        #endregion Constructor

        #region PublicProperties
        public List<Position> Positions;

        public int? Id { get; }

        public string Name { get; set; }

        public string Details { get; set; }
        public sealed override string ToString()
        {
            return $"No. {Id}";
        }

        public sbyte[] QueenList { get; }
        #endregion PublicProperties

        #region PrivateMembers
        private int BoardSize { get; }
        private string GetDetails()
        {
            const int noOfQueensPerLine = 40;
            int noOfLines = (BoardSize % noOfQueensPerLine == 0) ?
                BoardSize / noOfQueensPerLine :
                BoardSize / noOfQueensPerLine + 1;

            StringBuilder sb = new StringBuilder();
            for (int lineNo = 0; lineNo < noOfLines; lineNo++)
            {
                int maxQueensInLastLine = (lineNo < noOfLines - 1 || BoardSize % noOfQueensPerLine == 0) ?
                    noOfQueensPerLine :
                    Math.Min(BoardSize % noOfQueensPerLine, noOfQueensPerLine);

                for (int posInLine = 0; posInLine < maxQueensInLastLine; posInLine++)
                {
                    int posNo = noOfQueensPerLine * lineNo + posInLine;
                    sb.Append($"({Positions[posNo].RowNo + 1,0:N0}, {Positions[posNo].ColumnNo + 1,0:N0})");

                    if (posNo < BoardSize - 1)
                        sb.Append(", ");
                }

                if (lineNo < noOfLines - 1)
                { sb.AppendLine(); }
            }

            return sb.ToString();
        }

        private List<Position> SetPositions(IEnumerable<sbyte> queenList)
        {
            return queenList.Select((item, index) =>
                new Position((sbyte)index, item)).ToList();
        }
        #endregion PrivateMembers
    }
}