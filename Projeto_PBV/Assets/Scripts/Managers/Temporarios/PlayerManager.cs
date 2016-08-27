using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerState {
    MOVIMENTACAO,
    COMBATE,
    CUTSCENE,
}

public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance;

    [HideInInspector]
    public PlayerState playerState;

    [SerializeField]
    public List<CorteAnimacao> listaAnimacao = new List<CorteAnimacao>();

    // Use this for initialization
    void Start () {
        instance = this;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetPlayerState(PlayerState temp) {
        if (temp == PlayerState.MOVIMENTACAO || temp == PlayerState.COMBATE)
        {
            PlayerController.instance.SetPermiteInput(true);

            if (temp == PlayerState.COMBATE)
            {
                PlayerController.instance.SetInputCombate(true);
            }
            else {
                PlayerController.instance.SetInputCombate(false);
            }
        }
        else {
            PlayerController.instance.SetPermiteInput(false);
            PlayerController.instance.SetInputCombate(false);
        }

        //Debug.Log(temp);
        playerState = temp;
    }


    public void AtivaAudio()
    {
        if (DirectorsManager.instance == null) { return; }

        DirectorsManager.instance.ProximaLegenda();
    }
}
