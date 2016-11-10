using UnityEngine;
using System.Collections;

public enum EstadoAtual {
    INPUT,
    CUTSCENE
}

[System.Serializable]
public class MudancaDeIlha {
    public FaseAtual faseInicial = new FaseAtual();
    public FaseAtual faseFinal = new FaseAtual();
    public GameObject objetoIlha;
}

public class LevelManager : MonoBehaviour {

    public static LevelManager instance;
    private EstadoAtual estadoAtual;


    public delegate void CutsceneDelegate();
    public static event CutsceneDelegate eventoCutscene;

    // Use this for initialization
    void Start () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}


    public void AtivaCutscene() {
        estadoAtual = EstadoAtual.CUTSCENE;
        PlayerManager.instance.SetPlayerState(PlayerState.CUTSCENE);
        DirectorsManager.instance.AtivaDiretor();
    }

    public void AtivaInput() {
        if (DirectorsManager.instance.ativado) {
            DirectorsManager.instance.DesativaDiretor();
        }
        
        StartCoroutine(AguardaFimAnimParaInput());
    }

    public void DesativaInput() {
        
        if (!SceneLoader.instance.navegacao)
        {
            PlayerManager.instance.SetPlayerState(PlayerState.CUTSCENE);
        }
        else
        {
            PlayerBoatManager.instance.DesativaInput();
        }

        SetEstadoAtual(EstadoAtual.CUTSCENE);
    }

    private IEnumerator AguardaFimAnimParaInput() {
        yield return null;


        while (DirectorsManager.instance == null) {
            yield return null;
        }

        while (DirectorsManager.instance.alterando) {
            yield return null;
        }


        estadoAtual = EstadoAtual.INPUT;
        if (!SceneLoader.instance.navegacao)
        {
            PlayerManager.instance.SetPlayerState(PlayerState.MOVIMENTACAO);
        }
        else {
            PlayerManager.instance.SetPlayerState(PlayerState.NAVEGANDO);
            PlayerBoatManager.instance.AtivaInput();
        }
        
    }
    

    public void SetEstadoAtual(EstadoAtual temp) {
        estadoAtual = temp;
    }

    public void EventoCutscene() {
        eventoCutscene();
    }

    public EstadoAtual GetEstadoAtual() {
        return estadoAtual;
    }

    public FaseAtual GetFaseAtual() {
        if (ObjetivosController.instance == null)
            return null;
        return ObjetivosController.instance.GetFaseAtual();
    }
}

