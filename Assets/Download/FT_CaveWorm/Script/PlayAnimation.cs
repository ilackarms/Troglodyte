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
			creature.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			creature.GetComponent<Animation>().CrossFade("Idle");
		}

		if (GUI.Button(new Rect(Screen.width - 150, 50, 140, 25),"Attack")){
			creature.GetComponent<Animation>().wrapMode= WrapMode.Default;
			creature.GetComponent<Animation>().CrossFade("Attack");
		}

		if (GUI.Button(new Rect(Screen.width - 150, 80, 140, 25),"Walk")){
			creature.GetComponent<Animation>().wrapMode= WrapMode.Loop;
			creature.GetComponent<Animation>().CrossFade("Walk");
		}

		if (GUI.Button(new Rect(Screen.width - 150, 110, 140, 25),"Dead")){
			creature.GetComponent<Animation>().wrapMode= WrapMode.Default;
			creature.GetComponent<Animation>().CrossFade("Dead");
		}


	
	}
}
