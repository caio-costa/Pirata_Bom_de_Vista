using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ObjetivosContent : MonoBehaviour {
    public static ObjetivosContent instance;


    [SerializeField]
    public ListaObjetivos[] fases;

	// Use this for initialization
	void Start () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }


    }
	
	// Update is called once per frame
	void Update () {

#if UNITY_EDITOR

        for (int a = 0; a < fases.Length; a++)
        {
            for (int i = 0; i < fases[a].TodosObjetivos.Count; i++)
            {
                //Debug.Log("i: " + fases[a].TodosObjetivos.Count);
                Objetivo ObjetivoTemp = fases[a].GetObjetivo(i);
                if (ObjetivoTemp.transformSalvo == false && ObjetivoTemp.posCarregamento != null)
                {
                    ObjetivoTemp.posCarregamentoSave.position = ObjetivoTemp.posCarregamento.position;
                    ObjetivoTemp.posCarregamentoSave.rotation = ObjetivoTemp.posCarregamento.rotation;
                    ObjetivoTemp.posCarregamentoSave.localScale = ObjetivoTemp.posCarregamento.localScale;
                    ObjetivoTemp.transformSalvo = true;
                }
                else if (ObjetivoTemp.posCarregamento == null)
                {

                    ObjetivoTemp.transformSalvo = false;
                }
            }
        }
        
    
    
#endif
    }

    public Objetivo GetObjetivoAtual(FaseAtual faseAtual) {
        return CopiaObjetivo(fases[faseAtual.fase].GetObjetivo(faseAtual.indice));
    }

    public Objetivo CopiaObjetivo(Objetivo original) {
        Objetivo temporario = new Objetivo();
        
        temporario.Titulo = original.Titulo;
        temporario.Descricao = original.Descricao;
        temporario.Tipo = original.Tipo;
        temporario.podeSerCarregado = original.podeSerCarregado;
        temporario.transformSalvo = original.transformSalvo;
        temporario.posCarregamento = original.posCarregamento;


        if (temporario.podeSerCarregado && temporario.transformSalvo == true) { 
            temporario.posCarregamentoSave.position = original.posCarregamentoSave.position;
            temporario.posCarregamentoSave.rotation = original.posCarregamentoSave.rotation;
            temporario.posCarregamentoSave.localScale = original.posCarregamentoSave.localScale;
        }


        return temporario;
    }

    public int GetNumeroObjetivos(int temp) {
        return fases[temp].TodosObjetivos.Count;
    }

    public int GetNumeroFases() {
        return fases.Length;
    }
}
