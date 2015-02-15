using UnityEngine;
using System.Collections;
using AssemblyCSharp;
public class Weapon : Item {

	public float baseAttackSpeed;
	public float baseDamage;


	public override void performAction(){

	}
	
	public override void calculateDescription(){
		description = "";
		WeaponPropertyBundle weaponPropertyBundle = (WeaponPropertyBundle)propertyBundle;
		description += "Base Damage: " + (baseDamage/2) + " - " + baseDamage + "\n";
		description += "Base Attack Speed: "+ baseAttackSpeed + "\n";
		if (weaponPropertyBundle.quality > 0) {
			description += "Bonus Item Quality: "+(weaponPropertyBundle.quality)+ "%\n";
		}
		if (weaponPropertyBundle.bonusDamage > 0) {
			description += "Physical Damage increased by "+(weaponPropertyBundle.bonusDamage/2) + " - " + weaponPropertyBundle.bonusDamage + "\n";
		}
		if (weaponPropertyBundle.bonusAttackSpeed > 0) {
			description += "Attack Speed increased by "+ ((int) (weaponPropertyBundle.bonusAttackSpeed*100))+"%"+"\n";
		}
		if (weaponPropertyBundle.bonusSTR > 0) {
			description += "Strength increased by "+ (weaponPropertyBundle.bonusSTR)+"\n";
		}
		if (weaponPropertyBundle.bonusINT > 0) {
			description += "Intelligence increased by "+ (weaponPropertyBundle.bonusINT)+"\n";
		}
		if (weaponPropertyBundle.bonusDEX > 0) {
			description += "Dexterity increased by "+ (weaponPropertyBundle.bonusDEX)+"\n";
		}
		if (weaponPropertyBundle.fireDamage > 0) {
			description += "Fire Damage on hit "+(weaponPropertyBundle.fireDamage/2) + " - " + weaponPropertyBundle.fireDamage + "\n";
		}
		if (weaponPropertyBundle.iceDamage > 0) {
			description += "Ice Damage on hit "+(weaponPropertyBundle.iceDamage/2) + " - " + weaponPropertyBundle.iceDamage + "\n";
		}
		if (weaponPropertyBundle.lightningDamage > 0) {
			description += "Lightning Damage on hit "+(weaponPropertyBundle.lightningDamage/2) + " - " + weaponPropertyBundle.lightningDamage + "\n";
		}
		if (weaponPropertyBundle.chaosDamage > 0) {
			description += "Chaos Damage on hit "+(weaponPropertyBundle.chaosDamage/2) + " - " + weaponPropertyBundle.chaosDamage + "\n";
		}
		if (weaponPropertyBundle.lifeLeech > 0) {
			description += "Life stolen on hit "+(weaponPropertyBundle.lifeLeech/2) + " - " + weaponPropertyBundle.lifeLeech + "\n";
		}
		if (weaponPropertyBundle.manaLeech > 0) {
			description += "Mana drained on hit "+(weaponPropertyBundle.manaLeech/2) + " - " + weaponPropertyBundle.manaLeech + "\n";
		}
		if (weaponPropertyBundle.spellPower > 0) {
			description += "Increased Spell Power "+(weaponPropertyBundle.spellPower)+"\n";
		}
	}


	//random 
	public Weapon(int width, int height, Texture2D image, string name, float baseDamage, float baseAttackSpeed){
		this.width = width;
		this.height = height;
		this.image = image;
		this.name = name;
		this.baseDamage = baseDamage;
		this.baseAttackSpeed = baseAttackSpeed;

		//only for equippable
//		Debug.Log (name + " getting equippedmodel");
		if(name!=null){
			equippedModel = Global.equippedItemIndex.getEquippedModel (name);
		}

	}


	//ALWAYS CALL UNEQUIP BEFORE EQUIP; SO WE DON'T SETINACTIVE THE SAME ITEM MODEL IF WE SWAP 2 ITEMS
	//WITH SAME MODEL!
	public override void Equip(){
		equippedModel.SetActive (true);
		//Debug.LogError(equippedModel + "equipped");
		Global.combatSystem.notifyEquippedWeapon (equippedModel, this);
	}
	
	public override void Unequip(){
		equippedModel.SetActive (false);
		//Debug.LogError(equippedModel + "unequipped");
		Global.combatSystem.notifyEquippedWeapon (EquippedItemIndex.inputItemDatabase["Unarmed"], null);
	}

	public override void Sell(){

	}

	public class WeaponPropertyBundle : ItemPropertyBundle{
		public float bonusDamage = 0; 
		public float bonusAttackSpeed = 0;
		public float lightningDamage = 0; 
		public float iceDamage = 0;
		public float fireDamage = 0;
		public float chaosDamage = 0; //rare only
		public float lifeLeech = 0;
		public float manaLeech = 0;
		public int bonusSTR=0;
		public int bonusINT=0;
		public int bonusDEX=0;
		public int quality = 0; //% between 0 and 20, improves baseDamage
		public int spellPower=0;

		//randomized constructors
		//input item rarity + item level
		public WeaponPropertyBundle(ItemPropertyBundle.Rarity rarity, int itemLevel){
			int multiplier = 1;
			this.rarity = rarity;
			quality = Random.Range(0,21); //0-5 normal, 6-10 superior, 10-15 remarkable, 15-20 masterwork

			switch(rarity){
				case Rarity.Common:
					break;
				
				case Rarity.Magic:
					multiplier = 1;
					itemLevel*=multiplier;
					//physical damage and speed
					switch(Random.Range(0,4)){
					case 0:
						bonusDamage = Random.Range(0, 11) * itemLevel;
						break;
					case 1:
						bonusAttackSpeed = Random.Range(0, 0.1f) * itemLevel;
						break;
					}
					//elemental damage
					switch(Random.Range(0,5)){
					case 0:
						lightningDamage = Random.Range(0, 11) * itemLevel;
						break;
					case 1:
						iceDamage = Random.Range(0, 11) * itemLevel;
						break;
					case 2:
						fireDamage = Random.Range(0, 11) * itemLevel;
						break;
					}
					//stat bonuses
					switch(Random.Range(0,5)){
					case 0:
						bonusSTR = Random.Range(0, 4) * itemLevel;
						break;
					case 1:
						bonusINT = Random.Range(0, 4) * itemLevel;
						break;
					case 2:
						bonusDEX = Random.Range(0, 4) * itemLevel;
						break;
					}
					//chaos damage bonuses
					switch(Random.Range(0,5)){
					case 0:
						chaosDamage = Random.Range(0, 4) * itemLevel;
						break;
					}
					//spell power bonuses
					switch(Random.Range(0,4)){
						case 0:
							spellPower = Random.Range(0, 4) * itemLevel;
							break;
					}
					//chaos damage bonuses
					switch(Random.Range(0,5)){
					case 0:
						lifeLeech = Random.Range(0, 4) * itemLevel;
						break;
					case 1:
						manaLeech = Random.Range(0, 4) * itemLevel;
						break;
					}
					break; //end of magic

					case Rarity.Rare:
						multiplier = 2;
						itemLevel*=multiplier;
						//physical damage and speed
						switch(Random.Range(0,3)){
							case 0:
								bonusDamage = Random.Range(0, 11) * itemLevel;
								break;
							case 1:
								bonusAttackSpeed = Random.Range(0, 0.1f) * itemLevel;
								break;
						}
						//elemental damage
						switch(Random.Range(0,4)){
							case 0:
								lightningDamage = Random.Range(0, 11) * itemLevel;
								break;
							case 1:
								iceDamage = Random.Range(0, 11) * itemLevel;
								break;
							case 2:
								fireDamage = Random.Range(0, 11) * itemLevel;
								break;
						}
						//stat bonuses
						switch(Random.Range(0,3)){
							case 0:
								bonusSTR = Random.Range(0, 4) * itemLevel;
								break;
							case 1:
								bonusINT = Random.Range(0, 4) * itemLevel;
								break;
							case 2:
								bonusDEX = Random.Range(0, 4) * itemLevel;
								break;
				}
						//chaos damage bonuses
						switch(Random.Range(0,4)){
						case 0:
							chaosDamage = Random.Range(0, 4) * itemLevel;
							break;
						}
						//spell power bonuses
						switch(Random.Range(0,4)){
						case 0:
							spellPower = Random.Range(0, 4) * itemLevel;
							break;
						}
						//leech bonuses
						switch(Random.Range(0,4)){
							case 0:
								lifeLeech = Random.Range(0, 4) * itemLevel;
								break;
							case 1:
								manaLeech = Random.Range(0, 4) * itemLevel;
								break;
						}
						break; //end of rare
				}
			//make sure we don't have magic items with no attributes...
			if(rarity!=Rarity.Common){
				if(bonusAttackSpeed==0 &&
				   bonusDamage == 0 &&
				   bonusDEX == 0 &&
				   bonusINT == 0 &&
				   bonusSTR == 0 &&
				   iceDamage == 0 &&
				   lightningDamage == 0 &&
				   fireDamage == 0 &&
				   lifeLeech == 0 &&
				   chaosDamage == 0 &&
				   manaLeech == 0){
					Debug.Log ("Made a magic item with no affixes. Giving one random magical property");

					switch(Random.Range(0,7)){
					case 0:
						bonusDamage = Random.Range(1,4) * itemLevel;
						break;
					case 1:
						fireDamage = Random.Range(1, 11) * itemLevel;
						break;
					case 2:
						bonusINT = Random.Range(1, 4) * itemLevel;
						break;
					case 3:
						bonusSTR = Random.Range(1, 4) * itemLevel;
						break;
					case 4:
						bonusDEX = Random.Range(1, 4) * itemLevel;
						break;
					case 5:
						iceDamage = Random.Range(1, 11) * itemLevel;
						break;
					case 6:
						lightningDamage = Random.Range(1, 11) * itemLevel;
						break;
					}

					Debug.Log("Rarity is now" + rarity);
				}
			}
			//calculateValue();
			calculateSuffixesAndPrefixes();
			calculateRequirements();
		}

		
		
		public override void calculateRequirements(){
			requirements = new Requirements ();
			requirements.strReq=0;
			requirements.intReq=0;
			requirements.dexReq=0;

			requirements.dexReq+= (int) (bonusAttackSpeed * 25); //10% attack speed = 2.5 dex?
			
			requirements.intReq += (int) (lightningDamage/5); //100 lightning = 10 dex
			requirements.dexReq += (int) (iceDamage / 5);
			requirements.strReq += (int) (fireDamage / 5);
			
			requirements.strReq += (int) (chaosDamage / 5);
			requirements.intReq += (int) (chaosDamage / 5);
			requirements.dexReq += (int) (chaosDamage / 5);
			
			requirements.strReq += (int) (lifeLeech/2); //amulets, rings
			requirements.intReq += (int) (manaLeech/2);
			
			requirements.intReq += spellPower;
		}

		
		public override void calculateValue(){
			//TODO: FIGURE OUT HOW TO GET BASE ITEM VALUE AS STATIC VAR
			Debug.LogError ("SHOULD CALCULATE VALUE IN STORE OR OUTSIDE OF PROPERTYBUNDLE");
			value = (int) (0 + 
						bonusDamage * 5 + 
						bonusAttackSpeed * 10 +
						(lightningDamage + iceDamage + fireDamage) * 3 + 
						(chaosDamage + lifeLeech + manaLeech) * 10 + 
						(bonusSTR + bonusINT + bonusDEX) * 5);
			}
			
			public void calculateSuffixesAndPrefixes(){
				//quality
				if(quality>=5 && quality < 10){
					prefixes+="Superior ";
				}
				if(quality>=10 && quality < 15){
					prefixes+="Remarkable ";
				}
				if(quality>=15 && quality <= 20){
					prefixes+="Masterwork ";
				}
				
				//str
				if(bonusSTR>1 && bonusSTR <= 10){
					prefixes+="Hardy ";
				}
				if(bonusSTR>10 && bonusSTR <= 20){
					prefixes+="Bold ";
				}
				if(bonusSTR>20 && bonusSTR <= 40){
					prefixes+="Vigorous ";
				}
				if(bonusSTR>40){
					prefixes+="Indomitable ";
				}		
				
				//int
				if(bonusINT>1 && bonusINT <= 10){
					prefixes+="Astute ";
				}
				if(bonusINT>10 && bonusINT <= 20){
					prefixes+="Perceptive ";
				}
				if(bonusINT>20 && bonusINT <= 40){
					prefixes+="Wise ";
				}
				if(bonusINT>40){
					prefixes+="Brilliant";
				}		
				
				//dex
				if(bonusDEX>1 && bonusDEX <= 10){
					prefixes+="Agile ";
				}
				if(bonusDEX>10 && bonusDEX <= 20){
					prefixes+="Focused ";
				}
				if(bonusDEX>20 && bonusDEX <= 40){
					prefixes+="Balanced ";
				}
				if(bonusDEX>40){
					prefixes+="Mercurial ";
				}
				
				//bonus damage
				if (bonusDamage > 0 && bonusDamage <= 10) {
					suffixes+=" of Force";
				}
				if (bonusDamage > 10 && bonusDamage <= 40) {
					suffixes+=" of Ruin";
				}
				if (bonusDamage > 40 && bonusDamage <= 80) {
					suffixes+=" of Power";
				}
				if (bonusDamage > 80 && bonusDamage <= 140) {
					suffixes+=" of Might";
				}
				if (bonusDamage > 140) {
					suffixes+=" of Domination";
				}
				
				//attackspeed
				if(bonusAttackSpeed>0 && bonusAttackSpeed<=.2f){
					prefixes+="Nimble ";
				}
				if(bonusAttackSpeed>0.2f && bonusAttackSpeed<=.5f){
					prefixes+="Swift ";
				}
				if(bonusAttackSpeed>0.5f && bonusAttackSpeed<=.9f){
					prefixes+="Fleet ";
				}
				if(bonusAttackSpeed>0.9 && bonusAttackSpeed<=1.5f){
					prefixes+="Flying ";
				}
				if(bonusAttackSpeed>1.5f){
					prefixes+="Quicksilver ";
				}
				
				//lightnig damage
				if (lightningDamage > 0 && lightningDamage <= 10) {
					prefixes+="Sparking ";
				}
				if (lightningDamage > 10 && lightningDamage <= 40) {
					prefixes+="Shocking ";
				}
				if (lightningDamage > 40 && lightningDamage <= 80) {
					prefixes+="Crackling ";
				}
				if (lightningDamage > 80 && lightningDamage <= 140) {
					prefixes+="Arcing ";
				}
				if (lightningDamage > 140) {
					prefixes+="Electrocuting ";
				}
				
				//fire damage
				if (fireDamage > 0 && fireDamage <= 10) {
					prefixes+="Smoldering ";
				}
				if (fireDamage > 10 && fireDamage <= 40) {
					prefixes+="Burning ";
				}
				if (fireDamage > 40 && fireDamage <= 80) {
					prefixes+="Flaming ";
				}
				if (fireDamage > 80 && fireDamage <= 140) {
					prefixes+="Blasting ";
				}
				if (fireDamage > 140) {
					prefixes+="Cremating ";
				}
				
				//ice damage
				if (iceDamage > 0 && iceDamage <= 10) {
					prefixes+="Chilling ";
				}
				if (iceDamage > 10 && iceDamage <= 40) {
					prefixes+="Icy ";
				}
				if (iceDamage > 40 && iceDamage <= 80) {
					prefixes+="Frozen ";
				}
				if (iceDamage > 80 && iceDamage <= 140) {
					prefixes+="Arctic ";
				}
				if (iceDamage > 140) {
					prefixes+="Glacial ";
				}
				
				//chaos damage
				if (chaosDamage > 0 && chaosDamage <= 10) {
					prefixes+="Discord ";
				}
				if (chaosDamage > 10 && chaosDamage <= 40) {
					prefixes+="Madness ";
				}
				if (chaosDamage > 40 && chaosDamage <= 80) {
					prefixes+="Chaos ";
				}
				if (chaosDamage > 80 && chaosDamage <= 140) {
					prefixes+="Turmoil ";
				}
				if (chaosDamage > 140) {
					prefixes+="Pandemonium ";
				}
				
				//life steal
				if (lifeLeech > 0 && lifeLeech <= 10) {
					prefixes+="Hateful ";
				}
				if (lifeLeech > 10 && lifeLeech <= 40) {
					prefixes+="Vicious ";
				}
				if (lifeLeech > 40 && lifeLeech <= 80) {
					prefixes+="Despicable ";
				}
				if (lifeLeech > 80 && lifeLeech <= 140) {
					prefixes+="Vile ";
				}
				if (lifeLeech > 140) {
					prefixes+="Wretched ";
				}
				
				//mana steal
				if (manaLeech > 0 && manaLeech <= 10) {
					prefixes+="Sapping ";
				}
				if (manaLeech > 10 && manaLeech <= 40) {
					prefixes+="Draining ";
				}
				if (manaLeech > 40 && manaLeech <= 80) {
					prefixes+="Tapping ";
				}
				if (manaLeech > 80 && manaLeech <= 140) {
					prefixes+="Siphoning ";
				}
				if (manaLeech > 140) {
					prefixes+="Consuming ";
				}
			
				//spell power
				if (spellPower > 0 && spellPower <= 10) {
					prefixes+="Runed ";
				}
				if (spellPower > 10 && spellPower <= 40) {
					prefixes+="Glowing ";
				}
				if (spellPower > 40 && spellPower <= 80) {
					prefixes+="Divine ";
				}
				if (spellPower > 80 && spellPower <= 140) {
					prefixes+="Arcane ";
				}
				if (spellPower > 140) {
					prefixes+="Cosmic ";
				}
			}
		}
}
