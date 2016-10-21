using UnityEngine;
using System.Collections;

public class BirdsBehaviour : MonoBehaviour {

    public Vector3 direcao, posInicial, posFinal, posManager;
    private Transform myTransform;
    private float velocidadeVoo = 0;
    private float distanciaInicial = 0;
    public bool voando = false;

    // Use this for initialization
    void Start () {
        myTransform = this.transform;
        
    }
	
	// Update is called once per frame
	void Update () {
        //SetDirecao(posTemp.position);

    }

    public void SetPosicaoInicial(Vector3 posInicial) {
        myTransform = this.transform;
        this.posInicial = posInicial;
        myTransform.position = posInicial;
        posManager = BirdsManager.instace.GetPosManager();
        distanciaInicial = Vector3.Distance(myTransform.position, posManager);
    }

    public void SetDirecao(Vector3 destino, float velocidadeVoo) {
        this.posFinal = destino;
        this.velocidadeVoo = velocidadeVoo;
        this.posInicial = myTransform.position;

        direcao = -destino;
        myTransform.LookAt(destino);
        myTransform.Rotate(new Vector3(0, 180, 0));

        StartCoroutine(IrDirecao());
    }

    public IEnumerator IrDirecao() {
        voando = true;
        Vector3 destino = posFinal;
        distanciaInicial += 10;
        //Debug.Log("Distancia Inicial: " + distanciaInicial);

        while (Vector3.Distance(myTransform.position, posManager) <  distanciaInicial * 1.2f) {

            myTransform.position += -myTransform.forward * velocidadeVoo * Time.deltaTime;

            yield return null;
        }

        direcao = Vector3.zero;
        posInicial = Vector3.zero;
        posFinal = Vector3.zero;
        posManager = Vector3.zero;
        velocidadeVoo = 0;
        distanciaInicial = 0;
        myTransform.position = Vector3.zero;
        myTransform.eulerAngles = Vector3.zero;
        this.gameObject.SetActive(false);
        this.gameObject.name = "Sem Uso";
        voando = false;
        StopAllCoroutines();

    }
}
