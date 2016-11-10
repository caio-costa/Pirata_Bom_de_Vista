using UnityEngine;
using System.Collections;

public class LoaderManager : MonoBehaviour {

    public static LoaderManager instance;
    public SaveData loadedData;

	// Use this for initialization
	void Start () {
        instance = this;
        loadedData = SaveLoad.instance.loadedData;

        //AtualizaFaseCarregada();
    }

    public void AtualizaFaseCarregada() {
        FaseAtual loadedFase = loadedData.faseAtual;

        for (int i = 0; i < 15; i++)
        {
            if (ObjetivosContent.instance.GetObjetivoAtual(loadedFase).podeSerCarregado == false)
            {
                if (loadedFase.indice > 0)
                {
                    loadedFase.indice--;
                }
                else
                {
                    loadedFase.indice = 0;
                    loadedFase.fase--;
                }
            }
            else
            {
                break;
            }
        }



        loadedData.faseAtual = loadedFase;
    }

    public FaseAtual GetFaseCarregada() {
        AtualizaFaseCarregada();
        return loadedData.faseAtual;
    }

    public ConteudoAtual GetIndiceConteudo()
    {
        AtualizaFaseCarregada();
        return loadedData.indiceAnimacao;
    }

    public int GetIndiceLegenda() {
        return loadedData.indiceLegenda;
    }
}
