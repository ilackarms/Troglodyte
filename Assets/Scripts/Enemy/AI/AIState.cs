using UnityEngine;
using System.Collections;


public abstract class AIState {
	public string animationClip;
	public float animationSpeed;
	public float stateTime;

	public bool executedOnce;
	public bool executedThisCycle;

	protected BasicAI ai;
	
	public AIState(BasicAI ai, string animationClip, float animationSpeed){
		this.ai = ai;
		this.animationClip = animationClip;
		this.animationSpeed = animationSpeed;
		executedOnce = false;
		executedThisCycle = false;
		stateTime = 0;
	}
	
	public void update(){
		if(animationClip!=null){
			ai.GetComponent<Animation>().CrossFade (animationClip);
			ai.GetComponent<Animation>() [animationClip].speed = animationSpeed;
		}
		stateTime += Time.deltaTime;
		//ai.pathfinder.speed = ai.baseMoveSpeed;
		execute ();
	}

	protected abstract void execute ();
	protected abstract void executeOnce();
	protected abstract void executeOncePerCycle();

	public virtual void initialize(){
		stateTime = 0;
		executedThisCycle = false;
	}

}