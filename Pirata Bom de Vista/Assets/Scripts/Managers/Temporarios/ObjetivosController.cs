using UnityEngine;
using System.Collections;


public class ObjetivosController : MonoBehaviour {

    public static ObjetivosController instance;

    public Objetivo objetivoAtual;
    private FaseAtual faseAtual;
    private bool firstLoad = true;
    [HideInInspector]
    public bool atualizouPosInicial = false;


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

        //TODO: usar indice de fase e de objetivo salvo.
        faseAtual = LoaderManager.instance.GetFaseCarregada();

        //objetivoAtual = ObjetivosContent.instance.GetObjetivoAtual(faseAtual);
        AtualizaObjetivoAtual();
        AtivaObjetivo();

    }
	
	// Update is called once per frame
	void Update () {
	
	}


    public void AtivaObjetivo() {
        StartCoroutine(AtivaObjtivo_CO());
        
    }

    private IEnumerator AtivaObjtivo_CO() {
        //Debug.Log("Vendo qual é o objetivo");
        while (FadeManager.instance == null) {
            yield return 0;
        }

        while (PlayerManager.instance == null) {
            yield return 0;
        }

        //Se for a primeira vez que o jogo é aberto, atualizar a posição imediatamente.
        if (SaveLoad.instance.loading) {
            if (firstLoad) {
                firstLoad = false;
                //PlayerManager.instance.AtualizaPosPlayerObjetivo(objetivoAtual.posCarregamentoSave);
            }
        }

        LevelManager.instance.DesativaInput();

        //Desativar todos os inputs caso esteja esperando o fade.
       
        
        if (objetivoAtual.Tipo == TipoObjetivo.CUTSCENE)
        {
            //PlayerManager.instance.SetPlayerState(PlayerState.CUTSCENE);
            if (!atualizouPosInicial) {
                atualizouPosInicial = true;
                PlayerManager.instance.AtualizaPosPlayerObjetivoCutscene(objetivoAtual.posCarregamentoSave);
            }
            

            if (FadeManager.instance != null)
            {
                while (FadeManager.instance.fading || SceneLoader.instance.esperandoFade)
                {
                    yield return 0;
                }
            }


            yield return new WaitForSeconds(0.6f);
            UIManager.instance.DesativaTodaUI();

            while (UIManager.instance.alterandoUI)
            {
                yield return null;
            }


            LevelManager.instance.AtivaCutscene();
        }
        else if (objetivoAtual.Tipo == TipoObjetivo.LUGAR)
        {
            bool mudou = false;
            yield return null;
            if (!DirectorsManager.instance.ativado) {
                mudou = true;
                atualizouPosInicial = true;
                PlayerManager.instance.AtualizaPosPlayerObjetivo(objetivoAtual.posCarregamentoSave);
            }


            while (DirectorsManager.instance == null)
            {
                yield return null;
            }

            while (DirectorsManager.instance.alterando)
            {
                yield return null;
            }


            yield return new WaitForSeconds(0.6f);
            if (!mudou) {
                //PlayerManager.instance.AtualizaPosPlayerObjetivo(objetivoAtual.posCarregamentoSave);
            }

            //Debug.Log("Lugar??");
            //TODO: Enquanto não mostrar objetivo, não movimentar.
            if (FadeManager.instance.fading || SceneLoader.instance.esperandoFade)
            {
                while (FadeManager.instance.fading || SceneLoader.instance.esperandoFade)
                {
                   yield return 0;
                }
            }

            UIManager.instance.AtivaTituloObjetivo(objetivoAtual.Titulo);
            LevelManager.instance.AtivaInput();
        }
        else if (objetivoAtual.Tipo == TipoObjetivo.COMBATE)
        {
            atualizouPosInicial = true;
            PlayerManager.instance.AtualizaPosPlayerObjetivo(objetivoAtual.posCarregamentoSave);
            PlayerManager.instance.SetPlayerState(PlayerState.COMBATE);
        }


        yield return null;
    }


    public void ProximoObjetivo() {

        if (faseAtual.indice < ObjetivosContent.instance.GetNumeroObjetivos(faseAtual.fase) - 1)
        {
            //Debug.Log("Passou objetivo");
            faseAtual.indice++;
            Objetivo objetivoTemp = ObjetivosContent.instance.CopiaObjetivo(objetivoAtual);
            AtualizaObjetivoAtual();

            if (objetivoTemp.Tipo == TipoObjetivo.CUTSCENE && objetivoAtual.Tipo != TipoObjetivo.CUTSCENE) {
                //Debug.Log("Entrou cutscene");
                DirectorsManager.instance.DesativaDiretor();
            }
            AtivaObjetivo();
        }
        else if (faseAtual.fase < ObjetivosContent.instance.GetNumeroFases() - 1)
        {
            //Debug.Log("Passou a fase");
            faseAtual.fase++;
            AtualizaObjetivoAtual();
            AtivaObjetivo();
        }
        else {
            //PlayerManager.instance.SetPlayerState(PlayerState.CUTSCENE);
            //LevelManager.instance.DesativaInput();
            Debug.Log("Fim de jogo!!");
        }

        SaveLoad.instance.Save();

    }

    public void AtualizaObjetivoAtual() {
        objetivoAtual = ObjetivosContent.instance.GetObjetivoAtual(faseAtual);
    }

    public FaseAtual GetFaseAtual() {
        FaseAtual temp = new FaseAtual();
        temp.fase = faseAtual.fase;
        temp.indice = faseAtual.indice;
        return temp;
    }

    public Objetivo GetObjetivoAtual() {
        return objetivoAtual;
    }
}
