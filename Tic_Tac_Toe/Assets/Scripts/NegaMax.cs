using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class NegaMax
    {
        private const int NumberOfSubBoards = 9;
        private const int NumberOfLinesInSubBoard = 4;
        private const int NumberOfOtherCells = 3;
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
            return result;
        }

        private int[] Negamax(int depth, int color, int alpha, int beta)
        {
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
            var index = new Index[NumberOfOtherCells];
            /*int r1, c1, r2, c2, r3, c3;
            r1 = c1 = r2 = c2 = r3 = c3 = -1;*/
            switch (direction)
            {
                case Direction.Vertical:
                    index[0] = new Index { Row = startRow + 1, Column = startColumn };
                    index[1] = new Index { Row = startRow + 2, Column = startColumn };
                    index[2] = new Index { Row = startRow + 3, Column = startColumn };
                    break;
                case Direction.Horizontal:
                    index[0] = new Index { Row = startRow, Column = startColumn + 1 };
                    index[1] = new Index { Row = startRow, Column = startColumn + 2 };
                    index[2] = new Index { Row = startRow, Column = startColumn + 3 };
                    break;
                case Direction.Diagonal:
                    index[0] = new Index { Row = startRow + 1, Column = startColumn + 1 };
                    index[1] = new Index { Row = startRow + 2, Column = startColumn + 2 };
                    index[2] = new Index { Row = startRow + 3, Column = startColumn + 3 };
                    break;
                case Direction.ReverseDiagonal:
                    index[0] = new Index { Row = startRow + 1, Column = startColumn - 1 };
                    index[1] = new Index { Row = startRow + 2, Column = startColumn - 2 };
                    index[2] = new Index { Row = startRow + 3, Column = startColumn - 3 };
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

            for (var i = 0; i < NumberOfOtherCells; i++)
            {
                if (_cells[index[i].Row, index[i].Column].MyCellType == CellType.Human)
                {
                    // If cell 1 is human and previous cell is either human or empty
                    if (score < 0)
                    {
                        score *= 10;
                    }
                    // If cell 1 is computer and previous cell is either computer or empty
                    else if (score > 0)
                    {
                        return 0;
                    }
                    // If previous all cells are empty
                    else
                    {
                        score = -1;
                    }
                }
                else if (_cells[index[i].Row, index[i].Column].MyCellType == CellType.Computer)
                {
                    // If cell 1 is human and previous cell is either human or empty
                    if (score < 0)
                    {
                        return 0;
                    }
                    // If cell 1 is computer and previous cell is either computer or empty
                    else if (score > 0)
                    {
                        score *= 10;
                    }
                    // If previous all cells are empty
                    else
                    {
                        score = 1;
                    }
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
