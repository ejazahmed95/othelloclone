namespace Game {
    public enum EGameState {
        None,
        Init,
        TurnStart,
        TurnPlaying, // Action Complete; Animation Running
        TurnComplete, // Move has ended;
        Paused,
        End,
        Finish,
    }
}