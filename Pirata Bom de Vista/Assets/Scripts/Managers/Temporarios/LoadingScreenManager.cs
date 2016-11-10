using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour {

    public static LoadingScreenManager instance;

    [System.Serializable]
    public class Componentes {
        public Image imagens_image;
        public Text frase_text;
        public RectTransform fundoLoading;
        public RectTransform frenteLoading;
    }

    public Componentes components;
    public string[] frases;
    public Sprite[] spritesImagem;
    private int fraseAtual;
    public float loadingProgress;
    private float contador = 0, tempoMinimo = 3;
    private float incremento = 0;

    [HideInInspector]
    public bool carregando = true;

	// Use this for initialization
	void Start () {

        instance = this;

        RandomizaFrases();
        RandomizaImagens();

        incremento = components.fundoLoading.rect.width;
        //Debug.Log(incremento);

    }

    void Update() {

        if (loadingProgress >= 0.9f) {
            loadingProgress = 1;
        }

        float pos = loadingProgress * incremento;

        if (pos > components.fundoLoading.rect.width) {
            pos = components.fundoLoading.rect.width;
        }

        components.frenteLoading.offsetMax = new Vector2(pos - components.fundoLoading.rect.width, components.frenteLoading.offsetMax.y);

        if (contador < tempoMinimo)
        {
            contador += Time.deltaTime;
        }
        else {
            carregando = false;
        }
    }

    public void RandomizaFrases() {
        int rand = Random.Range(0, frases.Length);
        components.frase_text.text = frases[rand];
    }

    public void RandomizaImagens() {
        int rand = Random.Range(0, spritesImagem.Length);
        components.imagens_image.sprite = spritesImagem[rand];
    }
}
