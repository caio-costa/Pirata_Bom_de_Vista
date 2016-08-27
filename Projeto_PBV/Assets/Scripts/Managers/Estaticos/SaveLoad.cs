using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad : MonoBehaviour {

    public static SaveLoad instance;
    public int numero = 3;

	// Use this for initialization
	void Start () {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this) {
            Destroy(this.gameObject);
        }

	}


    public void Save() {
        BinaryFormatter bf = new BinaryFormatter();
        var pasta = Directory.CreateDirectory(Application.persistentDataPath + "/PirataBomDeVista/");
        FileStream file = File.Create(Application.persistentDataPath + "/PirataBomDeVista/" + "saveFile.pbv");
        SaveData data = new SaveData();
        data.numeroTeste = numero;

        bf.Serialize(file, data);
        file.Close();
        
    }

    public void Load() {
        if (CheckIfLoadExists())
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/PirataBomDeVista/" + "saveFile.pbv", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            
            Debug.Log(data.numeroTeste);
        }        
    }

    public void Delete() {
        if (CheckIfLoadExists()) {
            File.Delete(Application.persistentDataPath + "/PirataBomDeVista/" + "saveFile.pbv");
            //Debug.Log("deletou");
        }

    }

    public bool CheckIfLoadExists() {
        if (Directory.Exists(Application.persistentDataPath + "/PirataBomDeVista/"))
        {
            if (File.Exists(Application.persistentDataPath + "/PirataBomDeVista/" + "saveFile.pbv"))
            {
                return true;
            }
            else {
                return false;
            }
        }
        return false;
    }
}

[Serializable]
class SaveData {
    public int numeroTeste = 0;
}
