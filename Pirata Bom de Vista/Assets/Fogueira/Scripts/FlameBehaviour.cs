using UnityEngine;
using System.Collections;

public class FlameBehaviour : MonoBehaviour {

    private float forca = 3f;
    private Transform myTransform;
    private Vector3 temp;
    private bool ativar = false;

    private float startTime = 0, journeyLength = 0;
    private Vector3 destino, origem, escalaInicial, escalaFinal;


    private Camera m_Camera;
    private bool mudarEscala = false, mudarRotacao = false, fumaca = false;
    private float rotacaoZ = 0;

    private float distCovered = 0, fracJourney = 0;

    // Use this for initialization
    void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
        if (ativar) {
            distCovered = (Time.time - startTime) * forca;
            fracJourney = distCovered / journeyLength;

            myTransform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);
            Vector3 temp = myTransform.eulerAngles;

            if (mudarRotacao) {
                rotacaoZ -= 50f * Time.deltaTime;
            }

            temp.z = rotacaoZ;
            myTransform.eulerAngles = temp;


            if (fracJourney < 0.9f)
            {
                myTransform.position = Vector3.Lerp(origem, destino, fracJourney);

                if (fracJourney % 2 != 0 && mudarEscala)
                {
                    myTransform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, fracJourney);
                }

            }
            else {
                gameObject.SetActive(false);
                ativar = false;
                if (fumaca) {
                    EmissorFogueira.instance.SoltaFumaca(myTransform.position);
                }
            }
        }
    }


    public void AtivarParticula(Vector3 destino, bool mudarEscala, bool mudarRotacao, float rotacaoZ, bool soltarFumaca) {
        myTransform = this.transform;
        startTime = Time.time;
        ativar = true;
        this.destino = destino;
        this.mudarEscala = mudarEscala;
        origem = myTransform.position;
        journeyLength = Vector3.Distance(myTransform.position, this.destino);
        m_Camera = Camera.main;
        escalaFinal = myTransform.localScale / 2;
        escalaInicial = myTransform.localScale;
        this.rotacaoZ = rotacaoZ;
        this.mudarRotacao = mudarRotacao;
        this.fumaca = soltarFumaca;
    }
}
