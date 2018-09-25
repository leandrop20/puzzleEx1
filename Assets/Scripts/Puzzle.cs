using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour {

    public GameObject[] items;

    private Vector3 INIT_POSITION = new Vector2(-2.45f, 2.45f);
    private int ROWS = 8;
    private int COLS = 8;

    public void Create() {
        int id;
        for (int i = 0; i < ROWS; i++) {
            for (int j = 0; j < COLS; j++) {
                id = Random.Range(0, items.Length);
                GameObject item = Instantiate(items[id], 
                    new Vector3(0.7f * i, -0.7f * j, 0) + INIT_POSITION, Quaternion.identity, transform);
                item.name = i + "_" + j;
                Debug.Log(item.GetComponent<Item>().id);
            }
        }
    }

    public void Move() {
        Debug.Log("MOVE");
    }

}