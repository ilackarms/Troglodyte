using UnityEngine;
using System.Collections;

public class Arrow : WeaponCollision{
	float destroyTimer=1000;

	// Use this for initialization
	new void Start ()
	{
		attacking = true;
	}

	// Update is called once per frame
	protected override void Update ()
	{
		destroyTimer -= Time.deltaTime;
		if (destroyTimer <= 0)
						Destroy (gameObject);
	}
	
	//arrow prefab must have trigger collider and rigidbody
	void OnTriggerEnter(Collider other){
		if(weapon!=null){
			if (attacking){				
				RaycastHit hit;
				if(Physics.Raycast(transform.position, transform.forward, out hit)){
					Debug.Log("Point of contact :" + hit.point);
				}
				CustomSendMessage<Hittable>.SendMessageUpwards(other.transform, "GetHit", new DamageBundle(weapon, parent, hit), SendMessageOptions.DontRequireReceiver);
				CustomSendMessage<Hittable>.SendMessageUpwards(other.transform, "bleed", hit, SendMessageOptions.DontRequireReceiver);
				//other.transform.SendMessageUpwards ("GetHit", new DamageBundle(weapon, parent, hit), SendMessageOptions.DontRequireReceiver);
				//other.transform.SendMessageUpwards ("bleed", hit, SendMessageOptions.DontRequireReceiver);
				attacking=false;
				destroyTimer=0.05f;
			}
		}
	}
}

