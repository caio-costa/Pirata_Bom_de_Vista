using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    public static SceneLoader instance;
    public float tempoEspera = 1.5f;
    public bool esperandoFade = false;

    public class Settings
    {
        public bool fadeOUT = true, fadeIN = true, async = false, loadingScreen = false;
    }

    public Settings configs = new Settings();

    // Use this for initialization
    void Start () {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        
    }


    public void CarregaCena(int cena, Settings config) {
        StartCoroutine(Load(cena, config));
    }

    private IEnumerator Load(int cena, Settings config) {
        if (config.async == true && config.loadingScreen == false)
        {

        }
        else {

            if (config.fadeIN) {
                //Carrega cena agora... após Fade (caso tenha)
                while (FadeManager.instance.fading)
                {
                    yield return null;
                }

                FadeManager.instance.Fade(FadeMode.IN);
                yield return new WaitForEndOfFrame();

                while (FadeManager.instance.fading)
                {
                    //Debug.Log("Esperando");
                    yield return null;
                }
            }


            if (config.loadingScreen)
            {

                AsyncOperation sync = SceneManager.LoadSceneAsync(2);
                sync.allowSceneActivation = false;

                while (sync.progress < 0.9f)
                {
                    yield return null;
                }

                sync.allowSceneActivation = true;


                //CarregaLoadingScreen();
                if (config.fadeOUT)
                {
                    //yield return new WaitForSeconds(tempoEspera - 0.6f);
                    FadeManager.instance.Fade(FadeMode.OUT, 1.6f);
                }

                AsyncOperation syncCena = SceneManager.LoadSceneAsync(cena);
                syncCena.allowSceneActivation = false;


                while (syncCena.progress < 0.9f)
                {
                    //Debug.Log(syncCena.progress);
                    if (LoadingScreenManager.instance != null) {
                        LoadingScreenManager.instance.loadingProgress = syncCena.progress;
                    }
                    yield return null;
                }

                LoadingScreenManager.instance.loadingProgress = syncCena.progress;

                while (LoadingScreenManager.instance.carregando) {
                    yield return null;
                }

                FadeManager.instance.Fade(FadeMode.IN);
                yield return new WaitForEndOfFrame();

                while (FadeManager.instance.fading)
                {
                    //Debug.Log("Esperando");
                    yield return null;
                }
                
                syncCena.allowSceneActivation = true;
                esperandoFade = true;
                yield return new WaitForSeconds(tempoEspera);
                FadeManager.instance.Fade(FadeMode.OUT,1.3f);
                yield return new WaitForEndOfFrame();
                esperandoFade = false;

                while (FadeManager.instance.fading)
                {
                    //Debug.Log("Esperando");
                    yield return null;
                }

            }
            else
            {
                AsyncOperation sync = SceneManager.LoadSceneAsync(cena);
                sync.allowSceneActivation = false;

                if (config.fadeOUT)
                {
                    while (sync.progress < 0.9f)
                    {
                        yield return null;
                    }

                    while (FadeManager.instance.fading)
                    {
                        yield return null;
                    }

                    FadeManager.instance.fading = true;
                    sync.allowSceneActivation = true;
                    yield return new WaitForSeconds(tempoEspera);
                    FadeManager.instance.Fade(FadeMode.OUT, 1.3f);
                }
                else {
                    sync.allowSceneActivation = true;
                }
            }
        }

        yield return null;
    }


    private void CarregaLoadingScreen() {
        SceneManager.LoadScene(2);
    }

    private void ResetaSettings() {
        this.configs = new Settings();
    }
}
