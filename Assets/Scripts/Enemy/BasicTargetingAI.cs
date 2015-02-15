using UnityEngine;
using System.Collections;

public class BasicTargetingAI : MonoBehaviour {
	public Transform target;
	public float moveSpeed=1;
	public float rotationSpeed=1;

	private Transform myTransform;

	//awake is the first function called in our object
	void Awake() { 
		myTransform = transform;
	}

	// Use this for initialization
	void Start () {
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		target = player.transform;
	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawLine (target.position, myTransform.position, Color.white);

		//rotate towards target
		myTransform.rotation = Quaternion.Slerp (myTransform.rotation, 
		                                         Quaternion.LookRotation (target.position - myTransform.position),
		                                         rotationSpeed * Time.deltaTime);
		//move towards target
		myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;
	}
}
