using UnityEngine;
using System.Collections;

[RequireComponent (typeof(GUIText))]
public class HoveringText : MonoBehaviour {
	public Transform target;
	public Camera targetCamera;
	public Vector3 offset = Vector3.up;
	public bool clampToScreen = false;
	public float clampBorderSize = .05f;
	public bool useMainCamera = true;
	public Camera cameraToUse;
	private Camera myCamera;
	private Transform myTransform;
	private Transform cameraTransform;
	
	public Texture frame;
	public Texture background;
	
	
	// Use this for initialization
	void Start () {
		myTransform = transform;
		if (useMainCamera) {
			myCamera = Camera.main;
		} 
		else {
			myCamera = cameraToUse;
		}
		cameraTransform = myCamera.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (clampToScreen) {
			Vector3 relativePosition = cameraTransform.InverseTransformPoint (target.position);
			relativePosition.z = Mathf.Max (relativePosition.z, 1);
			myTransform.position = myCamera.WorldToViewportPoint (cameraTransform.TransformPoint (relativePosition + offset));
			myTransform.position = new Vector3 (Mathf.Clamp (myTransform.position.x, clampBorderSize, 1.0f - clampBorderSize),
			                                    Mathf.Clamp (myTransform.position.y, clampBorderSize, 1.0f - clampBorderSize),
			                                    myTransform.position.z);
		}
		else {
			myTransform.position = myCamera.WorldToViewportPoint(target.position + offset);
		}
	}
}
