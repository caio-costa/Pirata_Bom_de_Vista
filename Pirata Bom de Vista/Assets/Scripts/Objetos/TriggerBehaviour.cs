using UnityEngine;
using System.Collections;

public class TriggerBehaviour : MonoBehaviour {

    public FaseAtual faseCorrespondente = new FaseAtual();
    public bool ativo = true;
    public bool utilizarSphere = true;
    public bool objetivo = true;
    public bool interativo = false;
    private bool interagindo = false;

    public string triggerNameAnim = "";
    public float raio = 5.2f;


    public bool CheckSeFaseCorresponde() {
        return ObjetivosController.instance.GetFaseAtual().fase == faseCorrespondente.fase && ObjetivosController.instance.GetFaseAtual().indice == faseCorrespondente.indice;
    }

    void Update()
    {
        if (ativo)
        {
            if (CheckSeFaseCorresponde())
            {
                if (!utilizarSphere)
                {
                    Transform player = GameObject.FindGameObjectWithTag("Player").transform;
                    if (Vector3.Distance(player.position, this.transform.position) <= raio) {
                        if (objetivo)
                        {
                            ObjetivosController.instance.ProximoObjetivo();
                            ativo = false;
                        }
                        else if (interativo) {
                            //Debug.Log("Trigger");
                            PlayerManager.instance.AtivaTriggerInteracaoAnim(triggerNameAnim);
                            ativo = false;
                        }
                        
                    }
                }

            }

        }
    }


    void OnTriggerEnter(Collider objeto)
    {
        if (ativo)
        {
            if (CheckSeFaseCorresponde()) {
                if (utilizarSphere)
                {
                    if (objeto.gameObject.CompareTag("Player"))
                    {
                        ObjetivosController.instance.ProximoObjetivo();
                        ativo = false;
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider objeto)
    {
        if (utilizarSphere)
        {
            if (CheckSeFaseCorresponde())
            {
                if (objeto.gameObject.CompareTag("Player"))
                {
                    interagindo = false;
                }
            }
        }
    }

}
