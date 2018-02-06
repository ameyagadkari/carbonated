using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public enum CellType : short { Empty = 0, Human, Computer = ~Human }
    public class Cell : MonoBehaviour, IPointerDownHandler
    {
        private Image _image;
        private CellType _myCellType;
        public Action Reset;

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
                        break;
                    case CellType.Computer:
                        _image.sprite = Manager.IsHumanStarting
                            ? Manager.OSprite
                            : Manager.XSprite;
                        break;
                    case CellType.Empty:
                        _image.sprite = null;
                        break;
                }
            }
        }

        private void Awake()
        {
            _image = GetComponent<Image>();
            transform.localScale = Vector3.one;
            Reset += () => { MyCellType = CellType.Empty; };
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Manager.Gamestate == Gamestate.Play && MyCellType == CellType.Empty)
            {
                MyCellType = CellType.Human;
                Manager.Toggle(CellType.Computer);
            }
        }
    }
}
