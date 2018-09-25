using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Item : MonoBehaviour {

    [HideInInspector]
    public string id;

    private Vector2 initPosition;
    private float DRAG_LIMIT = 0.7f;
    private bool isDragging = false;

    void Awake() {
        id = name;
    }

    void OnMouseDown() {
        initPosition = transform.position;
        isDragging = true;
    }

    void OnMouseUp() {
        isDragging = false;
        Back();
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log("COLISSION: " + collision.gameObject.name);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        Debug.Log("TRIGGER: " + collision.gameObject.name);
    }

    void Update() {
        if (isDragging) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x, mousePosition.y, -1f);

            if ((initPosition.x + DRAG_LIMIT) < transform.position.x) {
                Debug.Log("DIREITA");
                Back();
            } else if ((initPosition.x - DRAG_LIMIT) > transform.position.x) {
                Debug.Log("ESQUERDA");
                Back();
            } else if ((initPosition.y + DRAG_LIMIT) < transform.position.y) {
                Debug.Log("CIMA");
                Back();
            } else if ((initPosition.y - DRAG_LIMIT) > transform.position.y) {
                Debug.Log("BAIXO");
                Back();
            }
        }
    }

    void Back() {
        transform.DOMove(initPosition, 0.3f);
    }

}