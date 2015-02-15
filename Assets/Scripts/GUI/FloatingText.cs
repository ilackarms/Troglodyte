using UnityEngine;
using System.Collections;

public class FloatingText : MonoBehaviour {
	public Camera myCamera;
	public Vector3 vec;

	public string textG = "text string";

	public float offsetY = 391.97f;
	public float offsetX = 0.0f;
	
	public float scale;
	public float distance;
	
	// Use this for initialization
	void Start () {
		myCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		distance = (myCamera.transform.position - transform.position).magnitude/1.5f;		
	}
	
	void OnGUI(){
		vec = myCamera.WorldToScreenPoint (transform.position);
		
		//GUIText textField = 
		textG = GUI.TextField(new Rect(0,vec.x,
		                        0,Screen.height -(vec.y+offsetY/distance)), textG, 40);

	}
}
