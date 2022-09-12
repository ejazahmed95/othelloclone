namespace Othello.Scripts.Command {
    public interface ICommand {
        void Execute();
        void Undo();
    }
}