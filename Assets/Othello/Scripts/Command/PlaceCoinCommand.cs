using Data;
using Game;
using UnityEngine;

namespace Othello.Scripts.Command {
    public class PlaceCoinCommand: ICommand {
        private IGameCommandHandler _commandHandler;
        private Vector2Int _cellIndex;
        private int _playerIndex;
        private GameSave _prevGameSave;
        
        public PlaceCoinCommand(Vector2Int cellIndex, int playerIndex, IGameCommandHandler commandHandler) {
            _cellIndex = cellIndex;
            _playerIndex = playerIndex;
            _commandHandler = commandHandler;
            _prevGameSave = commandHandler.GetNewGameSave();
        }
        
        public void Execute() {
            _commandHandler.PlaceCoin(_cellIndex, _playerIndex);
        }
        
        public void Undo() {
            _commandHandler.SyncState(_prevGameSave);
        }
        
    }
}