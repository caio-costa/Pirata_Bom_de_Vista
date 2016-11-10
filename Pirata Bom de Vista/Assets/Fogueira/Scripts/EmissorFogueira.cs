using UnityEngine;
using System.Collections;


public class EmissorFogueira : MonoBehaviour {

    public static EmissorFogueira instance;

    [Header("Referências")]
    public Transform localFogo;
    public Transform flamePool;
    public Transform smokePool;
    public Transform sparkPool;
    public GameObject[] flames;
    public GameObject[] smokes;
    public GameObject[] sparks;

    [Header("Configurações")]
    public bool ativar = true;
    public float raio = 0.5f;
    public float alturaFogo = 3;
    public float tempoCadaParticula = 0.2f;
    public float tempoCadaSmoke = 0.3f;
    public float limiteEscala = 0.5f;
    public bool fumaca = true;
    public bool mudarEscala = true;
    public bool rotacaoAleatoria = true;
    public bool mudarRotacao = true;
    public bool ativarIluminacao = false;
    public bool conjuntosAleatorios = false;
    public bool ativarSparks = false;
    public bool spriteSparkAleatoria = false;
    public bool corSparkAleatoria = false;
    [Range(0.2f, 3)]
    public float tempoTrocaLuz = 1;
    [Range(0.02f, 1f)]
    public float tempoCadaSpark = 0.2f;
    [Range(1,10)]
    public int particulasPorVez = 1;
    [Range(1, 10)]
    public int smokesPorVez = 1;
    [Range(1, 10)]
    public int sparksPorVez = 2;

    public Texture[] texturasSpark = new Texture[3];
    public Color[] coresSparks = new Color[3];

    private int indiceFlame = 0, indiceSmoke = 0, indiceSpark;

    private float contador = 0;
    private float contadorLight = 0;
    private float contadorSpark = 0;
    private Light iluminacao;
    private bool trocaLuz = true;
    private Material _materialFlame;
    private int contadorLoop = 0;


    // Use this for initialization
    void Start () {
        instance = this;

        if (localFogo == null) {
            localFogo = this.transform;
        }
        

        if (this.transform.childCount > 0) {
            iluminacao = this.transform.GetChild(0).GetComponent<Light>() as Light;
            if (localFogo != this.transform) {
                Vector3 temp = localFogo.transform.position;
                temp.y += 3.18f;
                iluminacao.transform.position = temp;
            }
        }


        flames = new GameObject[flamePool.childCount];
        for (int i = 0; i < flames.Length; i++)
        {
            flames[i] = flamePool.GetChild(i).gameObject;
        }

        smokes = new GameObject[smokePool.childCount];
        for (int i = 0; i < smokes.Length; i++)
        {
            smokes[i] = smokePool.GetChild(i).gameObject;
        }
        
        sparks = new GameObject[sparkPool.childCount];
        for (int i = 0; i < sparks.Length; i++)
        {
            sparks[i] = sparkPool.GetChild(i).gameObject;
        }

        _materialFlame = flames[0].gameObject.GetComponent<MeshRenderer>().material;
        if (ativar)
        {
            iluminacao.color = Color.white * _materialFlame.GetColor("_TintColor");
        }
        else {
            iluminacao.gameObject.SetActive(false);
        }

    }
	
	// Update is called once per frame
	void Update () {
        if (ativar) {
            
            //Fogo
            if (contador < tempoCadaParticula)
            {
                contador += Time.deltaTime;
            }
            else {
                contador = 0;
                SoltaParticulas();
            }

            //Light
            if (ativarIluminacao)
            {
                if (contadorLight < tempoTrocaLuz)
                {
                    contadorLight += Time.deltaTime;
                }
                else {
                    contadorLight = 0;
                    trocaLuz = !trocaLuz;
                    StartCoroutine(TrocaIluminacao(trocaLuz));
                }
            }

            //Sparks
            if (ativarSparks) {
                if (contadorSpark < tempoCadaSpark)
                {
                    contadorSpark += Time.deltaTime;

                }
                else {
                    contadorSpark = 0;
                    SoltaSpark();
                }
            }
        }
	}

    private void SoltaParticulas() {
        if (indiceFlame + particulasPorVez - 1 < flames.Length)
        {
            bool boost = false;
            if (conjuntosAleatorios) {
                if (contadorLoop % 4 == 0 && contadorLoop != 0)
                {
                    contadorLoop++;
                    particulasPorVez *= 15;
                    boost = true;
                    Debug.Log("Boost");

                    if (!(indiceFlame + particulasPorVez < flames.Length))
                    {
                        indiceFlame = 0;
                    }
                }
            }

            Vector3 posTemp = Vector3.zero;

            for (int i = 0; i < particulasPorVez; i++)
            {
                GameObject particula = flames[indiceFlame];
                particula.transform.position = localFogo.position;

                if (boost) {
                    raio /= 3; 
                }

                Vector3 pos = Random.insideUnitSphere * raio;
                Vector3 temp;

                if (boost)
                {
                    raio *= 3;
                    if (posTemp == Vector3.zero)
                    {
                        posTemp = pos;
                        int random = Random.Range(0, 2);
                        if (random == 0)
                        {
                            posTemp.x += -raio;
                        }
                        else {
                            posTemp.x += +raio;
                        }
                        
                    }
                    else {
                        pos = posTemp;
                    }
                }

                particula.transform.position += pos;
                temp = particula.transform.position;
                temp.y = 0;
                particula.transform.position = temp;

                float tempEscala = Random.Range(1 - limiteEscala, 1 + limiteEscala);
                particula.transform.localScale = new Vector3(tempEscala, tempEscala, tempEscala);

                float rotacaoZ = 0;

                if (rotacaoAleatoria) {
                    rotacaoZ = Random.Range(0, 360);
                    particula.transform.eulerAngles = new Vector3(0, 0, rotacaoZ);
                }

                particula.gameObject.SetActive(true);
                Vector3 DestinoPartícula = localFogo.position;
                DestinoPartícula.y += alturaFogo + Random.Range(-0.8f, 0.8f);
                particula.GetComponent<FlameBehaviour>().AtivarParticula(DestinoPartícula, mudarEscala, mudarRotacao, rotacaoZ, fumaca);
                indiceFlame++;
            }

            if (boost) {
                particulasPorVez /= 15;
            }

        }
        else {
            indiceFlame = 0;
            contadorLoop++;
        }
    }

    public void SoltaFumaca(Vector3 origem) {
        if (indiceSmoke + smokesPorVez - 1< smokes.Length)
        {



            for (int i = 0; i < smokesPorVez; i++) {
                GameObject smoke = smokes[indiceSmoke];
                float rotacaoZ = Random.Range(0, 360);
                Vector3 temp = origem;
                temp.y += Random.Range(0.3f, 2.0f);
                temp.x += Random.Range(-1.0f, 1.0f);
                smoke.transform.position = temp;

                Vector3 rotTemp = smoke.transform.eulerAngles;
                rotTemp.z = rotacaoZ;
                smoke.transform.eulerAngles = rotTemp;
                smoke.SetActive(true);

                Vector3 destino = origem;
                destino.y += Random.Range(4f, 9f);
                destino.x += Random.Range(0.5f, 6f);

                smoke.GetComponent<SmokeBehaviour>().AtivarParticula(destino, rotacaoZ);
                indiceSmoke++;
            }
        }
        else {
            indiceSmoke = 0;
        }
    }

    private void SoltaSpark() {
        if (indiceSpark + sparksPorVez - 1 < sparks.Length)
        {
            for (int i = 0; i < sparksPorVez; i++)
            {
                GameObject particula = sparks[indiceSpark].gameObject;
                particula.transform.position = localFogo.position;

                Vector3 temp = particula.transform.eulerAngles;
                temp.z = Random.Range(-20, 20);
                particula.transform.eulerAngles = temp;

                float tempoDuracao = Random.Range(2, 5);
                float rangeLargura = Random.Range(2,3);


                particula.SetActive(true);

                if (!spriteSparkAleatoria)
                {
                    if (corSparkAleatoria)
                    {
                        int random = Random.Range(0, coresSparks.Length);
                        particula.GetComponent<SparkBehaviour>().AtivarParticula(tempoDuracao, rangeLargura, corSparkAleatoria, coresSparks[random]);
                    }
                    else {
                        particula.GetComponent<SparkBehaviour>().AtivarParticula(tempoDuracao, rangeLargura, corSparkAleatoria, Color.white);
                    }
                    
                }
                else {
                    int random = Random.Range(0, texturasSpark.Length);
                    if (corSparkAleatoria)
                    {
                        int corRandom = Random.Range(0, coresSparks.Length);
                        particula.GetComponent<SparkBehaviour>().AtivarParticula(tempoDuracao, rangeLargura, texturasSpark[random], corSparkAleatoria, coresSparks[corRandom]);
                    }
                    else {
                        particula.GetComponent<SparkBehaviour>().AtivarParticula(tempoDuracao, rangeLargura, texturasSpark[random], corSparkAleatoria, Color.white);
                    }

                }
                
                indiceSpark++;
            }
        }
        else {
            indiceSpark = 0;
        }
    }


    private IEnumerator TrocaIluminacao(bool valorTomado) {
        bool temp = valorTomado;

        while (temp == this.trocaLuz && ativarIluminacao) {
            //Debug.Log("Trocando luz: " + this.trocaLuz);
            float incremento = 3f;

            if (!trocaLuz)
            {
                incremento -= incremento * 2f;
            }

            iluminacao.intensity += incremento * Time.deltaTime;

            yield return 0;
        }
    }
}
