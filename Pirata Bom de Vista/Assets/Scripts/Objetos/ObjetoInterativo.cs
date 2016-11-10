using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjetoInterativo : MonoBehaviour {

    public List<FaseAtual> listaFases = new List<FaseAtual>();
    public bool mexeMoedas = false;
    private AudioSource _audioSource;

	// Use this for initialization
	void Start () {    
        _audioSource = transform.GetChild(1).GetComponent<AudioSource>();


        if (mexeMoedas)
        {
            StartCoroutine(AtivaSomMoedasMexendo());
        }
    }


    void OnEnable()
    {
        ObjetivosController.eventoNovoObjetivo += Ativar;
    }

    void OnDisable()
    {
        ObjetivosController.eventoNovoObjetivo -= Ativar;
    }

    public void Ativar() {
        MudaFilhos(CheckIfFazParte());
   
    }

    public void MudaFilhos(bool temp) {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(temp);
        }
    }

    public bool CheckIfFazParte() {
        FaseAtual faseAtual = ObjetivosController.instance.GetFaseAtual();
        for (int i = 0; i < listaFases.Count; i++)
        {
            if (listaFases[i].fase == faseAtual.fase && listaFases[i].indice == faseAtual.indice)
            {
                //Debug.Log("Faz parte");
                return true;
            }
        }

        return false;
    }

    public IEnumerator AtivaSomMoedasMexendo() {

        yield return null;
        //Debug.Log("fora");
        while (CheckIfFazParte()) {
            //Debug.Log("Parte");
            if (!PlayerManager.instance.movimentando)
            {
                _audioSource.Stop();
            }
            else if (_audioSource.isPlaying == false) {
                //Debug.Log("Dentro");
                _audioSource.Play();
            }

            yield return null;
        }

        _audioSource.Stop();
    }

}
