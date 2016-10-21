using UnityEngine;
using System.Collections;

public class SoundOverride : MonoBehaviour {

    public SomPasso somOverride = new SomPasso();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerStay(Collider objeto)
    {
        if (objeto.gameObject.CompareTag("Player")) {
            PlayerManager.instance.overrideSom = true;
            PlayerManager.instance.newSomOverride = AudioManager.instance.CopiaSomPasso(somOverride);
        }
    }

    void OnTriggerExit(Collider objeto)
    {
        if (objeto.gameObject.CompareTag("Player"))
        {
            PlayerManager.instance.overrideSom = false;
            PlayerManager.instance.newSomOverride = new SomPasso();
        }
    }

    void OnDrawGizmos() {
        Color32 cor = Color.red;
        cor.a = 170;
        Gizmos.color = cor;
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();

        Gizmos.DrawCube(transform.position + collider.center, collider.size);
    }
}
