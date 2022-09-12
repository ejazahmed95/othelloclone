using EAUnity.Grid;
using Game;

public class OthCellInfo : CellInfo {
    private Coin _coin = null;

    public Coin Coin => _coin;
    public int CoinId => (_coin != null) ? _coin.Id : -1;

    public void UpdateCoin(Coin coin) {
        _coin = coin;
    }

}