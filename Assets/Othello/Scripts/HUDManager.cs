using EAUnity.Core;
using TMPro;
using UnityEngine;

public class HUDManager : SingletonBehaviour<HUDManager> {
    [SerializeField] private TMP_Text gameState;
    [SerializeField] private TMP_Text player1Score;
    [SerializeField] private TMP_Text player2Score;
    
    public void SyncGameState(string status) {
        gameState.text = status;
    }

    public void SyncScores(int score1, int score2) {
        player1Score.text = $"Player 1: {score1}";
        player2Score.text = $"Player 2: {score2}";
    }
}