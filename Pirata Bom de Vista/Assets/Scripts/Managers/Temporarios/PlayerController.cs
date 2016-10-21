using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public static PlayerController instance;

    private bool permiteInput = false;
    private bool combate = false;
    private Transform myTransform;


    //TP_MOTOR
    public bool jump = false;
    public float MoveSpeed = 10f;
    public float JumpSpeed = 6f;
    public float Gravity = 21f;
    public float TerminalVelocity = 20f;
    public float SlideThreshold = 0.6f;
    public float MaxControllableSlideMagnitude = 0.4f;

    private Vector3 slideDirection;

    public Vector3 MoveVector { get; set; }
    public float VerticalVelocity { get; set; }

    // Use this for initialization
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        myTransform = this.transform;


        SnapAlignCharacterWithCamera();
        ProcessMotion();

    }

    
    // Update is called once per frame
    void Update()
    {
       // myTransform.eulerAngles = Vector3.one;
    }
    

    public void UpdateMotor()
    {
        SnapAlignCharacterWithCamera();
        ProcessMotion();
    }



    void ProcessMotion()
    {

        //Transforma MoveVector em espaço do mundo
        MoveVector = myTransform.TransformDirection(MoveVector);

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

        //Debug.DrawRay(this.transform.position, MoveVector, Color.red, 1);
        //Debug.Log(MoveVector);

        /*
        //Rotaciona o personagem para aquela direção.
        float altura = Mathf.Abs(Camera.main.transform.position.y - myTransform.position.y);
        Vector3 temp = myTransform.position;
        temp.y += altura;


        Vector3 direcaoRot = Camera.main.transform.position - temp;
        Vector3.Normalize(direcaoRot);


        Debug.DrawRay(this.transform.position, direcaoRot, Color.red, 3);
        */

        //Mover o personagem pelo mundo
        //PlayerManager.CharacterController.detectCollisions = true;
        PlayerManager.CharacterController.Move(MoveVector * Time.deltaTime);
        
    }

    void ApplyGravity()
    {

        if (MoveVector.y > -TerminalVelocity)
            MoveVector = new Vector3(MoveVector.x, MoveVector.y - Gravity * Time.deltaTime, MoveVector.z);

        if (PlayerManager.CharacterController.isGrounded && MoveVector.y < -1)
            MoveVector = new Vector3(MoveVector.x, -1, MoveVector.z);

    }

    void ApllySlide()
    {

        if (!PlayerManager.CharacterController.isGrounded)
            return;

        slideDirection = Vector3.zero;

        RaycastHit hitInfo;

        if (Physics.Raycast(myTransform.position + Vector3.up, Vector3.down, out hitInfo))
        {
            if (hitInfo.normal.y < SlideThreshold)
                slideDirection = new Vector3(hitInfo.normal.x, -hitInfo.normal.y, hitInfo.normal.z);
        }

        if (slideDirection.magnitude < MaxControllableSlideMagnitude)
            MoveVector += slideDirection;

        else
        {
            MoveVector = slideDirection;
        }
    }

    public void Jump()
    {
        if (jump) {
            if (PlayerManager.CharacterController.isGrounded)
                VerticalVelocity = JumpSpeed;
        }
        
    }

    void SnapAlignCharacterWithCamera()
    {

        if (MoveVector.x != 0 || MoveVector.z != 0)
        {
            myTransform.rotation = Quaternion.Euler(0,
                    Camera.main.transform.eulerAngles.y,
                    0);
        }
    }


    public void InputCombate()
    {

    }

    public void SetPermiteInput(bool temp)
    {
        permiteInput = temp;
    }

    public void SetInputCombate(bool temp)
    {
        combate = temp;
    }
}
