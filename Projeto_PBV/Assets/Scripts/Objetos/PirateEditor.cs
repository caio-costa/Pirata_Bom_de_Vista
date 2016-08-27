using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PirateEditor : MonoBehaviour {

    public GameObject diretor;
    public DirectorsManager diretorManager;
    public bool addAnimCorte = false;
    
    [SerializeField]
    public CorteAnimacao corteAnimacao = new CorteAnimacao();


    private PirateBehaviour instanciaPirata;



    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update() {

#if UNITY_EDITOR
        corteAnimacao.indiceAnim = diretor.GetComponent<DirectorsContent>().numeroPos;

        if (addAnimCorte) {
            AddAnimacaoPirata();
        }

#endif
    }

    private void AddAnimacaoPirata() {

        instanciaPirata = GetComponent<PirateBehaviour>();
        if (instanciaPirata == null)
        {
            addAnimCorte = false;
            corteAnimacao.clipeAnim = null;
            corteAnimacao.decisivo = false;

            return;
        }
        if (corteAnimacao.clipeAnim == null) return;


        instanciaPirata.listaAnimacao.Add(diretorManager.CopiaCorteAnimacao(corteAnimacao));
        Debug.Log("Animacao Adicionada");



        addAnimCorte = false;
        corteAnimacao.clipeAnim = null;
        corteAnimacao.decisivo = false;
    }


}
