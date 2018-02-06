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
        public static Action<CellType> Toggle;

        public static Sprite XSprite { get; private set; }
        public static Sprite OSprite { get; private set; }

        public static bool IsHumanStarting { get; set; }

        public static Gamestate Gamestate { get; private set; }

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
            Toggle += cellType => { /*Gamestate = cellType == CellType.Computer ? Gamestate.Waiting : Gamestate.Play;*/ };
        }
    }
}
