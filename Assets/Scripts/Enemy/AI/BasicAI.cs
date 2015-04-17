using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
//using Pathfinding;

[RequireComponent (typeof(AIPath))]
public abstract class BasicAI : Hittable {
	int bloodAmount = 1; //change if u needa

	public AIState state;
	public VisualSensor visualSensor;
	public AudioSensor audioSensor;

	public BasicAI.DarklingPath pathfinder;
	
	public GameObject target;
	public GameObject destination; //the waypoint is an empty gameobject which gets moved to wherever

	public Claw[] claws;
	public FloatingHitPointBar healthBar;
	public Statistics statistics;
	public GameObject[] Allies;

	public List<string> loot = new List<string>();

	public bool alive = true;

	//define base stats:
	public float baseHP;
	public float baseArmor;
	public float baseFireResist;
	public float baseLightningResist;
	public float baseIceResist;
	public float baseChaosResist;
	public float basePhysicalDamage;
	public float baseFireDamage;
	public float baseLightningDamage;
	public float baseIceDamage;
	public float baseChaosDamage;
	public float baseHealthRegen;
	public float baseMoveSpeed;
	public float baseAttackSpeed;
	public float baseLifeLeech;
	public int baseSpellDamage;
	public int baseLevel;
	public float baseAttackRange;
	/// must be defined in base class!
	/// 
	/// 

	//renderer array to make sure we fade!
	Renderer[] renderers;
	bool fadeOut = false;

	public float BODY_DECAY_TIMER = 15.0f;

	// Use this for initialization
	protected void Start () {
		visualSensor = GetComponentInChildren<VisualSensor> ();
		audioSensor = GetComponentInChildren<AudioSensor> ();
		//pathfinder = transform.GetComponent<AIPath> ();
		claws = GetComponentsInChildren<Claw> ();
		healthBar = GetComponent<FloatingHitPointBar>();
		statistics = new Statistics(this);
		setLoot ();

		renderers = GetComponentsInChildren<Renderer> ();
		pathfinder = gameObject.AddComponent<BasicAI.DarklingPath>();
	}

	public abstract void setBaseStats(); //called at START, what base stats should i have
	public abstract void setLoot(); //called at START; what loot should i have
	public abstract void checkStateConditions (); //how to pick my next state
	public abstract void notifyHostileDetected(); //what to do when an ally notifies me to be hostile
	public abstract void alertDamage(); //what to do when i get damaged
	public abstract void onDeath(); //what to do when i die

	public void notifyAllies(){
		foreach (GameObject ally in Allies){
			if(ally.activeSelf) ally.SendMessage("notifyHostileEncountered", SendMessageOptions.DontRequireReceiver);
		}
	}

	public string stateName;
	// Update is called once per frame
	protected void Update () {
		if(state!=null){
			state.update ();
			stateName = state.animationClip;
		}
		checkStateConditions();
		//update health status
		healthBar.currentHealth = statistics.CurrentHP;
		healthBar.maxHealth = statistics.MaxHP;

		//fade out body if dead!
		if(fadeOut && gameObject!=null){
			foreach(Renderer r in renderers){
				Color c = new Color (r.material.color.r, r.material.color.g, r.material.color.b, 
				                     Mathf.Lerp(r.material.color.a,0,Time.deltaTime*2/BODY_DECAY_TIMER));
				r.material.color =  c;
			}
		}
	}

	protected void setState(AIState state){
//		Debug.Log (this+"'s state has been set to " + state);
		this.state = state;
		state.initialize ();
	}


	//  __   __     ______     __   __   __     ______     ______     ______   __     ______     __   __    
	// /\ "-.\ \   /\  __ \   /\ \ / /  /\ \   /\  ___\   /\  __ \   /\__  _\ /\ \   /\  __ \   /\ "-.\ \   
	// \ \ \-.  \  \ \  __ \  \ \ \'/   \ \ \  \ \ \__ \  \ \  __ \  \/_/\ \/ \ \ \  \ \ \/\ \  \ \ \-.  \  
	//  \ \_\\"\_\  \ \_\ \_\  \ \__|    \ \_\  \ \_____\  \ \_\ \_\    \ \_\  \ \_\  \ \_____\  \ \_\\"\_\ 
	//   \/_/ \/_/   \/_/\/_/   \/_/      \/_/   \/_____/   \/_/\/_/     \/_/   \/_/   \/_____/   \/_/ \/_/ 
	// 

	/// <summary>
	/// Determine if we are close enough to a Vector3 point
	/// </summary>
	/// <returns><c>true</c>, if within CloseEnoughDistance to destination, <c>false</c> otherwise.</returns>
	/// <param name="destination">Destination.</param>
	/// <param name="CloseEnoughDistance">Close enough distance.</param>
	/// 

	bool inRange(){//make sure angle is good
		if (!visualSensor.sensesObject(target)) return false;
		Vector3 distance = transform.position - target.transform.position;
		return (baseAttackRange <= distance.magnitude);
	}

	protected bool closeEnough(Vector3 destination, float CloseEnoughDistance){
		//Debug.Log("DISTANCE IS"+ (new Vector3 (transform.position.x, 0,transform.position.z) - new Vector3 (destination.x, 0, destination.z)).magnitude
		 //        +"\n Close enough distance is: "+CloseEnoughDistance);
		if ((new Vector3 (transform.position.x, 0, transform.position.z) - new Vector3 (destination.x, 0, destination.z)).magnitude <= CloseEnoughDistance) {
						//Debug.Log ("close enough!");
						return true;
				} else
						return false;

	}

	
	protected bool detectPlayer(){
		/*//if player is unreachable return false; we're not chasing/detecting these guys
		TODO: Implement new navigation System Astar SUCKS DICK

		GraphNode a = AstarPath.active.GetNearest ( transform.position, NNConstraint.Default ).node;
		GraphNode b = AstarPath.active.GetNearest ( target.transform.position, NNConstraint.Default ).node;
		if ( !PathUtilities.IsPathPossible (a,b) ) {
			return false;
		}
		*/

		//Debug.DrawLine(transform.position, target.transform.position);
		if(visualSensor.sensesObject(target)){
			//Debug.Log ("Player in cone");
			RaycastHit hit;
			//if(Physics.Raycast(transform.position, target.transform.position - transform.position, out hit))//distance is irrelevant because the collider takes care of that
			if(Physics.SphereCast(transform.position, 0.25f, target.transform.position - transform.position, out hit))
			{
				if(hit.collider.gameObject == target){
					//player is in line of sight
					//					Debug.Log("Player in Line of Sight");
					return true;
				}
			}
		}
		if (audioSensor.sensesObject (target))
			return true;
		return false;
	}


	/// <summary>
	/// Rotates towards target (used when stopped but still rotating)
	/// </summary>
	/// <param name="target">Target.</param>
	public void RotateTowards(Transform target){
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation (direction);
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime);
	}

	
	public void RotateTowards(Vector3 direction){
		Quaternion lookRotation = Quaternion.LookRotation (direction);
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime);
	}

	/// <summary>
	/// Sets the destination.
	/// </summary>
	/// <param name="destination">Destination.</param>
	public void setDestination(Vector3 dest){
		destination.transform.position = dest;
		//pathfinder.target = destination.transform;
	}

//			 ______     __         ______     __     __     ______    
//			/\  ___\   /\ \       /\  __ \   /\ \  _ \ \   /\  ___\   
//			\ \ \____  \ \ \____  \ \  __ \  \ \ \/ ".\ \  \ \___  \  
//			 \ \_____\  \ \_____\  \ \_\ \_\  \ \__/".~\_\  \/\_____\ 
//			  \/_____/   \/_____/   \/_/\/_/   \/_/   \/_/   \/_____/ 
	//notify claws attacking
	public void setClawsToAttack(Weapon weapon){
		foreach(Claw claw in claws){
			claw.notifyAttacking(true, weapon);
		}
	}





			
//	  	  _____   _____       ___   _____   _   _____   _____   _   _____   _____  
//		 /  ___/ |_   _|     /   | |_   _| | | /  ___/ |_   _| | | /  ___| /  ___/ 
//		 | |___    | |      / /| |   | |   | | | |___    | |   | | | |     | |___  
//	  	 \___  \   | |     / / | |   | |   | | \___  \   | |   | | | |     \___  \ 
//		  ___| |   | |    / /  | |   | |   | |  ___| |   | |   | | | |___   ___| | 
//		 /_____/   |_|   /_/   |_|   |_|   |_| /_____/   |_|   |_| \_____| /_____/ 

		
	public class Statistics {

		public BasicAI ai;
		public List<Buff> buffList = new List<Buff>();

		public int Level=1;

		public float TotalArmor;
		public float LightningResistance;
		public float FireResistance;
		public float IceResistance;
		public float ChaosResistance;

		public float PhysicalDamage;
		public float LightningDamage;
		public float FireDamage;
		public float IceDamage;
		public float ChaosDamage;
		
		public float CurrentHP;
		public float CurrentMana;
		public float MaxHP;
		public float MaxMana;
		
		public int TotalSTR;
		public int TotalDEX;
		public int TotalINT;

		public float AttackSpeed=1;
		public float SpellPower;
		public float MoveSpeed;
		
		public float HealthRegen;
		public float ManaRegen;

		public string stateName;


		public float calculateDamage(Weapon weapon){
//			Debug.LogWarning("Calculating damage for: "+weapon.name);
			Weapon.WeaponPropertyBundle weaponProperties = (Weapon.WeaponPropertyBundle) weapon.propertyBundle;
			float physicalDamage = Random.Range ((weapon.baseDamage + weaponProperties.bonusDamage) / 2,
			                                     (weapon.baseDamage + weaponProperties.bonusDamage));
			float chaosDamage = Random.Range ((weaponProperties.chaosDamage) / 2,
			                                  (weaponProperties.chaosDamage));
			float iceDamage = Random.Range ((weaponProperties.iceDamage) / 2,
			                                (weaponProperties.iceDamage));
			float fireDamage = Random.Range ((weaponProperties.fireDamage) / 2,
			                                 (weaponProperties.fireDamage));
			float lightningDamage = Random.Range ((weaponProperties.lightningDamage) / 2,
			                                      (weaponProperties.lightningDamage));
			
			physicalDamage = physicalDamage * (100/ (100 + TotalArmor));
			Debug.Log("Physical:"+ physicalDamage);
			chaosDamage = chaosDamage * (100/ (100 + ChaosResistance));
			Debug.Log("chaosDamage:"+ chaosDamage);
			iceDamage = iceDamage * (100/ (100 + IceResistance));
			Debug.Log("iceDamage:"+ iceDamage);
			fireDamage = fireDamage * (100/ (100 + FireResistance));
			Debug.Log("fireDamage:"+ fireDamage);
			lightningDamage = lightningDamage * (100/ (100 + LightningResistance));
			Debug.Log("lightningDamage:"+ lightningDamage);
			
			return (physicalDamage + chaosDamage + iceDamage + fireDamage + lightningDamage);
		}
		
		public Statistics(BasicAI ai){
			this.ai = ai;
			ai.setBaseStats();
			calculateStatistics();
		}
		
		//call from update!
		public void processBuffs(){
			List<Buff> removeBuffs = new List<Buff> ();
			foreach(Buff buff in buffList){
				if(buff!=null){
					buff.duration-=Time.deltaTime;
					CurrentHP+=Time.deltaTime * (buff.healthRestored/buff.totalDuration);
					CurrentMana+=Time.deltaTime * (buff.manaRestored/buff.totalDuration);
					if(buff.duration<=0){//notify removal of buff
						calculateStatistics ();
						removeBuffs.Add(buff);
					}
				}
			}
			foreach(Buff buff in removeBuffs){
				buffList.Remove(buff);
			}
		}
		
		public void notifyBuff(Buff buff){
			Debug.Log ("Added buff " + buff);
			buffList.Add (buff);
			calculateStatistics ();
		}

		public void calculateStatistics(){

			/*
			;
			ai.baseLightningDamage;
			ai.baseIceDamage;
			ai.baseChaosDamage;
			ai.baseHealthRegen;
			ai.baseMoveSpeed;
			ai.baseAttackSpeed;
			ai.baseSpellDamage;
			*/
			Level = ai.baseLevel;
			TotalArmor = ai.baseArmor;
			LightningResistance=ai.baseLightningResist;
			FireResistance=ai.baseFireResist;
			IceResistance=ai.baseIceResist;
			ChaosResistance=ai.baseChaosResist;
			
			MaxHP=ai.baseHP;
			CurrentHP = MaxHP;
			
			TotalSTR+=Level*10;
			TotalDEX+=Level*10;
			TotalINT+=Level*10;
			
			AttackSpeed=1;
			MoveSpeed = 1;
			SpellPower = 0;
			
			HealthRegen = 0;
			ManaRegen = 0;

			PhysicalDamage = ai.basePhysicalDamage;
			FireDamage = ai.baseFireDamage;
			IceDamage = ai.baseIceDamage;
			LightningDamage = ai.baseIceDamage;
			ChaosDamage = ai.baseChaosDamage;

			MoveSpeed = ai.baseMoveSpeed;
		}

	}


//			 ______     ______     ______   __  __     __     ______  
//			/\  ___\   /\  ___\   /\__  _\ /\ \_\ \   /\ \   /\__  _\ 
//			\ \ \__ \  \ \  __\   \/_/\ \/ \ \  __ \  \ \ \  \/_/\ \/ 
//			 \ \_____\  \ \_____\    \ \_\  \ \_\ \_\  \ \_\    \ \_\ 
//			  \/_____/   \/_____/     \/_/   \/_/\/_/   \/_/     \/_/ 
	
	public override void GetHit(DamageBundle damageBundle){
		if(!alive) return;
		if (damageBundle.source.Equals (gameObject)) {
			Debug.Log ("Why would I hit myself?");
		} 
		else {			
			float damage = statistics.calculateDamage(damageBundle.weapon);
			Debug.Log (gameObject + " was hit by " + damageBundle.source + " for " + damage);
			statistics.CurrentHP -= damage;
			bleed(damageBundle.hit);
			if (statistics.CurrentHP <= 0) {
				statistics.CurrentHP = 0;
				die ();
			}
		}
	}
	
	void GetHit(int straightDamage){
		statistics.CurrentHP -= straightDamage;
		if (statistics.CurrentHP <= 0) {
			die ();
		}
	}
	
	void die(){
		//ensure only one death!
		if(alive){
			alive=false;
			Debug.Log ("DEAD!");
			/*foreach(GameObject lootObject in loot){
				GameObject clone = (GameObject) Instantiate(lootObject,transform.position,transform.rotation);
				PickUpItem pickupClone = clone.GetComponentInChildren<PickUpItem>();
				Rigidbody cloneRigidBody  = clone.GetComponentInChildren<Rigidbody>();
				cloneRigidBody.velocity = new Vector3(0,6.5f,0);
				//cloneRigidBody.angularVelocity = new Vector3(Random.Range(-3,3),Random.Range(-3,3),Random.Range(-3,3));
			}*/
			foreach(string itemName in loot){
				Global.itemFactory.InstantiateItemFromPrefab (itemName,transform, statistics.Level);
			}
			Global.combatSystem.notifyExperienceGain (statistics.Level*3, 200);

			//ADD CONSUMABLE CORPSE
			gameObject.AddComponent<ConsumableCorpse>();

			//stop motion
			pathfinder.canMove = false;

			onDeath();
			StartCoroutine(destroyOnDeath());
		}
	}


	//make this more complicated for other monsters?
	void bleed(RaycastHit hit){
		//switch(Random.Range(0,2))
		//Instantiate(((GameObject) Resources.Load(bloodPrefabPaths[0])), hit.point, gameObject.transform.rotation);//instantiation
		//Instantiate(((GameObject) Resources.Load(bloodPrefabPaths[1])), hit.point, gameObject.transform.rotation);//instantiation
		StartCoroutine(CreateBlood(hit));
	}

	IEnumerator CreateBlood(RaycastHit hit){
		for(int i = 0; i<bloodAmount; i++){
			yield return new WaitForSeconds(0.02f);
			Instantiate(((GameObject) Resources.Load("Prefabs/Blood/BloodExplosion")), gameObject.transform.position, gameObject.transform.rotation);//instantiation
		}
	}

	IEnumerator destroyOnDeath(){
		StartCoroutine (fadeOutBody ());
		yield return new WaitForSeconds(BODY_DECAY_TIMER);
		Destroy (gameObject);
	}

	IEnumerator fadeOutBody(){
		yield return new WaitForSeconds(BODY_DECAY_TIMER/3);
		//Set shader to fading out
		fadeOut = true;
		foreach (Renderer r in renderers) {
			r.material.shader = Shader.Find ("Transparent/VertexLit with Z");
		}
	}
	
    public class DarklingPath : MonoBehaviour
    {
        //fill in a path interface here :)
		public float endReachedDistance;
		public Transform target;
		public float speed;
		public bool canMove;
		public AIPath aiPath;

		public Vector3 getRandomReachablePoint(){
			return AstarExtension.findRandomConnectedNodeFast(transform.position, 5);;
		}

        public void Start()
        {
			aiPath = GetComponent<AIPath> ();
			if (aiPath == null) {
				aiPath = gameObject.AddComponent<AIPath>();
				gameObject.AddComponent<Pathfinding.SimpleSmoothModifier>();
				gameObject.AddComponent<Pathfinding.AlternativePath>();
			}
			canMove = true;
        }

        public void Update()
        {
            if (canMove)
            {
				aiPath.target = target;
				aiPath.speed = speed/2.5f;
				aiPath.endReachedDistance = endReachedDistance;
            }
			else{
				aiPath.target = transform;
				aiPath.speed = 0;
			}
        }

    }


}
