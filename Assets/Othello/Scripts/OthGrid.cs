using System;
using EAUnity.Grid;

namespace Othello.Scripts {
    public class OthGrid : Grid2D<OthCellInfo> {

        private void Start() {
            InitializeGrid();
            CreateView(gameObject.transform);
        }
    }
}