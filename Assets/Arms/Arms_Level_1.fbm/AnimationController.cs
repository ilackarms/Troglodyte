using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {

	public const float DEFAULT_SPEED = 1.0f;

	private string animationName="idle";
	private AnimationClip clip;

	private float animationSpeed=1;
	private float animTime;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if(!GetComponent<Animation>()[animationName].enabled){
			GetComponent<Animation>().CrossFade(animationName);
		}
		GetComponent<Animation>()[animationName].speed = animationSpeed;
	}

	public void notifyAnimation(string animationName, float speed){
		this.animationName = animationName;
		animationSpeed=speed;
	}

	public float getAnimationTime(string animaationName, float speed){
		float originalSpeed = GetComponent<Animation>()[animationName].speed;
		GetComponent<Animation>()[animationName].speed = speed;
		float animTime = GetComponent<Animation>()[animationName].clip.length;
//		Debug.LogWarning("Animation Time: "+animTime);
		//animation[animationName].speed = originalSpeed;
		if(animationName == "shootRelease") animTime *= 3;
		return animTime/3;
	}
}
