using UnityEngine;
using System.Collections;

//		::::::::::. :::      :::.  .-:.     ::-..,:::::: :::::::..        .::::::.:::::::::::::::. ::::::::::::.,::::::  
//		 ` ;;;```.;;;;;;      ;;`;;  ';;.   ;;;;';;;;'''' ;;;;``;;;;      ;;;`    `;;;;;;;;'''';;`;;;;;;;;;;'''';;;;''''  
//	      `]]nnn]]' [[[     ,[[ '[[,  '[[,[[['   [[cccc   [[[,/[[['      '[==/[[[[,    [[    ,[[ '[[,   [[      [[cccc   
//	       $$$""    $$'    c$$$cc$$$c   c$$"     $$""""   $$$$$$c          '''    $    $$   c$$$cc$$$c  $$      $$""""   
//	       888o    o88oo,.__888   888,,8P"`      888oo,__ 888b "88bo,     88b    dP    88,   888   888, 88,     888oo,__ 
//	       YMMMb   """"YUMMMYMM   ""`mM"         """"YUMMMMMMM   "W"       "YMmMY"     MMM   YMM   ""`  MMM     """"YUMMM

abstract public class PlayerState {
	protected CombatSystem player;
	
	public string animationClip;
	public float animationSpeed;
	public float stateTime;
	
	public bool executedOnce;
	public bool executedThisCycle;
	
	public PlayerState(CombatSystem player){
		this.player = player;
		executedOnce = false;
		executedThisCycle = false;
	}
	
	virtual public void execute(){

		player.animate(animationClip, animationSpeed);
		if(!executedThisCycle){
			executePerCycle();
			executedThisCycle = true;
		}
		stateTime -= Time.deltaTime;
	}

	virtual public void executePerCycle(){
		//do nothing
	}
	
	virtual public void reset(){
		executedThisCycle  = false;
		setSpeed();
		stateTime = Global.arms.getAnimationTime(animationClip, animationSpeed);
	}
	
	virtual public void setSpeed(){
		animationSpeed = 1.0f;
	}


	public void detectInput(){
		if(Input.GetButtonDown("Fire1")){
//			Debug.LogWarning("Fire1 detected. Weapon is:"+CombatSystem.weaponCollider);
			if(CombatSystem.WeaponCollider is Bow){
//				Debug.LogError("BOW!!");
				player.setState("BowLoad");
				if(Input.GetButtonUp("Fire1") && player.currentState is BowLoadState){
					((BowLoadState) player.currentState).bowDrawn= false;
				}
				return;
			}
			else{
				switch(Random.Range(0,3)){
					case 0:
						player.setState("Slashing");
						break;
					case 1:
						player.setState("Slamming");
						break;
					case 2:
						player.setState("Stabbing");
						break;				
				}
				return;
			}
		}
		if(Input.GetButtonDown("Spell1")){	//todo: change this to be handled by spell system
			player.setState("Spell1");
			return;
		}
		if(Input.GetButtonDown("Spell2")){
			player.setState("Spell2");
			return;
		}
		if(Input.GetButtonDown("Spell3")){
			player.setState("Spell3");
			return;
		}
		if(Input.GetButtonDown("Spell4")){
			player.setState("Spell4");
			return;
		}
	}

	public class WalkingState : PlayerState {
		public WalkingState(CombatSystem player) : base(player){
			animationClip = "idle";
		}
		
		override public void execute(){
			base.execute();
			detectInput();
		}
	}

	public class IdleState : PlayerState {
		public IdleState(CombatSystem player) : base(player){
			animationClip = "idle";
		}
		
		override public void execute(){
			base.execute();
			if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0){
				player.setState("Walking");
				return;
			}
			detectInput();
		}
	}

	public class SlashingState : PlayerState {
		public SlashingState(CombatSystem player) : base(player){
			animationClip = "slash";
		}
		
		override public void execute(){
			base.execute();
			if(stateTime <= 0) {
				player.setState("Idle");
				//if animation ends, make sure weapon collider is no longer active
				CombatSystem.WeaponCollider.notifyAttacking(false, CombatSystem.Weapon);
			}
		}

		override public void executePerCycle(){
			//weapon collider activates once per swing
			if(CombatSystem.WeaponCollider==null) Debug.LogWarning("weapon Collider ="+CombatSystem.WeaponCollider);
			CombatSystem.WeaponCollider.notifyAttacking(true, CombatSystem.Weapon);
//			Debug.LogWarning("Weapon I'm attacking with is "+CombatSystem.Weapon.name);
		}

		override public void setSpeed(){
			animationSpeed = CombatSystem.Statistics.AttackSpeed;
		}

	}

	public class SlammingState : PlayerState {
		public SlammingState(CombatSystem player) : base(player){
			animationClip = "slam";
			animationSpeed = 1.0f;
		}
		
		override public void execute(){
			base.execute();
			
			if(stateTime <= 0) {
				player.setState("Idle");
				//if animation ends, make sure weapon collider is no longer active
				CombatSystem.WeaponCollider.notifyAttacking(false, CombatSystem.Weapon);
			}
		}
		
		override public void executePerCycle(){
			//weapon collider activates once per swing
			//if(CombatSystem.weaponCollider==null) Debug.LogWarning("weapon Collider ="+CombatSystem.weaponCollider);
			CombatSystem.WeaponCollider.notifyAttacking(true, CombatSystem.Weapon);
		}
		
		override public void setSpeed(){
			animationSpeed = CombatSystem.Statistics.AttackSpeed;
		}
	}

	public class StabbingState : PlayerState {
		public StabbingState(CombatSystem player) : base(player){
			animationClip = "stab";
		}
		
		override public void execute(){
			base.execute();
			
			if(stateTime <= 0) {
				player.setState("Idle");
				//if animation ends, make sure weapon collider is no longer active
				CombatSystem.WeaponCollider.notifyAttacking(false, CombatSystem.Weapon);
			}
		}
		
		override public void executePerCycle(){
			//weapon collider activates once per swing
			if(CombatSystem.WeaponCollider==null) Debug.LogWarning("weapon Collider ="+CombatSystem.WeaponCollider);
			CombatSystem.WeaponCollider.notifyAttacking(true, CombatSystem.Weapon);
		}
		
		override public void setSpeed(){
			animationSpeed = CombatSystem.Statistics.AttackSpeed;
		}
	}	

	public class BowLoadState : PlayerState {
		public bool bowDrawn = true;

		public BowLoadState(CombatSystem player) : base(player){
			animationClip = "shootLoad";
		}
		
		override public void execute(){
			base.execute();
			//if player releases click or somehow switches weapons
			if(Input.GetButtonUp("Fire1")) bowDrawn = false;
			if(stateTime <= 0 && !bowDrawn) {
				player.setState("BowRelease");
//				Debug.LogWarning("bow release is a go!");
			}
		}

		override public void reset(){
			base.reset();
			bowDrawn = true;
		}
		
		override public void setSpeed(){
			animationSpeed = CombatSystem.Statistics.AttackSpeed;
		}

		override public void executePerCycle(){
			base.executePerCycle();
			Global.LoadArrow.SetActive(true);
		}
	}  	

	public class BowReleaseState : PlayerState {
		public BowReleaseState(CombatSystem player) : base(player){
			animationClip = "shootRelease";
		}
		
		override public void execute(){
			base.execute();
			if(stateTime <= 0) {
				player.setState("Idle");
				//Debug.LogError("Weapon collider shouold be a bow collider. Is it?" + CombatSystem.WeaponCollider);
				//if animation ends, make sure weapon collider is no longer active
				CombatSystem.WeaponCollider.notifyAttacking(false, CombatSystem.Weapon);
			}
		}
		
		override public void executePerCycle(){
			//weapon collider activates once per swing
			Global.LoadArrow.SetActive(false);
			((Bow) CombatSystem.WeaponCollider).notifyAttacking(true, CombatSystem.Weapon);
		}
		
		override public void setSpeed(){
			animationSpeed = CombatSystem.Statistics.AttackSpeed;
		}
	}



	public class Spell1State : PlayerState {
		public Spell1State(CombatSystem player) : base(player){
			animationClip = "spell1";
		}
		
		override public void execute(){
			base.execute();
			if(stateTime <= 0) {
				player.setState("Idle");
			}
		}
	}



	public class Spell2State : PlayerState {
		public Spell2State(CombatSystem player) : base(player){
			animationClip = "spell2";
		}
		
		override public void execute(){
			base.execute();
			if(stateTime <= 0) {
				player.setState("Idle");
			}
		}
	}



	public class Spell3State : PlayerState {
		public Spell3State(CombatSystem player) : base(player){
			animationClip = "spell3";
		}
		
		override public void execute(){
			base.execute();
			if(stateTime <= 0) {
				player.setState("Idle");
			}
		}
	}



	public class Spell4State : PlayerState {
		public Spell4State(CombatSystem player) : base(player){
			animationClip = "spell4";
		}
		
		override public void execute(){
			base.execute();
			if(stateTime <= 0) {
				player.setState("Idle");
			}
		}
	}

	//using decorators?
	/*
	abstract public class StateDecorator : PlayerState {
		PlayerState innerState;
		public StateDecorator(PlayerState state) {
			this.innerState = state;
		}
		
		override public void execute(){
			innerState.execute();
		}		
	}*/
}
