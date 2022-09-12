using Data;
using UnityEngine;

namespace Othello.Scripts.Game {
    public interface IGameCommandHandler {
        GameSave GetNewGameSave();
        void PlaceCoin(Vector2Int cellIndex, int playerIndex);
        void SyncState(GameSave prevGameSave);
    }
}