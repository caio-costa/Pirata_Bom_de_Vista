using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour {

    public static MainMenuManager instance;

    public Transform menuPai;
    public float escalaMax = 1.5f;
    public Color corSelecionado, corOriginal;

    private GameObject[] itens;
    private int numeroItens = 0, itemAtual = 0;
    private float axisV = 0, limitePad = 0.3f;
    private bool troca = false, inputPermitido = false;
    private bool teclas = false;
    [HideInInspector]
    public bool menuFading = true;
    public bool moveOceano = true;
    private float offset = 0;
    public Material oceano;

    private AudioSource _source;

    // Use this for initialization
    IEnumerator Start () {
        instance = this;
        menuPai.gameObject.SetActive(false);
        _source = GetComponent<AudioSource>();

        if (!SaveLoad.instance.CheckIfLoadExists()){
            Destroy(menuPai.GetChild(0).gameObject);
        }

        yield return null;

        numeroItens = menuPai.childCount;
        itens = new GameObject[numeroItens];


        AddListeners();
        StartCoroutine(EsperaTerminoFade());
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(menuFading);

        if (inputPermitido) {
            axisV = Input.GetAxis("Vertical");

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            {
                teclas = true;
                limitePad = 0.05f;
            }
            else
            {
                teclas = false;
                limitePad = 0.3f;
            }

           

            if (Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames().Length < 2 || teclas)
            {

                if (axisV > -limitePad && axisV < limitePad)
                {
                    troca = false;
                }
                else if (axisV < -limitePad)
                {
                    if (!troca)
                    {
                        //Debug.Log("Desce");
                        troca = true;
                        Desce();
                    }
                }
                else if (axisV > limitePad)
                {
                    if (!troca)
                    {
                        //Debug.Log("Sobe");
                        troca = true;
                        Sobe();
                    }
                }
            }
        }

        if (moveOceano) {
            offset += Time.deltaTime * 0.03f;
            oceano.mainTextureOffset = new Vector2(-offset, offset/4);
        }

    }

    private void Desce() {
        if (itemAtual < numeroItens - 1)
        {

            itemAtual++;
            SelecionaAtual();
        }
    }

    private void Sobe() {
        if (itemAtual > 0)
        {

            itemAtual--;
            SelecionaAtual();
        }
    }

    private void SelecionaAtual()
    {
        if (inputPermitido) {
            NormalizaTodos();
            _source.Play();
            itens[itemAtual].transform.localScale = Vector3.one * escalaMax;
            itens[itemAtual].GetComponent<Image>().color = corSelecionado;
        }
        
    }

    private void NormalizaTodos() {
        for (int i = 0; i < itens.Length; i++)
        {
            itens[i].transform.localScale = Vector3.one;
            itens[i].gameObject.GetComponent<Image>().color = corOriginal;
        }
    }


    private void AumentaSelecionado() {
        itens[itemAtual].transform.localScale = Vector3.one * escalaMax;
        itens[itemAtual].GetComponent<Image>().color = corSelecionado;
    }

    public void MudaItemSelecionado(int i) {

        if (itemAtual != i)
        {
            itemAtual = i;
            SelecionaAtual();
        }
        else {
            AtivaBotao();
        }
        
    }

    private void MudaItemSelecionadoMouse(int i) {
        if (itemAtual != i)
        {
            itemAtual = i;
            SelecionaAtual();
        }
    }

    public void AtivaBotao() {
        itens[itemAtual].GetComponent<MenuItem>().AtivaBotao();
    }

    public void FadeINMenu() {
        StartCoroutine(FadeMenu(true));
    }

    public void FadeOutMenu() {
        StartCoroutine(FadeMenu(false));
    }

    private IEnumerator FadeMenu(bool IN) {
        menuFading = true;
        inputPermitido = false;

        NormalizaTodos();

        Color32 corTempBotao = corOriginal;
        if (IN)
        {
            corTempBotao.a = 0;
        }
        else {
            corTempBotao.a = 255;
        }
        

        Color32 corBranca = Color.white;
        if (IN)
        {
            corBranca.a = 0;
        }
        else {
            corBranca.a = 255;
        }
        

        Image[] imagens = new Image[menuPai.childCount];
        Text[] textos = new Text[menuPai.childCount];


        for (int i = 0; i < menuPai.childCount; i++)
        {
            imagens[i] = menuPai.GetChild(i).gameObject.GetComponent<Image>();
            imagens[i].color = corTempBotao;

            for (int a = 0; a < menuPai.GetChild(i).childCount; a++)
            {
                textos[i] = menuPai.GetChild(i).GetChild(a).gameObject.GetComponent<Text>();
                textos[i].color = corBranca;
            }
        }

        yield return new WaitForEndOfFrame();
        menuPai.gameObject.SetActive(true);


        float alfa = 0, speed = 0.3f;
        float alfaOriginal = corOriginal.a;


        if (!IN) {
            alfa = alfaOriginal;
            alfaOriginal = 0;
        }

       // Debug.Log("Alfa: " + alfa + "\nDestino: " + alfaOriginal);


        bool entrar = true;

        while (entrar)
        {
            if (IN)
            {
                if (alfa < alfaOriginal)
                {
                    entrar = true;
                }
                else {
                    entrar = false;
                }

                alfa += speed * Time.deltaTime;

            }
            else {

                if (alfaOriginal < alfa)
                {
                    entrar = true;
                }
                else
                {
                    entrar = false;
                }

                alfa -= speed * Time.deltaTime;
            }

            //Debug.Log("Entrou fade");

            for (int i = 0; i < imagens.Length; i++)
            {
                Color temporaria = imagens[i].color;
                temporaria.a = alfa;
                imagens[i].color = temporaria;
            }

            for (int a = 0; a < textos.Length; a++)
            {

                Color temporariaTexto = textos[a].color;
                temporariaTexto.a = alfa * 4.35f;
                textos[a].color = temporariaTexto;
            }
            yield return null;
        }

        if (!IN) {
           // Debug.Log("Acabou fading");
            //menuPai.gameObject.SetActive(false);
        }

        menuFading = false;
       
    }

    private IEnumerator EsperaTerminoFade() {
        yield return new WaitForEndOfFrame();
        while (FadeManager.instance.fading) {
            yield return null;
        }

        FadeINMenu();

        while (menuFading) {
            yield return null;
        }

        yield return null;
        inputPermitido = true;
        SelecionaAtual();
    }

    private void AddListeners()
    {
        for (int i = 0; i < itens.Length; i++)
        {
            itens[i] = menuPai.GetChild(i).gameObject;


            Button botao = itens[i].GetComponent<Button>();
            botao.onClick.RemoveAllListeners();
            int localInt = i;
            //Debug.Log(localInt);
            botao.onClick.AddListener(() => { MudaItemSelecionado(localInt); });
            EventTrigger trigger = itens[i].GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { MudaItemSelecionadoMouse(localInt); });
            trigger.triggers.Add(entry);
        }
    }

    public bool GetMenuFading() {
        return menuFading;
    }
}
