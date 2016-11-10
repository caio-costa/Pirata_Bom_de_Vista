using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager instance;

    [SerializeField]
    private Text objetivo;
    public bool alterandoUI = false;
    public GameObject ponteiroBussola, referencia, bussola;
    public GameObject destino;
    private Vector3 upVector;

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

        //upVector = -ponteiroBussola.transform.up;
        objetivo.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        AtualizaPonteiroBussola();
	}


    public void AtualizaPonteiroBussola() {
        if (SceneLoader.instance.FaseNavegacao()) {
            Vector3 destinoCorrigido = destino.transform.position;
            destinoCorrigido.y = Camera.main.transform.position.y;

            Vector3 direcao = destinoCorrigido - Camera.main.transform.position;
            //Vector3 direcao = destino.transform.position - bussola.transform.position;
            direcao.Normalize();
            Vector3 localDirecao = bussola.transform.InverseTransformDirection(direcao);

            //ponteiroBussola.transform.rotation.SetLookRotation(destino.transform.position, upVector);
            //ponteiroBussola.transform.eulerAngles = new Vector3(0, 0, 0);

            //direcao = ponteiroBussola.transform.InverseTransformDirection(direcao);
            //Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + direcao * 10, Color.red, 10);
            Debug.DrawLine(ponteiroBussola.transform.position, ponteiroBussola.transform.position + direcao * 10, Color.red, 10);
            Debug.DrawLine(ponteiroBussola.transform.position, ponteiroBussola.transform.position + -localDirecao * 3, Color.blue, 10);

            //ponteiroBussola.transform.rotation = Quaternion.LookRotation(-direcao);
            //ponteiroBussola.transform.Rotate(new Vector3(Camera.main.transform.eulerAngles.x, 0, 180));
            //Vector3 temp = ponteiroBussola.transform.localEulerAngles;
            ponteiroBussola.transform.Rotate(0, 3, 0);
        }
        
    }

    public void DesativaTodaUI() {
        alterandoUI = true;
        DesativaTituloObjetivo();
    }

    public void AtivaTituloObjetivo(string texto) {
        SetObjetivoTitulo(texto);
        StartCoroutine(ApareceTituloObjetivo(true));
    }

    public void DesativaTituloObjetivo() {
        StartCoroutine(ApareceTituloObjetivo(false));
    }

    private IEnumerator ApareceTituloObjetivo(bool ativar) {


        Color tempColor = Color.white;
        Color corFinal = Color.white;

        if (ativar)
        {
            tempColor.a = 0;
            objetivo.color = tempColor;
        }
        else {
            corFinal.a = 0;
            objetivo.color = tempColor;
        }



        yield return null;
        if (ativar) {
            objetivo.gameObject.SetActive(true);
        }
        

        float currentLerpTime = 0, tempoLerp = 1.0f;

        while (currentLerpTime < tempoLerp) {

            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > tempoLerp)
            {
                currentLerpTime = tempoLerp;
            }

            float perc = currentLerpTime / tempoLerp;
            objetivo.color = Color.Lerp(tempColor, corFinal, perc);

            yield return null;
        }

        if (!ativar) {
            objetivo.gameObject.SetActive(false);
            SetObjetivoTitulo("");
        }

        alterandoUI = false;

    }

    public void SetObjetivoTitulo(string texto) {
        if (texto != "")
        {
            //objetivo.text = "<color=#ffa500ff>" + "Objetivo: " + "</color>" + "\n" + "<size=35>" + texto + "</size>";
            objetivo.text = "Objetivo: " + "\n" + "<size=35>" + texto + "</size>";
        }
        else {
            objetivo.text = texto;
        }
        
    }
}
