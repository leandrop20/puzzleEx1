﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour {

    [Header("Board Variables")]
    public int column;
    public int row;
    public int previowsColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private Animator anime;
    private float shineDelay;
    private float shineDelaySeconds;
    private EndGameManager endGameManager;
    private HintManager hintManager;
    private FindMatches findMatches;
    private Board board;
    public GameObject otherDot;
    private Vector2 firstTouchPosition = Vector2.zero;
    private Vector2 finalTouchPosition = Vector2.zero;
    private Vector2 tempPosition;

    [Header("Swiper Stuff")]
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    [Header("Powerup Stuff")]
    public bool isColorBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isAdjacentBomb;
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;
    public GameObject adjacentBomb;

    private void Start() {
        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;

        shineDelay = Random.Range(3f, 6f);
        shineDelaySeconds = shineDelay;

        hintManager = FindObjectOfType<HintManager>();
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
        //board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        endGameManager = FindObjectOfType<EndGameManager>();
        anime = GetComponent<Animator>();
    }

    //This is for testing and Debug only; //////////////////////////////////////////////
    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(1)) {
            isColorBomb = true;
            Instantiate(colorBomb, transform.position, Quaternion.identity,
                transform);
        }
    }

    private void Update() {
        shineDelaySeconds -= Time.deltaTime;
        if (shineDelaySeconds <= 0) {
            shineDelaySeconds = shineDelay;
            StartCoroutine(StartShineCo());
        }

        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1) {//MOVE TOWARDS THE TARGET
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != this.gameObject) {
                board.allDots[column, row] = this.gameObject;
                findMatches.FindAllMatches();
            }
        } else {//DIRECTLY SET THE POSITION
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1) {//MOVE TOWARDS THE TARGET
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != this.gameObject) {
                board.allDots[column, row] = this.gameObject;
                findMatches.FindAllMatches();
            }
        } else {//DIRECTLY SET THE POSITION
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    IEnumerator StartShineCo() {
        anime.SetBool("Shine", true);
        yield return null;
        anime.SetBool("Shine", false);
    }

    public void PopAnimation() {
        anime.SetBool("Popped", true);
    }

    public IEnumerator CheckMoveCo() {
        if (isColumnBomb) {
            findMatches.MatchPiecesOfColor(otherDot.tag);
            isMatched = true;
        } else if (otherDot.GetComponent<Dot>().isColorBomb) {
            findMatches.MatchPiecesOfColor(this.gameObject.tag);
            otherDot.GetComponent<Dot>().isMatched = true;
        }
        yield return new WaitForSeconds(.5f);
        if (otherDot != null) {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched) {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previowsColumn;

                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.move;
            } else {
                if (endGameManager != null) {
                    if (endGameManager.requirements.gameType == GameType.Moves) {
                        endGameManager.DecreaseCounterValue();
                    }
                }
                board.DestroyMatches();
            }
        }
    }

    private void OnMouseDown() {
        if (hintManager != null) {
            hintManager.DestroyHint();
        }

        if (board.currentState == GameState.move) {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp() {
        if (board.currentState == GameState.move) {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    private void CalculateAngle() {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist ||
                Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist) {
            board.currentState = GameState.wait;

            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y,
                finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            
            board.currentDot = this;
        } else {
            board.currentState = GameState.move;
        }
    }

    private void MovePieceActual(Vector2 direction) {
        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previowsColumn = column;
        if (board.lockTiles[column, row] == null && 
                board.lockTiles[column + (int)direction.x, row + (int)direction.y] == null) {
            if (otherDot != null) {
                otherDot.GetComponent<Dot>().column += -1 * (int)direction.x;
                otherDot.GetComponent<Dot>().row += -1 * (int)direction.y;
                column += (int)direction.x;
                row += (int)direction.y;
                StartCoroutine(CheckMoveCo());
            } else {
                board.currentState = GameState.move;
            }
        } else {
            board.currentState = GameState.move;
        }
    }

    private void MovePieces() {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1) {//RIGHT SWIPE
            MovePieceActual(Vector2.right);
        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) {//UP SWIPE
            MovePieceActual(Vector2.up);
        } else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0) {//LEFT SWIPE
            MovePieceActual(Vector2.left);
        } else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0) {//RIGHT SWIPE
            MovePieceActual(Vector2.down);
        }
        board.currentState = GameState.move;
    }

    private void FindMatches() {
        if (column > 0 && column < board.width - 1) {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];
            if (leftDot1 != null && rightDot1 != null) {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag) {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }

        if (row > 0 && row < board.height - 1) {
            GameObject upDot1 = board.allDots[column, row + 1];
            GameObject downDot1 = board.allDots[column, row - 1];
            if (upDot1 != null && downDot1 != null) {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag) {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

    public void MakeRowBomb() {
        if (!isColumnBomb && !isColorBomb && !isAdjacentBomb) {
            isRowBomb = true;
            Instantiate(rowArrow, transform.position, Quaternion.identity, transform);
        }
    }

    public void MakeColumnBomb() {
        if (!isRowBomb && !isColorBomb && !isAdjacentBomb) {
            isColumnBomb = true;
            Instantiate(columnArrow, transform.position, Quaternion.identity, transform);
        }
    }

    public void MakeColorBomb() {
        if (!isColumnBomb && !isRowBomb && !isAdjacentBomb) {
            isColorBomb = true;
            Instantiate(colorBomb, transform.position, Quaternion.identity, transform);
            this.gameObject.tag = "Color";
        }
    }

    public void MakeAdjacentBomb() {
        if (!isColumnBomb && !isRowBomb && !isColorBomb) {
            isAdjacentBomb = true;
            Instantiate(adjacentBomb, transform.position, Quaternion.identity, transform);
        }
    }

}