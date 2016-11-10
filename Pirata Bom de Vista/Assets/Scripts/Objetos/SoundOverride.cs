using UnityEngine;
using System.Collections;

public class SoundOverride : MonoBehaviour {

    public FaseAtual[] faseCorrespondente;
    public SomPasso somOverride = new SomPasso();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerStay(Collider objeto)
    {
        if (CheckSeFazParte()) {
            if (objeto.gameObject.CompareTag("Player"))
            {
                PlayerManager.instance.overrideSom = true;
                PlayerManager.instance.newSomOverride = AudioManager.instance.CopiaSomPasso(somOverride);
            }
        }
    }

    void OnTriggerExit(Collider objeto)
    {
        if (CheckSeFazParte())
        {
            if (objeto.gameObject.CompareTag("Player"))
            {
                PlayerManager.instance.overrideSom = false;
                PlayerManager.instance.newSomOverride = new SomPasso();
            }
        }
    }

    public bool CheckSeFazParte() {
        FaseAtual temp = ObjetivosController.instance.GetFaseAtual();

        for (int i = 0; i < faseCorrespondente.Length; i++)
        {
            if (temp.fase == faseCorrespondente[i].fase && temp.indice == faseCorrespondente[i].indice)
            {
                return true;
            }
        }


        return false;
    }

    void OnDrawGizmos() {
        Color32 cor = Color.red;
        cor.a = 170;
        Gizmos.color = cor;
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();

        Gizmos.DrawCube(transform.position + collider.center, collider.size);
    }
}
