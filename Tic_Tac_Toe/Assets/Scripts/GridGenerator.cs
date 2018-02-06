using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts
{
    public class GridGenerator : MonoBehaviour
    {
        private const int Numberofrows = 6;
        private const int Numberofcolumns = Numberofrows;
        private GameObject _cellPrefab;
        private Cell[,] _cells;


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
                        _cells[row, column].Reset();
                    }
                }
            };
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
                }
            }
        }
    }
}
