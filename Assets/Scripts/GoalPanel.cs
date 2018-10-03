using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalPanel : MonoBehaviour {

    public Image thisImage;
    public Sprite thisSprite;
    public Text thisText;
    public string thisString;

    private void Start() {
        Setup();
    }

    private void Setup() {
        thisImage.sprite = this.thisSprite;
        thisText.text = thisString;
    }

}