using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {

	//these values set in the .Shake() method
	private float ShakeSpeed;	//speed
	private float ShakeMagnitude; //maximum shake range from origin
	//private float ShakeTimer = 0f; //timer= delay?
	private float ShakeTime;	//time to shake

	private bool isShaking = false;
	private Vector3 originalPosition;
	private Quaternion originalRotation;

	private Color bloodColor;
	Texture2D bloodBorder;

	// Use this for initialization
	void Start () {
		//Shake (15.0f);
		bloodBorder = (Texture2D) Resources.Load("Sprites/BloodBorder");
		bloodColor = Color.white;
		bloodColor.a = 0;
	}

	Vector3 randomTarget;
	Quaternion randomRotation;
	float targetChangeTime;
	float currentChangeTime;
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.N)){
			Shake (15.0f);
			flashBloodBorder(0.5f);
		}
		if(isShaking){
			if(ShakeTime <= 0){
				isShaking = false;
				transform.localPosition = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z);
				transform.localRotation = new Quaternion(originalRotation.x, originalRotation.y, originalRotation.z, originalRotation.w);
			}
			else{
				ShakeTime -= Time.deltaTime;
				if(currentChangeTime <= 0){
					//randomTarget = Random.insideUnitSphere * ShakeMagnitude;
					randomTarget = new Vector3(Random.Range(ShakeMagnitude*-1,ShakeMagnitude) * Mathf.Sin(ShakeTime),
					                           Random.Range(ShakeMagnitude*-1,ShakeMagnitude) * Mathf.Sin(ShakeTime),
					                           Random.Range(ShakeMagnitude*-1,ShakeMagnitude) * Mathf.Sin(ShakeTime));
					randomRotation = new Quaternion(transform.localRotation.x + Random.Range(ShakeMagnitude*-1,ShakeMagnitude) * Random.value* 0.001f,
					                                transform.localRotation.y + Random.Range(ShakeMagnitude*-1,ShakeMagnitude) * Random.value* 0.001f,
					                                transform.localRotation.z + Random.Range(ShakeMagnitude*-1,ShakeMagnitude) * Random.value* 0.001f,
					                                transform.localRotation.w + Random.Range(ShakeMagnitude*-1,ShakeMagnitude) * Random.value* 0.001f);
					//randomTarget = SmoothRandom.GetVector3(ShakeMagnitude);
					//randomRotation = new Quaternion(
					currentChangeTime = targetChangeTime;
				}
				transform.localPosition = Vector3.Lerp(transform.localPosition, randomTarget, Time.deltaTime * ShakeSpeed);
				transform.localRotation = Quaternion.Lerp(transform.localRotation, randomRotation, Time.deltaTime * ShakeSpeed);
				transform.localRotation = randomRotation;

				currentChangeTime -= Time.deltaTime;
				//rotation?
				//1 white frame?
			}
		}

		//lerp the transparency of the blood border
		bloodColor.a = Mathf.Lerp(bloodColor.a, 0, Time.deltaTime);
	}

	
	public void Shake(float ShakeMagnitude){
		isShaking = true;
		Shake (10.0f, ShakeMagnitude, 0.2f);
		Debug.Log("Shake");
	}
	
	void Shake(float ShakeSpeed, float ShakeMagnitude, float ShakeTime){
		isShaking = true;
		this.ShakeSpeed = ShakeSpeed;
		this.ShakeMagnitude = ShakeMagnitude;
		this.ShakeTime = ShakeTime;
		//this.ShakeTimer = 0f;
		targetChangeTime = ShakeTime / ShakeSpeed;
	}



	//Blood splash!
	void OnGUI(){
		GUI.color = bloodColor;
		GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), bloodBorder);
		GUI.color = Color.white;
	}

	//take a float between 0 to 1, set the alpha!
	public void flashBloodBorder(float a){
		bloodColor.a = a;
	}
}
