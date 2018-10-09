using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScalar : MonoBehaviour {

    public Board board;
    public float cameraOffset;
    public float aspectRatio = 0.625f;
    public float padding = 1;
    public float yOffset = 1;

    private void Start() {
        board = FindObjectOfType<Board>();
        if (board != null) {
            RepositionCamera(board.width - 1, board.height - 1);
        }
    }

    private void RepositionCamera(float x, float y) {
        Vector3 tempPosition = new Vector3(x / 2, y / 2 + yOffset, cameraOffset);
        transform.position = tempPosition;
        if (board.width >= board.height) {
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;
        } else {
            //Camera.main.orthographicSize = board.width / 2 + padding;
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;
        }
    }
}