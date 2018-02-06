using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts
{
    public enum Gamestate : byte { NotStarted, Waiting, Play }
    public class Manager : MonoBehaviour
    {
        public static Action Reset;
        public static Action Set;
        public static Action<CellType, int, int> Toggle;

        public static Sprite XSprite { get; private set; }
        public static Sprite OSprite { get; private set; }

        public static bool IsHumanStarting { get; set; }

        public static Gamestate Gamestate { get; private set; }

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

        private void Awake()
        {
            IsHumanStarting = true;
            Gamestate = Gamestate.NotStarted;
            XSprite = Resources.Load<Sprite>("Sprites/X");
            OSprite = Resources.Load<Sprite>("Sprites/O");
            Assert.IsNotNull(XSprite, "X sprite not found");
            Assert.IsNotNull(OSprite, "O sprite not found");
            Reset += () =>
            {
                IsHumanStarting = true;
                Gamestate = Gamestate.NotStarted;
            };
            Set += () => { Gamestate = IsHumanStarting ? Gamestate.Play : Gamestate.Waiting; };
            Toggle += (cellType, row, column) => { /*Gamestate = cellType == CellType.Computer ? Gamestate.Waiting : Gamestate.Play;*/ };
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
