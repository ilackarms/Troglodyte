using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioSensor : MonoBehaviour {
	public List<GameObject> sensedObjects;
	public float detectedTime;
	
	// Use this for initialization
	void Start () {
		sensedObjects = new List<GameObject> ();
		//Debug.Log ("START");
	}
	
	void Update(){
		detectedTime -= Time.deltaTime;
		//if (detectedTime < 0)
		//				sensedObjects = new List<GameObject>();
	}
	
	void OnTriggerEnter(Collider other)
	{
		//Debug.Log("Detected collision between " + gameObject.name + " and " + other.name);
		if(other.gameObject.GetComponent<DetectMe>()!=null){
			if(!sensedObjects.Contains(other.gameObject)){
				sensedObjects.Add(other.gameObject);
			}
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		//Debug.Log("Detected collision between " + gameObject.name + " and " + other.name);
		if(other.gameObject.GetComponent<DetectMe>()!=null){
			if(!sensedObjects.Contains(other.gameObject)){
				sensedObjects.Add(other.gameObject);
			}
			detectedTime = 10;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.GetComponent<DetectMe>()!=null){
			if(sensedObjects.Contains(other.gameObject)){
				sensedObjects.Remove(other.gameObject);
			}
		}
	}
	
	public bool sensesObject(GameObject obj){
		return (sensedObjects.Contains(obj));
	}
}
