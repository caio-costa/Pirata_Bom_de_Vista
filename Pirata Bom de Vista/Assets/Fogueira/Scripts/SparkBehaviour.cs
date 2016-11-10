using UnityEngine;
using System.Collections;

public class SparkBehaviour : MonoBehaviour {

    private Transform myTransform;
    private MeshRenderer _renderer;
    private bool ativar = false;
    private Camera m_Camera;

    private float tempoDuracao = 0;
    private float contador = 0;
    private float rangeLargura = 0;
    private float velocidade = 0f;
    private float contadorLargura = 0;
    private bool ida = true;
    private float rotacaoZ = 0;
    private float decremento = -15;
    private float velocidadeParametro = 5f;
    private float tempoLargura = 0.2f;
    private Material matTemporario;
    private Color32 corPadrao = new Color32(255,26,0,255);

    // Use this for initialization

    // Update is called once per frame
    void Update () {
        if (ativar)
        {
            if (contador < tempoDuracao)
            {
                contador += Time.deltaTime;
                float viajado = contador / tempoDuracao;
                if (velocidade > 1f)
                {
                    velocidade -= viajado / 5;
                }
                else
                {
                    ativar = false;
                }

                //Debug.Log("Porcentagem Viajada: " + viajado);
                myTransform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);

                Vector3 temp = myTransform.eulerAngles;
                temp.z = rotacaoZ;
                myTransform.eulerAngles = temp;

                Vector3 direcao = myTransform.up * velocidade * Time.deltaTime;
                myTransform.position += direcao;

                if (contadorLargura < tempoLargura)
                {
                    if (ida)
                    {
                        //Debug.Log("ida");
                        myTransform.localPosition += myTransform.right * velocidade / 2 * Time.deltaTime;

                    }
                    else
                    {
                        //Debug.Log("Volta");
                        myTransform.localPosition -= myTransform.right * velocidade / 2 * Time.deltaTime;
                    }

                    contadorLargura += Time.deltaTime;

                }
                else
                {
                    ida = !ida;
                    contadorLargura = 0;

                    if (ida)
                    {
                        rotacaoZ += decremento;
                    }
                    else
                    {
                        rotacaoZ -= decremento;
                    }
                }

            }
            else
            {
                ativar = false;
                this.gameObject.SetActive(false);
            }
        }
        else {
            this.gameObject.SetActive(false);
        } 
	}

    public void AtivarParticula(float tempoDuracao, float rangeLargura, bool novaCor, Color corNova) {

        if (_renderer == null) {
            _renderer = gameObject.GetComponent<MeshRenderer>();
        }

        if (myTransform == null) {
            myTransform = this.transform;
        }

        if (m_Camera == null)
        {
            this.m_Camera = Camera.main;
        }

        if (novaCor)
        {
            _renderer.material.SetColor("_EmissionColor", corNova);
            _renderer.material.SetColor("_Color", corNova);
        }
        else if(_renderer.material.GetColor("_EmissionColor") != corPadrao) {
            _renderer.material.SetColor("_EmissionColor", corPadrao);
            _renderer.material.SetColor("_Color", corPadrao);
        }

        contador = 0;
        ativar = true;
        this.tempoDuracao = tempoDuracao;
        this.rangeLargura = rangeLargura;
        this.rotacaoZ = myTransform.eulerAngles.z;
        velocidadeParametro = 5 + Random.Range(-1.7f, 1.0f);
        tempoLargura = 0.2f + Random.Range(-0.15f,0.15f);
        velocidade = velocidadeParametro;
    }

    /// <summary>
    /// Caso a spark tenha uma textura aleatória
    /// </summary>
    /// <param name="tempoDuracao"></param>
    /// <param name="rangeLargura"></param>
    /// <param name="textura"></param>
    public void AtivarParticula(float tempoDuracao, float rangeLargura, Texture textura, bool novaCor, Color corNova)
    {
        if (_renderer == null)
        {
            _renderer = gameObject.GetComponent<MeshRenderer>();
        }

        if (myTransform == null)
        {
            myTransform = this.transform;
        }

        if(m_Camera == null){
            this.m_Camera = Camera.main;
        }

        if (novaCor){
            _renderer.material.SetColor("_EmissionColor", corNova);
            _renderer.material.SetColor("_Color", corNova);
        }
        else if (_renderer.material.GetColor("_EmissionColor") != corPadrao){
            _renderer.material.SetColor("_EmissionColor", corPadrao);
            _renderer.material.SetColor("_Color", corPadrao);
        }

        _renderer.material.mainTexture = textura;
        _renderer.material.SetTexture("_EmissionMap", textura);
        contador = 0;
        ativar = true;
        this.tempoDuracao = tempoDuracao;
        this.rangeLargura = rangeLargura;
        this.rotacaoZ = myTransform.eulerAngles.z;
        velocidadeParametro = 5 + Random.Range(-1.7f, 1.0f);
        tempoLargura = 0.2f + Random.Range(-0.15f, 0.15f);
        velocidade = velocidadeParametro;
    }
}
