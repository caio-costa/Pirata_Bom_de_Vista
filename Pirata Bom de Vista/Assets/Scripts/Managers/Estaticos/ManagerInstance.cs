using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManagerInstance : MonoBehaviour {

    public static ManagerInstance instance;
    private string[] classes = new string[] { "SceneLoader", "FadeManager", "SaveLoad" };

    // Use this for initialization
    void Start() {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            DestroiObjetosIguais();
        }
    }

    private void DestroiObjetosIguais() {
        GameObject[] filhosDoOutro = new GameObject[this.transform.childCount];
        GameObject[] filhosDeste = new GameObject[ManagerInstance.instance.gameObject.transform.childCount];
        List<string> classesDeste = new List<string>();

        //Add os filhos em array
        for (int i = 0; i < filhosDeste.Length; i++)
        {
            filhosDeste[i] = ManagerInstance.instance.gameObject.transform.GetChild(i).gameObject;
            //Debug.Log("Filho: " + i + "\nNome: " + filhosDeste[i]);

            //Verifica quais tipos de script são filhos deste
            for (int a = 0; a < classes.Length; a++) {
                if (filhosDeste[i].GetComponent(classes[a]) != null) {
                    classesDeste.Add(classes[a]);
                    //Debug.Log("Objeto que existia na cena anterior: " + classes[a].ToString());
                }
            }
        }


        //Add os filhos em array
        for (int i = 0; i < filhosDoOutro.Length; i++)
        {
            filhosDoOutro[i] = this.transform.GetChild(i).gameObject;

            bool existe = false;
            //Verifica se existe scripts diferentes deste
            for (int a = 0; a < classesDeste.Count; a++) {
                if (filhosDoOutro[i].GetComponent(classesDeste[a]) != null)
                {
                    existe = true;
                    break;
                }
            }

            if (!existe) {
                filhosDoOutro[i].transform.SetParent(ManagerInstance.instance.gameObject.transform);
                //Debug.Log("Um filho não existia antes: " + filhosDoOutro[i]);
            }
        }

        Destroy(this.gameObject);

    }

}
