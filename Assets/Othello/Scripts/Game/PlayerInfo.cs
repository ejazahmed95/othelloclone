using UnityEngine;

namespace Game {
    [CreateAssetMenu(fileName = "PlayerInfo", menuName = "Game/PlayerInfo")]
    public class PlayerInfo: ScriptableObject {
        public int id;
        public Sprite coinSprite;
        public Color color;
    }
}