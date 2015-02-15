using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatSystem : Hittable {

	private CharacterMotor characterMotor;

	public static WeaponCollision WeaponCollider;
	public static Weapon Weapon;
	public PlayerHealth healthStatus;

	public float actionCooldown;

	public Dictionary<string, Slot> equipmentSlots;

	public ResistanceInfo resistanceInfo = new ResistanceInfo();

	public CameraShake cameraShake;

	public Vignetting vignetting;

	public Dictionary<string, PlayerState> stateDict;
	public PlayerState currentState;
	
	void Awake(){
		characterMotor = this.GetComponent<CharacterMotor> ();
		vignetting = this.GetComponentInChildren<Vignetting> ();
		cameraShake = GetComponentInChildren<CameraShake>();
		stateDict = new Dictionary<string, PlayerState>();
		stateDict.Add("Walking", new PlayerState.WalkingState(this));
		stateDict.Add ("Idle", new PlayerState.IdleState(this));
		stateDict.Add ("Slashing", new PlayerState.SlashingState(this));
		stateDict.Add ("Stabbing", new PlayerState.StabbingState(this));
		stateDict.Add ("Slamming", new PlayerState.SlammingState(this));
		stateDict.Add ("BowLoad", new PlayerState.BowLoadState(this));
		stateDict.Add ("BowRelease", new PlayerState.BowReleaseState(this));
		stateDict.Add ("Spell1", new PlayerState.Spell1State(this));
		stateDict.Add ("Spell2", new PlayerState.Spell2State(this));
		stateDict.Add ("Spell3", new PlayerState.Spell3State(this));
		stateDict.Add ("Spell4", new PlayerState.Spell4State(this));
		currentState = stateDict["Idle"];
	}

	// Use this for initialization
	void Start () {
		Global.arms = GetComponentInChildren<AnimationController> ();
		healthStatus = GetComponentInChildren<PlayerHealth> ();
		equipmentSlots = Global.inventory.equipmentSlots;
		notifyEquipmentChange ();
		//excess we don't need?
		if(Global.arms==null) Global.arms = GetComponentInChildren<AnimationController> ();
		GameObject unarmed = GameObject.FindGameObjectWithTag("Unarmed");
		WeaponCollider = unarmed.GetComponentInChildren<WeaponCollision> (); //should get unarmed
		//set arrow to inactive
		Global.LoadArrow.SetActive(false);
	}

	// Update is called once per frame
	void Update () {

		//set vignetting range from 0-10:
		if(vignetting!=null) vignetting.intensity = (1-(Statistics.CurrentHP)/Statistics.MaxHP) * 10;


		//clamp stats
		if(Statistics.CurrentHP<0){
			Statistics.CurrentHP=0;
		}
		if(Statistics.CurrentHP>Statistics.MaxHP){
			Statistics.CurrentHP=Statistics.MaxHP;
		}

		if(Statistics.CurrentMana<0){
			Statistics.CurrentMana=0;
		}
		if(Statistics.CurrentMana>Statistics.MaxMana){
			Statistics.CurrentMana=Statistics.MaxMana;
		}
		if(Statistics.Hunger<0){
			Statistics.Hunger=0;
		}
		if(Statistics.Hunger>1000){
			Statistics.Hunger=1000;
		}

		if(Time.timeScale != 0){
			currentState.execute();
		}

		Statistics.CurrentHP += Statistics.HealthRegen * Time.deltaTime * 0.1f;
		Statistics.CurrentMana += Statistics.ManaRegen * Time.deltaTime * 0.1f;
		Statistics.Hunger += Statistics.HungeRate * Time.deltaTime * 0.1f;
		healthStatus.notifyHealthChange (Statistics.CurrentHP, Statistics.MaxHP, Statistics.CurrentMana, Statistics.MaxMana, Statistics.Hunger, 1000);

		Statistics.processBuffs ();
	}

	public void animate(string clipName, float speed){
		if(Global.arms==null) Global.arms = GetComponentInChildren<AnimationController> ();
		Global.arms.notifyAnimation(clipName, speed);
	}

	public void setState(string stateName){
//		Debug.LogWarning("State name = "+stateName);
		this.currentState = stateDict[stateName];
		currentState.reset();
	}

	
	public override void GetHit(DamageBundle damageBundle) {
		if(damageBundle.source.Equals(gameObject)){
			//		Debug.Log("Why would I hit myself?");
		}
		else{
//			Debug.LogWarning("Damage Source = "+damageBundle.source);
			float damage = resistanceInfo.calculateDamage(damageBundle.weapon);
			Debug.Log(damage + " damage dealt to player by "+damageBundle.source);
			Statistics.CurrentHP -= damage;
			if(cameraShake!=null) cameraShake.Shake (damage * 2);
			if(cameraShake!=null) cameraShake.flashBloodBorder (damage/Statistics.MaxHP);
		}
	}

	void GetHit(float straightDamage) {
		Statistics.CurrentHP -= straightDamage;
		cameraShake.Shake (straightDamage);
		cameraShake.flashBloodBorder (straightDamage/Statistics.MaxHP);
	}



	//potion drinking
	public void notifyHealthPotion(int health){
		//todo: make this restore over time
		Statistics.CurrentHP += health;
	}
	public void notifyManaPotion(int mana){
		//todo: make this restore over time
		Statistics.CurrentMana += mana;
	}

	public void notifyEquippedWeapon(GameObject weapon, Weapon weaponItem){
		WeaponCollider = weapon.GetComponentInChildren<WeaponCollision> ();
//		Debug.LogError("My weapon collider is now a "+ WeaponCollider);
		Weapon = weaponItem;
//		Debug.LogWarning("Weapon equipped is now: "+weapon.name);
	}

	public void notifyEquippedArmor(Armor armor){
		//TODO;
	}
		
	public void notifyEquipmentChange(){
		Statistics.calculateStatistics ();
		Global.characterPage.notifyChangesToStats ();
		if(characterMotor!=null) characterMotor.movement.movementSpeedAdjust = Statistics.MoveSpeed;
	}
	
	
	public void notifyExperienceGain(int monsterLevel, int XP){
		Statistics.XP += (int) ((float) XP*((float) monsterLevel/(float) Statistics.Level));
		Debug.Log("EXPERIENCE GAINED!: "+((int) ((float) XP*((float) monsterLevel/(float) Statistics.Level))));
		while (Statistics.XP >= Statistics.NextLevelXP) {
			Statistics.XP-=Statistics.NextLevelXP;
			Statistics.NextLevelXP+=Statistics.GetNextLevelXP(Statistics.Level+1);
			LevelUp();
		}
		notifyEquipmentChange ();
	}

	public void LevelUp(){
		Statistics.Level += 1;
		Statistics.SkillPointsToAssign += 10;
	}

	public bool canUseItem(Item item){
		if(Statistics.TotalSTR<item.propertyBundle.requirements.strReq){
			return false;
		}
		if(Statistics.TotalDEX<item.propertyBundle.requirements.dexReq){
			return false;
		}
		if(Statistics.TotalINT<item.propertyBundle.requirements.intReq){
			return false;
		}
		return true;
	}

//		 	 _____    _____   _____   _   _____   _____       ___   __   _   _____   _____        _   __   _   _____   _____  
//			|  _  \  | ____| /  ___/ | | /  ___/ |_   _|     /   | |  \ | | /  ___| | ____|      | | |  \ | | |  ___| /  _  \ 
//			| |_| |  | |__   | |___  | | | |___    | |      / /| | |   \| | | |     | |__        | | |   \| | | |__   | | | | 
//			|  _  /  |  __|  \___  \ | | \___  \   | |     / / | | | |\   | | |     |  __|       | | | |\   | |  __|  | | | | 
//			| | \ \  | |___   ___| | | |  ___| |   | |    / /  | | | | \  | | |___  | |___       | | | | \  | | |     | |_| | 
//			|_|  \_\ |_____| /_____/ |_| /_____/   |_|   /_/   |_| |_|  \_| \_____| |_____|      |_| |_|  \_| |_|     \_____/ 

	public class ResistanceInfo{
		public float armor=0;
		public float magicResist=0;
		public float fireResist=0;
		public float iceResist=0;
		public float lightningResist=0;
		public float chaosResist=0; // should be set to zero all the time
		
		public float calculateDamage(Weapon weapon){
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
			
			physicalDamage = physicalDamage * (100/ (100 + armor));
			chaosDamage = chaosDamage * (100/ (100 + chaosResist));
			iceDamage = iceDamage * (100/ (100 + iceResist));
			fireDamage = fireDamage * (100/ (100 + fireResist));
			lightningDamage = lightningDamage * (100/ (100 + lightningResist));
			
			return (physicalDamage + chaosDamage + iceDamage + fireDamage + lightningDamage);
		}
	}


	
	//	  	  _____   _____       ___   _____   _   _____   _____   _   _____   _____  
	//		 /  ___/ |_   _|     /   | |_   _| | | /  ___/ |_   _| | | /  ___| /  ___/ 
	//		 | |___    | |      / /| |   | |   | | | |___    | |   | | | |     | |___  
	//	  	 \___  \   | |     / / | |   | |   | | \___  \   | |   | | | |     \___  \ 
	//		  ___| |   | |    / /  | |   | |   | |  ___| |   | |   | | | |___   ___| | 
	//		 /_____/   |_|   /_/   |_|   |_|   |_| /_____/   |_|   |_| \_____| /_____/ 

	public static class Statistics {

		public static List<Buff> buffList = new List<Buff>();

		public static int SkillPointsToAssign=0;

		public static string Name="Mircol";
		public static int XP;//starting values
		public static int Level=1;
		public static int NextLevelXP=400;

		public static int MinTotalDamage; //base + str modifier + 
		public static int MaxTotalDamage; //base + str modifier + 

		public static int TotalArmor;
		public static int LightningResistance;
		public static int FireResistance;
		public static int IceResistance;
		public static int ChaosResistance;

		public static float CurrentHP;
		public static float CurrentMana;
		public static float MaxHP;
		public static float MaxMana;

		public static int TotalSTR;
		public static int TotalDEX;
		public static int TotalINT;

		public static int levelUpSTR;
		public static int levelUpINT;
		public static int levelUpDEX;

		public static float AttackSpeed=1;
		public static float SpellPower;
		public static float MoveSpeed;

		public static float HealthRegen;
		public static float ManaRegen;

		public static float Hunger=0;
		public static float HungeRate=1;
		public static int Evil=0;

		//call from update!
		public static void processBuffs(){
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

		public static void notifyBuff(Buff buff){
			Debug.Log ("Added buff " + buff);
			buffList.Add (buff);
			calculateStatistics ();
		}

		public static void calculateStatistics(){
			List<Slot> slots = new List<Slot>(Global.inventory.equipmentSlots.Values);
			MinTotalDamage=0; //base + str modifier + 
			MaxTotalDamage=0; //base + str modifier + 

			TotalArmor = 0;
			LightningResistance=0;
			FireResistance=0;
			IceResistance=0;
			ChaosResistance=0;

			MaxHP=0;
			MaxMana=0;
			
			TotalSTR=Level;
			TotalDEX=Level;
			TotalINT=Level;

			AttackSpeed=0;
			MoveSpeed = 1;
			SpellPower = 0;

			HealthRegen = 0;
			ManaRegen = 0;


			foreach(Slot slot in slots){
				if(slot.item is Armor){
					Armor armor = (Armor) slot.item;
					Armor.ArmorPropertyBundle armorProperties = (Armor.ArmorPropertyBundle) armor.propertyBundle;

					TotalArmor+=(int) (armor.baseArmor+armorProperties.bonusArmor *(100+armorProperties.quality)/100);

					AttackSpeed+=armorProperties.bonusAttackSpeed;
					TotalDEX+=armorProperties.bonusDEX;
					SpellPower+=armorProperties.spellPower;
					ManaRegen+=armorProperties.manaRegen;
					LightningResistance+=(int) armorProperties.lightningResist;
					IceResistance+=(int) armorProperties.iceResist;
					FireResistance+=(int) armorProperties.fireResist;
					ChaosResistance+=(int)armorProperties.chaosResist;
					TotalSTR+=armorProperties.bonusSTR;
					MoveSpeed+=armorProperties.bonusMoveSpeed;
					MaxMana+=armorProperties.bonusMana;
					TotalINT+=armorProperties.bonusINT;
					MaxHP+=armorProperties.bonusHealth;

				}


				if(slot.item is Weapon){
					Weapon weapon = (Weapon) slot.item;
					Weapon.WeaponPropertyBundle weaponProperties = (Weapon.WeaponPropertyBundle) weapon.propertyBundle;

					TotalSTR+=weaponProperties.bonusSTR;
					TotalDEX+=weaponProperties.bonusDEX;
					TotalINT+=weaponProperties.bonusINT; 

					float minPhysicalDamage = (weapon.baseDamage + weaponProperties.bonusDamage) / 2 *(100+weaponProperties.quality)/100;
					float minChaosDamage = (weaponProperties.chaosDamage) / 2;
					float minIceDamage = (weaponProperties.iceDamage) / 2;
					float minFireDamage = (weaponProperties.fireDamage) / 2;
					float minLightningDamage = (weaponProperties.lightningDamage) / 2;

					AttackSpeed+=weapon.baseAttackSpeed+weaponProperties.bonusAttackSpeed;

					MinTotalDamage = (int) (minPhysicalDamage + minChaosDamage + minIceDamage + minFireDamage + minLightningDamage);

				}
			}

			//apply buffs
			foreach(Buff buff in buffList){
				if(buff.duration!=0){
					TotalArmor += (int) buff.armorBuff;
					AttackSpeed += buff.attackSpeedBuff;
					ChaosResistance += buff.chaosResistBuff;
					MinTotalDamage += buff.damageBuff/2;
					FireResistance += buff.fireResistBuff;
					IceResistance += buff.iceResistBuff;
					LightningResistance += buff.lightningResistBuff;
					MoveSpeed += buff.movementSpeedBuff;
				}
			}
			
			TotalSTR += levelUpSTR;
			TotalDEX += levelUpDEX;
			TotalINT += levelUpINT;
			
			MaxHP+=TotalSTR/2+10*Level;
			MinTotalDamage+=TotalSTR/2;
			MaxTotalDamage=MinTotalDamage*2;

			HealthRegen += MaxHP * 0.01f;

			MaxMana+=TotalINT/2+10*Level;
			ManaRegen += MaxMana * 0.01f;

			SpellPower+=TotalINT/2;
			
			AttackSpeed+=TotalDEX/2 * 0.01f;
			MoveSpeed+=TotalDEX/2 * 0.01f;
		}

		
		//call before we adjust level
		//adjust level LAST
		public static int GetNextLevelXP(int level){
			switch(level){
			case 1:
				return 400;
			case 2:
				return 900;
			case 3:
				return 1400;
			case 4:
				return 2100;
			case 5:
				return 2800;
			case 6:
				return 3600;
			case 7:
				return 4500;
			case 8:
				return 5400;
			case 9:
				return 6500;
			case 10:
				return 7600;
			case 11:
				return 8700;
			case 12:
				return 9800;
			case 13:
				return 11000;
			case 14:
				return 12300;
			case 15:
				return 13600;
			case 16:
				return 15000;
			case 17:
				return 16400;
			case 18:
				return 17800;
			case 19:
				return 19300;
			case 20:
				return 20800;
			case 21:
				return 22400;
			case 22:
				return 24000;
			case 23:
				return 25500;
			case 24:
				return 27200;
			case 25:
				return 28900;
			case 26:
				return 30500;
			case 27:
				return 32300;
			case 28:
				return 33900;
			case 29:
				return 36300;
			case 30:
				return 38300;
			case 31:
				return 41600;
			case 32:
				return 44600;
			case 33:
				return 48000;
			case 34:
				return 51400;
			case 35:
				return 55000;
			case 36:
				return 58700;
			case 37:
				return 62400;
			case 38:
				return 66200;
			case 39:
				return 70200;
			case 40:
				return 74300;
			case 41:
				return 78500;
			case 42:
				return 82800;
			case 43:
				return 87100;
			case 44:
				return 91600;
			case 45:
				return 96300;
			case 46:
				return 101000;
			case 47:
				return 105800;
			case 48:
				return 110700;
			case 49:
				return 115700;
			case 50:
				return 120900;
			case 51:
				return 126100;
			case 52:
				return 131500;
			case 53:
				return 137000;
			case 54:
				return 142500;
			case 55:
				return 148200;
			case 56:
				return 154000;
			case 57:
				return 159900;
			case 58:
				return 165800;
			case 59:
				return 172000;
			case 60:
				return 290000;//TODO: add more levels
			}
			return 1000000;//should never reach
		}
		 
		public static string TransformationState(int evil){ //i.e. human, dark human, twisted human, demihuman, wraith, demonic etc..
			switch (evil) {
			case 0:
				return "Human";
			case 1:
				return "Human";
			case 2:
				return "Fallen Human";
			case 3:
				return "Dark Human";
			}
			return "Human";

		}
	}

}	