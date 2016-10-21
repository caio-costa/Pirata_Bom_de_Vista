using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager instance;

    [SerializeField]
    private Text objetivo;
    public bool alterandoUI = false;

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

        objetivo.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
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
