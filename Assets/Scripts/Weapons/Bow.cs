using System;	
using UnityEngine;

public class Bow : WeaponCollision {
	//public static float ARROW_COOLDOWN=1; //2 second cooldown
	public float arrowCooldown=0;

	//bows never actually collide or anythig! no collider!
	public override void notifyAttacking(bool value, Weapon weapon){
		if(value){
			//Debug.Log ("Attacking with bow! (this is the bow collider speaking");
			attacking = value;
			this.weapon = weapon;
			Invoke ("fireArrow",0.001f);
			//fireArrow ();
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
			GameObject arrow = Global.itemFactory.InstantiateProjectile ("Arrow", arrowTransform, 40.0f);
			arrow.transform.RotateAround (arrow.transform.position, arrow.transform.up, 90);
			arrow.AddComponent<Arrow>();
			Arrow arrowCollision = arrow.GetComponent<Arrow> ();
			arrowCollision.weapon = weapon;
			arrowCollision.parent = parent;
		}
	}
}

