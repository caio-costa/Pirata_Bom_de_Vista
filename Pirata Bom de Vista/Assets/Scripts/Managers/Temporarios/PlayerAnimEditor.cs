using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PlayerAnimEditor : MonoBehaviour {

    public GameObject diretor;
    public GameObject conteudo;
    public bool addAnimCorte = false;

    [SerializeField]
    public CorteAnimacao corteAnimacao = new CorteAnimacao();

    private PlayerManager manager;
    private DirectorsManager content;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        corteAnimacao.indiceAnim = diretor.GetComponent<DirectorsManager>().GetIndiceAnimacao();

        if (addAnimCorte)
        {
            AddAnimacaoPirata();
        }

#endif
    }

    private void AddAnimacaoPirata()
    {

        manager = GetComponent<PlayerManager>();
        content = conteudo.GetComponent<DirectorsManager>();
        if (manager == null) return;
        if (corteAnimacao.clipeAnim == null) return;


        manager.listaAnimacao.Add(content.CopiaCorteAnimacao(corteAnimacao));
        Debug.Log("Animacao Adicionada");



        addAnimCorte = false;
        corteAnimacao.posicao = Vector3.zero;
        corteAnimacao.rotacao = Vector3.zero;

        corteAnimacao.clipeAnim = null;
        corteAnimacao.decisivo = false;
    }
}

