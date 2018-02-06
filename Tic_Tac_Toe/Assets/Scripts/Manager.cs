using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts
{
    public class Manager : MonoBehaviour
    {
        public static Action Reset;
        /*private static Manager _manager;
        public static Manager Singleton
        {
            get
            {
                if (_manager == null)
                {
                    _manager = FindObjectOfType<Manager>();
                }
                return _manager;
            }
        }*/

        public static Sprite XSprite { get; private set; }
        public static Sprite OSprite { get; private set; }
        public static bool IsHumanStarting { get; set; }

        private void Awake()
        {
            IsHumanStarting = true;
            XSprite = Resources.Load<Sprite>("Sprites/X");
            OSprite = Resources.Load<Sprite>("Sprites/O");
            Assert.IsNotNull(XSprite, "X sprite not found");
            Assert.IsNotNull(OSprite, "O sprite not found");
            Reset += () => { IsHumanStarting = true; };
        }
    }
}
