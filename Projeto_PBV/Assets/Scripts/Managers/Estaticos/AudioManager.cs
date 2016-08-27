using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;
    public AudioMixer mixer;

    AudioSource[] pirata;

    private float volume = 1;

    // Use this for initialization
    void Start () {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    #region
    public void PlayPirata()
    {
        int random = Random.Range(0, pirata.Length);
        pirata[random].Play();
    }

    public void PlayPirata(int indice)
    {
        pirata[indice].Play();
    }
    #endregion


    #region Loader

    #endregion

    #region SettersGetters
    public void SetVolumeMaster(float volume) {
        this.volume = volume;
        mixer.SetFloat("VolumeMaster", this.volume);
    }
    #endregion
}
