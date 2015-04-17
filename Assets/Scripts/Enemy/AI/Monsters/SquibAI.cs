using UnityEngine;
using System.Collections;

public class SquibAI : BasicAI {

	AIState wanderState;
	AIState attackingState;
	AIState chasing;
	AIState dead;
	
	public int CurrentDest = 1;

	float becomeAgressiveChance = 0.001f;

	const float WANDER_TIME = 30.0f;
	const float SEARCH_TIME = 15.0f;
	float ATTACK_TIME;
		
	// Use this for initialization
	new void Start () {
		base.Start ();
		pathfinder.endReachedDistance = baseAttackRange / 2;
		ATTACK_TIME = GetComponent<Animation>()["Attack"].clip.length / statistics.AttackSpeed;
		
		target = GameObject.FindGameObjectWithTag("Player");

		wanderState = new WanderingState (this);
		attackingState = new AttackingState (this);
		chasing = new ChasingState (this);
		dead = new DeadState(this);
		
		setState(wanderState);
		//Debug.Log("State initialized to"+state);

		destination = new GameObject(); //make empty gameobject, move position to random point and set Waypoint as the target
	}
	
	/// <summary>
	/// Sets the base stats.
	/// </summary>
	public override void setBaseStats(){
		baseLevel = 1;
		baseHP = 50;
		baseArmor = 5 ;
		baseFireResist = 0;
		baseLightningResist = 0;
		baseIceResist = 0;
		baseChaosResist = 0;
		basePhysicalDamage = 5;
		baseFireDamage = 0;
		baseLightningDamage =0;
		baseIceDamage = 0;
		baseChaosDamage = 0;
		baseHealthRegen = 0;
		baseMoveSpeed = 3f;
		baseAttackSpeed = 1;
		baseLifeLeech = 0;
		baseSpellDamage = 0; //int
		baseAttackRange = 2.5f;
	}
	
	/// <summary>
	/// Randomly calculate the loot.
	/// </summary>
	public override void setLoot ()
	{		
		loot.Add ("Scroll of FlameArrow");
		loot.Add ("Health Potion");
	}
	
	/// <summary>
	/// Called by ALLIES when they detect a hostile. Turns all allies hostile.
	/// </summary>
	public override void notifyHostileDetected(){
		if(!state.Equals(chasing)){
			setState(chasing);
			Debug.Log("I was notified by a friend.");
		}
	}
	
	/// <summary>
	/// Called when getHit; for caveworm, just sets hostile
	/// </summary>
	public override void alertDamage(){
		if(!state.Equals(chasing)){
			setState(chasing);
		}
	}
	
	/// <summary>
	/// anything that happens on death unique to the subclass. Death animation defined here.
	/// </summary>
	public override void onDeath(){
		setState(dead);
		
	}
	
	// Update is called once per frame
	new void Update () {
		
		//		Debug.Log(idleState);
		//		Debug.Log(state);
		//default state actions
		base.Update();
		//	checkStateConditions ();
	}
	
	//called in update
	public override void checkStateConditions(){
		//state switch conditions
		//very low chance of squib to attack player
		//longer he lingers, higher chance of monster attacking
		if (state.Equals (wanderState)) {
			if (detectPlayer () && Random.Range(0.0f,1.1f)<= becomeAgressiveChance) {
				setState (chasing);
			} else if (closeEnough (((WanderingState)state).wanderDestination, 2.1f)) {
				setState (wanderState);
			}
		} 
		else if (state.Equals (chasing)) {
			if (closeEnough (target.transform.position, baseAttackRange)) {
				setState (attackingState);
			} 
			else if (detectPlayer ()) {
				setState (chasing);//refresh cooldown on search timer
			} else if (state.stateTime >= SEARCH_TIME) {
				setState (wanderState);
			}
		} 
		else if (state.Equals (attackingState)) {
			if(state.stateTime >= ATTACK_TIME){
				setState (chasing);
			}
		}
	}
		
	//idle state
	public class IdleState : AIState {
		public IdleState(BasicAI ai) : base(ai, "Floating", 1){
			
		}
		protected override void execute ()
		{
			ai.pathfinder.target = ai.transform;
			//ai.navComponent.SetDestination (ai.target.transform.position);
		}
		protected override void executeOnce ()
		{
			//throw new System.NotImplementedException ();
		}
		protected override void executeOncePerCycle ()
		{
			//throw new System.NotImplementedException ();
		}
	}
	
	//wandering state
	public class WanderingState : AIState {
		public Vector3 wanderDestination;
		public const float WANDER_RANGE = 5.0f;
		public Vector3 randomPoint;
		
		public WanderingState(BasicAI ai) : base(ai, "Floating", 0.5f){
			
		}
		protected override void execute ()
		{
			//wandering script
			//wander to a random point on the grid
			executeOncePerCycle();
			ai.setDestination(randomPoint);
		}
		
		public override void initialize() {
			base.initialize ();
		}
		
		protected override void executeOnce ()
		{

		}
		protected override void executeOncePerCycle ()
		{
			if(!executedThisCycle){
				executedThisCycle = true;
				randomPoint = ai.pathfinder.getRandomReachablePoint();
			}
		}
	}
	
	//chasing state
	public class ChasingState : AIState {
		public ChasingState(BasicAI ai) : base(ai, "Floating", 1.0f){
			
		}
		protected override void execute ()
		{
			//wandering script
			ai.pathfinder.target = ai.target.transform;
			executeOncePerCycle ();
		}
		protected override void executeOnce ()
		{
			//throw new System.NotImplementedException ();
		}
		protected override void executeOncePerCycle ()
		{
			if(!executedThisCycle){
				executedThisCycle = true;
				ai.notifyAllies();
//				Debug.Log("Trying to notify allies");
			}
		}
	}
	
	//attacking state
	public class AttackingState : AIState {
		
		Weapon w = new Weapon(0,0,null,"Claws",5,ai.statistics.AttackSpeed);
		
		public AttackingState(BasicAI ai) : base(ai, "Attack", ai.statistics.AttackSpeed){
			Weapon.WeaponPropertyBundle clawProperties = new Weapon.WeaponPropertyBundle(Item.ItemPropertyBundle.Rarity.Common, ai.statistics.Level);
			clawProperties.bonusAttackSpeed = ai.statistics.AttackSpeed;
			clawProperties.fireDamage = ai.statistics.FireDamage;
			clawProperties.iceDamage = ai.statistics.IceDamage;
			clawProperties.lightningDamage = ai.statistics.LightningDamage;
			clawProperties.lifeLeech = ai.baseLifeLeech;
			clawProperties.chaosDamage = ai.statistics.ChaosDamage;
			w.propertyBundle = clawProperties;
		}
		protected override void execute ()
		{			

			executeOncePerCycle();
		}
		protected override void executeOnce ()
		{
			//throw new System.NotImplementedException ();
		}
		protected override void executeOncePerCycle ()
		{
			if(!executedThisCycle){
				executedThisCycle = true;
				ai.setClawsToAttack(w);
			}
		}
	}
	
	
	//idle state
	public class DeadState : AIState {
		public DeadState(BasicAI ai) : base(ai, "Dead", 1){
			
		}
		protected override void execute ()
		{
			ai.pathfinder.canMove = false;
			//so animation only plays once
			animationClip = null; 
			
		}
		protected override void executeOnce ()
		{
			
		}
		protected override void executeOncePerCycle ()
		{
			//throw new System.NotImplementedException ();
		}
	}
	
}
