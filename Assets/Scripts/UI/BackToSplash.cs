﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToSplash : MonoBehaviour {

    public string sceneToLoad;

	public void Ok() {
        SceneManager.LoadScene(sceneToLoad);
    }

}