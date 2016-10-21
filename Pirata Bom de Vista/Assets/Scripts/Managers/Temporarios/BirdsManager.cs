using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode]
public class BirdsManager : MonoBehaviour {

    public static BirdsManager instace;

    [Header("Referências")]
    public Transform birdsList;
    public Transform ilha;
    public GameObject birdPrefab;
    public bool editor = true;
    public bool debugRay = false;

     [Header("Configurações de Grupos")]
    [Range(1,10)]
    [Tooltip("A quantidade de grupos que podem estar presentes na mesma hora")]
    public int numeroDeGrupos = 1;
    [Range(3, 11)]
    [Tooltip("Quantos passaraos haverão em um grupo. Apenas números ímpares")]
    public int passarosPorGrupo = 3;
    [Range(0, 4)]
    [Tooltip("Quanto o número de pássaros pode variar. Apenas números pares")]
    public int limitePassarosEmGrupo = 3;
    [Tooltip("Altura média que os grupos voarão")]
    public float altura = 200;
    [Range(10,50)]
    [Tooltip("Quanto a altura pode variar")]
    public float limiteAltura = 40;
    [Range(5,12)]
    [Tooltip("O espacamento entre uma linha e a proxima")]
    public float espacamentoPassaros = 7.5f;
    [Range(1,50)]
    [Tooltip("A velocidade média do grupo de pássaros")]
    public float velocidadeVoo = 6f;
    [Range(0,1)]
    [Tooltip("O limite de aleatoriedade na velocidade do grupo. Em relação à velocidade média")]
    public float limiteVelocidade = 0.5f;
    [Range(100, 500)]
    [Tooltip("O raio minimo de spawn. Os grupos não serão spawnados neste raio")]
    public float raioMinimoSpawn = 200;
    [Range(1.1f, 5)]
    [Tooltip("A diferença de espaço para spawn dos passaros")]
    public float multiplicadorRaioSpawn = 2;
    [Tooltip("Se os pássaros devem animar")]
    public bool animação = true;


    public List<GameObject> passaros = new List<GameObject>();
    public int numeroPassarosTotais = 0;
    private int indiceAtual = 0;

    // Use this for initialization
    void Start () {
        instace = this;

        CriacaoGeral();

    }

    // Update is called once per frame
    void Update () {
        if (editor) {
            #region MoverParaStart
            if (passarosPorGrupo % 2 == 0)
            {
                passarosPorGrupo++;
            }

            if (limitePassarosEmGrupo % 2 != 0)
            {
                limitePassarosEmGrupo++;
            }

            passaros = new List<GameObject>();
            for (int a = 1; a < birdsList.childCount; a++)
            {
                DestroyImmediate(birdsList.GetChild(a).gameObject);

            }

            numeroPassarosTotais = birdsList.childCount;


            if (numeroPassarosTotais < numeroDeGrupos * (passarosPorGrupo + limitePassarosEmGrupo))
            {
                //Debug.Log("Quantos tenho: " + numeroPassarosTotais + "\nQuantos preciso: " + numeroDeGrupos * (passarosPorGrupo + limitePassarosEmGrupo));
                InstanciaPassarosRestantes((int)(numeroDeGrupos * (passarosPorGrupo + limitePassarosEmGrupo)) - numeroPassarosTotais);
            }

            passaros = new List<GameObject>();
            //Debug.Log(birdsList.childCount);
            for (int i = 0; i < birdsList.childCount; i++)
            {
                passaros.Add(birdsList.GetChild(i).gameObject);
            }


            CriacaoDeGrupos();
            //ApagaBirdsNaoUsados(indiceAtual);

            #endregion
        }

    }

    public void InstanciaPassarosRestantes(int quantidade) {
        //Debug.Log("Quantidade restante: " + quantidade);

        for (int i = 0; i < quantidade; i++)
        {
            GameObject novoPassaro = Instantiate(birdPrefab);
            novoPassaro.transform.SetParent(birdsList);
            novoPassaro.transform.eulerAngles = new Vector3(0, 90, 0);
            novoPassaro.transform.position = Vector3.zero;
            //novoPassaro.transform.position = passaros[0].transform.position;
        }
    }

    public void CriacaoDeGrupos() {
        indiceAtual = 0;

        for (int i = 0; i < numeroDeGrupos; i++)
        {
            int variacaoPorGrupo = Random.Range(-limitePassarosEmGrupo, limitePassarosEmGrupo);
            if (variacaoPorGrupo < 0 && Mathf.Abs(variacaoPorGrupo) > passarosPorGrupo) {
                variacaoPorGrupo = -passarosPorGrupo + 1;
            }

            int totalDesteGrupo = passarosPorGrupo + variacaoPorGrupo;
            int numeroColunas = (int)((totalDesteGrupo / 2) + 1);
            int indiceInicial = indiceAtual;
            float limiteAltura = Random.Range(-this.limiteAltura, this.limiteAltura);
            float alturaGrupo = altura + limiteAltura;
            float porcentagemLimiteVelocidade = velocidadeVoo * limiteVelocidade;
            float velocidadeOffset = Random.Range(-porcentagemLimiteVelocidade, porcentagemLimiteVelocidade);
            float velocidadeGrupo = velocidadeVoo + velocidadeOffset;

            //Debug.Log(totalDesteGrupo);

            Vector3 posicaoInicial = this.transform.position;
            Vector3 direcaoGrupo = Vector3.zero;
            Vector3 minhaPosTemp = Vector3.zero;
            Vector3 posFinalGrupo = Vector3.zero;
            float offsetX = Random.Range(-15.0f, 1.0f);
            float offsetZ = Random.Range(-15.0f, 1.0f);
            direcaoGrupo.x += offsetX;
            direcaoGrupo.z += offsetZ;


            for (int a = 0; a < numeroColunas; a++)
            {
                if (a != 0)
                {
                    for (int b = 0; b < 2; b++)
                    {

                        Vector3 posPassaro = posicaoInicial;

                        Vector3 frente = passaros[indiceInicial].gameObject.transform.forward;
                        Vector3 tras = -frente * espacamentoPassaros * a;
                        Vector3 lado = Vector3.Cross(passaros[indiceInicial].gameObject.transform.up, passaros[indiceInicial].gameObject.transform.forward);

                        Vector3 destinoRotacao = minhaPosTemp;
                        posPassaro -= tras;
                        if (b % 2 == 0)
                        {
                            lado *= espacamentoPassaros * a;
                        }
                        else
                        {
                            lado *= -espacamentoPassaros * a;

                        }

                        posPassaro += lado;

                        Vector3 destino = destinoRotacao += lado;
                        destino.y = alturaGrupo;
                        if (debugRay) {
                            Debug.DrawLine(posPassaro, posPassaro - lado, Color.red, 10);
                            Debug.DrawLine(posPassaro, destinoRotacao, Color.blue, 10);
                            Debug.DrawLine(posPassaro, destino, Color.green, 10);
                        }

                        passaros[indiceAtual].name = "Grupo " + i.ToString();
                        passaros[indiceAtual].GetComponent<BirdsBehaviour>().SetPosicaoInicial(posPassaro);
                        passaros[indiceAtual].GetComponent<BirdsBehaviour>().SetDirecao(destino, velocidadeGrupo);
                        indiceAtual++;
                    }
                }
                else {

                    Vector3 randomSphere = Random.insideUnitSphere * raioMinimoSpawn * multiplicadorRaioSpawn;
                    randomSphere.y = altura;

                    //Debug.Log(Vector3.Distance(randomSphere, this.transform.position));

                    while (Vector3.Distance(randomSphere, this.transform.position) < raioMinimoSpawn + raioMinimoSpawn/10)
                    {
                        randomSphere = Random.insideUnitCircle * raioMinimoSpawn * multiplicadorRaioSpawn;
                        //Debug.Log("Dentro");
                    }

                    posicaoInicial = this.transform.position;
                    posicaoInicial.x += randomSphere.x;
                    posicaoInicial.z += randomSphere.y;
                    posicaoInicial.y = alturaGrupo;

                    minhaPosTemp = this.transform.position;
                    minhaPosTemp.y = 200;
                    float offsetRandomX = Random.Range(-raioMinimoSpawn, raioMinimoSpawn);
                    float offsetRandomZ = Random.Range(-raioMinimoSpawn, raioMinimoSpawn);
                    minhaPosTemp.z += offsetRandomX;
                    minhaPosTemp.x += offsetRandomZ;

                    direcaoGrupo = minhaPosTemp - posicaoInicial;
                    direcaoGrupo.Normalize();

                    Vector3 destino = minhaPosTemp;
                    destino.y = alturaGrupo;
                    if (debugRay) {
                        Debug.DrawLine(posicaoInicial, destino, Color.red, 10);
                    }
                    //Debug.DrawRay(posicaoInicial, direcaoGrupo * 3, Color.blue, 10);

                    //passaros[indiceInicial].gameObject.SetActive(true);
                    passaros[indiceInicial].GetComponent<BirdsBehaviour>().SetPosicaoInicial(posicaoInicial);
                    passaros[indiceInicial].GetComponent<BirdsBehaviour>().SetDirecao(destino, velocidadeGrupo);
                    passaros[indiceInicial].name = "Grupo " + i.ToString();
                    indiceAtual++;
                }
            }
        }
    }



    public void ApagaBirdsNaoUsados(int indice) {
        for (int i = indice; i < birdsList.childCount; i++)
        {
            birdsList.GetChild(i).gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        if (debugRay) {
            // Display the explosion radius when selected
            Gizmos.color = new Color32(255, 0, 0, 150);
            Gizmos.DrawSphere(this.transform.position, raioMinimoSpawn);


            Gizmos.color = new Color32(0, 255, 0, 220);
            Gizmos.DrawSphere(this.transform.position, raioMinimoSpawn * multiplicadorRaioSpawn);

        }

    }

    public Vector3 GetPosManager() {
        return transform.position;
    }

    public IEnumerator ReutilizarBirds() {
        int birdsNaoUsados = 0;

        while (birdsNaoUsados != birdsList.childCount) {
            yield return new WaitForSeconds(6);


            birdsNaoUsados = 0;
            for (int i = 0; i < birdsList.childCount; i++)
            {
                if (birdsList.GetChild(i).gameObject.GetComponent<BirdsBehaviour>().voando == false) {
                    birdsNaoUsados++;
                }
            }

            //Debug.Log("Nao usados: " + birdsNaoUsados + "\nNumero de filhos: " + (birdsList.childCount - 1).ToString());
        }

        //Debug.Log("Renovou Tudo");
        for (int i = 0; i < birdsList.childCount; i++)
        {
            birdsList.GetChild(i).gameObject.SetActive(true);
        }

        CriacaoGeral();
    }

    public void CriacaoGeral() {
        if (passarosPorGrupo % 2 == 0)
        {
            passarosPorGrupo++;
        }

        if (limitePassarosEmGrupo % 2 != 0)
        {
            limitePassarosEmGrupo++;
        }

        numeroPassarosTotais = birdsList.childCount;


        if (numeroPassarosTotais < numeroDeGrupos * (passarosPorGrupo + limitePassarosEmGrupo))
        {
            //Debug.Log("Quantos tenho: " + numeroPassarosTotais + "\nQuantos preciso: " + numeroDeGrupos * (passarosPorGrupo + limitePassarosEmGrupo));
            InstanciaPassarosRestantes((int)(numeroDeGrupos * (passarosPorGrupo + limitePassarosEmGrupo)) - numeroPassarosTotais);
        }

        passaros = new List<GameObject>();
        //Debug.Log(birdsList.childCount);
        for (int i = 0; i < birdsList.childCount; i++)
        {
            passaros.Add(birdsList.GetChild(i).gameObject);
        }


        CriacaoDeGrupos();
        //Debug.Log(indiceAtual);
        ApagaBirdsNaoUsados(indiceAtual);
        StartCoroutine(ReutilizarBirds());
    }

}
