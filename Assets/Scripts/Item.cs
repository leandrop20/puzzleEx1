using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Item : MonoBehaviour {

    private Vector2 initPosition;
    private float DRAG_LIMIT = 0.7f;
    private float MOVE_LIMIT = 0.1f;
    private bool isDragging = false;
    private bool inAnimate = false;

    void OnMouseDown() {
        if (!isDragging && !inAnimate) {
            initPosition = transform.position;
            isDragging = true;
        }
    }

    void OnMouseUp() {
        if (isDragging) {
            CallChangeObject(GetDirection());
        }
    }

    void Update() {
        if (isDragging && !inAnimate) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (CanMove(mousePosition)) {
                transform.position = new Vector3(mousePosition.x, mousePosition.y, -1f);
            }
        }
    }

    public void CallChangeObject(Puzzle.DIRECTION direction) {
        isDragging = false;
        if (!GetComponentInParent<Puzzle>().ChangeObject(gameObject, initPosition, direction)) {
            Back();
        }
    }

    float GetAngle(Vector2 a, Vector2 b) {
        Vector2 direction = (Vector2)b - a;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        return angle;
    }

    Puzzle.DIRECTION GetDirection() {
        float angle = GetAngle(initPosition, transform.position);
        if (angle >= 0 && angle <= 45  || angle <= 360 && angle >= 315) {
            return Puzzle.DIRECTION.RIGHT;
        } else if (angle >= 135 && angle <= 225) {
            return Puzzle.DIRECTION.LEFT;
        } else if (angle < 135 && angle > 45) {
            return Puzzle.DIRECTION.UP;
        } else if  (angle > 225 && angle < 315) {
            return Puzzle.DIRECTION.BOTTOM;
        }
        return Puzzle.DIRECTION.NULL;
    }

    bool CanMove(Vector2 mousePosition) {
        if ((initPosition.x + DRAG_LIMIT) > mousePosition.x &&
            (initPosition.x - DRAG_LIMIT) < mousePosition.x &&
            (initPosition.y + DRAG_LIMIT) > mousePosition.y &&
            (initPosition.y - DRAG_LIMIT) < mousePosition.y) {
            return true;
        }
        return false;
    }

    public void Move(Vector2 p) {
        inAnimate = true;
        transform.DOMove(p, 0.3f).OnComplete(() => {
            inAnimate = false;
            initPosition = transform.position;
        });
    }

    void Back() {
        inAnimate = true;
        transform.DOMove(initPosition, 0.3f).OnComplete(() => { inAnimate = false; });
    }

}