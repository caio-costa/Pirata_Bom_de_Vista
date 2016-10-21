using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad : MonoBehaviour {

    public static SaveLoad instance;
    public SaveData loadedData;
    public bool loadDebug = true;
    public bool loading = false;

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

        if (loadDebug) {
            Load();
        }
	}


    public void Save() {
        BinaryFormatter bf = new BinaryFormatter();
        var pasta = Directory.CreateDirectory(Application.persistentDataPath + "/PirataBomDeVista/");
        FileStream file = File.Create(Application.persistentDataPath + "/PirataBomDeVista/" + "saveFile.pbv");
        SaveData data = new SaveData();
        data.faseAtual = LevelManager.instance.GetFaseAtual();
        data.indiceAnimacao = DirectorsManager.instance.GetIndiceAnimacao();

        //Debug.Log("Salvou");

        bf.Serialize(file, data);
        file.Close();
        
    }

    public void Load() {
        if (CheckIfLoadExists())
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/PirataBomDeVista/" + "saveFile.pbv", FileMode.Open);
            loadedData = (SaveData)bf.Deserialize(file);
            file.Close();
            loading = true;
            Debug.Log("Carregando jogo salvo");
        }else {
            loading = false;
        }
    }

    public void Delete() {
        if (CheckIfLoadExists())
        {
            File.Delete(Application.persistentDataPath + "/PirataBomDeVista/" + "saveFile.pbv");
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

    public SaveData GetLoadedData() {
        //Debug.Log(loadedData.faseAtual.indice);
        if (loadedData == null) {
            Load();
        }

        return loadedData;
    }
}

[Serializable]
public class SaveData {
    public FaseAtual faseAtual = new FaseAtual();
    public ConteudoAtual indiceAnimacao = new ConteudoAtual();
}
