using UnityEngine;
using System.Collections;

public class PlayerBoatManager : MonoBehaviour {
    public static PlayerBoatManager instance;
    public float velocidadeMovimentacao = 0;

    private bool input = false;
    private Transform myTransform;

	// Use this for initialization
	void Start () {
        instance = this;
        myTransform = this.transform;
    }
	
	// Update is called once per frame
	void Update () {

        if (input) {
            float deadZone = 0.1f;
            Vector3 MoveVector = Vector3.zero;

            if (Input.GetAxis("Vertical") > deadZone || Input.GetAxis("Vertical") < -deadZone)
            {
                MoveVector.x += Input.GetAxis("Vertical") * velocidadeMovimentacao * Time.deltaTime;
            }

            if (Input.GetAxis("Horizontal") > deadZone || Input.GetAxis("Horizontal") < -deadZone)
            {
                MoveVector.z -= Input.GetAxis("Horizontal") * velocidadeMovimentacao * Time.deltaTime;
            }

            //Debug.DrawLine(myTransform.position, myTransform.position + MoveVector, Color.red, 10);

            myTransform.position += MoveVector;
            //gameObject.GetComponent<Rigidbody>().AddForce(MoveVector * 100);

            Quaternion newRotation = Quaternion.LookRotation(MoveVector);
            myTransform.rotation = newRotation;

            if (MoveVector != Vector3.zero)
            {
                //SnapAlignCharacterWithCamera();
                myTransform.Rotate(0, 180, 0);
            }
            else {
                myTransform.Rotate(0, 270, 0);
            }
            
        }

	}

    void SnapAlignCharacterWithCamera()
    {
            myTransform.rotation = Quaternion.Euler(0,
                    Camera.main.transform.eulerAngles.y,
                    0);
    }

    public void AtivaInput(){
        input = true;
        //Debug.Log("Posso me mover");
    }

    public void DesativaInput() {
        input = false;
        //Debug.Log("Nao posso me mover");
    }

    public void AtualizaPosPlayerObjetivo(SaveTransform transformTemp) {
        myTransform.position = transformTemp.position;
        myTransform.rotation = transformTemp.rotation;
        myTransform.localScale = transformTemp.localScale;

        //CameraManager.instance.ResetPosCameraPlayer(myTransform.eulerAngles.y, myTransform.position);
        CameraManager.instance.ResetPosCameraBarco();
    }
}
