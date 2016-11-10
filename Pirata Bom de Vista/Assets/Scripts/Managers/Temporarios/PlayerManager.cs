using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerState {
    MOVIMENTACAO,
    COMBATE,
    CUTSCENE,
    NAVEGANDO
}

public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance;
    public bool movimentando = false;

    [HideInInspector]
    public PlayerState playerState;
    //public AudioClip[] falas;
    //public int debugRand = 0;

    [SerializeField]
    public List<CorteAnimacao> listaAnimacao = new List<CorteAnimacao>();

    [HideInInspector]
    public bool overrideSom = false;
    [HideInInspector]
    public SomPasso newSomOverride = new SomPasso();
    private Animator animator;
    private int indiceNome = 1;
    private CorteAnimacao corteAtual;
    private AudioSource _audioSource;
    private AudioSource _audioSourcePassos;
    private Transform myTransform;

    private float contador = 0;
    //TP_CONTROLLER

    public static CharacterController CharacterController;


    // Use this for initialization
    void Start () {
        instance = this;
        myTransform = this.transform;
        animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        //AtivaAudioReclamacao();
        for (int i = 0; i < myTransform.childCount; i++)
        {
            if (myTransform.GetChild(i).name == "SonsPassos")
            {
                _audioSourcePassos = myTransform.GetChild(i).gameObject.GetComponent<AudioSource>();
            }
            else if (myTransform.GetChild(i).name == "CorpoPirata") {
                animator = myTransform.GetChild(i).gameObject.GetComponent<Animator>();
            }
        }

        CharacterController = GetComponent("CharacterController") as CharacterController;
        CameraManager.UseExistingOrCreateNewMainCamera();

        //TODO: Trocar essa função por passos na animação.
        StartCoroutine(AtivaSonsPassosTemp());
    }
    // Update is called once per frame
    void Update () {

        switch (playerState)
        {
            case PlayerState.MOVIMENTACAO:

                float limite = 10;
                bool quebrou = false;

                if (Camera.main == null)
                    return;
                GetLocomotionInput();

                
                if (PlayerController.instance.MoveVector != Vector3.zero)
                {
                    if (!movimentando) {
                        quebrou = true;
                    }

                    movimentando = true;
                }
                else {
                    if (movimentando) {
                        quebrou = true;
                    }

                    movimentando = false;
                }

                if (!quebrou)
                {
                    if (contador < limite)
                    {
                        contador += Time.deltaTime;
                    }
                    else
                    {
                        contador = 0;
                        AtivaIdleAleatorio();
                    }
                }

                if (movimentando) {
                    contador = 0;
                }

                animator.SetBool("Mover", movimentando);
                
                HandleActionInput();

                PlayerController.instance.UpdateMotor();
                break;

            case PlayerState.CUTSCENE:
                movimentando = false;
                contador = 0;

                animator.SetBool("Mover", movimentando);
                break;
        }
    }

    public void AtivaIdleAleatorio(){
        int numeroIdles = 4;
        int rand = Random.Range(0, 10);

        //rand = debugRand;

        if (rand < 3)
        {
            animator.SetTrigger("VariacaoIdle1");
        }
        else if (rand >= 3 && rand < 6)
        {
            animator.SetTrigger("VariacaoIdle2");
        }
        else if (rand >= 6 && rand < 9)
        {
            animator.SetTrigger("VariacaoIdle3");
        }
        else if (rand == 9) {
            animator.SetTrigger("VariacaoIdle4");
        }
    }

    void OnEnable()
    {
        LevelManager.eventoCutscene += PlayAnimacaoCutscene;
    }

    void OnDisable()
    {
        LevelManager.eventoCutscene -= PlayAnimacaoCutscene;
    }

    public void SetPlayerState(PlayerState temp) {
        if (temp == PlayerState.MOVIMENTACAO || temp == PlayerState.COMBATE)
        {
            //PlayerController.instance.SetPermiteInput(true);

            if (temp == PlayerState.COMBATE)
            {
                //PlayerController.instance.SetInputCombate(true);
            }
            else {
                //PlayerController.instance.SetInputCombate(false);
            }
        }
        else {
            //PlayerController.instance.SetPermiteInput(false);
            //PlayerController.instance.SetInputCombate(false);
        }

        //Debug.Log(temp);
        playerState = temp;
    }


    public void PlayAnimacaoCutscene()
    {
        ConteudoAtual indiceAnim = DirectorsManager.instance.GetCorteAtual();


        if (FazParteAnimacao(indiceAnim))
        {
            RuntimeAnimatorController myController = animator.runtimeAnimatorController;
            AnimatorOverrideController anim_OverrideController = new AnimatorOverrideController();
            anim_OverrideController.runtimeAnimatorController = myController;
            AnimationClip clipe = corteAtual.clipeAnim;
            string nome = "";
            //for (int i = 0; i < anim_OverrideController.clips.Length; i++) {
            //Debug.Log(anim_OverrideController.clips[i].overrideClip.name.ToString());
            //  if (anim_OverrideController.clips[i].ToString() != "Idle") {

            //} 
            //}


            
            if (indiceNome > 1)
            {

                nome = "ct_" + (indiceNome - 1).ToString();
            }
            else
            {
                nome = "ct_" + (indiceNome).ToString();
            }


            //Debug.Log("Nome anterior: " + nome);
            indiceNome++;
            

            anim_OverrideController[nome] = clipe;
            animator.runtimeAnimatorController = anim_OverrideController;

            if (corteAtual.posicao != Vector3.zero) {
                myTransform.position = corteAtual.posicao;
            }

            if (corteAtual.rotacao != Vector3.zero) {
                myTransform.eulerAngles = corteAtual.rotacao;
            }


            StartCoroutine(AtivaAnimacaoCutscene());

            if (corteAtual.decisivo == true)
            {
                //Debug.Log("Decisivo");
                StartCoroutine(EsperaTerminoAnimacao());
            }
        }
    }

    private IEnumerator AtivaAnimacaoCutscene()
    {
        yield return new WaitForSeconds(DirectorsManager.instance.tempoAguardoCutscenes);
        animator.SetTrigger("Cutscene");
    }

    private IEnumerator EsperaTerminoAnimacao()
    {
        DirectorsManager.instance.IniciarAnimDecisiva();

        //Debug.Log("Esperando");
        //Aguarda o começo
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
          //  Debug.Log("Esperando2");
            yield return null;
        }

        //Aguarda o termino
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("ct_Anim"))
        {
            //Debug.Log("Esperando3");
            yield return null;
        }



        DirectorsManager.instance.AcabarAnimDecisiva();

        yield return 0;
    }



    public bool FazParteAnimacao(ConteudoAtual indice)
    {
        /*
        Debug.Log("Indice: " +
            "\nFase: " + indice.fase +
            "\nCutscene: " + indice.cutscene +
            "\nCorte: " + indice.corte
            );
        */

        for (int i = 0; i < listaAnimacao.Count; i++)
        {
            if (indice.fase == listaAnimacao[i].indiceAnim.fase)
            {
                if (indice.cutscene == listaAnimacao[i].indiceAnim.cutscene) {
                    if (indice.corte == listaAnimacao[i].indiceAnim.corte) {
                        corteAtual = DirectorsManager.instance.CopiaCorteAnimacao(listaAnimacao[i]);
                        //Debug.Log("Faz corte");
                        return true;
                    }
                }
            }
        }

        //Debug.Log("Faz parte?");
        corteAtual = new CorteAnimacao();
        return false;


    }


    public void AtivaAudio()
    {
        if (DirectorsManager.instance == null) { return; }

        StartCoroutine(TextoLegenda());
        //DirectorsManager.instance.ProximaLegenda();
    }

    public void AtivaLegenda() {
        DirectorsManager.instance.ProximaLegenda();
    }

    public void CortaAudio() {
        _audioSource.Stop();
    }

    public void AtivaAudioReclamacao()
    {
        AudioClip clipe = Resources.Load<AudioClip>("Falas/Reclamacao") as AudioClip;
        _audioSource.clip = clipe;
        _audioSource.Play();

        StartCoroutine(EsperaIdleAnimTrigger());
    }

    public void AtivaAudioCaldeirao() {
        AudioClip clipe = Resources.Load<AudioClip>("Falas/Caldeirao") as AudioClip;
        _audioSource.clip = clipe;
        _audioSource.Play();

        StartCoroutine(EsperaIdleAnimTrigger());
    }

    private IEnumerator TextoLegenda()
    {
        
        DirectorsManager.instance.ProximaLegenda();
        
        yield return new WaitForSeconds(0.5f);
        AtivaFala();
    }

    public void AtivaFala()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
            return;
        }

        if (corteAtual.fala == null) return;

        _audioSource.clip = corteAtual.fala;
        _audioSource.Play();
    }


    void GetLocomotionInput()
    {

        var deadZone = 0.1f;

        PlayerController.instance.VerticalVelocity = PlayerController.instance.MoveVector.y;
        PlayerController.instance.MoveVector = Vector3.zero;

        if (Input.GetAxis("Vertical") > deadZone || Input.GetAxis("Vertical") < -deadZone)
        {
            PlayerController.instance.MoveVector += new Vector3(0, 0, Input.GetAxis("Vertical"));
        }

        if (Input.GetAxis("Horizontal") > deadZone || Input.GetAxis("Horizontal") < -deadZone)
        {
            PlayerController.instance.MoveVector += new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        }
    }

    void HandleActionInput()
    {

        if (Input.GetButton("Jump"))
        {
            Jump();
        }

    }

    void Jump()
    {

        PlayerController.instance.Jump();
    }

    public void AtualizaPosPlayerObjetivo(SaveTransform newTransform) {
        //Debug.Log(newTransform.rotation.eulerAngles.y);
        myTransform.position = newTransform.position;
        myTransform.rotation = newTransform.rotation;
        myTransform.localScale = newTransform.localScale;

        //Debug.Log(myTransform.eulerAngles.y);
        CameraManager.instance.ResetPosCameraPlayer(myTransform.eulerAngles.y, myTransform.position);
    }

    public void AtualizaPosPlayerObjetivoCutscene(SaveTransform newTransform) {
        //Debug.Log("Atualizou pos2");
        myTransform.position = newTransform.position;
        myTransform.rotation = newTransform.rotation;
        myTransform.localScale = newTransform.localScale;

        //Codigo para setarCameraEmPosCutscene
        StartCoroutine(AguardaReferenciasCortes());

        //CameraManager.instance.ResetPosCameraPlayer(myTransform.eulerAngles.y, myTransform.position);
    }

    public IEnumerator AguardaReferenciasCortes() {
        while (DirectorsContent.instance == null) {
            yield return 0;
        }

        while (DirectorsManager.instance == null) {
            yield return null;
        }

        //Debug.Log("Foi");
        CorteSave corte = DirectorsContent.instance.GetCorte(DirectorsManager.instance.indiceConteudoAtual);
        CameraManager.instance.transform.position = corte.posicao;
        CameraManager.instance.transform.rotation = corte.rotacao;
    }

    public void AtivaTriggerInteracaoAnim(string nomeTrigger) {
        //Debug.Log("Anim");
        //CameraManager.instance.ResetPosCameraPlayer(myTransform.eulerAngles.y, myTransform.position);
        SetPlayerState(PlayerState.CUTSCENE);

        animator.SetTrigger(nomeTrigger);

 
    }


    public IEnumerator EsperaIdleAnimTrigger() {
        yield return null;

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            yield return null;
        }

        SetPlayerState(PlayerState.MOVIMENTACAO);
    }


    public IEnumerator AtivaSonsPassosTemp() {
        bool showDebug = false;
        while (true) {

            //yield return null;
            yield return new WaitForSeconds(0.4f);
            if (!movimentando) {
                continue;
            }

            if (overrideSom) {
                //Debug.Log(newSomOverride.nome);
                AudioSom som = newSomOverride.GetClipeAleatorio();
                _audioSourcePassos.volume = som.volume;
                float variacaoPitch = Random.Range(-som.pitchOffset, som.pitchOffset);
                _audioSourcePassos.pitch = 1 + variacaoPitch;

                if (som.clipe != null)
                {
                    _audioSourcePassos.clip = som.clipe;
                    _audioSourcePassos.Play();
                }
                continue;
            }


            if (AudioManager.instance != null) {

                Vector3 direcao = -myTransform.up;
                Vector3 posicao = myTransform.position;
                


                Ray ray = new Ray(posicao, direcao);
                //RaycastHit[] hits = Physics.RaycastAll(ray, 100);
                RaycastHit hit = new RaycastHit();
                /*
                bool bateu = false;

                if (showDebug) {
                    Debug.Log(hits.Length);
                }
                
                for (int a = 0; a < hits.Length; a++)
                {

                    if (hits[a].collider.gameObject.tag == "Continente") {
                        hit = hits[a];
                        bateu = true;
                        break;
                    }
                }
                */
                //if (bateu) {

                if (Physics.Raycast(ray, out hit, 100)) {
                    //Texture2D texturaBaixo = hit.collider.gameObject.GetComponent<MeshRenderer>


                    if (showDebug) {
                        Debug.Log(hit.collider.gameObject);
                        Debug.DrawLine(ray.origin, hit.point, Color.red, 10);
                    }
                    

                    MeshCollider meshCollider = hit.collider as MeshCollider;
                    if (meshCollider == null || meshCollider.sharedMesh == null) {
                        if (showDebug)
                        {
                            Debug.LogError("ERRO");
                        }
                        continue;
                    }

                    

                    Mesh mesh = hit.collider.gameObject.GetComponent<MeshFilter>().mesh;
                    MeshRenderer MeshRenderer = hit.collider.gameObject.GetComponent<MeshRenderer>();
                    int todasSubmeshes = mesh.subMeshCount;
                    int trianguloBatido = hit.triangleIndex;
                    int indiceMaterial = -1;

                    //Debug.Log("Comprimento array triangulos: " + mesh.triangles.Length + "\nNumero interessado: " + trianguloBatido * 3);

                    int lookupIdx1 = mesh.triangles[trianguloBatido * 3];
                    int lookupIdx2 = mesh.triangles[trianguloBatido * 3 + 1];
                    int lookupIdx3 = mesh.triangles[trianguloBatido * 3 + 2];

                    for (int i = 0; i < todasSubmeshes; i++)
                    {
                        int[] triangulosSubmesh = mesh.GetTriangles(i);
                        for (int a = 0; a < triangulosSubmesh.Length; a++)
                        {
                            if (triangulosSubmesh[a] == lookupIdx1 && triangulosSubmesh[a + 1] == lookupIdx2 && triangulosSubmesh[a + 2] == lookupIdx3) {
                                indiceMaterial = i;
                            }
                        }
                    }



                    if (indiceMaterial != -1) {
                        if (showDebug) {
                            Debug.Log(MeshRenderer.materials[indiceMaterial].mainTexture.name);
                        }
                        Texture texturaBaixo = MeshRenderer.materials[indiceMaterial].mainTexture;
                        AudioSom som = AudioManager.instance.GetSonsDeTextura(texturaBaixo);
                        _audioSourcePassos.volume = som.volume;
                        float variacaoPitch = Random.Range(-som.pitchOffset, som.pitchOffset);
                        _audioSourcePassos.pitch = 1 + variacaoPitch;

                        if (som.clipe != null) {
                            _audioSourcePassos.clip = som.clipe;
                            _audioSourcePassos.Play();
                        }
                    }

                    //hit.collider.gameObject.GetComponent<MeshFilter>().mesh.GetTriangles(hit.triangleIndex);


                }
            }
        }
    }
}
