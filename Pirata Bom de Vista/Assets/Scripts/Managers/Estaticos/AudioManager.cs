using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;
    public AudioMixer mixer;

    [Header("Sons de passos")]
    public SomPasso[] somPassos = new SomPasso[0];

    private AudioSource[] pirata;
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

    public AudioSom GetSonsDeTextura(Texture textura) {
        Vector2 indice = Vector2.zero;

        for (int i = 0; i < somPassos.Length; i++)
        {
            for (int a = 0; a < somPassos[i].texturas.Length; a++)
            {
                if (somPassos[i].texturas[a] == textura) {
                    
                    return CopiaAudioSom(somPassos[i].GetClipeAleatorio());
                }
            }
        }

        return null;
    }
    #endregion

    public AudioSom CopiaAudioSom(AudioSom original) {
        AudioSom copia = new AudioSom();
        copia.clipe = original.clipe;
        copia.volume = original.volume;
        copia.pitchOffset = original.pitchOffset;

        return copia;
    }

    public SomPasso CopiaSomPasso(SomPasso original) {
        SomPasso copia = new SomPasso();
        copia.clipesCorrespondentes = original.clipesCorrespondentes;
        copia.nome = original.nome;
        copia.texturas = original.texturas;
        return copia;

    }
}

[System.Serializable]
public class SomPasso {
    public string nome = "";
    public Texture2D[] texturas;
    public AudioSom[] clipesCorrespondentes = new AudioSom[0];

    public AudioSom GetClipeAleatorio() {
        int random = Random.Range(0, this.clipesCorrespondentes.Length);
        return clipesCorrespondentes[random];
    }
}

[System.Serializable]
public class AudioSom {
    public AudioClip clipe = new AudioClip();
    [Range(0,1)]
    public float volume = 0;
    [Range(0f,0.4f)]
    public float pitchOffset = 0;
}
