using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public static PlayerController instance;

    private bool permiteInput = false;
    private bool combate = false;
    private Transform myTransform;

    // Use this for initialization
    void Start () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null && instance != this) {
            Destroy(this.gameObject);
        }

        myTransform = this.transform;
        PlayerManager.instance.SetPlayerState(PlayerState.MOVIMENTACAO);
    }
	
	// Update is called once per frame
	void Update () {
        //TODO: Codigo para controle de Input
        if (permiteInput) {
            if (combate)
            {
                InputCombate();
            }
            else {
                InputMovimentacao();
            }
        }
	}


    public void InputMovimentacao() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float CameraHorizontal = Input.GetAxis("CameraHorizontal");

        float limite = 0.3f;
        float velocidade = 6.0f;

        if (Mathf.Abs(horizontal) > limite) {
            Vector3 temp = myTransform.position;
            temp.x += horizontal * Time.deltaTime * velocidade;
            myTransform.position = temp;
        }

        if (Mathf.Abs(vertical) > limite) {
            Vector3 temp = myTransform.position;
            temp.z += vertical * Time.deltaTime * velocidade;
            myTransform.position = temp;
        }

        if (Mathf.Abs(CameraHorizontal) > limite) {
            CameraManager.instance.MovimentarHorizontal(CameraHorizontal);
        }

    }


    public void InputCombate()
    {

    }

    public void SetPermiteInput(bool temp) {
        permiteInput = temp;
    }

    public void SetInputCombate(bool temp) {
        combate = temp;
    }
}
