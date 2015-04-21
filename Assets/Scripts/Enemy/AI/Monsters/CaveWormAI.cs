/// <summary>
/// TODO: GetHit changes state to Chasing
/// Spherecast in detectPlayer to chase
/// </summary>

using UnityEngine;
using System.Collections;

public class CaveWormAI : BasicAI {
	
	AIState idleState;
	AIState wanderState;
	AIState attackingState;
	AIState chasing;
	AIState dead;
	
	const float IDLE_TIME = 10.0f;
	const float WANDER_TIME = 15.0f;
	const float SEARCH_TIME = 30.0f;
	float ATTACK_TIME;

	// Use this for initialization
	new void Start () {
		base.Start ();
		pathfinder.endReachedDistance = baseAttackRange / 2;
		ATTACK_TIME = GetComponent<Animation>()["Attack"].clip.length / statistics.AttackSpeed;
		
		target = GameObject.FindGameObjectWithTag("Player");
		
		idleState = new IdleState (this);
		wanderState = new WanderingState (this);
		attackingState = new AttackingState (this);
		chasing = new ChasingState (this);
		dead = new DeadState(this);
		
		setState(wanderState);
		Debug.Log("State initialized to"+state);

		destination = new GameObject(); //make empty gameobject, move position to random point and set Waypoint as the target
		
	}

	/// <summary>
	/// Sets the base stats.
	/// </summary>
	public override void setBaseStats(){
		baseLevel = 2;
		baseHP = 100;
		baseArmor = 10 ;
		baseFireResist = 15;
		baseLightningResist = 5;
		baseIceResist = 5;
		baseChaosResist = 0;
		basePhysicalDamage = 10;
		baseFireDamage = 0;
		baseLightningDamage =0;
		baseIceDamage = 0;
		baseChaosDamage = 0;
		baseHealthRegen = 0;
		baseMoveSpeed = 5f;
		baseAttackSpeed = 1;
		baseLifeLeech = 0;
		baseSpellDamage = 0; //int
		baseAttackRange = 3.0f;
	}
    
    /// <summary>
    /// Become aggressive when getting hit
    /// </summary>
    public override void becomeAggressive()
    {
        setState(chasing);
    }

	/// <summary>
	/// Randomly calculate the loot.
	/// </summary>
	public override void setLoot ()
	{	
		loot.Add ("Shaman Staff");
		loot.Add ("Health Potion");
		loot.Add ("Cloth Armor");
		loot.Add ("Compound Bow");
		loot.Add ("Scroll of FlameArrow");
		loot.Add ("Health Potion");
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
		//don't idle for more than 5 seconds
		if (state.Equals (idleState)) {
			if (detectPlayer ()) {
					setState (chasing);
			} else if (state.stateTime >= IDLE_TIME) {
					setState (wanderState);
			}
		} 
		else if (state.Equals (wanderState)) {
			if (detectPlayer ()) {
					setState (chasing);
			} else if (state.stateTime >= WANDER_TIME) {
					setState (idleState);
			} else if (closeEnough (((WanderingState)state).randomPoint, 2.1f)) {
					setState (idleState);
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
    
    ///!!!STATES!!!
    ///

	//idle state
	public class IdleState : AIState {
		public IdleState(BasicAI ai) : base(ai, "Idle", 1){
			
		}
		protected override void execute ()
		{
			//stand still
			ai.pathfinder.target = ai.transform;
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
		public Vector3 randomPoint;
		
		public WanderingState(BasicAI ai) : base(ai, "Walk", 0.5f){
			
		}
		protected override void execute ()
		{
			//wandering script
			//wander to a random point on the grid
			executeOncePerCycle();
			ai.setDestination(randomPoint);
            ai.pathfinder.target = ai.destination.transform;
			ai.pathfinder.speed = ai.baseMoveSpeed / 2;
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
		public ChasingState(BasicAI ai) : base(ai, "Walk", 1.0f){
			
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
			}
		}
	}
	
	//attacking state
	public class AttackingState : AIState {

        Weapon w;

        public AttackingState(BasicAI ai)
            : base(ai, "Attack", ai.statistics.AttackSpeed)
        {
            w = new Weapon(0, 0, null, "Claws", ai.statistics.PhysicalDamage, ai.statistics.AttackSpeed);
			Weapon.WeaponPropertyBundle clawProperties = new Weapon.WeaponPropertyBundle(Item.ItemPropertyBundle.Rarity.Common, ai.statistics.Level);
			//w.baseDamage += ai.statistics
			//Debug.Log("Damage = "+ai.statistics.PhysicalDamage);
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
			//ai.pathfinder.target = ai.transform;
			//ai.pathfinder.canMove = false;
			executeOncePerCycle();
			ai.RotateTowards(ai.target.transform);
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
			ai.pathfinder.speed = 0;
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
