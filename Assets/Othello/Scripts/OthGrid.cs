using System;
using EAUnity.Event;
using EAUnity.Grid;
using UnityEngine;

public class OthGrid : Grid2D<OthCellInfo> {
    [SerializeField] private GameEvent cellClickEvent;
    
    private void Start() {
        OthCell.CellClickEvent = cellClickEvent;
        InitializeGrid();
        CreateView(gameObject.transform);
    }
}
