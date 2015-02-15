using UnityEngine;
using System.Collections;

[RequireComponent (typeof(NavMeshAgent))]
public class NavAI : MonoBehaviour {
	public NavMeshAgent navComponent;
	public Transform target;

	// Use this for initialization
	void Start () {
		navComponent = transform.GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (target) {
			navComponent.SetDestination (target.position);
		}
	}
}
