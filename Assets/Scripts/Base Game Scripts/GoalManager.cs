﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal {
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}

public class GoalManager : MonoBehaviour {

    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;
    private Board board;
    private EndGameManager endGameManager;

    private void Start() {
        endGameManager = FindObjectOfType<EndGameManager>();
        board = FindObjectOfType<Board>();
        GetGoals();
        SetupGoals();
    }

    private void GetGoals() {
        if (board != null) {
            if (board.world != null) {
                if (board.level < board.world.levels.Length) {
                    if (board.world.levels[board.level] != null) {
                        levelGoals = board.world.levels[board.level].levelGoals;
                        for (int i = 0; i < levelGoals.Length; i++) {
                            levelGoals[i].numberCollected = 0;
                        }
                    }
                }
            }
        }
    }

    private void SetupGoals() {
        for (int i = 0; i < levelGoals.Length; i++) {
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, 
                Quaternion.identity, goalIntroParent.transform);

            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;

            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position,
                Quaternion.identity, goalGameParent.transform);

            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;
        }
    }

    public void UpdateGoals() {
        int goalsCompleted = 0;
        for (int i = 0; i < levelGoals.Length; i++) {
            currentGoals[i].thisText.text = 
                "" + levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded) {
                goalsCompleted++;
                currentGoals[i].thisText.text =
                    "" + levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
            }

            if (goalsCompleted >= levelGoals.Length) {
                if (endGameManager != null) {
                    endGameManager.WinGame();
                }
            }
        }
    }

    public void CompareGoal(string goalToCompare) {
        for (int i = 0; i < levelGoals.Length; i++) {
            if (goalToCompare == levelGoals[i].matchValue) {
                levelGoals[i].numberCollected++;
            }
        }
    }

}