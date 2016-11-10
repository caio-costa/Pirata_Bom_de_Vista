using UnityEngine;
using System.Collections;

public class MoedasSpawner : MonoBehaviour {

    public GameObject moedaPrefab;
    public Transform paiMoedas;
    public float segundosEspera = 1;

	// Use this for initialization
	void OnEnable() {
        StartCoroutine(SpawnarMoedas());
	}


    public IEnumerator SpawnarMoedas() {
        while (true) {
            yield return new WaitForSeconds(segundosEspera);

            if (PlayerManager.instance.movimentando) {
                GameObject moeda = Instantiate(moedaPrefab);
                moeda.transform.parent = paiMoedas;
                moeda.transform.position = this.transform.position;

                float randomRot = Random.value;
                randomRot *= 30;
                float randomRotMult = Random.value;
                randomRotMult *= 3;

                //Debug.Log(randomRot);
                moeda.transform.eulerAngles = new Vector3(-randomRot * randomRotMult, 0, 0);
            }
            
        }
    }


}
