using System;
using EAUnity.Grid;

public class OthGrid : Grid2D<OthCellInfo> {

    private void Start() {
        InitializeGrid();
        CreateView(gameObject.transform);
    }
}
