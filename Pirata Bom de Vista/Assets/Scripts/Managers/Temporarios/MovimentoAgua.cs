using UnityEngine;
using System.Collections;

public class MovimentoAgua : MonoBehaviour {

    public bool moverAgua = false;
    public float velocidade = 0.04f;

    private float offset = 0;
    private Material mat;

	// Use this for initialization
	void Start () {
        mat = gameObject.GetComponent<MeshRenderer>().material;

    }

    // Update is called once per frame
    void Update() {

        if (moverAgua){
            offset += Time.deltaTime * velocidade;
            Vector2 temp = new Vector2(offset, offset / 4);

            mat.mainTextureOffset = temp;
        }
	}
}
