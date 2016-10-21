using UnityEngine;
using System.Collections;

public class ItemdeMenu : MonoBehaviour {

    public enum tipoBotao {
        NOVO_JOGO,
        LOAD,
        OPTIONS
    }

    public tipoBotao meuTipo;
    private bool teste = false;

	// Use this for initialization
	void Start () {
	
	}

    public void AtivaBotao() {
        StartCoroutine(AtivaBotaoCo());
    }

    public IEnumerator AtivaBotaoCo() {
        MainMenuManager.instance.ResetaPopUps();

        switch (meuTipo){
            case tipoBotao.NOVO_JOGO: {
                    Debug.Log("Novo Jogo");

                    if (!SaveLoad.instance.CheckIfLoadExists())
                    {
                        MainMenuManager.instance.FadeOutMenu();

                        //while (MainMenuManager.instance.GetMenuFading()) {
                        // yield return new WaitForSeconds(0.3f);
                        // }

                        SceneLoader.Settings options = new SceneLoader.Settings();
                        options.async = false;
                        options.fadeIN = true;
                        options.fadeOUT = true;
                        options.loadingScreen = true;

                        SceneLoader.instance.CarregaCena(3, options);

                    }
                    else {
                        MainMenuManager.instance.ConfirmacaoNovoJogo();
                        //Debug.Log("Aviso de save existente");
                    }
                }break;

            case tipoBotao.LOAD: {
                    Debug.Log("Load");

                    SaveLoad.instance.Load();

                    MainMenuManager.instance.FadeOutMenu();

                    //while (MainMenuManager.instance.GetMenuFading()) {
                    // yield return new WaitForSeconds(0.3f);
                    // }

                    SceneLoader.Settings options = new SceneLoader.Settings();
                    options.async = false;
                    options.fadeIN = true;
                    options.fadeOUT = true;
                    options.loadingScreen = true;

                    SceneLoader.instance.CarregaCena(3, options);
                }
                break;

            case tipoBotao.OPTIONS: {
                    Debug.Log("Options");
                }break;
        }
        yield return null;
    }
}
