using UnityEngine;
using System.Collections;
using UnityEditor;

//[ExecuteInEditMode]
public class CriadorModeasPath : MonoBehaviour {

    public Mesh moeda;
    public GameObject prefabMoeda;
    public float distanciaClique;
    public bool ativar = false;


    private Vector3 position, scale;
    private Quaternion rotacao;


    public float playbackModifier = 1;
    private float lastTime;

	// Use this for initialization
	void Start () {
	
	}

    void OnEnable() {
        //EditorApplication.update -= OnUpdate;
        //EditorApplication.update += OnUpdate;
    }

    void OnDisable() {
        //EditorApplication.update -= OnUpdate;
    }

    void OnUpdate() {
        if (playbackModifier != 0.0f) {
            //Debug.Log("Aqui");
            PreviewTime.Time += (Time.realtimeSinceStartup - lastTime) * playbackModifier;

            //Repaint();

            CastRay();
            SceneView.RepaintAll();
        }

        lastTime = Time.realtimeSinceStartup;
    }

	// Update is called once per frame
	void CastRay () {
        if (ativar) {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            

            if (Physics.Raycast(ray, out hit, distanciaClique)) {
                Debug.DrawLine(Input.mousePosition, hit.collider.transform.position, Color.red, 10);
                Debug.Log("bateu mais");

                position = hit.collider.transform.position;
                scale = new Vector3(6.170388f, 6.170388f, 6.170388f);
                rotacao = hit.collider.transform.rotation;               
            }

        }
	}

    void OnDrawGizmos() {
        if (ativar) {
            Gizmos.color = Color.green;
            for (int i = 0; i < moeda.subMeshCount; i++)
            {
                //Gizmos.DrawMesh(moeda, i, position, rotacao, scale);
            }
           
        }
    }
}
