using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
    public static CameraManager instance;

    public Transform delta;

    private Vector3 offset;
    private Transform myTransform;

	// Use this for initialization
	void Start () {
        instance = this;
        myTransform = this.transform;
        offset = myTransform.position - delta.position;

        Debug.Log(offset);
    }
	
	// Update is called once per frame
	void Update () {

        if (LevelManager.instance == null) return;

        switch (LevelManager.instance.GetEstadoAtual()) {
            case (EstadoAtual.INPUT):

                myTransform.LookAt(delta);
                myTransform.position = delta.position + offset;
                break;
        }
	}

    public void MovimentarHorizontal(float axis){
    }
}
