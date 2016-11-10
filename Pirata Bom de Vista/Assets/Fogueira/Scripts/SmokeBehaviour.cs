using UnityEngine;
using System.Collections;

public class SmokeBehaviour : MonoBehaviour {


    private bool ativar = false;
    private Vector3 origem, destino;

    private float forca = 2.5f, rotacaoZ = 0;
    private float startTime = 0, journeyLength = 0;
    private float distCovered = 0, fracJourney = 0;

    private Camera m_Camera;
    private Transform myTransform;

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (ativar)
        {
            distCovered = (Time.time - startTime) * forca;
            fracJourney = distCovered / journeyLength;

            myTransform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);
            Vector3 temp = myTransform.eulerAngles;
            temp.z = rotacaoZ;
            myTransform.eulerAngles = temp;


            if (fracJourney < 0.9f)
            {
                myTransform.position = Vector3.Lerp(origem, destino, fracJourney);
            }
            else {
                gameObject.SetActive(false);
                ativar = false;
            }
        }
    }

    public void AtivarParticula(Vector3 destino, float rotacaoZ) {
        myTransform = this.transform;
        ativar = true;
        this.destino = destino;
        origem = myTransform.position;
        startTime = Time.time;
        journeyLength = Vector3.Distance(myTransform.position, this.destino);
        m_Camera = Camera.main;
        this.rotacaoZ = rotacaoZ;
    }
}
