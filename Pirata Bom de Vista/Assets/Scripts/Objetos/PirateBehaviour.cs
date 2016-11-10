using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public enum PirataState {
    IDLE,
    CUTSCENE
}

[Serializable]
public class PirateBehaviour : MonoBehaviour
{

    public Transform posMaos;
    public GameObject ultimoObjeto;
    public GameObject ultimoDestino;


    [SerializeField]
    public List<CorteAnimacao> listaAnimacao = new List<CorteAnimacao>();
    public List<PosObjetivos> listaPosicoes = new List<PosObjetivos>();


    private int indiceNome = 1;
    private Animator animator;
    private LevelManager.CutsceneDelegate myDelegate;
    private CorteAnimacao corteAtual;
    private Transform myTransform;
    private PosObjetivos posObjetivoAtual = new PosObjetivos();
    private AudioSource _audioSource;

    // Use this for initialization
    void Start()
    {

        
        
        //PlayAnimacaoCutscene();
        myTransform = this.transform;
        animator = myTransform.GetChild(0).GetChild(1).GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        LevelManager.eventoCutscene += PlayAnimacaoCutscene;
        ObjetivosController.eventoNovoObjetivo += ConfiguraPosObjetivo;
    }

    void OnDisable()
    {
        LevelManager.eventoCutscene -= PlayAnimacaoCutscene;
        ObjetivosController.eventoNovoObjetivo -= ConfiguraPosObjetivo;
    }


    public void ConfiguraPosObjetivo()
    {
        //Debug.Log("Novo Objetivo, mudar de pos");


        //if (!CheckSeFazPartePosicaoObjetivo()) { return; }
        //posObjetivoAtual

    }

    public bool CheckSeFazPartePosicaoObjetivo()
    {
        FaseAtual faseAtual = ObjetivosController.instance.GetFaseAtual();

        for (int i = 0; i < listaPosicoes.Count; i++)
        {
            if (listaPosicoes[i].faseCorrespondente.fase == faseAtual.fase && listaPosicoes[i].faseCorrespondente.indice == faseAtual.indice)
            {
                //posObjetivoAtual = CopiaPosObjetivos(posObjetivoAtual);
                //Debug.Log("Faz parte");
                return true;
            }
        }

        return false;
    }


    public void AtivaAudio()
    {
        if (DirectorsManager.instance == null) { return; }

        StartCoroutine(TextoLegenda());
        //DirectorsManager.instance.ProximaLegenda();
        //AtivaFala();
    }

    private IEnumerator TextoLegenda()
    {
        
        DirectorsManager.instance.ProximaLegenda();
        
        yield return new WaitForSeconds(0.5f);
        AtivaFala();
    }

    public void AtivaFala()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }

        if (corteAtual.fala == null) return;

        _audioSource.clip = corteAtual.fala;
        _audioSource.Play();
    }

    public void PlayAnimacaoCutscene()
    {
        ConteudoAtual indiceAnim = DirectorsManager.instance.indiceConteudoAtual;


        if (FazParteAnimacao(indiceAnim))
        {

            RuntimeAnimatorController myController = animator.runtimeAnimatorController;
            AnimatorOverrideController anim_OverrideController = new AnimatorOverrideController();
            anim_OverrideController.runtimeAnimatorController = myController;
            AnimationClip clipe = corteAtual.clipeAnim;
            //Debug.Log(clipe.name);
            string nome = "";
            //for (int i = 0; i < anim_OverrideController.clips.Length; i++) {
            //Debug.Log(anim_OverrideController.clips[i].overrideClip.name.ToString());
            //  if (anim_OverrideController.clips[i].ToString() != "Idle") {

            //} 
            //}

            /*
            if (indiceNome > 1)
            {
                
                nome = "ct_" + (indiceNome - 1).ToString();
            }
            else {
                nome = "ct_" + (indiceNome).ToString();
            }


            //Debug.Log("Nome anterior: " + nome);
            indiceNome++;
            */

            anim_OverrideController[nome] = clipe;
            animator.runtimeAnimatorController = anim_OverrideController;

            StartCoroutine(AtivaAnimacaoCutscene());

            if (corteAtual.decisivo == true)
            {
                //Debug.Log("Decisivo");
                StartCoroutine(EsperaTerminoAnimacao());
            }
        }

    }

    private IEnumerator AtivaAnimacaoCutscene()
    {
        yield return new WaitForSeconds(DirectorsManager.instance.tempoAguardoCutscenes);
        animator.SetTrigger("Cutscene");
    }

    private IEnumerator EsperaTerminoAnimacao()
    {
        DirectorsManager.instance.IniciarAnimDecisiva();

        

        //Aguarda o começo
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            yield return null;
        }

        //Aguarda o termino
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("ct_Anim"))
        {
            yield return null;
        }



        DirectorsManager.instance.AcabarAnimDecisiva();

        yield return 0;
    }

    public bool FazParteAnimacao(ConteudoAtual indice)
    {
        //Debug.Log(indice);
        for (int i = 0; i < listaAnimacao.Count; i++)
        {
            if (indice.fase == listaAnimacao[i].indiceAnim.fase)
            {
                if (indice.cutscene == listaAnimacao[i].indiceAnim.cutscene)
                {
                    if (indice.corte == listaAnimacao[i].indiceAnim.corte)
                    {
                        corteAtual = DirectorsManager.instance.CopiaCorteAnimacao(listaAnimacao[i]);
                        //Debug.Log("Marujo faz parte");
                        return true;
                    }
                }
            }
        }

        corteAtual = new CorteAnimacao();
        return false;
    }

    public void AtivaLegenda()
    {
        if (DirectorsManager.instance == null) { return; }
        DirectorsManager.instance.ProximaLegenda();
    }



    public void PassaObjeto()
    {
        ultimoObjeto.transform.SetParent(ultimoDestino.transform);
    }

    public void NovoObjeto()
    {

    }



    public void DesativaObjeto()
    {

        for (int i = 0; i < posMaos.childCount; i++)
        {
            if (posMaos.GetChild(i).gameObject.CompareTag("ObjetoCutscene"))
            {
                posMaos.GetChild(i).position = ultimoDestino.transform.position;
                posMaos.GetChild(i).SetParent(ultimoDestino.transform);

                break;
            }
        }
    }
}


