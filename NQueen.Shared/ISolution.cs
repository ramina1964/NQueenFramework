﻿namespace NQueen.Shared
{
    public interface ISolution
    {
        string Details { get; set; }

        int? Id { get; }

        string Name { get; set; }

        sbyte[] QueenList { get; }

        string ToString();
    }
}