namespace Game {
    public enum EGameState {
        None,
        Init,
        TurnStart, // Control Not given yet
        TurnPlaying, // Timer Running, Control in Use
        TurnComplete, // Move has ended;
        Paused,
        End,
        Finish,
    }
}