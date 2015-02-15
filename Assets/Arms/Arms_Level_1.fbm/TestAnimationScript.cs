using UnityEngine;
using System.Collections;

public class TestAnimationScript : MonoBehaviour {

	private Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		Debug.Log ("Start log:");
		anim.SetBool("spell1",false);
		Time.timeScale = 1;
	}
	
	// Update is called once per frame
	void Update () {
		anim.SetBool("spell1",false);
		if(Input.GetButtonDown("Fire1")){
			Debug.Log("True");
			anim.SetBool("spell1",true);
			AnimationInfo[] currentAnimationInfo = anim.GetCurrentAnimationClipState (0);
			AnimationClip currentClip = currentAnimationInfo [0].clip;
			float animTime = currentClip.length/3;
			Debug.Log(currentClip.name);
			Debug.Log("Step "+animTime);
		}
	}
}
