using UnityEngine;
using System.Collections;

public class RotacaoBarcoSimples : MonoBehaviour {

    public bool rotacionar = false;
    public bool levitar = false;
    public float limite = 3;
    public float limiteSubida = 0.4f;
    public float velocidade = 0.5f;
    public float rotacaoInicial = 180;

    private float rotacao = 0, contadorSubida = 0, inicial = 0;
    private Transform myTransform;
    private bool ida = false, idaSubida = true;

	// Use this for initialization
	void Start () {
        myTransform = this.transform;
        inicial = myTransform.position.y;
        
    }

    // Update is called once per frame
    void Update() {
        if (rotacionar) {
            if (ida)
            {
                if (rotacao < limite)
                {

                    rotacao += Time.deltaTime * velocidade;
                    myTransform.eulerAngles = new Vector3(0, rotacaoInicial, rotacao);
                }
                else
                {
                    //rotacao = limite;
                    ida = !ida;
                }
            }
            else {
                if (rotacao > -limite)
                {

                    rotacao -= Time.deltaTime * velocidade;
                    myTransform.eulerAngles = new Vector3(0, rotacaoInicial, rotacao);
                }
                else
                {
                    
                    //rotacao = -limite;
                    ida = !ida;
                }
            }
        }
        else {

            myTransform.eulerAngles = Vector3.zero;

            }

        if (levitar) {
            if (idaSubida)
            {
                if (contadorSubida < limiteSubida)
                {
                    //Debug.Log("Limite Subida: " + limiteSubida + "\nContador: " + contadorSubida);
                    contadorSubida += Time.deltaTime * velocidade / 7;

                    Vector3 temp = myTransform.position;
                    temp.y = inicial + contadorSubida;
                    myTransform.position = temp;
                }
                else
                {
                    idaSubida = !idaSubida;
                }

            }
            else {
                if (contadorSubida > -limiteSubida)
                {
                    
                    contadorSubida -= Time.deltaTime * velocidade / 7;

                    Vector3 temp = myTransform.position;
                    temp.y = inicial + contadorSubida;
                    myTransform.position = temp;
                }
                else {
                    //Debug.Log("Descendo");
                    idaSubida = !idaSubida;
                }
            }
        }

    }
}
