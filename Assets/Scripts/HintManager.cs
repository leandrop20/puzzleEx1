using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour {

    private Board board;
    public float hintDelay;
    private float hintDelaySeconds;
    public GameObject hintParticle;
    public GameObject currentHint;

    private void Start() {
        board = FindObjectOfType<Board>();
        hintDelaySeconds = hintDelay;
    }

    private void Update() {
        hintDelaySeconds -= Time.deltaTime;
        if (hintDelaySeconds <= 0 && currentHint == null) {
            MarkHint();
            hintDelaySeconds = hintDelay;
        }
    }

    private List<GameObject> FindAllMatches() {
        List<GameObject> possiblesMoves = new List<GameObject>();

        for (int i = 0; i < board.width; i++) {
            for (int j = 0; j < board.height; j++) {
                if (board.allDots[i, j] != null) {
                    if (i < board.width - 1) {
                        if (board.SwitchAndCheck(i, j, Vector2.right)) {
                            possiblesMoves.Add(board.allDots[i, j]);
                        }
                    }
                    if (j < board.height - 1) {
                        if (board.SwitchAndCheck(i, j, Vector2.up)) {
                            possiblesMoves.Add(board.allDots[i, j]);
                        }
                    }
                }
            }
        }
        return possiblesMoves;
    }

    private GameObject PickOneRandomly() {
        List<GameObject> possiblesMoves = new List<GameObject>();
        possiblesMoves = FindAllMatches();
        if (possiblesMoves.Count > 0) {
            int pieceToUse = Random.Range(0, possiblesMoves.Count);
            return possiblesMoves[pieceToUse];
        }
        return null;
    }

    private void MarkHint() {
        GameObject move = PickOneRandomly();
        if (move != null) {
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
        }
    }

    public void DestroyHint() {
        if (currentHint != null) {
            Destroy(currentHint);
            currentHint = null;
            hintDelaySeconds = hintDelay;
        }
    }

}