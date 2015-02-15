using UnityEngine;
using System.Collections;

[RequireComponent(typeof(FloatingHitPointBar))]
public class EnemyHighlighting : Interactable {

	public FloatingHitPointBar healthPointBarScript;
	public float MAX_DISTANCE = 12.0f;

	// Use this for initialization
	void Start () {
		healthPointBarScript = gameObject.GetComponent<FloatingHitPointBar> ();
		//outlineColor = new Color(219/255f, 173/255f, 173/255f);
		outlineColor = Color.red;
		outlineThickness = 0.001f;
	}


	override public void Outline(){
		if(hit.distance<=MAX_DISTANCE){
			//base.Outline ();
			if(healthPointBarScript==null){
				healthPointBarScript = gameObject.AddComponent<FloatingHitPointBar>();
			}
			healthPointBarScript.draw = true;
		}
	}

	override public void RemoveOutline(){
		//base.RemoveOutline ();
		healthPointBarScript.draw = false;
	}
}
