using UnityEngine;
using System.Collections;

public class moedasBehaviour : MonoBehaviour {

    private bool ativou = false;
	// Use this for initialization
	void Start () {

    }

    public void OnCollisionEnter(Collision objeto) {
        if (objeto.gameObject.CompareTag("Cais")) {
            if (!ativou)
            {
                GetComponent<AudioSource>().Play();
                ativou = true;
            }
        }
        
    }


}
