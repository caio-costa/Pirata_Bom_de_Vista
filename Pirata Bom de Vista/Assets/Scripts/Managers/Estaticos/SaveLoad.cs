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
    private string path = "";

    public bool CarregarFaseFalsa = false;
    public SaveData faseParaCarregar = new SaveData();

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

        path = Application.persistentDataPath;

        //Debug.Log(path);
        if (loadDebug) {
            Load();
        }
	}


    public void Save() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path + "\\saveFile.pbv");
        SaveData data = new SaveData();
        data.faseAtual = LevelManager.instance.GetFaseAtual();
        data.indiceAnimacao = DirectorsManager.instance.GetIndiceAnimacao();
        data.indiceLegenda = DirectorsManager.instance.GetIndiceLegenda();

        Debug.Log("Salvou");

        bf.Serialize(file, data);
        file.Close();
        
    }

    public void Load() {
        if (CheckIfLoadExists())
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path + "\\saveFile.pbv", FileMode.Open);
            loadedData = (SaveData)bf.Deserialize(file);
            file.Close();
            if (CarregarFaseFalsa) {
                //Debug.Log("aLO");
                loadedData = faseParaCarregar;
            }

            loading = true;
            Debug.Log("Carregando jogo salvo");
        }else {
            loading = false;
            if (CarregarFaseFalsa) {
                loading = true;
                loadedData = faseParaCarregar;
            }
            
        }
    }

    public void Delete() {
        if (CheckIfLoadExists())
        {
            //Debug.Log(path + "\\saveFile.pbv");
            loadedData = new SaveData();
            File.Delete(path + "\\saveFile.pbv");
            
            
        }
    }

    public bool CheckIfLoadExists() {
        if (Directory.Exists(path))
        {
            if (File.Exists(path + "\\saveFile.pbv"))
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
    public int indiceLegenda = 0;
}
