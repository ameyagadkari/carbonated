using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts
{
    public enum Gamestate : byte { NotStarted, ComputerTurn, HumanTurn, Ended }
    public class Manager : MonoBehaviour
    {
        public static Action Reset;
        public static Action Set;
        public static Action End;
        public static Action<CellType, int, int> Toggle;

        public static Sprite XSprite { get; private set; }
        public static Sprite OSprite { get; private set; }

        public static bool IsHumanStarting { get; set; }

        public static Gamestate Gamestate { get; private set; }
        public static int XScore { get; set; }
        public static int OScore { get; set; }
        public static int NumberOfMovesDone { get; set; }
        public static readonly CellCheckHints[,][] Patterns =
        {
            //Row 0
            {
                //Col 0
                new[] {CellCheckHints.U3, CellCheckHints.R3, CellCheckHints.Ur3},
                //Col 1
                new[] {CellCheckHints.U3, CellCheckHints.R3, CellCheckHints.Ur3, CellCheckHints.L1R2},
                //Col 2
                new[] {CellCheckHints.U3, CellCheckHints.R3, CellCheckHints.Ur3, CellCheckHints.L1R2, CellCheckHints.R1L2},
                //Col 3
                new[] {CellCheckHints.U3, CellCheckHints.L3, CellCheckHints.Ul3, CellCheckHints.L1R2, CellCheckHints.R1L2},
                //Col 4
                new[] {CellCheckHints.U3, CellCheckHints.L3, CellCheckHints.Ul3, CellCheckHints.R1L2},
                //Col 5
                new[] {CellCheckHints.U3,  CellCheckHints.L3, CellCheckHints.Ul3},
            },

            //Row 1
            {
                //Col 0
                new[] {CellCheckHints.U3, CellCheckHints.R3, CellCheckHints.Ur3, CellCheckHints.D1U2},
                //Col 1
                new[] {CellCheckHints.U3, CellCheckHints.R3, CellCheckHints.Ur3, CellCheckHints.L1R2, CellCheckHints.D1U2, CellCheckHints.Dl1Ur2},
                //Col 2
                new[] {CellCheckHints.U3, CellCheckHints.R3, CellCheckHints.Ur3, CellCheckHints.L1R2, CellCheckHints.R1L2, CellCheckHints.D1U2, CellCheckHints.Dl1Ur2, CellCheckHints.Dr1Ul2},
                //Col 3
                new[] {CellCheckHints.U3, CellCheckHints.L3, CellCheckHints.Ul3, CellCheckHints.L1R2, CellCheckHints.R1L2, CellCheckHints.D1U2, CellCheckHints.Dl1Ur2, CellCheckHints.Dr1Ul2},
                //Col 4
                new[] {CellCheckHints.U3, CellCheckHints.L3, CellCheckHints.Ul3, CellCheckHints.R1L2, CellCheckHints.D1U2, CellCheckHints.Dr1Ul2},
                //Col 5
                new[] {CellCheckHints.U3, CellCheckHints.L3, CellCheckHints.Ul3, CellCheckHints.D1U2},
            },

            //Row 2
            {
                //Col 0
                new[] {CellCheckHints.U3, CellCheckHints.R3, CellCheckHints.Ur3, CellCheckHints.D1U2, CellCheckHints.U1D2},
                //Col 1
                new[] {CellCheckHints.U3, CellCheckHints.R3, CellCheckHints.Ur3, CellCheckHints.L1R2, CellCheckHints.D1U2, CellCheckHints.U1D2, CellCheckHints.Dl1Ur2, CellCheckHints.Ul1Dr2},
                //Col 2
                new[] {CellCheckHints.U3, CellCheckHints.R3, CellCheckHints.Ur3, CellCheckHints.L1R2, CellCheckHints.R1L2, CellCheckHints.D1U2, CellCheckHints.U1D2, CellCheckHints.Dl1Ur2, CellCheckHints.Ul1Dr2, CellCheckHints.Dr1Ul2, CellCheckHints.Ur1Dl2},
                //Col 3
                new[] {CellCheckHints.U3, CellCheckHints.L3, CellCheckHints.Ul3, CellCheckHints.L1R2, CellCheckHints.R1L2, CellCheckHints.D1U2, CellCheckHints.U1D2, CellCheckHints.Dl1Ur2, CellCheckHints.Ul1Dr2, CellCheckHints.Dr1Ul2, CellCheckHints.Ur1Dl2},
                //Col 4
                new[] {CellCheckHints.U3, CellCheckHints.L3, CellCheckHints.Ul3, CellCheckHints.R1L2, CellCheckHints.D1U2, CellCheckHints.U1D2, CellCheckHints.Dr1Ul2, CellCheckHints.Ur1Dl2},
                //Col 5
                new[] {CellCheckHints.U3, CellCheckHints.L3, CellCheckHints.Ul3, CellCheckHints.D1U2, CellCheckHints.U1D2},
            },

            //Row 3
            {
                //Col 0
                new[] {CellCheckHints.D3, CellCheckHints.R3, CellCheckHints.Dr3, CellCheckHints.D1U2, CellCheckHints.U1D2},
                //Col 1
                new[] {CellCheckHints.D3, CellCheckHints.R3, CellCheckHints.Dr3, CellCheckHints.L1R2, CellCheckHints.D1U2, CellCheckHints.U1D2, CellCheckHints.Dl1Ur2, CellCheckHints.Ul1Dr2},
                //Col 2
                new[] {CellCheckHints.D3, CellCheckHints.R3, CellCheckHints.Dr3, CellCheckHints.L1R2, CellCheckHints.R1L2, CellCheckHints.D1U2, CellCheckHints.U1D2, CellCheckHints.Dl1Ur2, CellCheckHints.Ul1Dr2, CellCheckHints.Dr1Ul2, CellCheckHints.Ur1Dl2},
                //Col 3
                new[] {CellCheckHints.D3, CellCheckHints.L3, CellCheckHints.Dl3, CellCheckHints.L1R2, CellCheckHints.R1L2, CellCheckHints.D1U2, CellCheckHints.U1D2, CellCheckHints.Dl1Ur2, CellCheckHints.Ul1Dr2, CellCheckHints.Dr1Ul2, CellCheckHints.Ur1Dl2},
                //Col 4
                new[] {CellCheckHints.D3, CellCheckHints.L3, CellCheckHints.Dl3, CellCheckHints.R1L2, CellCheckHints.D1U2, CellCheckHints.U1D2, CellCheckHints.Dr1Ul2, CellCheckHints.Ur1Dl2},
                //Col 5
                new[] {CellCheckHints.D3, CellCheckHints.L3, CellCheckHints.Dl3, CellCheckHints.D1U2, CellCheckHints.U1D2},
            },

            //Row 4
            {
                //Col 0
                new[] {CellCheckHints.D3, CellCheckHints.R3, CellCheckHints.Dr3, CellCheckHints.U1D2},
                //Col 1
                new[] {CellCheckHints.D3, CellCheckHints.R3, CellCheckHints.Dr3, CellCheckHints.L1R2, CellCheckHints.U1D2, CellCheckHints.Ul1Dr2},
                //Col 2
                new[] {CellCheckHints.D3, CellCheckHints.R3, CellCheckHints.Dr3, CellCheckHints.L1R2, CellCheckHints.R1L2, CellCheckHints.U1D2, CellCheckHints.Ul1Dr2, CellCheckHints.Ur1Dl2},
                //Col 3
                new[] {CellCheckHints.D3, CellCheckHints.L3, CellCheckHints.Dl3, CellCheckHints.L1R2, CellCheckHints.R1L2, CellCheckHints.U1D2, CellCheckHints.Ul1Dr2, CellCheckHints.Ur1Dl2},
                //Col 4
                new[] {CellCheckHints.D3, CellCheckHints.L3, CellCheckHints.Dl3, CellCheckHints.R1L2, CellCheckHints.U1D2, CellCheckHints.Ur1Dl2},
                //Col 5
                new[] {CellCheckHints.D3, CellCheckHints.L3, CellCheckHints.Dl3, CellCheckHints.U1D2},
            },

            //Row 5
            {
                //Col 0
                new[] {CellCheckHints.D3, CellCheckHints.R3, CellCheckHints.Dr3},
                //Col 1
                new[] {CellCheckHints.D3, CellCheckHints.R3, CellCheckHints.Dr3, CellCheckHints.L1R2},
                //Col 2
                new[] {CellCheckHints.D3, CellCheckHints.R3, CellCheckHints.Dr3, CellCheckHints.L1R2, CellCheckHints.R1L2},
                //Col 3
                new[] {CellCheckHints.D3, CellCheckHints.L3, CellCheckHints.Dl3, CellCheckHints.L1R2, CellCheckHints.R1L2},
                //Col 4
                new[] {CellCheckHints.D3, CellCheckHints.L3, CellCheckHints.Dl3, CellCheckHints.R1L2},
                //Col 5
                new[] {CellCheckHints.D3, CellCheckHints.L3, CellCheckHints.Dl3},
            }

    };

        private GameObject _eventSystem;

        private void Awake()
        {
            _eventSystem = GameObject.FindGameObjectWithTag("ES");
            IsHumanStarting = true;
            Gamestate = Gamestate.NotStarted;
            XSprite = Resources.Load<Sprite>("Sprites/X");
            OSprite = Resources.Load<Sprite>("Sprites/O");
            Assert.IsNotNull(XSprite, "X sprite not found");
            Assert.IsNotNull(OSprite, "O sprite not found");
            Reset += () =>
            {
                IsHumanStarting = true;
                _eventSystem.SetActive(true);
                Gamestate = Gamestate.NotStarted;
                if (CanvasManager.OnScoreChanged != null)
                {
                    CanvasManager.OnScoreChanged.Invoke(0, true);
                    CanvasManager.OnScoreChanged.Invoke(0, false);
                }
            };
            Toggle += (previousCellType, row, column) =>
            {
                Gamestate = previousCellType == CellType.Computer ? Gamestate.HumanTurn : Gamestate.ComputerTurn;
                _eventSystem.SetActive(Gamestate == Gamestate.HumanTurn);
            };
            Set += () =>
            {
                NumberOfMovesDone = 0;
                XScore = 0;
                OScore = 0;
                Gamestate = IsHumanStarting ? Gamestate.HumanTurn : Gamestate.ComputerTurn;
                if (!IsHumanStarting)
                {
                    if (Toggle != null)
                    {
                        Toggle(CellType.Human, -1, -1);
                    }
                }
            };
            End += () =>
            {
                Gamestate = Gamestate.Ended;
                _eventSystem.SetActive(true);
            };
        }
    }

    /**
     * U/u = Up, D/d = Down, L/l = Left, R/r = Right
     */
    public enum CellCheckHints : byte
    {
        U3, D3, L3, R3, Ul3, Ur3, Dl3, Dr3, L1R2, R1L2, U1D2, D1U2, Ul1Dr2, Dr1Ul2, Ur1Dl2, Dl1Ur2
    }
}
