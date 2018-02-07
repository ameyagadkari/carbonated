using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts
{
    public class GridGenerator : MonoBehaviour
    {
        private const int MaxDepth = 4;
        private const int Numberofrows = 6;
        private const int Numberofcolumns = Numberofrows;
        private const int TotalNumberOfMoves = Numberofrows * Numberofcolumns;
        private GameObject _cellPrefab;
        private Cell[,] _cells;
        private NegaMax _negaMax;

        private void Awake()
        {
            _cellPrefab = Resources.Load<GameObject>("Prefabs/Cell");
            Assert.IsNotNull(_cellPrefab, "Cell prefab not found");
            _cells = new Cell[Numberofrows, Numberofcolumns];
            Manager.Reset += () =>
            {
                for (var row = 0; row < Numberofrows; row++)
                {
                    for (var column = 0; column < Numberofcolumns; column++)
                    {
                        if (_cells[row, column].Reset != null)
                        {
                            _cells[row, column].Reset();
                        }
                    }
                }
            };
            Manager.Toggle += CalculateScore;
            Manager.Toggle += ComputerPlays;
            _negaMax = new NegaMax(_cells);
        }

        private void Start()
        {
            for (var row = 0; row < Numberofrows; row++)
            {
                for (var column = 0; column < Numberofcolumns; column++)
                {
                    _cells[row, column] = Instantiate(_cellPrefab, transform).GetComponent<Cell>();
#if UNITY_EDITOR
                    _cells[row, column].gameObject.name = "Cell (" + row + "," + column + ")";
#endif
                    _cells[row, column].Setup(row, column);
                }
            }
        }

        private void ComputerPlays(CellType previousCellType, int row, int column)
        {
            if (previousCellType == CellType.Human && Manager.Gamestate == Gamestate.ComputerTurn)
            {
                if (Manager.NumberOfMovesDone == 0)
                {
                    var r = Random.Range(0, Numberofrows);
                    var c = Random.Range(0, Numberofcolumns);
                    _cells[r, c].MyCellType = CellType.Computer;
                    Manager.Toggle(CellType.Computer, r, c);
                }
                else
                {
                    StartCoroutine(BestMoveCoroutine());
                }
            }
        }

        private IEnumerator BestMoveCoroutine()
        {
            yield return new WaitForEndOfFrame(); // Required
            yield return new WaitForEndOfFrame(); // Better safe than sorry
            var result = _negaMax.GetBestMove(MaxDepth);
            //print("Score for move:" + result[0]);  
            _cells[result[1], result[2]].MyCellType = CellType.Computer;
            Manager.Toggle(CellType.Computer, result[1], result[2]);
        }

        private void CalculateScore(CellType previousCellType, int row, int column)
        {
            if (row == -1 || column == -1) return;
            if (Manager.NumberOfMovesDone < 7) return;
            var cellHintArray = Manager.Patterns[row, column];
            int r1, c1, r2, c2, r3, c3;
            r1 = c1 = r2 = c2 = r3 = c3 = -1;
            var result = cellHintArray.Sum(cellHint =>
            {
                switch (cellHint)
                {
                    case CellCheckHints.U3:
                        r1 = row + 1;
                        r2 = row + 2;
                        r3 = row + 3;
                        c1 = c2 = c3 = column;
                        break;
                    case CellCheckHints.D3:
                        r1 = row - 1;
                        r2 = row - 2;
                        r3 = row - 3;
                        c1 = c2 = c3 = column;
                        break;
                    case CellCheckHints.L3:
                        r1 = r2 = r3 = row;
                        c1 = column - 1;
                        c2 = column - 2;
                        c3 = column - 3;
                        break;
                    case CellCheckHints.R3:
                        r1 = r2 = r3 = row;
                        c1 = column + 1;
                        c2 = column + 2;
                        c3 = column + 3;
                        break;
                    case CellCheckHints.Ul3:
                        r1 = row + 1;
                        r2 = row + 2;
                        r3 = row + 3;
                        c1 = column - 1;
                        c2 = column - 2;
                        c3 = column - 3;
                        break;
                    case CellCheckHints.Ur3:
                        r1 = row + 1;
                        r2 = row + 2;
                        r3 = row + 3;
                        c1 = column + 1;
                        c2 = column + 2;
                        c3 = column + 3;
                        break;
                    case CellCheckHints.Dl3:
                        r1 = row - 1;
                        r2 = row - 2;
                        r3 = row - 3;
                        c1 = column - 1;
                        c2 = column - 2;
                        c3 = column - 3;
                        break;
                    case CellCheckHints.Dr3:
                        r1 = row - 1;
                        r2 = row - 2;
                        r3 = row - 3;
                        c1 = column + 1;
                        c2 = column + 2;
                        c3 = column + 3;
                        break;
                    case CellCheckHints.L1R2:
                        r1 = r2 = r3 = row;
                        c1 = column - 1;
                        c2 = column + 1;
                        c3 = column + 2;
                        break;
                    case CellCheckHints.R1L2:
                        r1 = r2 = r3 = row;
                        c1 = column + 1;
                        c2 = column - 1;
                        c3 = column - 2;
                        break;
                    case CellCheckHints.U1D2:
                        r1 = row + 1;
                        r2 = row - 1;
                        r3 = row - 2;
                        c1 = c2 = c3 = column;
                        break;
                    case CellCheckHints.D1U2:
                        r1 = row - 1;
                        r2 = row + 1;
                        r3 = row + 2;
                        c1 = c2 = c3 = column;
                        break;
                    case CellCheckHints.Ul1Dr2:
                        r1 = row + 1;
                        r2 = row - 1;
                        r3 = row - 2;
                        c1 = column - 1;
                        c2 = column + 1;
                        c3 = column + 2;
                        break;
                    case CellCheckHints.Dr1Ul2:
                        r1 = row - 1;
                        r2 = row + 1;
                        r3 = row + 2;
                        c1 = column + 1;
                        c2 = column - 1;
                        c3 = column - 2;
                        break;
                    case CellCheckHints.Ur1Dl2:
                        r1 = row + 1;
                        r2 = row - 1;
                        r3 = row - 2;
                        c1 = column + 1;
                        c2 = column - 1;
                        c3 = column - 2;
                        break;
                    case CellCheckHints.Dl1Ur2:
                        r1 = row - 1;
                        r2 = row + 1;
                        r3 = row + 2;
                        c1 = column - 1;
                        c2 = column + 1;
                        c3 = column + 2;
                        break;
                    default:
                        Debug.LogError("This should not happen");
                        break;
                }
                return (_cells[row, column].MyCellType & _cells[r1, c1].MyCellType & _cells[r2, c2].MyCellType & _cells[r3, c3].MyCellType) != 0 ? 1 : 0;
            });
            AssignScore(result, previousCellType);
            if (Manager.NumberOfMovesDone == TotalNumberOfMoves)
            {
                if (Manager.End != null)
                {
                    Manager.End();
                }
            }
        }

        private static void AssignScore(int result, CellType previousCellType)
        {
            if (Manager.IsHumanStarting)
            {
                if (previousCellType == CellType.Human)
                {
                    Manager.XScore += result;
                    if (CanvasManager.OnScoreChanged != null)
                    {
                        CanvasManager.OnScoreChanged.Invoke(Manager.XScore, true);
                    }
                }
                else
                {
                    Manager.OScore += result;
                    if (CanvasManager.OnScoreChanged != null)
                    {
                        CanvasManager.OnScoreChanged.Invoke(Manager.OScore, false);
                    }
                }
            }
            else
            {
                if (previousCellType == CellType.Human)
                {
                    Manager.OScore += result;
                    if (CanvasManager.OnScoreChanged != null)
                    {
                        CanvasManager.OnScoreChanged.Invoke(Manager.OScore, false);
                    }
                }
                else
                {
                    Manager.XScore += result;
                    if (CanvasManager.OnScoreChanged != null)
                    {
                        CanvasManager.OnScoreChanged.Invoke(Manager.XScore, true);
                    }
                }
            }
        }
    }
}
