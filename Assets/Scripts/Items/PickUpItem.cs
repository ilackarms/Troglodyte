using UnityEngine;
using System.Collections;

public class PickUpItem : Interactable {

	//set this when dropping intentionally
	public Item item = null;

	public float MAX_DISTANCE = 6.0f;
	
	void Start () {
		outlineColor = Color.blue;
	}

	void LateUpdate() {
		base.Outline ();
	}

	bool drawTooltip = false;

	
	override public void Outline(){
		if(hit.distance<=MAX_DISTANCE){
			base.Outline ();
			drawTooltip=true;
			//Debug.Log("blue outline");
			if(Input.GetButtonDown("Interact")){
				if(item != null){
					if(Global.inventory.PickUpItem(item)){
						Destroy(gameObject);
					}
					else{
						rigidbody.velocity = new Vector3(0,8.5f,0);
						rigidbody.angularVelocity = new Vector3(Random.Range(-6,6),Random.Range(-6,6),Random.Range(-6,6));
					}
				}
				else{
					Debug.LogError (name + " not assigned Item object!");
				}
			}

		}
	}

	override public void RemoveOutline(){
		base.RemoveOutline ();
		drawTooltip = false;
	}

	void OnGUI(){
		if(drawTooltip){
			Vector2 onScreenTooltipPosition = Camera.main.WorldToScreenPoint(transform.position);
			item.drawItemTooltip(new Vector2(onScreenTooltipPosition.x,Screen.height-onScreenTooltipPosition.y));
		}
	}
}
