﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource[] destroyNoise;

    public void PLayRandomDestroyNoise() {
        int clipToPlay = Random.Range(0, destroyNoise.Length);
        destroyNoise[clipToPlay].Play();
    }

}