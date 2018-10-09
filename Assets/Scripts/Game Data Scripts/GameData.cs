using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData {

    public bool[] isActive;
    public int[] highScores;
    public int[] stars;

}

public class GameData : MonoBehaviour {

    public static GameData gameData;
    public SaveData saveData;

    private void Awake() {
        //Reset();////////////////////////////////////////////////////////////////////
        if (gameData == null) {
            DontDestroyOnLoad(this.gameObject);
            gameData = this;
        } else {
            Destroy(this.gameObject);
        }
        Load();
    }

    public void Save() {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/player.dat");
        SaveData data = new SaveData();
        data = saveData;
        formatter.Serialize(file, data);
        file.Close();
    }

    public void Load() {
        if (File.Exists(Application.persistentDataPath + "/player.dat")) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
        }
    }

    public void Reset() {
        if (File.Exists(Application.persistentDataPath + "/player.dat")) {
            File.Delete(Application.persistentDataPath + "/player.dat");
        }
    }

    private void OnApplicationQuit() {
        Save();
    }

    private void OnDisable() {
        Save();
    }

}