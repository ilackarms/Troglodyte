using System;	
using System.Collections;
using UnityEngine;

public class Bow : WeaponCollision {
	//public static float ARROW_COOLDOWN=1; //2 second cooldown
	public float arrowCooldown=0;
	public Boolean fireDelay = false;

	//bows never actually collide or anythig! no collider!
	public override void notifyAttacking(bool value, Weapon weapon){
		if(value && arrowCooldown<=0){
			//Debug.Log ("Attacking with bow! (this is the bow collider speaking");
			attacking = value;
			this.weapon = weapon;
			fireArrow();
			//fireArrow ();
			arrowCooldown = 0.25f;
		}
	}

	protected override void Update(){
		base.Update ();
		if (arrowCooldown > 0) {
			arrowCooldown -= Time.deltaTime;
		}
	}

	//BOW WILL NOT HAVE A RIGIDBODY
	//NO NEED FOR TRIGGER ENTER
//	void OnTriggerEnter(Collider other){
//		if (attacking){
//			//other.transform.SendMessage ("GetHit", new DamageBundle(weapon, parent), SendMessageOptions.DontRequireReceiver);
//		}
//	}
	public void fireOnMouseRelease(){
			
	}

	public void fireArrow(){
		if(weapon!=null){
			GameObject camera = GameObject.FindGameObjectWithTag ("MainCamera");
			Transform arrowTransform = camera.transform;
			GameObject arrow = Global.itemFactory.InstantiateArrow (arrowTransform, 40.0f);
			//arrow.transform.RotateAround (arrow.transform.position, arrow.transform.up, 90);
			Arrow arrowCollision = arrow.AddComponent<Arrow>();
			arrowCollision.weapon = weapon;
			arrowCollision.parent = parent;
			StartCoroutine(destroyArrow(3.0f, arrow));
		}
	}

	IEnumerator destroyArrow(float waitTime, GameObject arrow){
		yield return new WaitForSeconds(waitTime);
		GameObject.Destroy(arrow);
	}
}

