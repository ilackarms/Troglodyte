using UnityEngine;
using System.Collections;

public class ItemInteractionSystem : MonoBehaviour {

	RaycastHit hit;

	// Use this for initialization
	void Start () {
		hit = new RaycastHit ();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(Physics.SphereCast(transform.position, 0.25f, transform.forward, out hit)){
			if(hit.transform.gameObject.GetComponentInParent<Interactable>()!=null){
				Interactable interactable = hit.transform.gameObject.GetComponentInParent<Interactable>();
				interactable.Outline();
				interactable.SetRaycastHit(hit);
//				Debug.Log("Yes");
			}
		}
		/*else {
			if(hit.transform!=null && hit.transform.gameObject.GetComponent<Interactable>()!=null){
				Interactable interactable = hit.transform.gameObject.GetComponent<Interactable>();
				interactable.RemoveOutline();
				Debug.Log("No");
			}
		}*/
	}

}
