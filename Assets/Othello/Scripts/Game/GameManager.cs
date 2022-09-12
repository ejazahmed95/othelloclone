using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using EAUnity.Core;
using EAUnity.Event;
using Othello.Scripts.Command;
using Othello.Scripts.Game;
using UnityEngine;

namespace Game {
    public class GameManager : MonoBehaviour, IGameCommandHandler {
        [SerializeField] private OthGrid grid;
        [SerializeField] private int numPlayers;
        [SerializeField] private List<PlayerInfo> players;
        [SerializeField] private float flipAnimTime = 1.5f;

        private int _currentPlayerId;

        private EGameState _currentState = EGameState.None;
        private Stack<ICommand> _commandStack;
        private Stack<ICommand> _redoStack;

        private void Start() {
            StartNewGame();
        }
        
        public void StartNewGame() {
            if (!SetState(EGameState.Init)) return;
            _commandStack = new Stack<ICommand>();
            _redoStack = new Stack<ICommand>();
            //StartNewTurnOrEnd
        }

        #region GameEvent Handling
        public void OnCellClicked(IGameEventData eventData) {
            if (_currentState != EGameState.TurnStart) return;
            if (!Utils.TryConvertVal(eventData, out CellClickEventData cellClickData)) {
                return;
            }
            if (!Validate(cellClickData, _currentPlayerId)) {
                Log.Info("Invalid Move");
                return;
            }
            var command = new PlaceCoinCommand(cellClickData.CellInfo.Position, _currentPlayerId, this);
            _commandStack.Push(command);
            command.Execute();
        }
        
        private bool Validate(CellClickEventData cellClickData, int currentPlayerId) {
            var cellIndex = cellClickData.CellInfo.Position;
            if (cellClickData.CellInfo.CoinId != -1) return false;
            
            foreach (var dirVec in OthGrid.DirectionVectors) {
                var dist = 1;
                var pos = cellIndex + dist * dirVec;
                while (grid.IsValid(pos)) {
                    var coinId = grid.GetCell(pos).CoinId;
                    if (coinId == -1) break;
                    if (coinId == currentPlayerId) {
                        if (dist > 1) return true;
                        break;
                    }
                    dist++;
                    pos = cellIndex + dist * dirVec;
                }
            }
            return false;
        }

        public void OnUndoRedo(IGameEventData eventData) {
            
        }
        
        #endregion

        #region CommandHandler Implementation

        public GameSave GetNewGameSave() {
            var gridState = new int[grid.GridDimensions.x*grid.GridDimensions.y];
            var i = 0;
            foreach (var cellInfo in grid.CellsInfo.SelectMany(rowCells => rowCells)) {
                gridState[i] = cellInfo.CoinId;
                i++;
            }
            return new GameSave {
                NumPlayers = numPlayers,
                CurrentTurn = _currentPlayerId,
                GridSize = grid.GridDimensions,
                GridState = gridState
            };
        }
        
        public void PlaceCoin(Vector2Int cellIndex, int playerIndex) {
            SetState(EGameState.TurnPlaying);
            List<Vector2Int> updateDirs = new List<Vector2Int>();
            grid.GetCell(cellIndex).CoinId = playerIndex;
            foreach (var dirVec in OthGrid.DirectionVectors) {
                var dist = 1;
                var pos = cellIndex + dist * dirVec;
                while (grid.IsValid(pos)) {
                    var coinId = grid.GetCell(pos).CoinId;
                    if (coinId == -1) break;
                    if (coinId == _currentPlayerId) {
                        if (dist > 1) updateDirs.Add(dirVec);
                        break;
                    }
                    dist++;
                    pos = cellIndex + dist * dirVec;
                }
            }

            foreach (var dirVec in updateDirs) {
                var dist = 1;
                var pos = cellIndex + dist * dirVec;
                while (grid.GetCell(pos).CoinId != playerIndex) {
                    grid.GetCell(pos).CoinId = playerIndex;
                    dist++;
                    pos = cellIndex + dist * dirVec;
                }
            }

            StartCoroutine(WaitAndCompleteTurn());
        }
        private IEnumerator WaitAndCompleteTurn() {
            yield return new WaitForSeconds(flipAnimTime);
            SetState(EGameState.TurnComplete);
            yield return new WaitForSeconds(1f);
            SetState(EGameState.TurnStart);
            _currentPlayerId = (_currentPlayerId + 1) % numPlayers;
        }

        public void SyncState(GameSave prevGameSave) {
            
        }
        #endregion
        
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