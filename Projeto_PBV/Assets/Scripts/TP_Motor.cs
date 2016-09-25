using UnityEngine;
using System.Collections;

public class TP_Motor : MonoBehaviour {

    public static TP_Motor Instance;


    public float MoveSpeed = 10f;
    public float JumpSpeed = 6f;
    public float Gravity = 21f;
    public float TerminalVelocity = 20f;
    public float SlideThreshold = 0.6f;
    public float MaxControllableSlideMagnitude = 0.4f;

    private Vector3 slideDirection;

    public Vector3 MoveVector { get; set; }
    public float VerticalVelocity { get; set; }

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

        //Aplicar sliding caso aplicável
        ApllySlide();

        //Multiplicar MoveVector por MoveSpeed
        MoveVector *= MoveSpeed;

        // Reaplicar a velocidade vertical para o MoveVector.y
        MoveVector = new Vector3(MoveVector.x, VerticalVelocity, MoveVector.z);

        //Aplicar a gravidade
        ApplyGravity();

        //Mover o personagem pelo mundo
        TP_Controller.CharacterController.Move(MoveVector * Time.deltaTime);
    }

    void ApplyGravity(){

    	if(MoveVector.y > -TerminalVelocity)
    		MoveVector = new Vector3(MoveVector.x, MoveVector.y - Gravity * Time.deltaTime, MoveVector.z);

    	if(TP_Controller.CharacterController.isGrounded && MoveVector.y < -1)
    		MoveVector = new Vector3(MoveVector.x, -1, MoveVector.z);

    }

    void ApllySlide(){

    	if(!TP_Controller.CharacterController.isGrounded)
    		return;

    	slideDirection = Vector3.zero;

    	RaycastHit hitInfo;

    	if(Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo)){
    		if(hitInfo.normal.y < SlideThreshold)
    			slideDirection = new Vector3(hitInfo.normal.x, -hitInfo.normal.y, hitInfo.normal.z);
    	}

    	if(slideDirection.magnitude < MaxControllableSlideMagnitude)
    		MoveVector += slideDirection;

    	else{
    		MoveVector = slideDirection;
    	}
    }

    public void Jump(){

    	if(TP_Controller.CharacterController.isGrounded)
    		VerticalVelocity = JumpSpeed;
    }

    void SnapAlignCharacterWithCamera(){

        if(MoveVector.x != 0 || MoveVector.z != 0){
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, 
                    Camera.main.transform.eulerAngles.y, 
                    transform.eulerAngles.z);
        }
    }
}
