using EAUnity.Event;
using EAUnity.Grid;

namespace Data {
    public struct CellClickEventData: IGameEventData {
        public OthCellInfo CellInfo;
    }
}