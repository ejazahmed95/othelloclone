using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using EAUnity.Core;
using EAUnity.Event;
using Othello.Scripts;
using Othello.Scripts.Command;
using Othello.Scripts.Game;
using UnityEngine;

namespace Game {
    public class GameManager : MonoBehaviour, IGameCommandHandler {
        [SerializeField] private OthGrid grid;
        [SerializeField] private HUDManager hud;
        [SerializeField] private int numPlayers = 2;
        [SerializeField] private List<PlayerInfo> players;
        [SerializeField] private CoinObjectPool coinPool;
        [SerializeField] private float flipAnimTime = 1.5f;
        [SerializeField] private SfxList sfx;

        private int _currentPlayerId;

        private EGameState _currentState = EGameState.None;
        private Stack<ICommand> _commandStack;
        private Stack<ICommand> _redoStack;

        public void StartNewGame() {
            if (!SetState(EGameState.Init)) return;
            _commandStack = new Stack<ICommand>();
            _redoStack = new Stack<ICommand>();
            StartCoroutine(InitCoins());
            foreach (var player in players) {
                player.score = 0;
            }
        }
        
        private IEnumerator InitCoins() {
            yield return null;
            int x = (grid.GridDimensions.x - numPlayers)/2;
            int y = (grid.GridDimensions.y - numPlayers)/2;
            for (int j = 0; j < numPlayers; j++) {
                for (int i = 0; i < numPlayers; i++) {
                    var cellIndex = new Vector2Int(x + i, y + j);
                    var playerIndex = (i + j) % numPlayers;
                    var coin = coinPool.GetNewCoin().Init(players[playerIndex]);
                    coin.transform.localPosition = grid.GetCellPosition(cellIndex);
                    grid.GetCell(cellIndex).UpdateCoin(coin);
                }
            }
            _currentPlayerId = 0;
            yield return new WaitForSeconds(0.5f);
            SetState(EGameState.TurnStart);
            hud.SyncGameState($"Player {_currentPlayerId+1}'s turn!");
            AudioManager.Instance.Play(sfx.turnChange);
            SyncScores();
        }

        private void SyncScores() {
            hud.SyncScores(players[0].score, players[1].score);
        }

        #region GameEvent Handling
        public void OnCellClicked(IGameEventData eventData) {
            if (_currentState != EGameState.TurnStart) return;
            if (!Utils.TryConvertVal(eventData, out CellClickEventData cellClickData)) {
                return;
            }
            if (!Validate(cellClickData, _currentPlayerId)) {
                Log.Warn($"Invalid Move Index = {cellClickData.CellInfo.Position}");
                AudioManager.Instance.Play(sfx.errorClick);
                return;
            }
            hud.SyncGameState("Turn in Progress");
            var command = new PlaceCoinCommand(cellClickData.CellInfo.Position, _currentPlayerId, this);
            _commandStack.Push(command);
            _redoStack.Clear();
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

        public void Undo() {
            if (_currentState != EGameState.TurnStart || _commandStack.Count == 0) {
                AudioManager.Instance.Play(sfx.errorClick);
                return;
            }
            hud.SyncGameState("Undoing Move");
            SetState(EGameState.TurnPlaying);
            var lastCommand = _commandStack.Pop();
            lastCommand.Undo();
            _redoStack.Push(lastCommand);
        }

        public void Redo() {
            if (_currentState != EGameState.TurnStart || _redoStack.Count == 0) {
                AudioManager.Instance.Play(sfx.errorClick);
                return;
            }
            hud.SyncGameState("Redoing Move");
            var command = _redoStack.Pop();
            _commandStack.Push(command);
            command.Execute();
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
            _currentPlayerId = playerIndex;
            var currentPlayer = players[_currentPlayerId];
            var coin = coinPool.GetNewCoin().Init(currentPlayer);
            coin.transform.localPosition = grid.GetCellPosition(cellIndex);
            grid.GetCell(cellIndex).UpdateCoin(coin);
            
            // Find Valid Directions
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
            
            // Flip Coins
            foreach (var dirVec in updateDirs) {
                var dist = 1;
                var pos = cellIndex + dist * dirVec;
                while (grid.GetCell(pos).CoinId != playerIndex) {
                    grid.GetCell(pos).Coin.FlipCoin(currentPlayer);
                    dist++;
                    pos = cellIndex + dist * dirVec;
                }
            }
            AudioManager.Instance.Play(sfx.placeCoin);
            StartCoroutine(WaitAndCompleteTurn(true));
        }
        
        private IEnumerator WaitAndCompleteTurn(bool changeTurn) {
            yield return new WaitForSeconds(flipAnimTime);
            SetState(EGameState.TurnComplete);
            SyncScores();
            if (CheckGameEnd()) {
                hud.SyncGameState($"Game Ended!");
                SetState(EGameState.End);
                yield return new WaitForSeconds(1f);
                CustomSceneLoader.LoadScene("GameOver");
                yield break;
            }
            yield return new WaitForSeconds(1f);
            SetState(EGameState.TurnStart);
            if(changeTurn) _currentPlayerId = (_currentPlayerId + 1) % numPlayers;
            AudioManager.Instance.Play(sfx.turnChange);
            hud.SyncGameState($"Player {_currentPlayerId+1}'s turn!");
        }
        
        private bool CheckGameEnd() {
            if (players[0].score == 0 || players[1].score == 0) {
                return true;
            }
            if (players[0].score + players[1].score == grid.GridDimensions.x * grid.GridDimensions.y) {
                return true;
            }
            return false;
        }

        public void SyncState(GameSave prevGameSave) {
            _currentPlayerId = prevGameSave.CurrentTurn;
            // foreach (var player in players) {
                // player.score = 0;
            // }
            for (int i = 0; i < prevGameSave.GridState.Length; i++) {
                int x = i % prevGameSave.GridSize.x;
                int y = i / prevGameSave.GridSize.x;
                int ownerId = prevGameSave.GridState[i];
                var cellInfo = grid.GetCell(new Vector2Int(x, y));
                // if (ownerId != -1) players[ownerId].score++;
                if (ownerId == -1) {
                    if (cellInfo.Coin != null) {
                        coinPool.DisableCoin(cellInfo.Coin);
                        cellInfo.UpdateCoin(null);
                    }
                } else if (cellInfo.Coin != null) {
                    cellInfo.Coin.FlipCoin(players[ownerId]);
                } else {
                    var coin = coinPool.GetNewCoin().Init(players[ownerId]);
                    coin.transform.localPosition = grid.GetCellPosition(cellInfo.Position);
                    cellInfo.UpdateCoin(coin);
                }
            }
            StartCoroutine(WaitAndCompleteTurn(false));
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