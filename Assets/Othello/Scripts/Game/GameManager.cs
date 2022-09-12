using EAUnity.Core;

using UnityEngine;

namespace Game {
    public class GameManager : MonoBehaviour {
        [SerializeField] private OthGrid grid;
        
        
        private EGameState _currentState = EGameState.None;

        public void StartNewGame() {
            if (!SetState(EGameState.Init)) return;
            
        }

        private bool SetState(EGameState newState) {
            var valid = false;
            switch (_currentState) {
                case EGameState.None:
                    valid = newState is EGameState.Init;
                    break;
                case EGameState.Init:
                    valid = newState is EGameState.TurnStart;
                    break;
                case EGameState.TurnStart:
                    valid = newState is EGameState.TurnPlaying or EGameState.TurnComplete;
                    break;
                case EGameState.TurnPlaying:
                    valid = newState is EGameState.TurnComplete;
                    break;
                case EGameState.TurnComplete:
                    valid = newState is EGameState.TurnStart or EGameState.End;
                    break;
                case EGameState.Paused:
                    valid = true;
                    break;
                case EGameState.End:
                    valid = newState is EGameState.Finish;
                    break;
                case EGameState.Finish:
                    break;
                default:
                    break;
            }
            if (valid) {
                Log.Info($"State changed from {_currentState} to {newState}", "GameState");
                _currentState = newState;
            } else {
                Log.Err($"Invalid State change from {_currentState} to {newState}", "GameState");
            }
            return valid;
        }
    }
}