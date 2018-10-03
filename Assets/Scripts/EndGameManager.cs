using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameType {
    Moves, Time
}

[System.Serializable]
public class EndGameRequirements {
    public GameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour {

    public GameObject movesLabel;
    public GameObject timeLabel;
    public GameObject youWinPanel;
    public GameObject tryAgainPanel;
    public Text counterText;
    public EndGameRequirements requirements;
    public int currentCounterValue;
    private Board board;
    private float timerSeconds;

    private void Start() {
        board = FindObjectOfType<Board>();
        SetGameType();
        SetupGame();
    }

    private void SetGameType() {
         if (board.world != null) {
            if (board.level < board.world.levels.Length) {
                if (board.world.levels[board.level] != null) {
                    requirements = board.world.levels[board.level].endGameRequirements;
                }
            }
        }
    }

    private void SetupGame() {
        currentCounterValue = requirements.counterValue;
        if (requirements.gameType == GameType.Moves) {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        } else {
            timerSeconds = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }
        counterText.text = "" + currentCounterValue;
    }

    public void DecreaseCounterValue() {
        if (board.currentState != GameState.pause) {
            currentCounterValue--;
            counterText.text = "" + currentCounterValue;
            if (currentCounterValue <= 0) {
                LoseGame();
            }
        }
    }

    public void WinGame() {
        youWinPanel.SetActive(true);
        board.currentState = GameState.win;
        currentCounterValue = 0;
        counterText.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    public void LoseGame() {
        tryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;
        currentCounterValue = 0;
        counterText.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    private void Update() {
        if (requirements.gameType == GameType.Time && currentCounterValue > 0) {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0) {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }

}