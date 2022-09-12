using Unity.VisualScripting;
using UnityEngine;

namespace Othello.Scripts.Game {
    [CreateAssetMenu(fileName = "PlayerInfo", menuName = "Game/PlayerInfo")]
    public class PlayerInfo: ScriptableObject {
        public int id;
        
    }
}