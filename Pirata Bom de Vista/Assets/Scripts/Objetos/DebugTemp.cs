using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class DebugTemp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100)) {
            if (hit.collider.gameObject.CompareTag("Continente")) {
                Debug.DrawLine(ray.origin, hit.point, Color.red, 10);
            }
            
        }

	}
}
