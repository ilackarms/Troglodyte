using UnityEngine;
using System.Collections;

public class WaypointPath : MonoBehaviour {
	public GameObject [] Waypoints;
	public int currentlySelectedWaypoint = 0;

	public Vector3 getNextWaypoint(){
		return (Waypoints [++currentlySelectedWaypoint % (Waypoints.Length)].transform.position);
	}

	public Vector3 getRandomWaypoint(){
		int rand = Random.Range (0, Waypoints.Length - 1);
		currentlySelectedWaypoint = rand;
		return (Waypoints [currentlySelectedWaypoint % (Waypoints.Length)].transform.position);
	}

	public Vector3 getDifferentRandomWaypoint(){
		int rand = Random.Range (0, Waypoints.Length - 1);
		while (rand==currentlySelectedWaypoint) {
			rand = Random.Range (0, Waypoints.Length - 1);
		}		 
		currentlySelectedWaypoint = rand;
		return (Waypoints [currentlySelectedWaypoint % (Waypoints.Length)].transform.position);
	}
}
