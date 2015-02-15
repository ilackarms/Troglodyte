using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FloatingHitPointBar : MonoBehaviour {
	public Camera myCamera;
	public Vector3 vec;
		
//	public Texture backgroundTexture;
	public Texture foregroundTexture;
//	public Texture frameTexture;
	
	public int fixedHealthWidth = 197;
	float healthWidth;
	public int healthHeight = 28;

	public int healthMarginLeft = -28;
	public int healthMarginTop  = 25;
	
	public int frameWidth = 266;
	public int frameHeight = 65;

	public float offsetY = 391.97f;
	public float offsetX = 0.0f;

	public float scale;
	public float distance;

	public bool draw = false;


	//combat info
	public float maxHealth;
	public float currentHealth;

	// Use this for initialization
	void Start () {
		myCamera = Camera.main;
		foregroundTexture = (Texture2D) Resources.Load("Sprites/HealthStatus/healthbar");
	}
	
	// Update is called once per frame
	void Update () {
		healthWidth = fixedHealthWidth * (currentHealth/maxHealth);
		
		distance = (myCamera.transform.position - transform.position).magnitude/1.5f;

	}

	void OnGUI(){
		if(draw){
			vec = myCamera.WorldToScreenPoint (transform.position);
			
			float faceAngle = Vector3.Angle (myCamera.transform.forward, myCamera.transform.position - transform.position);				
			if (faceAngle< -60 || faceAngle > 60) {
				/*GUI.DrawTexture (new Rect (vec.x - (frameWidth / 2.0f) / distance,
		                 Screen.height - (vec.y + offsetY / distance), 
		                 frameWidth / distance, 
		                 frameHeight / distance), 
					backgroundTexture, ScaleMode.ScaleToFit, true, 0);
				*/
				GUI.DrawTexture (new Rect (vec.x - (healthWidth / 2.0f + healthMarginLeft) / distance,
		                 Screen.height - (vec.y + offsetY / distance) + healthMarginTop / distance, 
		                 healthWidth / distance, 
		                 healthHeight / distance), 
					foregroundTexture, ScaleMode.ScaleAndCrop, true, 0);

				/*GUI.DrawTexture (new Rect (vec.x - (frameWidth / 2.0f) / distance,
		                 Screen.height - (vec.y + offsetY / distance), 
		                 frameWidth / distance, 
		                 frameHeight / distance), 
		        	frameTexture, ScaleMode.ScaleToFit, true, 0);*/
			}
		}
	}


}
