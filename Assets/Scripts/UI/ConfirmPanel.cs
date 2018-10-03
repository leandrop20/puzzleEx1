using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmPanel : MonoBehaviour {

    public string levelToLoad;
    public Image[] stars;
    public int level;

    private void Start() {
        ActivateStars();
    }

    private void ActivateStars() {
        for (int i = 0; i < stars.Length; i++) {
            stars[i].enabled = false;
        }
    }

    public void Cancel() {
        this.gameObject.SetActive(false);
    }

    public void Play() {
        PlayerPrefs.SetInt("Current Level", level - 1);
        SceneManager.LoadScene(levelToLoad);
    }

}