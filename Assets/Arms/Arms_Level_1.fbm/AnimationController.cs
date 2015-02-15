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
		animation.CrossFade(animationName);
		animation[animationName].speed = animationSpeed;
	}

	public void notifyAnimation(string animationName, float speed){
		this.animationName = animationName;
	}

	public float getAnimationTime(string animaationName, float speed){
		float originalSpeed = animation[animationName].speed;
		animation[animationName].speed = speed;
		float animTime = animation[animationName].clip.length;
//		Debug.LogWarning("Animation Time: "+animTime);
		//animation[animationName].speed = originalSpeed;
		if(animationName == "shootRelease") animTime *= 3;
		return animTime/3;
	}
}
