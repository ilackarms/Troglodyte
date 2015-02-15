using UnityEngine;
using System.Collections;

[RequireComponent (typeof(EnemyHighlighting))]
public class Claw : MonoBehaviour {

	public Weapon weapon;
	public GameObject parent;
	public bool attacking;
	// Use this for initialization
	void Start () {
		parent = GetComponentInParent<BasicAI>().gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if (attacking){
			//hit does nothing when monsters hit the player
			RaycastHit hit;
			if(Physics.Raycast(transform.position, transform.forward, out hit)){
				Debug.Log("Point of contact :" + hit.point);
			}
			CustomSendMessage<Hittable>.SendMessageUpwards(other.transform, "GetHit", new DamageBundle(weapon, parent, hit), SendMessageOptions.DontRequireReceiver);
			//other.transform.SendMessageUpwards ("GetHit", new DamageBundle(weapon, parent, hit), SendMessageOptions.DontRequireReceiver);

			attacking=false;
		}
	}


	public virtual void notifyAttacking(bool value, Weapon weapon){
		attacking = value;
		this.weapon = weapon;
//		if(attacking) Debug.Log ("Notified: Attacking");
	}
	

}
