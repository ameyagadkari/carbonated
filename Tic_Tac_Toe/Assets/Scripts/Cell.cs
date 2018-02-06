using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public enum CellType { Empty, Human, Computer }
    public class Cell : MonoBehaviour
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
    }
}
