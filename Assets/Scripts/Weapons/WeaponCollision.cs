using UnityEngine;
using System.Collections;

public class WeaponCollision : MonoBehaviour {

	public Weapon weapon;
	public GameObject parent;
	public bool attacking;

	public AnimationController armsAnimations;

	//public Animator anim;

	// Use this for initialization
	protected void Start () {
		//collider = GetComponent<Collider> ();
		parent = GetComponentInParent<CombatSystem>().gameObject;
        Debug.LogWarning("Parent of Weapon " + name + " is " + parent);
		armsAnimations = GetComponentInParent<AnimationController>();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		//foo
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "Sensor"){
			Physics.IgnoreCollision(other.GetComponent<Collider>(), transform.root.GetComponent<Collider>()); // return; //ignore collisions with sensor layer
			return;
		}
		if (attacking){
			RaycastHit hit;
			if(Physics.Raycast(transform.position, transform.forward, out hit))
			{
				Debug.Log("Point of contact :" + hit.point);
			}
			//CustomSendMessage<Hittable>.SendMessageUpwards(other.transform, "GetHit", new DamageBundle(weapon, parent, hit), SendMessageOptions.DontRequireReceiver);
			other.transform.SendMessageUpwards("GetHit", new DamageBundle(weapon, parent, hit), SendMessageOptions.DontRequireReceiver);
			other.transform.BroadcastMessage("GetHit", new DamageBundle(weapon, parent, hit), SendMessageOptions.DontRequireReceiver);
//			other.transform.SendMessageUpwards("bleed", new DamageBundle(weapon, parent, hit), SendMessageOptions.DontRequireReceiver);
//			other.transform.BroadcastMessage("bleed", new DamageBundle(weapon, parent, hit), SendMessageOptions.DontRequireReceiver);
//			Debug.LogError("HIt: "+ other.gameObject.name);
			//make them bleed!
			//CustomSendMessage<Hittable>.SendMessageUpwards(other.transform, "bleed", hit, SendMessageOptions.DontRequireReceiver);
			attacking=false;
		}
	}

	public virtual void notifyAttacking(bool value, Weapon weapon){
		attacking = value;
		this.weapon = weapon;
		//if(attacking) Debug.Log ("Notified: Attacking");
	}
}
