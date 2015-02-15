using UnityEngine;
using System.Collections;

//require character controller
//attach this script to player object
[RequireComponent(typeof(CharacterController))]
public class FallDamage : MonoBehaviour {
	//transform.SendMessage("GetHit",fallDistance/2, SendMessageOptions.DontRequireReceiver);
	float lastVelocity;
	public float fallingSpeedThreshHold = -5f; //should be negative

	CharacterController controller;
	void Start(){
		controller = GetComponent<CharacterController>();
		//lastVelocity = controller.velocity.y;
	}

	void Update(){
		if(lastVelocity <= fallingSpeedThreshHold && controller.isGrounded){
			transform.SendMessage("GetHit",lastVelocity/2*-1, SendMessageOptions.DontRequireReceiver);
			Debug.Log("HIT GROUND TRAVELING AT "+lastVelocity);
		}
		lastVelocity = controller.velocity.y;
	}
}
