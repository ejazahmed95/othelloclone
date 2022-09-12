using System;
using System.Collections;
using EAUnity.Event;
using EAUnity.Grid;
using UnityEngine;

public class OthGrid : Grid2D<OthCellInfo> {
    [SerializeField] private GameEvent cellClickEvent;
    [SerializeField] private GameEvent boardSetupEvent;
    [SerializeField] private float setupDelay = 1f;
    
    private void Start() {
        OthCell.CellClickEvent = cellClickEvent;
        InitializeGrid();
        CreateView(gridParent.transform);
        StartCoroutine(GetReady());
    }
    private IEnumerator GetReady() {
        yield return new WaitForSeconds(setupDelay);
        boardSetupEvent.Raise();
    }
}
