using UnityEngine;
using System.Collections;

public class ExampleScene : MonoBehaviour {

	Shake.ShakeType shakeType = Shake.ShakeType.standard;
	Shake cameraShake, objectShake;
	public GUITexture[] guiTextures, controlTextures;
	public GameObject[] guiValues;
	public GUIText valueText, decayText;
	public Texture2D buttonUp, buttonDown;

	public GameObject cameraObject, cubeObject;

	float maxShake = 0.050f;
	float shakeAmount = 5f;
	float shakeIncrement = 5f;
	float addDecayIntensity = 0.0001f;
	bool targetIsCamera = true;
	bool addDecay = false;

	void Start () {
		cameraShake = cameraObject.GetComponent(typeof(Shake)) as Shake;
		objectShake = cubeObject.GetComponent(typeof(Shake)) as Shake;
	}
	
	void Update () {
		if(Input.GetMouseButtonUp(0)) {
			if(guiTextures[0].HitTest(Input.mousePosition)) {
				StartShake();
				StartCoroutine( WaitForTexture(guiTextures[0], 0.20f) );
			}
			if(guiTextures[1].HitTest(Input.mousePosition)) {
				cameraShake.StopShake();
				objectShake.StopShake();
				StartCoroutine( WaitForTexture(guiTextures[1], 0.20f) );
			}

			if(guiTextures[2].HitTest(Input.mousePosition)) {
				shakeType = Shake.ShakeType.standard;
				ResetTextures(2);
			}
			if(guiTextures[3].HitTest(Input.mousePosition)) {
				shakeType = Shake.ShakeType.rumble;
				ResetTextures(3);
			}
			if(guiTextures[4].HitTest(Input.mousePosition)) {
				shakeType = Shake.ShakeType.explosion;
				ResetTextures(4);
			}
			if(guiTextures[5].HitTest(Input.mousePosition)) {
				shakeType = Shake.ShakeType.earthquake;
				ResetTextures(5);
			}
			if(guiTextures[6].HitTest(Input.mousePosition)) {
				shakeType = Shake.ShakeType.custom;
				ResetTextures(6);
				ToggleValueObjects(true);
			}
			if(guiTextures[7].HitTest(Input.mousePosition)) {
				shakeType = Shake.ShakeType.random;
				ResetTextures(7);
			}
			if(guiTextures[8].HitTest(Input.mousePosition)) {
				StartCoroutine( WaitForTexture(guiTextures[8], 0.05f) );
				shakeAmount += shakeIncrement;
				if(shakeAmount > 100) shakeAmount = shakeIncrement;
				valueText.text = shakeAmount+"%";
			}
			if(guiTextures[9].HitTest(Input.mousePosition)) {
				StartCoroutine( WaitForTexture(guiTextures[9], 0.15f) );
				addDecay = !addDecay;
				decayText.text = addDecay == true ? "DELAY" : "IMMEDIATE";
			}
			
			if(controlTextures[0].HitTest(Input.mousePosition)) {
				targetIsCamera = true;
				ToggleShakeTarget(0);
			}
			if(controlTextures[1].HitTest(Input.mousePosition)) {
				targetIsCamera = false;
				ToggleShakeTarget(1);
			}
		}
	}

	void StartShake() {
		Shake objectToShake = targetIsCamera == true ? cameraShake : objectShake;
		if(shakeType == Shake.ShakeType.custom) {
			if(addDecay) objectToShake.StartShake( addDecayIntensity, 0, maxShake*(shakeAmount/100), true );
			else objectToShake.StartShake( maxShake*(shakeAmount/100), 0, 0, false );
		} else {
			objectToShake.StartShake(shakeType);
		}
	}

	IEnumerator WaitForTexture( GUITexture guiTexture, float timeDelay ) {
		guiTexture.texture = buttonDown;
		yield return new WaitForSeconds(timeDelay);
		guiTexture.texture = buttonUp;
	}

	void ToggleShakeTarget( int shakeTarget ) {
		controlTextures[ shakeTarget ].texture = buttonDown;
		for(int i = 0; i < controlTextures.Length; i++) {
			if( i != shakeTarget ) controlTextures[i].texture = buttonUp;
		}
	}

	void ToggleValueObjects( bool state ) {
		for(int i = 0; i < guiValues.Length; i++) {
			guiValues[i].SetActive(state);
		}
	}

	void ResetTextures( int skipTexture ) {
		guiTextures[ skipTexture ].texture = buttonDown;
		for(int i = 2; i < guiTextures.Length; i++) {
			if( i != skipTexture ) guiTextures[i].texture = buttonUp;
		}
		// Turn off Custom controls.
		if(skipTexture != 8 && skipTexture != 9) ToggleValueObjects(false);
	}
}
