using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum FadeMode { IN, OUT, INOUT };

public class FadeManager : MonoBehaviour {

    public static FadeManager instance;


	[HideInInspector]
	public bool fading = false;
	[HideInInspector]
	public bool fadingIN = false;
	[HideInInspector]
	public bool fadingOUT = false;




	GameObject canvasFade;
	Image canvasImage;
	public Color fadeColor = Color.black;
	Color oriColor;
	public float speed = 3.0f;
	public bool startFade = false;
	public bool disableInput = true; //Disable Input while fading
	public bool worldSpace = false; //for VR and such

	void Start(){

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this){
            Destroy(this.gameObject);
        }

        
        canvasFade = new GameObject ("CanvasFade");
		canvasFade.AddComponent (typeof(Canvas));
		canvasFade.AddComponent (typeof(CanvasScaler));
		canvasFade.AddComponent (typeof(GraphicRaycaster));
		canvasFade.AddComponent (typeof(CanvasRenderer));
		canvasFade.AddComponent (typeof(Image));

		canvasFade.GetComponent<Canvas> ().renderMode = worldSpace ? RenderMode.WorldSpace : RenderMode.ScreenSpaceOverlay;
		canvasFade.GetComponent<Canvas> ().sortingOrder = 10;
		canvasFade.GetComponent<GraphicRaycaster> ().enabled = false;
		fadeColor.a = 0;
		canvasFade.GetComponent<Image> ().color = fadeColor;

		canvasImage = canvasFade.GetComponent<Image> ();
		oriColor = fadeColor;

        canvasFade.transform.parent = this.transform;

        if (worldSpace) {
			canvasFade.transform.position = Camera.main.transform.position;
			canvasFade.transform.rotation = Camera.main.transform.rotation;
			canvasFade.transform.Translate(0,0,5);
			canvasFade.transform.SetParent(Camera.main.transform);
		}

		if (startFade) {
			Fade(FadeMode.OUT);
		}

	}


	public void FadeSwitch (){
		StartCoroutine (StartFade (FadeMode.INOUT, this.speed, fadeColor));
	}
	public void Fade (FadeMode type){
		StartCoroutine (StartFade (type, this.speed, fadeColor));
	}
	public void Fade (FadeMode type, float speed){
		StartCoroutine (StartFade (type, speed, fadeColor));
	}
	public void Fade (FadeMode type, float speed, Color newColor){
		StartCoroutine (StartFade (type, speed, newColor));
	}

	public IEnumerator StartFade(FadeMode type, float speed, Color color)
	{

		yield return new WaitForEndOfFrame ();
		if (fading) {
			yield return false;
		}

		fadeColor = color;
		fadeColor.a = 0;
		fading = true;
		canvasFade.GetComponent<GraphicRaycaster> ().enabled = disableInput;
		
		switch (type)
		{
		case FadeMode.IN:
			fadingIN = true;
			fadeColor.a = 0;
			while (fadeColor.a<1)
			{
				fadeColor.a += Time.deltaTime * speed;
				Apply();
				yield return null;
			}
			fadeColor.a = 1;
			Apply();
			break;
			
		case FadeMode.OUT:
			fadingOUT = true;
			fadeColor.a = 1;
			Apply();
			while (fadeColor.a > 0)
			{
				fadeColor.a -= Time.deltaTime * speed;
				Apply();
				yield return null;
			}
			fadeColor.a = 0;
			Apply();
			break;
			
		case FadeMode.INOUT:
			fadingIN = true;
			fadeColor.a = 0;
			Apply();
			while (fadeColor.a<1)
			{
				fadeColor.a += Time.deltaTime * speed;
				Apply();
				yield return null;
			}
			fadingIN = false;
			fadingOUT = true;
			fadeColor.a = 1;
			Apply();
			while (fadeColor.a > 0)
			{
				fadeColor.a -= Time.deltaTime * speed;
				Apply();
				yield return null;
			}
			fadeColor.a = 0;
			Apply();
			break;
			
		default:
			break;
		}
		
		fading = false;
		fadingIN = false;
		fadingOUT = false;
		fadeColor = oriColor;
		canvasFade.GetComponent<GraphicRaycaster> ().enabled = false;;
	}

	void Apply(){
		canvasImage.color = fadeColor;
	}
}
