using UnityEngine;
using System.Collections;


public class CameraManager : MonoBehaviour {
    public static CameraManager instance;
    private Transform myTransform;

    x360_Gamepad gamepad;

    //TP_CAMERA

    public Transform TargetLookAt;

    public bool zoom = true;
    public float Distance = 5f;
    public float DistanceMin = 3f;
    public float DistanceMax = 10f;
    public float DistanceSmooth = 0.05f;
    public float DistanceResumeSmooth = 1f;
    public float X_MouseSensitivity = 5f;
    public float Y_MouseSensitivity = 5f;
    public float MouseWheelSensitivity = 5f;
    public float X_Smooth = 0.05f;
    public float Y_Smooth = 0.1f;
    public float Y_MinLimit = -40f;
    public float Y_MaxLimit = 80f;
    public float OcclusionDistanceStep = 0.5f;
    public int MaxOcclusionChecks = 10;
    public float rotacaoInicialX = 90;

    private float mouseX = 0f;
    private float mouseY = 0f;
    private float velX = 0f;
    private float velY = 0f;
    private float velZ = 0f;
    private float velDistance = 0f;
    private float startDistance = 0f;
    public Vector3 position = new Vector3(0.02f, 5.49f, 1.5f);
    private Vector3 desiredPosition = new Vector3(0.02f, 5.49f, 1.5f);
    private float desiredDistance = 0f;
    private float distanceSmooth = 0;
    private float preOccludedDistance = 0;
    public float alturaInicial;
    public float mouseYInicial;


    // Use this for initialization
    IEnumerator Start() {
        instance = this;


        myTransform = this.transform;
        Vector3 posInicial = Vector3.zero;
        while (ObjetivosController.instance == null) {
            yield return null;
        }

        SaveTransform transformSave = ObjetivosController.instance.GetObjetivoAtual().posCarregamentoSave;
        //Debug.Log(transform.rotation.eulerAngles.y);


        posInicial.y = transformSave.position.y + alturaInicial;
        posInicial.z = transformSave.position.z;

        while (!ObjetivosController.instance.atualizouPosInicial) {
            //Debug.Log("Esperando");
            yield return null;
        }

        if (SceneLoader.instance.FaseNavegacao() == false)
        {
            if (PlayerManager.instance.transform.eulerAngles.y == 270 || PlayerManager.instance.transform.eulerAngles.y == -90)
            {

                posInicial.x = transformSave.position.x + 8.86f;
            }
            else
            {
                //Debug.Log("Entrou aqui!");
                posInicial.x = transformSave.position.x - 8.86f;
            }
        }
        else {
            //Debug.Log((int)PlayerBoatManager.instance.transform.eulerAngles.y);
            if ((int)PlayerBoatManager.instance.transform.eulerAngles.y == 270 || (int)PlayerBoatManager.instance.transform.eulerAngles.y == -90)
            {
                posInicial.x = transformSave.position.x - 60f;
            }
            else {
                posInicial.x = transformSave.position.x + 60f;
            }

        }
        //Debug.Log(PlayerManager.instance.transform.eulerAngles.y);



        myTransform.position = posInicial;
        myTransform.LookAt(TargetLookAt);
        position = myTransform.position;

        //mouseX = myTransform.eulerAngles.y;

        //Vector3 tempEuler = myTransform.eulerAngles;
        //tempEuler.y = PlayerController.instance.gameObject.transform.eulerAngles.y;
        //Debug.Log(tempEuler.y);
        //myTransform.eulerAngles = tempEuler;
        if (SceneLoader.instance.FaseNavegacao() == false)
        {
            rotacaoInicialX = PlayerManager.instance.transform.eulerAngles.y;
        }
        else {
            rotacaoInicialX = -PlayerBoatManager.instance.transform.eulerAngles.y;
        }
        //Debug.Log(rotacaoInicialX);
        position = myTransform.position;
        desiredPosition = position;
        if (!SceneLoader.instance.FaseNavegacao())
        {
            Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
        }else
        {
            Distance = 80;

        }
        startDistance = Distance;
        Reset();

        if (SceneLoader.instance.FaseNavegacao()){
            HandlePlayerInput();

            CalculateDesiredPosition();

            position = desiredPosition;

            UpdatePosition();


        }


        
    }

    void Update() {
        if (LevelManager.instance == null) return;

        if (GamepadManager.Instance.ConnectedTotal() > 0)
        {
            gamepad = GamepadManager.Instance.GetGamepad(1);
        }
        else
        {
            gamepad = null;
        }
    }

	// Update is called once per frame
	void LateUpdate () {

        switch (LevelManager.instance.GetEstadoAtual()) {
            case (EstadoAtual.INPUT):

                //Debug.Log("InputCamera");
                HandlePlayerInput();

                var count = 0;

                do{
                	CalculateDesiredPosition();
                	count++;
                	Debug.Log(CheckIfOccluded(count));
                }while(CheckIfOccluded(count));

                UpdatePosition();

                break;
        }
	}



    void HandlePlayerInput()
    {
        float deadZone = 0.01f;

        if (Input.GetMouseButton(1))
        {
            mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
            mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;
            
        }
        else if(gamepad != null){
            mouseX += gamepad.GetStick_RX() * X_MouseSensitivity;
            mouseY -= gamepad.GetStick_RY() * Y_MouseSensitivity;
        }

        // Limitar a rotação do mouseY
        mouseY = Helper.ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);


        if (zoom) {
            if (Input.GetAxis("Mouse ScrollWheel") < -deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone)
            {
                desiredDistance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity, DistanceMin, DistanceMax);
                preOccludedDistance = desiredDistance;
                distanceSmooth = DistanceSmooth;
            }

            if (gamepad != null) {
                if (gamepad.GetTrigger_L > deadZone)
                {
                    desiredDistance = Mathf.Clamp(Distance - gamepad.GetTrigger_L * -MouseWheelSensitivity, DistanceMin, DistanceMax);
                    preOccludedDistance = desiredDistance;
                	distanceSmooth = DistanceSmooth;
                }

                else if (gamepad.GetTrigger_R > deadZone)
                {
                    desiredDistance = Mathf.Clamp(Distance - gamepad.GetTrigger_R * MouseWheelSensitivity, DistanceMin, DistanceMax);
                    preOccludedDistance = desiredDistance;
                	distanceSmooth = DistanceSmooth;
                }

                if (gamepad.GetButton("R3")) {
                    Reset();
                }
            }
        }
        

        if (Input.GetMouseButton(2))
        {
            Reset();
        }

    }

    void CalculateDesiredPosition()
    {
		ResetDesiredDistance();
        Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, distanceSmooth);

        desiredPosition = CalculatePosition(mouseY, mouseX, Distance);

    }

    Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
    {
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);

        return TargetLookAt.position + rotation * direction;
    }

    bool CheckIfOccluded(int count)
    {
    	var isOccluded = false;
    	var nearestDistance = CheckCameraPoints(TargetLookAt.position, desiredPosition);

    	if(nearestDistance != -1){
    		if(count < MaxOcclusionChecks){
    			isOccluded = true;
    			Distance -= OcclusionDistanceStep;

    			if(Distance < 0.9f)
    				Distance = 0.9f;
    		}

    		else
    			Distance = nearestDistance - Camera.main.nearClipPlane;

    		desiredDistance = Distance;
    		distanceSmooth = DistanceResumeSmooth;
    	}


    	return isOccluded;
    }

    float CheckCameraPoints(Vector3 from, Vector3 to)
    {
    	var nearestDistance = -1f;

    	RaycastHit hitInfo;

    	Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to);

    	/*

    	Desenha linhas no editor para debug

		Debug.DrawLine(from, to + transform.forward * -Camera.main.nearClipPlane, Color.red);
    	Debug.DrawLine(from, clipPlanePoints.UpperLeft);
    	Debug.DrawLine(from, clipPlanePoints.UpperRight);
    	Debug.DrawLine(from, clipPlanePoints.LowerLeft);
    	Debug.DrawLine(from, clipPlanePoints.LowerRight);

    	Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight);
    	Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight);
    	Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft);
    	Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft);
    	
    	*/

    	if (Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo) 
    		&& hitInfo.collider.tag != "Player")
    			nearestDistance = hitInfo.distance;

    	if (Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo) 
    		&& hitInfo.collider.tag != "Player")
    			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
    				nearestDistance = hitInfo.distance;

    	if (Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo) 
    		&& hitInfo.collider.tag != "Player")
    			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
    				nearestDistance = hitInfo.distance;

    	if (Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo) 
    		&& hitInfo.collider.tag != "Player")
    			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
    				nearestDistance = hitInfo.distance;

    	if (Physics.Linecast(from, to + transform.forward * -Camera.main.nearClipPlane, out hitInfo) 
    		&& hitInfo.collider.tag != "Player")
    			if (hitInfo.distance < nearestDistance || nearestDistance == -1)
    				nearestDistance = hitInfo.distance;

    	return nearestDistance;
    }

    void ResetDesiredDistance()
    {
    	if(desiredDistance < preOccludedDistance){

    		var pos = CalculatePosition(mouseY, mouseX, preOccludedDistance);

			var nearestDistance = CheckCameraPoints(TargetLookAt.position, pos);

    		if(nearestDistance == -1 || nearestDistance > preOccludedDistance){ //não houve colisão
    			desiredDistance = preOccludedDistance;
    		}
    	}
    }

    void UpdatePosition()
    {
        var posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, X_Smooth);
        var posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, Y_Smooth);
        var posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, X_Smooth);
        position = new Vector3(posX, posY, posZ);

        myTransform.position = position;

        myTransform.LookAt(TargetLookAt);

    }

    public void Reset()
    {
        mouseX = rotacaoInicialX;
        mouseY = mouseYInicial;
        Distance = startDistance;
        desiredDistance = Distance;
        preOccludedDistance = Distance;
    }

    public void ResetPosCameraPlayer(float anguloInicial, Vector3 posicao) {
        Vector3 offset = Vector3.zero;
        //Debug.Log("Teste");
        //A camera sempre ficará no centro do personagem.

        if (!SceneLoader.instance.FaseNavegacao()) {
            if (anguloInicial == 270 || anguloInicial == -90)
            {
                offset.x = +8.86f;
            }
            else
            {
                offset.x = -8.86f;
            }
        }else
        {
            if (anguloInicial == 270 || anguloInicial == -90)
            {
                offset.x = +60;
            }
            else
            {
                offset.x = -60f;
            }
            }

        

        offset.z = 0;

        offset.y = alturaInicial;

        myTransform.position = offset + posicao;
        myTransform.LookAt(TargetLookAt);
        position = myTransform.position;
        desiredPosition = position;

        mouseX = anguloInicial;
        //mouseX = anguloInicial;
        mouseY = 10;
        Distance = startDistance;

        preOccludedDistance = Distance;
    }

    public void ResetPosCameraBarco() {
        HandlePlayerInput();

        CalculateDesiredPosition();

        position = desiredPosition;

        UpdatePosition();
    }

    public static void UseExistingOrCreateNewMainCamera()
    {
        GameObject tempCamera;

        if (Camera.main != null)
        {
            tempCamera = Camera.main.gameObject;
        }
    }


}
