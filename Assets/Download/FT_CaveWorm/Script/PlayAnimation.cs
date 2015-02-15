using UnityEngine;
using System.Collections;

public class PlayAnimation : MonoBehaviour {

	public GameObject creature;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnGUI() {
		if (GUI.Button(new Rect(Screen.width - 150, 20, 140, 25),"Idle")){
			creature.animation.wrapMode= WrapMode.Loop;
			creature.animation.CrossFade("Idle");
		}

		if (GUI.Button(new Rect(Screen.width - 150, 50, 140, 25),"Attack")){
			creature.animation.wrapMode= WrapMode.Default;
			creature.animation.CrossFade("Attack");
		}

		if (GUI.Button(new Rect(Screen.width - 150, 80, 140, 25),"Walk")){
			creature.animation.wrapMode= WrapMode.Loop;
			creature.animation.CrossFade("Walk");
		}

		if (GUI.Button(new Rect(Screen.width - 150, 110, 140, 25),"Dead")){
			creature.animation.wrapMode= WrapMode.Default;
			creature.animation.CrossFade("Dead");
		}


	
	}
}
