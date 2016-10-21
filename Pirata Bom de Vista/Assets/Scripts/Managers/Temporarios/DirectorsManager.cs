using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[Serializable]
public class CorteAnimacao
{
    public ConteudoAtual indiceAnim = new ConteudoAtual();
    public bool decisivo = false;
    public AnimationClip clipeAnim;
    public Vector3 posicao, rotacao;
    public Transform objetoPassado;
    public Transform destinoObjeto;
}


public class DirectorsManager : MonoBehaviour {

    public static DirectorsManager instance;
    private Camera camera;
    public bool ativar = false;
    [HideInInspector]
    public bool ativado = false;
    private float tamanho = 0.8f, posY = 0;

    private float tempoAnim = 1f;
    private float currentLerpTime;
    [HideInInspector]
    public bool alterando = false;
    [HideInInspector]
    public bool rodandoCorte = false;

    [HideInInspector]
    public bool terminaCorteEmergencial = false;
    //public int indiceAnimacao = 0;
    private int indiceLegenda = 0;

    public ConteudoAtual indiceConteudoAtual = new ConteudoAtual();

    [HideInInspector]
    public bool temDecisivo = false;
    [HideInInspector]
    public bool esperandoAnim = false;
    [HideInInspector]
    public float tempoAguardoCutscenes = 0.6f;
    public bool fadeAposCutscene = true;

    private LegendaContainer legendas;

    public Text textoLegenda;

    private bool ativarInicio = true;

    // Use this for initialization
    void Start () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this.gameObject);
        }

        camera = Camera.main;
        posY = (1 - tamanho) / 2;

        //Debug.Log(SaveLoad.instance.loadedData.faseAtual.indice);

        //indiceAnimacao = SaveLoad.instance.loadedData.indiceAnimacao;
        //indiceAnimacao = LoaderManager.instance.GetIndiceAnimacao();
        indiceConteudoAtual = LoaderManager.instance.GetIndiceConteudo();

        legendas = LegendaContainer.Load();
        if (ativarInicio) {
            //StartCoroutine(AtivarComeco());
        }
        
        //ProximaLegenda();
    }

    void Update() {
        if (ativar) {
            ativar = false;
            AtivaDiretor();
        }
    }

    public void AtivaDiretor() {
        //Debug.Log("Ativou diretor");
        if (!alterando) {
            if (ativado == false)
            {
                StartCoroutine(Ativar());
            }
            else
            {
                StartCoroutine(Desativar());
            }
        }
    }

    public void DesativaDiretor() {
        if (!alterando)
        {
            StartCoroutine(Desativar());
        }
    }

    public void ProximaCutscene() {
        //Debug.Log("Chamou a outra");
        if (!rodandoCorte) {
            if (DirectorsContent.instance.GetNumeroCortes(indiceConteudoAtual) > indiceConteudoAtual.corte) {

            }
            CorteSave corte = DirectorsContent.instance.GetCorte(indiceConteudoAtual);

            if (corte == null) {
                NovaCutsceneAdjust();
                ObjetivosController.instance.ProximoObjetivo();

                return;
            }
            //Debug.Log("Entrou");
            LevelManager.instance.EventoCutscene();
            StartCoroutine(AtivaCorte(corte));
        }  
    }

    public IEnumerator FadeTerminoCutscene() {

        while (FadeManager.instance.fading) {
            yield return null;
        }

        
        FadeManager.instance.Fade(FadeMode.IN);
        yield return null;

        while (FadeManager.instance.fading)
        {
            yield return null;
        }


        PlayerManager.instance.AtualizaPosPlayerObjetivo(ObjetivosController.instance.GetObjetivoAtual().posCarregamentoSave);
        yield return new WaitForSeconds(0.6f);

        alterando = false;
        FadeManager.instance.Fade(FadeMode.OUT);
        yield return null;

        while (FadeManager.instance.fading)
        {
            yield return null;
        }


    }

    public void NovaCutsceneAdjust() {
        indiceConteudoAtual.corte = 0;
        if (indiceConteudoAtual.cutscene < DirectorsContent.instance.GetNumeroCutscenes(indiceConteudoAtual) - 1) {
            indiceConteudoAtual.cutscene++;
        } else if (indiceConteudoAtual.fase < DirectorsContent.instance.GetNumeroFases() - 1) {
            indiceConteudoAtual.fase++;
        } else {
            Debug.Log("Acabaram todas cutscenes do jogo");
        }
    }

    public void ProximaLegenda() {
        textoLegenda.gameObject.SetActive(true);
        textoLegenda.text = legendas.Legendas[indiceLegenda].texto;
        indiceLegenda++;
        
    }

    public void ZeraLegenda() {
        textoLegenda.gameObject.SetActive(false);
        textoLegenda.text = "";
    }


    private IEnumerator AtivaCorte(CorteSave corte) {
        rodandoCorte = true;
        terminaCorteEmergencial = false;
        //Debug.Log("Indice: " + indiceAnimacao + "\nPosCamera: " + corte.posicao);


        PlayerManager.instance.AtualizaPosPlayerObjetivo(ObjetivosController.instance.GetObjetivoAtual().posCarregamentoSave);
        GameObject camera = Camera.main.gameObject;
        Transform cameraTransform = camera.transform;

        camera.transform.position = corte.posicao;
        if (corte.lookAt == null)
        {
            camera.transform.rotation = corte.rotacao;
        }
        else {
            camera.transform.LookAt(corte.lookAt);
        }

        
        if (temDecisivo)
        {
            //Debug.Log("Tem decisivo");
            esperandoAnim = true;
        }


        if (corte.tempoViagem <= 0) {
            corte.tempoViagem = 1;
        }

        float viagem = corte.tempoViagem;
        float currentLerpTime = 0;
        float perc = 0;

        if (corte.fixa == false)
        {
            if (corte.posFinal != null)
            {
                if (corte.delay)
                {
                    float temp = 0;
                    while (temp < corte.tempoDelay)
                    {
                        temp += Time.deltaTime;
                        yield return 0;
                    }
                }

                while (perc < 0.95f)
                {
                    if (!terminaCorteEmergencial)
                    {
                        currentLerpTime += Time.deltaTime;
                        if (currentLerpTime > viagem)
                        {
                            currentLerpTime = viagem;
                        }

                        //lerp!
                        perc = currentLerpTime / viagem;
                        if (corte.velocidadeGradativa)
                        {
                            cameraTransform.position = Vector3.Lerp(corte.posicao, corte.posFinal.position, Mathf.SmoothStep(0.0f, 1.0f, perc));
                        }
                        else
                        {
                            cameraTransform.position = Vector3.Lerp(corte.posicao, corte.posFinal.position, perc);
                        }


                        if (corte.lookAt != null)
                        {
                            camera.transform.LookAt(corte.lookAt);
                        }
                    }
                    else
                    {
                        terminaCorteEmergencial = false;
                        break;
                    }


                    yield return null;
                }
            }
        }

        yield return null;

        if (temDecisivo) {
            //esperandoAnim = true;

            while (esperandoAnim) {
                //Debug.Log("Esperando animacao decisiva terminar");
                yield return null;
            }
        }

        ZeraLegenda();
        yield return new WaitForSeconds(tempoAguardoCutscenes);
        


        if (corte.delayTermino > 0)
        {
            yield return new WaitForSeconds(corte.delayTermino);
        }


        yield return null;
        rodandoCorte = false;
        yield return null;
        indiceConteudoAtual.corte++;
        ProximaCutscene();
    }

    private IEnumerator Ativar() {
        ativado = true;
        alterando = true;

        Vector2 WH_final = new Vector2(1, tamanho);
        Vector2 XY_final = new Vector2(0, posY);
        Vector2 WH_inicial = Vector2.one;
        Vector2 XY_inicial = Vector2.zero;

        camera.rect = new Rect(XY_inicial, WH_inicial);
        float perc = 0;
        currentLerpTime = 0;

        while (perc < 1) {

            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > tempoAnim)
            {
                currentLerpTime = tempoAnim;
            }

            perc = currentLerpTime / tempoAnim;

            camera.rect = new Rect(Vector2.Lerp(XY_inicial, XY_final, perc), Vector2.Lerp(WH_inicial, WH_final, perc));

            yield return null;
        }

        camera.rect = new Rect(XY_final, WH_final);
        yield return new WaitForSeconds(tempoAguardoCutscenes);
        alterando = false;
        ProximaCutscene();
    }

    private IEnumerator Desativar() {
        alterando = true;

        Vector2 WH_inicial = new Vector2(1, tamanho);
        Vector2 XY_inicial = new Vector2(0, posY);
        Vector2 WH_final = Vector2.one;
        Vector2 XY_final = Vector2.zero;

        camera.rect = new Rect(XY_inicial, WH_inicial);
        float perc = 0;
        currentLerpTime = 0;

        while (perc < 1)
        {

            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > tempoAnim)
            {
                currentLerpTime = tempoAnim;
            }

            perc = currentLerpTime / tempoAnim;

            camera.rect = new Rect(Vector2.Lerp(XY_inicial, XY_final, perc), Vector2.Lerp(WH_inicial, WH_final, perc));

            yield return null;
        }


        camera.rect = new Rect(XY_final, WH_final);
        ativado = false;
        yield return null;

        if (fadeAposCutscene) {
            StartCoroutine(FadeTerminoCutscene());
        }else
        {
            alterando = false;
        }

        
    }

    private IEnumerator AtivarComeco() {

        yield return 0;
        LevelManager.instance.SetEstadoAtual(EstadoAtual.CUTSCENE);
        PlayerManager.instance.SetPlayerState(PlayerState.CUTSCENE);

        if (FadeManager.instance != null) {
            while (FadeManager.instance.fading || SceneLoader.instance.esperandoFade) {
                yield return 0;
            }
        }
        
        yield return new WaitForSeconds(1);
        LevelManager.instance.AtivaCutscene();
        yield return null;

        while (alterando) {
            yield return null;
        }

        //ProximaCutscene();
    }

    /// <summary>
    /// Retorna o indice da animação atual
    /// </summary>
    /// <returns></returns>
    public ConteudoAtual GetIndiceAnimacao() {
        //Debug.Log("indice anim: " + indiceAnimacao);
        return indiceConteudoAtual;
    }

    
    public CorteAnimacao CopiaCorteAnimacao(CorteAnimacao original)
    {
        CorteAnimacao copia = new CorteAnimacao();

        copia.clipeAnim = original.clipeAnim;
        copia.indiceAnim = original.indiceAnim;
        copia.decisivo = original.decisivo;
        copia.objetoPassado = original.objetoPassado;
        copia.destinoObjeto = original.destinoObjeto;
        copia.posicao = original.posicao;
        copia.rotacao = original.rotacao;

        return copia;
    }

    public void IniciarAnimDecisiva()
    {
        temDecisivo = true;
    }

    public void AcabarAnimDecisiva() {
        esperandoAnim = false;
        temDecisivo = false;
    }

    public ConteudoAtual GetCorteAtual() {
        return indiceConteudoAtual;
    }

    public Vector3 GetPosCameraLoadCutscene() {
        return Vector3.zero;
    }
}
