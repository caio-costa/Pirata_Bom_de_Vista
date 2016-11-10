using UnityEngine;
using System.Collections;

public class GarrafaBehaviour : MonoBehaviour {

    public AudioClip garrafaQuebrando;
    private AudioSource _audioSource;
    private bool tocou = false;

	// Use this for initialization
	void Start () {
        _audioSource = gameObject.GetComponent<AudioSource>();

    }


    void OnCollisionEnter(Collision objeto) {
        //Debug.Log("bateu");
        if (!tocou) {
            if (objeto.gameObject.CompareTag("Continente"))
            {
                _audioSource.clip = garrafaQuebrando;
                _audioSource.volume = 0.4f;
                _audioSource.Play();
                tocou = true;
            }
        }
        
    }

}
