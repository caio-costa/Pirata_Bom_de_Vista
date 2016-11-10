using UnityEngine;
using System.Collections;

public class AnimatorRecieverEvents : MonoBehaviour {

    public GameObject garrafaMao, sacoMoedas;
    public PirateBehaviour pirataVingativo;

    // Use this for initialization
    void Start () {
	
	}

    public void AtivaAudio() {
        PlayerManager.instance.AtivaAudio();
    }

    public void AtivaFala() {
        PlayerManager.instance.AtivaFala();
    }

    public void AtivaAudioPirata() {
        if (pirataVingativo == null) {
            EncontraPirata();
        }

        pirataVingativo.AtivaAudio();
    }

    public void AtivaApenasAudioPirata() {
        if (pirataVingativo == null)
        {
            EncontraPirata();
        }

        pirataVingativo.AtivaFala();
    }

    public void AtivaLegendaPirata() {
        if (pirataVingativo == null)
        {
            EncontraPirata();
        }

        pirataVingativo.AtivaLegenda();
    }

    public void CortaAudio() {
        PlayerManager.instance.CortaAudio();
    }

    public void AtivaAudioReclamacao() {
        PlayerManager.instance.AtivaAudioReclamacao();
    }

    public void AtivaAudioCaldeirao() {
        PlayerManager.instance.AtivaAudioCaldeirao();
    }

    public void AtivaSomPasso() {
//        PlayerManager.instance.Ativa
    }

    public void AtivaLegenda() {
        PlayerManager.instance.AtivaLegenda();
    }

    public void AtivaInput() {
        //Debug.Log("Foi");
        PlayerManager.instance.SetPlayerState(PlayerState.MOVIMENTACAO);
    }

    public void AtivaGarrafaMao() {
        garrafaMao.SetActive(true);
    }

    public void SoltaGarrafaMao() {
        StartCoroutine(SoltaGarrafa());
    }

    public IEnumerator SoltaGarrafa() {
        garrafaMao.transform.SetParent(null);
        yield return null;
        garrafaMao.GetComponent<Rigidbody>().isKinematic = false;
        garrafaMao.GetComponent<Rigidbody>().useGravity = true;
        garrafaMao.GetComponent<AudioSource>().Stop();
    }

    public void SomBatidaPirata() {
        GetComponent<AudioSource>().Play();
    }

    public void EncontraPirata() {
        pirataVingativo = GameObject.FindGameObjectWithTag("PirataVingativo").GetComponent<PirateBehaviour>();
    }

    public void SoltaSacoMoedas() {
        sacoMoedas.transform.SetParent(null);
        sacoMoedas.transform.position = new Vector3(-105.02f, 1.79f, 33.81f);
        sacoMoedas.transform.eulerAngles = new Vector3(270,0,0);
    }

}

