using UnityEngine;

namespace Data {
    public struct GameSave {
        public int NumPlayers;
        public Vector2Int GridSize;
        public int[] GridState; // -1 for empty, 0 - N-1 for players
        public int CurrentTurn;
    }
}