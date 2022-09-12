using System;
using System.Collections.Generic;
using UnityEngine;

namespace EAUnity.Grid {
    public class Grid2D<T> : MonoBehaviour where T: CellInfo, new() {
        [SerializeField] protected Vector2Int gridDimensions;
        [SerializeField] protected CellBehavior<T> cellPrefab;
        [SerializeField] protected GameObject gridParent;
        [SerializeField] protected Vector2 cellSize;

        private List<List<CellBehavior<T>>> _cells;
        private List<List<T>> _cellsInfo;
        private Vector2 _zeroCellPosition; // Position of bottom left cell
        
        protected bool Initialized = false;

        protected void Awake() {
            _cells = new List<List<CellBehavior<T>>>();
            _cellsInfo = new List<List<T>>();
        }

        public void InitializeGrid() {
            for (var y = 0; y < gridDimensions.y; y++) {
                var row = new List<T>();
                for (var x = 0; x < gridDimensions.x; x++) {
                    var cellInfo = new T();
                    cellInfo.Init(new Vector2Int(x, y));
                    row.Add(cellInfo);
                }
                _cellsInfo.Add(row);
            }
            _zeroCellPosition = - (cellSize*gridDimensions)/2 + (cellSize / 2);
            Initialized = true;
        }

        
        public void CreateView(Transform parent, CellBehavior<T> prefab = null) {
            if (!Initialized) return;
            if (prefab == null) prefab = cellPrefab;
            for (var y = 0; y < gridDimensions.y; y++) {
                var row = new List<CellBehavior<T>>();
                for (var x = 0; x < gridDimensions.x; x++) {
                    CellBehavior<T> cell = Instantiate(prefab, parent).Init(_cellsInfo[x][y]);
                    cell.transform.localPosition = _zeroCellPosition + new Vector2(x * cellSize.x, y * cellSize.y);
                    row.Add(cell);
                }
                _cells.Add(row);
            }
        }
        
    }
}