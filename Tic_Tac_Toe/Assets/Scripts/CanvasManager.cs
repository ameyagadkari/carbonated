using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class CanvasManager : MonoBehaviour
    {
        private const string HumanStarts = "Human Starts";
        private const string ComputerStarts = "Computer Starts";
        private const string HumanTurn = "Human's Turn";
        private const string ComputerTurn = "Computer's Turn";
        private const string DefaultMessage = "Toggle to change who starts and press Start button to play";

        public Button ExitButton;
        public Button ResetButton;
        public Button StartButton;
        public Text XScoreText;
        public Text OScoreText;
        public Text PlayerToggleText;
        public Toggle PlayerToggle;
        public Text MessageText;

        public class ScoreEvent : UnityEvent<int, bool> { }
        public static ScoreEvent OnScoreChanged;

        private void Awake()
        {
            Assert.IsNotNull(ExitButton, "ExitButton not found");
            Assert.IsNotNull(ResetButton, "ResetButton not found");
            Assert.IsNotNull(StartButton, "StartButton not found");
            Assert.IsNotNull(XScoreText, "XScoreText not found");
            Assert.IsNotNull(OScoreText, "OScoreText not found");
            Assert.IsNotNull(PlayerToggleText, "PlayerToggle not found");
            Assert.IsNotNull(PlayerToggle, "PlayerToggle not found");
            Assert.IsNotNull(MessageText, "MessageText not found");

            if (OnScoreChanged == null)
            {
                OnScoreChanged = new ScoreEvent();
            }
            Manager.Toggle += (previousCellType, row, column) =>
            {
                MessageText.text = previousCellType == CellType.Human ? ComputerTurn : HumanTurn;
            };
        }

        private void OnEnable()
        {
            ExitButton.onClick.AddListener(OnExitClicked);
            ResetButton.onClick.AddListener(OnResetClicked);
            StartButton.onClick.AddListener(OnStartClicked);
            PlayerToggle.onValueChanged.AddListener(OnPlayerToggleValueChanged);
            OnScoreChanged.AddListener(OnScoreValueChanged);
        }

        private void OnDisable()
        {
            ExitButton.onClick.RemoveListener(OnExitClicked);
            ResetButton.onClick.RemoveListener(OnResetClicked);
            StartButton.onClick.RemoveListener(OnStartClicked);
            PlayerToggle.onValueChanged.RemoveListener(OnPlayerToggleValueChanged);
            OnScoreChanged.RemoveListener(OnScoreValueChanged);
        }

        private static void OnExitClicked()
        {
            Application.Quit();
        }

        private void OnResetClicked()
        {
            PlayerToggle.enabled = true;
            MessageText.text = DefaultMessage;
            StartButton.enabled = true;
            if (Manager.Reset != null)
            {
                Manager.Reset();
            }
            PlayerToggleText.text = HumanStarts;
            PlayerToggle.isOn = true;
        }

        private void OnStartClicked()
        {
            PlayerToggle.enabled = false;
            MessageText.text = Manager.IsHumanStarting ? HumanTurn : ComputerTurn;
            StartButton.enabled = false;
            if (Manager.Set != null)
            {
                Manager.Set();
            }
        }

        private void OnPlayerToggleValueChanged(bool value)
        {
            Manager.IsHumanStarting = value;
            PlayerToggleText.text = value ? HumanStarts : ComputerStarts;
        }

        private void OnScoreValueChanged(int score, bool isXscore)
        {
            if (isXscore)
            {
                XScoreText.text = "X:" + score;
            }
            else
            {
                OScoreText.text = "O:" + score;
            }
        }
    }
}
