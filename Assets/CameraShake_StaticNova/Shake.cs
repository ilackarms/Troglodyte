using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera Shake System/Shake")]
public class Shake : MonoBehaviour {

	// Logic flags.
	bool shaking = false;
	bool startDecay = false;
	bool addDecay = false;
 
	// Shake values, with default settings.
	float decay = 0.00025f;
	float intensity = 0.0920f;
	float intensityLimit = 0.095f;

	// Initial values so we can reset after shaking.
	Vector3 initialPosition;
//	Quaternion initialRotation;
	float initialIntensity, initialIntensityLimit;

	// Stores our values when calling a custom shake. 
	float customDecay = 0.00025f;
	float customIntensity = 0.0320f;
	float customIntensityLimit = 0.015f;
	bool customAddDecay = false;

//	int cycle=0;
	float shakeDuration;
	void Update(){
		if(Input.GetKeyDown(KeyCode.N)){
		/*	switch(cycle++){
			case 0:
				StartShake(ShakeType.random);
				break;
			case 1:
				StartShake(ShakeType.earthquake);
				break;
			case 2:
				StartShake(ShakeType.explosion);
				break;
			case 3:
				StartShake(ShakeType.rumble);
				break;
			case 4:
				StartShake(ShakeType.standard);
				break;
			case 5:
				StartShake(ShakeType.custom);
				break;
			}*/
			StartShake(ShakeType.explosion);
			shakeDuration=0.5f;
		}
		shakeDuration-=Time.deltaTime;
		if(shakeDuration<0){
			StopShake();
		}
	}


	// Different shake types, call Shake() with one of these.
	public enum ShakeType {
	    standard,
	    rumble,
	    explosion,
	    earthquake,
	    random,
	    custom
	}

	// Simple singleton pattern.
	static Shake _instance;
	public static Shake Instance { get{ return _instance; } }
	public static Shake GetInstance() {
	   if(!_instance) Debug.Log("Shake() - Please assign the script to the object in the scene before trying to access it.");
	   return _instance;   
	}

	void Awake() {
	    _instance = this;
		initialPosition = transform.localPosition;
	    //initialRotation = transform.localRotation;
	}

	public bool GetShakeState() {
	    return shaking;
	}

	public void StartShake( float shakeIntensity, float shakeDecay, float shakeLimit, bool shakeAddDecay ) {
		customIntensity = shakeIntensity > 0 ? shakeIntensity : intensity;
	    customIntensityLimit = shakeLimit > 0 ? shakeLimit : intensityLimit;
	    customDecay = shakeDecay > 0 ? shakeDecay : decay;
	    customAddDecay = shakeAddDecay;
	    StartShake( ShakeType.custom );
	} 
	public void StartShake() { StartShake( ShakeType.standard ); }
	public void StartShake( ShakeType type ) {
	    if(!shaking) {
	        switch(type) {
	            case ShakeType.rumble:       Rumble();          break;
	            case ShakeType.explosion:    Explosion();       break;
	            case ShakeType.earthquake:   Earthquake();      break;
	            case ShakeType.random: 		 RandomShake(); 	break;
	            case ShakeType.custom: 		 CustomShake(); 	break;
	            default:  					 DefaultShake();    break;                   
	        }
	    }
	}

	public void StopShake() {
		shaking = false;
	    startDecay = false;
		intensity = initialIntensity;
	    intensityLimit = initialIntensityLimit;
		transform.localPosition = initialPosition;
	  	//transform.localRotation = initialRotation;
	}

	IEnumerator BeginShake() {
	    while(shaking){
	        // Shake Algorythm.
			transform.localPosition = initialPosition + Random.insideUnitSphere * intensity;
			transform.localRotation = new Quaternion( transform.localRotation.x + Random.Range( -intensity, intensity ) * Random.value,
			                                         transform.localRotation.y + Random.Range( -intensity, intensity ) * Random.value,
			                                         transform.localRotation.z + Random.Range( -intensity, intensity ) * Random.value,
			                                         transform.localRotation.w + Random.Range( -intensity, intensity ) * Random.value );

	        // Specific behaviour for presets.
	        if(addDecay) {
	        	if(!startDecay) {
                    intensity += decay;
                    if(intensity >= intensityLimit) startDecay = true;
                } else {
                    intensity -= decay;
                }
	        } else {
	        	intensity -= decay;    
	        }
	        
	        // Check to see if we need to stop the shake.
	        if(intensity <= 0f) StopShake();   
	        yield return null;
	    }
	}

	  /////////////
	 // Presets //
	/////////////

	void Rumble() {
	    if(!shaking) {
	        initialIntensity = intensity;
	        initialIntensityLimit = intensityLimit;
	        shaking = true;
	        addDecay = true;
	        intensityLimit = 0.015f;
	        intensity = 0.000125f;
	        decay = 0.00000825f;
	        StartCoroutine("BeginShake");
	    }
	}

	void Explosion() {
	    if(!shaking) {
	        initialIntensity = intensity;
	        initialIntensityLimit = intensityLimit;
	        shaking = true;
	        addDecay = true;
	        intensityLimit = 0.020f;
	        intensity = 0.0050f;
	        decay = 0.00015f;
	        StartCoroutine("BeginShake");
	    }
	}

	void Earthquake() {
	    if(!shaking) {
	        initialIntensity = intensity;
	        initialIntensityLimit = intensityLimit;
	        shaking = true;
	        addDecay = true;
	        intensityLimit = 0.015f;
	        intensity = 0.000125f;
	        decay = 0.00000825f;
	        StartCoroutine("BeginShake");
	    }
	}

	void RandomShake() {
	    if(!shaking) {
	        initialIntensity = intensity;
	        initialIntensityLimit = intensityLimit;
	        shaking = true;
	        if( Random.Range(0.0f, 100.0f) > 50.0f ) addDecay = true;
	        else addDecay = false;
	        intensityLimit = Random.Range(0.010f, 0.035f);
	        intensity = Random.Range(0.00025f, 0.0055f);
	        decay = Random.Range(0.00000825f, 0.00015f);
	        StartCoroutine("BeginShake");
	    }
	}

	void CustomShake() {
		if(!shaking) {
	        initialIntensity = intensity;
	        initialIntensityLimit = intensityLimit;
	        shaking = true;
	        addDecay = customAddDecay;
	        intensity = customIntensity;
	        intensityLimit = customIntensityLimit;
	        decay = customDecay;
	        StartCoroutine("BeginShake");
	    }
	}

	void DefaultShake() {
		if(!shaking) {
			initialIntensity = intensity;
	   	 	initialIntensityLimit = intensityLimit;
	   	 	shaking = true;
	   	 	addDecay = false;
	   	 	intensityLimit = 0.015f;
			intensity = 0.0320f;
			decay = 0.00025f;
	   	 	StartCoroutine("BeginShake"); 
	   	}
	}
}
