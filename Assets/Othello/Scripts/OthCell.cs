using Data;
using EAUnity.Event;
using EAUnity.Grid;

public class OthCell : CellBehavior<OthCellInfo> {
    public static GameEvent CellClickEvent = null;
    

    private void OnMouseDown() {
        if(CellClickEvent != null) CellClickEvent.Raise(new CellClickEventData{CellInfo = Info});
    }
}