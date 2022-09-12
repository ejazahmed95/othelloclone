using EAUnity.Grid;

public class OthCellInfo : CellInfo {
    private int _coinId;
    
    public int CoinId {
        get => _coinId;
        set {
            _coinId = value;
            Notify();
        }
    }
}