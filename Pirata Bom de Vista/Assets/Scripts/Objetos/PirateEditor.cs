using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PirateEditor : MonoBehaviour
{

    public GameObject diretor;
    public DirectorsManager diretorManager;
    public bool addAnimCorte = false;

    [SerializeField]
    public CorteAnimacao corteAnimacao = new CorteAnimacao();


    private PirateBehaviour instanciaPirata;



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        corteAnimacao.indiceAnim = diretorManager.indiceConteudoAtual;

        if (addAnimCorte)
        {
            AddAnimacaoPirata();
        }

#endif
    }

    private void AddAnimacaoPirata()
    {

        instanciaPirata = GetComponent<PirateBehaviour>();
        if (instanciaPirata == null)
        {
            addAnimCorte = false;
            corteAnimacao.clipeAnim = null;
            corteAnimacao.decisivo = false;
            Debug.Log("Erro ao adicionar a animação");
            return;
        }
        if (corteAnimacao.clipeAnim == null)
        {
            addAnimCorte = false;
            corteAnimacao.clipeAnim = null;
            corteAnimacao.decisivo = false;
            Debug.Log("Adicione um clipe");
            return;
        }


        instanciaPirata.listaAnimacao.Add(diretorManager.CopiaCorteAnimacao(corteAnimacao));
        Debug.Log("Animacao Adicionada");



        addAnimCorte = false;
        corteAnimacao.clipeAnim = null;
        corteAnimacao.decisivo = false;
    }
}
