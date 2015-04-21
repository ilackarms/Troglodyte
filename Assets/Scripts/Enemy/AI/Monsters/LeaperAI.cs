using UnityEngine;
using System.Collections;

public class LeaperAI : BasicAI {
    
    AIState idling;
    AIState wandering;
    AIState chasing;
    AIState crouching;
    AIState pouncing;
    AIState swiping;
    AIState dead;

    const float POUNCE_RANGE = 15.0f;
    const float IDLE_TIME = 5.0f;
    const float WANDER_TIME = 15.0f;
    const float SEARCH_TIME = 30.0f; //time searching for player in chase state when player is no longer detected by sensor
    float CROUCH_TIME; //time it takes to complete crouch animation
    float POUNCE_TIME; //time it takes to complete pounce animation (+ extra time to land)
    float SWIPE_TIME; //time it takes to perform the swipe animation

    //FOR JUMPING BEHAVIOR
    public Vector3 pounceVector;
    CharacterController cc;
    AIPath aiPath;

	// Use this for initialization
	new void Start () {
        
        base.Start();
        pathfinder.endReachedDistance = baseAttackRange / 2;
        SWIPE_TIME = GetComponent<Animation>()["swipe"].clip.length / statistics.AttackSpeed;
        CROUCH_TIME = GetComponent<Animation>()["crouch"].clip.length / statistics.AttackSpeed;
        POUNCE_TIME = GetComponent<Animation>()["pounce"].clip.length; //5 Seconds fixed time after animation completes

        target = GameObject.FindGameObjectWithTag("Player");
        
        idling = new IdleState(this);
        wandering = new WanderingState(this);
        crouching = new CrouchingState(this);
        pouncing = new PouncingState(this);
        swiping = new SwipingState(this);
        chasing = new ChasingState(this);
        dead = new DeadState(this);

        setState(idling);
        Debug.Log("State initialized to" + state);

        //FOR JUMPING BEHAVIOR
        pounceVector = new Vector3(0, 0, 0);
        cc = GetComponent<CharacterController>();
        aiPath = GetComponent<AIPath>();

        destination = new GameObject(); //make empty gameobject, move position to random point and set Waypoint as the target
	}

    /// <summary>
    /// Sets the base stats.
    /// </summary>
    public override void setBaseStats()
    {
        baseLevel = 1;
        baseHP = 100;
        baseArmor = 0;
        baseFireResist = 15;
        baseLightningResist = 5;
        baseIceResist = 5;
        baseChaosResist = 0;
        basePhysicalDamage = 10;
        baseFireDamage = 0;
        baseLightningDamage = 0;
        baseIceDamage = 0;
        baseChaosDamage = 0;
        baseHealthRegen = 0;
        baseMoveSpeed = 3f;
        baseAttackSpeed = 1;
        baseLifeLeech = 0;
        baseSpellDamage = 0; //int
        baseAttackRange = 2.0f;
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
    public override void setLoot()
    {
        loot.Add("Health Potion");
    }
    
    /// <summary>
    /// anything that happens on death unique to the subclass. Death animation defined here.
    /// </summary>
    public override void onDeath()
    {
        setState(dead);
    }
	
	// Update is called once per frame
	new void Update () {
        base.Update();
	}

    void LateUpdate()
    {
        //jumping behavior
        pounceVector.y -= 9.81f * Time.deltaTime;
        aiPath.enabled = true;
        if (pounceVector.y <= 0)
        {
            pounceVector.x = 0;
            pounceVector.y = 0;
            pounceVector.z = 0;
        }
        else
        {
            aiPath.enabled = false;
            cc.SimpleMove(pounceVector);
            //gameObject.transform.position += pounceVector * Time.deltaTime;
        }
    }


    //called in update
    public override void checkStateConditions()
    {
        //state switch conditions
        //don't idle for more than 5 seconds
        if (state.Equals(idling))
        {
            if (detectPlayer())
            {
                setState(chasing);
            }
            else if (state.stateTime >= IDLE_TIME)
            {
                setState(wandering);
            }
        }
        else if (state.Equals(wandering))
        {
            if (detectPlayer())
            {
                setState(chasing);
            }
            else if (state.stateTime >= WANDER_TIME)
            {
                setState(idling);
            }
            else if (closeEnough(((WanderingState)wandering).randomPoint, 2.0f))
            {
                setState(idling);
            }
        }
        else if (state.Equals(chasing))
        {
            //swipe if close, jump if far
            if (closeEnough(target.transform.position, baseAttackRange))
            {
                setState(swiping);
            }
            else if (closeEnough(target.transform.position, POUNCE_RANGE))
            {
                setState(crouching);
            }
            else if (detectPlayer())
            {
                setState(chasing);//refresh cooldown on search timer
            }
            else if (state.stateTime >= SEARCH_TIME)
            {
                setState(wandering);
            }
        }
        else if (state.Equals(crouching))
        {
            if (state.stateTime >= CROUCH_TIME)
            {
                setState(pouncing);
            }
        }
        else if (state.Equals(pouncing))
        {
            if (state.stateTime >= POUNCE_TIME)
            {
                setState(chasing);
            }
        }
        else if (state.Equals(swiping))
        {
            if (state.stateTime >= SWIPE_TIME)
            {
                setState(chasing);
            }
        }
    }

    ///!!!STATES!!!
    ///

    //idle state
    public class IdleState : AIState
    {
        public IdleState(BasicAI ai)
            : base(ai, "idle", 1)
        {

        }
        protected override void execute()
        {
            //stand still
            ai.pathfinder.target = ai.transform;
        }
        protected override void executeOnce()
        {
            //throw new System.NotImplementedException ();
        }
        protected override void executeOncePerCycle()
        {
            //throw new System.NotImplementedException ();
        }
    }

    //wandering state
    public class WanderingState : AIState
    {
        public Vector3 randomPoint;

        public WanderingState(BasicAI ai)
            : base(ai, "walk", 3f)
        {

        }
        protected override void execute()
        {
            //wandering script
            //wander to a random point on the grid
            executeOncePerCycle();
            ai.setDestination(randomPoint);
            ai.pathfinder.target = ai.destination.transform;
            ai.pathfinder.speed = ai.baseMoveSpeed;
        }

        public override void initialize()
        {
            base.initialize();
        }

        protected override void executeOnce()
        {

        }
        protected override void executeOncePerCycle()
        {
            if (!executedThisCycle)
            {
                executedThisCycle = true;
                randomPoint = ai.pathfinder.getRandomReachablePoint();
            }
        }
    }

    //chasing state
    public class ChasingState : AIState
    {
        public ChasingState(BasicAI ai)
            : base(ai, "walk", 3f)
        {

        }
        protected override void execute()
        {
            //wandering script
            ai.pathfinder.target = ai.target.transform;
            ai.pathfinder.speed = ai.baseMoveSpeed;
            executeOncePerCycle();
        }
        protected override void executeOnce()
        {
            //throw new System.NotImplementedException ();
        }
        protected override void executeOncePerCycle()
        {
            if (!executedThisCycle)
            {
                executedThisCycle = true;
            }
        }
    }

    //swiping state
    public class SwipingState : AIState
    {
        Weapon w;

        public SwipingState(BasicAI ai)
            : base(ai, "swipe", ai.statistics.AttackSpeed)
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
        protected override void execute()
        {
            //ai.pathfinder.target = ai.transform;
            //ai.pathfinder.canMove = false;
            executeOncePerCycle();
            ai.RotateTowards(ai.target.transform);
        }
        protected override void executeOnce()
        {
            //throw new System.NotImplementedException ();
        }
        protected override void executeOncePerCycle()
        {
            if (!executedThisCycle)
            {
                executedThisCycle = true;
                ai.setClawsToAttack(w);
            }
        }
    }

    //crouching state
    public class CrouchingState : AIState
    {
        //still attacking in this state?
        Weapon w;

        public CrouchingState(BasicAI ai)
            : base(ai, "crouch", 3f)
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
        protected override void execute()
        {
            //ai.pathfinder.target = ai.transform;
            //ai.pathfinder.canMove = false;
            executeOncePerCycle();
            ai.RotateTowards(ai.target.transform);
            ai.pathfinder.speed = 0;
        }
        protected override void executeOnce()
        {
            //throw new System.NotImplementedException ();
        }
        protected override void executeOncePerCycle()
        {
            if (!executedThisCycle)
            {
                executedThisCycle = true;
                ai.setClawsToAttack(w);
            }
        }
    }

    //pouncing state
    public class PouncingState : AIState
    {
        //still attacking in this state?
        Weapon w;

        public PouncingState(BasicAI ai)
            : base(ai, "pounce", ai.statistics.AttackSpeed * 2)
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
        protected override void execute()
        {
            //ai.pathfinder.target = ai.transform;
            //ai.pathfinder.canMove = false;
            executeOncePerCycle();
            ai.RotateTowards(ai.target.transform);
            ai.pathfinder.speed = 0;
        }
        protected override void executeOnce()
        {
            //throw new System.NotImplementedException ();
        }
        protected override void executeOncePerCycle()
        {
            if (!executedThisCycle)
            {
                //JUMP!
                float jumpSpeed = 12.5f;
                Vector3 jumpDirection = (ai.target.transform.position - ai.transform.position).normalized;                
                jumpDirection.y = 2; //jump height
                ((LeaperAI)ai).pounceVector = jumpDirection * jumpSpeed;

                executedThisCycle = true;
                ai.setClawsToAttack(w);
            }
        }
    }


    //idle state
    public class DeadState : AIState
    {
        public DeadState(BasicAI ai)
            : base(ai, "death", 1)
        {

        }
        protected override void execute()
        {
            ai.pathfinder.canMove = false;
            ai.pathfinder.speed = 0;
            //so animation only plays once
            animationClip = null;

        }
        protected override void executeOnce()
        {

        }
        protected override void executeOncePerCycle()
        {
            //throw new System.NotImplementedException ();
        }
    }


}
