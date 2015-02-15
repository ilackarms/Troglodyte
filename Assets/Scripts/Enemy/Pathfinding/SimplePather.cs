 using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(CharacterController))]
public class SimplePather : MonoBehaviour {

	public Transform target;

	private AIPath aiPath;


	// Use this for initialization
	void Start () {
		target = GameObject.FindGameObjectWithTag("Player").transform;
		aiPath = GetComponent<AIPath>();
		aiPath.target = target.transform;

	}

}
