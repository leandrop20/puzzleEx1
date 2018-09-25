using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour {

    public enum DIRECTION { LEFT, RIGHT, UP, BOTTOM, NULL };

    public GameObject[] textures;

    float[][] matriz;
    List<GameObject> items;

    private Vector3 INIT_POSITION = new Vector2(-2.45f, 2.45f);
    private const int ROWS = 8;
    private const int COLS = 8;
    private const float SPACE = 0.7f;

    void Start() {
        matriz = new float[COLS][];
        items = new List<GameObject>();
    }

    public void Create() {
        int itemID;

        for (int c = 0; c < COLS; c++) {
            matriz[c] = new float[ROWS];
            for (int r = 0; r < ROWS; r++) {
                itemID = Random.Range(0, textures.Length);
                GameObject item = Instantiate(textures[itemID], 
                    new Vector3(c * SPACE, -SPACE * r, 0) + INIT_POSITION, Quaternion.identity, transform);
                items.Add(item);
            }
        }
    }

    public bool ChangeObject(GameObject obj, Vector2 position, DIRECTION direction) {
        Vector2 space = new Vector2(0, 0);
        switch (direction) {
            case DIRECTION.RIGHT: space.x = SPACE; break;
            case DIRECTION.LEFT: space.x = -SPACE; break;
            case DIRECTION.UP: space.y = SPACE; break;
            case DIRECTION.BOTTOM: space.y = -SPACE; break;
        }

        GameObject otherObj = GetChildByPosition((Vector2) position + space);
        if (otherObj) {
            obj.transform.position = otherObj.transform.position;
            otherObj.GetComponent<Item>().Move(position);
            return true;
        }
        
        return false;
    }

    GameObject GetChildByPosition(Vector2 p) {
        foreach (Transform child in transform) {
            if (Vector2.Distance(child.transform.position, p) < 0.2f) {
                return child.gameObject;
            }
        }
        return null;
    }

    public void Move() {
        Debug.Log("MOVE");
    }

}