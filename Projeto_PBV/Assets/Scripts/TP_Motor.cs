using UnityEngine;
using System.Collections;

public class TP_Motor : MonoBehaviour {

    public static TP_Motor Instance;
    public float MoveSpeed = 10f;
    public Vector3 MoveVector { get; set; }

	void Awake() {
        Instance = this;
	}
	
	public void UpdateMotor() {
        SnapAlignCharacterWithCamera();
        ProcessMotion();
    }

    void ProcessMotion(){
        //Transforma MoveVector em espaço do mundo
        MoveVector = transform.TransformDirection(MoveVector);

        //Normalizar MoveVector caso tamanho > 1
        if (MoveVector.magnitude > 1)
            MoveVector = Vector3.Normalize(MoveVector);

        //Multiplicar MoveVector por MoveSpeed
        MoveVector *= MoveSpeed;

        //Multiplicar MoveVector por DeltaTime
        MoveVector *= Time.deltaTime;

        //Mover o personagem pelo espaço do mundo
        TP_Controller.CharacterController.Move(MoveVector);
    }

    void SnapAlignCharacterWithCamera(){
        if(MoveVector.x != 0 || MoveVector.z != 0){
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, 
                    Camera.main.transform.eulerAngles.y, 
                    transform.eulerAngles.z);
        }
    }
}
