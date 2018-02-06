using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace Assets.Scripts
{
    public class CanvasManager : MonoBehaviour
    {
        private const string HumanStarts= "Human Starts";
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

            Manager.Toggle += (cellType, row, column) =>
            {
                MessageText.text = cellType == CellType.Human ? HumanTurn : ComputerTurn;
            };
        }

        private void OnEnable()
        {
            ExitButton.onClick.AddListener(OnExitClicked);
            ResetButton.onClick.AddListener(OnResetClicked);
            StartButton.onClick.AddListener(OnStartClicked);
            PlayerToggle.onValueChanged.AddListener(OnPlayerToggleValueChanged);
        }

        private void OnDisable()
        {
            ExitButton.onClick.RemoveListener(OnExitClicked);
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
            Manager.Reset();
            PlayerToggleText.text = HumanStarts;
            PlayerToggle.isOn = true;
        }

        private void OnStartClicked()
        {
            PlayerToggle.enabled = false;
            MessageText.text = Manager.IsHumanStarting ? HumanTurn : ComputerTurn;
            StartButton.enabled = false;
            Manager.Set();
        }

        private void OnPlayerToggleValueChanged(bool value)
        {
            Manager.IsHumanStarting = value;
            PlayerToggleText.text = value ? HumanStarts : ComputerStarts;
        }
    }
}
