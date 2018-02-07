using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [Flags]
    public enum CellType : short { Empty = 0, Human = 1 << 0, Computer = 1 << 1 }
    public class Cell : MonoBehaviour, IPointerDownHandler
    {
        private Image _image;
        public Action Reset;
        private int _row;
        private int _column;

        private CellType _myCellType;
        public CellType MyCellType
        {
            get { return _myCellType; }
            set
            {
                _myCellType = value;
                switch (_myCellType)
                {
                    case CellType.Human:
                        _image.sprite = Manager.IsHumanStarting
                            ? Manager.XSprite
                            : Manager.OSprite;
                        Manager.NumberOfMovesDone++;
                        break;
                    case CellType.Computer:
                        _image.sprite = Manager.IsHumanStarting
                            ? Manager.OSprite
                            : Manager.XSprite;
                        Manager.NumberOfMovesDone++;
                        break;
                    case CellType.Empty:
                        _image.sprite = null;
                        Manager.NumberOfMovesDone--;
                        break;
                }
            }
        }

        public void Setup(int row, int column)
        {
            _row = row;
            _column = column;

            _image = GetComponent<Image>();
            transform.localScale = Vector3.one;
            Reset += () => { MyCellType = CellType.Empty; };
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Manager.Gamestate == Gamestate.Play && MyCellType == CellType.Empty)
            {
                MyCellType = CellType.Human;
                if (Manager.Toggle != null)
                {
                    Manager.Toggle(CellType.Human, _row, _column);
                }
            }
        }
    }
}
