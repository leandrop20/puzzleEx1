using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {

    [Header("Active Stuff")]
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;
    private  Image buttonImage;
    private Button myButton;

    public Image[] stars;
    public Text levelText;
    public int level;
    public GameObject confirmPanel;

    private void Start() {
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
        levelText.enabled = true;
        ActivateStars();
        ShowLevel();
        DecideSprite();
    }

    private void ActivateStars() {
        for (int i = 0; i < stars.Length; i++) {
            stars[i].enabled = false;
        }
    }

    private void DecideSprite() {
        if (isActive) {
            buttonImage.sprite = activeSprite;
            myButton.enabled = true;
            levelText.enabled = true;
        } else {
            buttonImage.sprite = lockedSprite;
            myButton.enabled = false;
            levelText.enabled = false;
        }
    }

    private void ShowLevel() {
        levelText.text = "" + level;
    }

    public void ConfirmPanel(int level) {
        confirmPanel.GetComponent<ConfirmPanel>().level = level;
        confirmPanel.SetActive(true);
    }

}