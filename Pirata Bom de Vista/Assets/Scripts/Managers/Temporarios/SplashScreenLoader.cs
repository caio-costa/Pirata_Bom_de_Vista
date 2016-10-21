using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreenLoader : MonoBehaviour {

    public static SplashScreenLoader instance;

    public float velocidadeIN = 0.5f, esperaINOUT = 2, multEscala = 1.5f, tempoINOUT = 2f, esperaCadaSplash = 2;
    public int numeroSplashes = 2;
    public Transform canvasPai;
    public bool loadSegundoPlano = false;

    private int indiceSplash = 0;
    private float contador = 0;
    private RawImage[] splashes;
    private float tempoTotal = 0;
    private bool fimSplashes = false;
    private AsyncOperation sync;

    // Use this for initialization
    void Start () {
        instance = this;

        splashes = new RawImage[numeroSplashes];
        for (int i = 0; i < numeroSplashes; i++) {
            splashes[i] = canvasPai.GetChild(i).gameObject.GetComponent<RawImage>();
            splashes[i].gameObject.SetActive(false);
        }

       
        tempoTotal = esperaINOUT + tempoINOUT*2;

        StartCoroutine(SplashIN());
        StartCoroutine(SplashScale());

        if (loadSegundoPlano)
        {
            sync = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
            sync.allowSceneActivation = false;
        }
    }


    void Update()
    {
        if (loadSegundoPlano)
        {
            if (sync.progress >= 0.9f && fimSplashes)
            {
                sync.allowSceneActivation = true;
                Debug.Log("Carregou");
            }
        }
    }

    private IEnumerator SplashScale() {
        int indiceInicial = indiceSplash;
        Vector3 escalaInicial = splashes[indiceSplash].transform.localScale;
        Vector3 escalaFinal = escalaInicial;



        escalaFinal *= multEscala;
        float lerpAtual = 0;


        while (lerpAtual < tempoTotal && indiceInicial == indiceSplash) {
            lerpAtual += Time.deltaTime;


            if (lerpAtual > tempoTotal)
            {
                lerpAtual = tempoTotal;
            }

            float percEscala = lerpAtual / tempoTotal;
            splashes[indiceSplash].transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, percEscala);
            yield return 0;
        }


    }

    private IEnumerator SplashIN() {
        Color corInicial = splashes[indiceSplash].color;
        Color corFinal = corInicial;
        corInicial.a = 0;
        splashes[indiceSplash].gameObject.SetActive(true);
        splashes[indiceSplash].color = corInicial;

        float lerpAtual = 0;

        while (lerpAtual < tempoINOUT) {
            lerpAtual += Time.deltaTime;

            if (lerpAtual > tempoINOUT) {
                lerpAtual = tempoINOUT;
            }


            float perc = lerpAtual / tempoINOUT;
            splashes[indiceSplash].color = Color.Lerp(corInicial, corFinal, perc);

            yield return 0;
        }

        corInicial.a = 1;
        splashes[indiceSplash].color = corInicial;

        float tempoEspera = 0;

        while (tempoEspera < esperaINOUT) {
            tempoEspera += Time.deltaTime;
            yield return 0;
        }

        StartCoroutine(SplashOUT());
    }

    private IEnumerator SplashOUT() {
        Color corInicial = splashes[indiceSplash].color;
        Color corFinal = corInicial;
        corFinal.a = 0;
        splashes[indiceSplash].color = corInicial;

        float lerpAtual = 0;

        while (lerpAtual < tempoINOUT)
        {
            lerpAtual += Time.deltaTime;

            if (lerpAtual > tempoINOUT)
            {
                lerpAtual = tempoINOUT;
            }


            float perc = lerpAtual / tempoINOUT;
            splashes[indiceSplash].color = Color.Lerp(corInicial, corFinal, perc);

            yield return 0;
        }

        corInicial.a = 0;
        splashes[indiceSplash].color = corInicial;
        splashes[indiceSplash].gameObject.SetActive(false);


        if (indiceSplash + 1 < numeroSplashes)
        {

            float tempoEspera = 0;
            while (tempoEspera < esperaCadaSplash)
            {
                tempoEspera += Time.deltaTime;
                yield return 0;
            }

            indiceSplash++;
            StartCoroutine(SplashIN());
            StartCoroutine(SplashScale());
        }
        else
        {
            fimSplashes = true;

            SceneLoader.Settings configs = new SceneLoader.Settings();
            configs.async = false;
            configs.fadeIN = true;
            configs.fadeOUT = true;

            SceneLoader.instance.CarregaCena(1, configs);
            
        }
    }
}
