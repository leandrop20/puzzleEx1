﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    private Board board;
    public Text scoreText;
    public int score;
    public Image scoreBar;

    private void Start() {
        board = FindObjectOfType<Board>();
    }

    private void Update() {
        scoreText.text = "" + score;
    }

    public void IncreaseScore(int amountToIncrease) {
        score += amountToIncrease;
        if (board != null && scoreBar != null) {
            int length = board.scoreGoals.Length;
            scoreBar.fillAmount = (float)score / (float)board.scoreGoals[length - 1];
        }
    }

}