using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class NegaMax
    {
        private const int NumberOfSubBoards = 9;
        private const int NumberOfLinesInSubBoard = 4;
        private enum Direction { Vertical, Horizontal, Diagonal, ReverseDiagonal }
        private readonly Cell[,] _cells;
        private readonly int[] _sign = { 1, -1 };//0 is c, 1 is h

        public NegaMax(Cell[,] cells)
        {
            _cells = cells;
        }
        public int[] GetBestMove(int depth)
        {
            var result = Negamax(depth, 0, int.MinValue + 1, int.MaxValue - 1);
            return result;// bestscore, row, col
        }

        private int[] Negamax(int depth, int color, int alpha, int beta)
        {
            // Generate possible next moves in a List of int[2] of {row, col}.
            List<Index> nextMoves;
            GenerateMoves(out nextMoves);

            var maxScore = int.MinValue + 1;
            var bestRow = -1;
            var bestCol = -1;
            var returnAlpha = false;

            if (nextMoves.Count == 0 || depth == 0)
            {
                return new[] { _sign[color] * EvaluateBoard(), bestRow, bestCol };
            }

            foreach (var move in nextMoves)
            {
                if (_cells[move.Row, move.Column].MyCellType == CellType.Empty)
                {
                    // Make Move
                    _cells[move.Row, move.Column].MyCellType = color == 0 ? CellType.Computer : CellType.Human;
                    var score = -Negamax(depth - 1, 1 - color, -beta, -alpha)[0];
                    if (score > maxScore)
                    {
                        maxScore = score;
                        bestRow = move.Row;
                        bestCol = move.Column;
                    }

                    if (score > alpha)
                    {
                        alpha = score;
                    }

                    // Undo Move
                    _cells[move.Row, move.Column].MyCellType = CellType.Empty;

                    // cut-off
                    if (alpha >= beta)
                    {
                        returnAlpha = true;
                        break;
                    }
                }
            }

            return new[] { returnAlpha ? alpha : maxScore, bestRow, bestCol };
        }

        private void GenerateMoves(out List<Index> nextMoves)
        {
            nextMoves = new List<Index>();

            for (var row = 0; row < 6; ++row)
            {
                for (var column = 0; column < 6; ++column)
                {
                    if (_cells[row, column].MyCellType == CellType.Empty)
                    {
                        nextMoves.Add(new Index { Row = row, Column = column });
                    }
                }
            }
        }

        private int EvaluateBoard()
        {
            var score = 0;
            for (int i = 0, startRow = 0, startColumn = 0; i < NumberOfSubBoards; i++, startColumn++)
            {
                if (i != 0 && i % 3 == 0)
                {
                    startRow++;
                    startColumn = 0;
                }
                score += EvaluateSubBoard(startRow, startColumn);
            }
            return score;
        }

        private int EvaluateSubBoard(int startRow, int startColumn)
        {
            var score = 0;

            for (var i = 0; i < NumberOfLinesInSubBoard; i++)
            {
                score += EvaluateLine(startRow, startColumn + i, Direction.Vertical);
                score += EvaluateLine(startRow + i, startColumn, Direction.Horizontal);
            }

            score += EvaluateLine(startRow, startColumn, Direction.Diagonal);
            score += EvaluateLine(startRow, startColumn + 3, Direction.ReverseDiagonal);

            return score;
        }

        private int EvaluateLine(int startRow, int startColumn, Direction direction)
        {
            int r1, c1, r2, c2, r3, c3;
            r1 = c1 = r2 = c2 = r3 = c3 = -1;
            switch (direction)
            {
                case Direction.Vertical:
                    r1 = startRow + 1;
                    r2 = startRow + 2;
                    r3 = startRow + 3;
                    c1 = c2 = c3 = startColumn;
                    break;
                case Direction.Horizontal:
                    r1 = r2 = r3 = startRow;
                    c1 = startColumn + 1;
                    c2 = startColumn + 2;
                    c3 = startColumn + 3;
                    break;
                case Direction.Diagonal:
                    r1 = startRow + 1;
                    r2 = startRow + 2;
                    r3 = startRow + 3;
                    c1 = startColumn + 1;
                    c2 = startColumn + 2;
                    c3 = startColumn + 3;
                    break;
                case Direction.ReverseDiagonal:
                    r1 = startRow + 1;
                    r2 = startRow + 2;
                    r3 = startRow + 3;
                    c1 = startColumn - 1;
                    c2 = startColumn - 2;
                    c3 = startColumn - 3;
                    break;
                default:
                    Debug.LogError("This should not happen");
                    break;
            }
            /**
             * Bad if human scores; Good if computer scores; Neutal if empty
             */

            // Assuming cell 1 empty
            var score = 0;

            // Cell 1
            if (_cells[startRow, startColumn].MyCellType == CellType.Human)
            {
                score = -1;
            }
            else if (_cells[startRow, startColumn].MyCellType == CellType.Computer)
            {
                score = 1;
            }

            // Todo: write a for loop

            // Cell 2
            if (_cells[r1, c1].MyCellType == CellType.Human)
            {
                // If cell 1 is human
                if (score < 0)
                {
                    score *= 10;
                }
                // If cell 1 is computer
                else if (score > 0)
                {
                    return 0;
                }
                // If cell 1 is empty
                else
                {
                    score = -1;
                }
            }
            else if (_cells[r1, c1].MyCellType == CellType.Computer)
            {
                // If cell 1 is human
                if (score < 0)
                {
                    return 0;
                }
                // If cell 1 is computer
                else if (score > 0)
                {
                    score *= 10;
                }
                // If cell 1 is empty
                else
                {
                    score = 1;
                }
            }

            // Cell 3
            if (_cells[r2, c2].MyCellType == CellType.Human)
            {
                // If cell 1 is human and cell 2 is human or empty
                if (score < 0)
                {
                    score *= 10;
                }
                // If cell 1 is computer and cell 2 is computer or empty
                else if (score > 0)
                {
                    return 0;
                }
                // If cell 1 is empty and cell 2 is empty
                else
                {
                    score = -1;
                }
            }
            else if (_cells[r2, c2].MyCellType == CellType.Computer)
            {
                // If cell 1 is human and cell 2 is human or empty
                if (score < 0)
                {
                    return 0;
                }
                // If cell 1 is computer and cell 2 is computer or empty
                else if (score > 0)
                {
                    score *= 10;
                }
                // If cell 1 is empty and cell 2 is empty
                else
                {
                    score = 1;
                }
            }

            // Cell 4
            if (_cells[r3, c3].MyCellType == CellType.Human)
            {
                // If cell 1 is human and cell 2 is human or empty and cell 3 is human or empty
                if (score < 0)
                {
                    score *= 10;
                }
                // If cell 1 is computer and cell 2 is computer or empty and cell 3 is computer or empty
                else if (score > 1)
                {
                    return 0;
                }
                // If cell 1 is empty and cell 2 is empty and cell 3 is empty
                else
                {
                    score = -1;
                }
            }
            else if (_cells[r3, c3].MyCellType == CellType.Computer)
            {
                // If cell 1 is human and cell 2 is human or empty and cell 3 is human or empty
                if (score < 0)
                {
                    return 0;
                }
                // If cell 1 is computer and cell 2 is computer or empty and cell 3 is computer or empty
                else if (score > 1)
                {
                    score *= 10;
                }
                // If cell 1 is empty and cell 2 is empty and cell 3 is empty
                else
                {
                    score = 1;
                }
            }

            return score;
        }
    }

    internal struct Index
    {
        internal int Row;
        internal int Column;
    }
}
