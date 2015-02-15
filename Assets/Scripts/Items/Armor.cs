using UnityEngine;
using System.Collections;

public class Armor : Item
{
	public enum ArmorType{
		Headgear, Chest, Gloves, Ring, Amulet, Boots, Belt
	}

	public float baseArmor;	
	public ArmorType armorType;
	public ArmorPropertyBundle armorPropertyBundle;
		
	public override void performAction(){
		
	}
	
	public override void calculateDescription(){
		description = "";
		armorPropertyBundle = (ArmorPropertyBundle) propertyBundle;
		description += "Base Armor: " + (baseArmor)+"\n";
		if (armorPropertyBundle.quality > 0) {
			description += "Bonus Item Quality: "+(armorPropertyBundle.quality)+ "%\n";
		}
		if (armorPropertyBundle.bonusArmor > 0) {
			description += "Armor increased by "+(armorPropertyBundle.bonusArmor)+"\n";
		}
		if (armorPropertyBundle.bonusAttackSpeed > 0) {
			description += "Attack Speed increased by "+ ((int) (armorPropertyBundle.bonusAttackSpeed*100))+"%"+"\n";
		}
		if (armorPropertyBundle.bonusSTR > 0) {
			description += "Strength increased by "+ (armorPropertyBundle.bonusSTR)+"\n";
		}
		if (armorPropertyBundle.bonusINT > 0) {
			description += "Intelligence increased by "+ (armorPropertyBundle.bonusINT)+"\n";
		}
		if (armorPropertyBundle.bonusDEX > 0) {
			description += "Dexterity increased by "+ (armorPropertyBundle.bonusDEX)+"\n";
		}
		if (armorPropertyBundle.fireResist > 0) {
			description += "Fire Resistance: "+(armorPropertyBundle.fireResist)+ "\n";
		}
		if (armorPropertyBundle.iceResist > 0) {
			description += "Ice Resistance: "+(armorPropertyBundle.iceResist)+"\n";
		}
		if (armorPropertyBundle.lightningResist > 0) {
			description += "Lightning Resistance: "+(armorPropertyBundle.lightningResist) + "\n";
		}
		if (armorPropertyBundle.chaosResist > 0) {
			description += "Chaos Resistance: "+(armorPropertyBundle.chaosResist) + "\n";
		}
		if (armorPropertyBundle.spellPower > 0) {
			description += "Spell Power: "+(armorPropertyBundle.spellPower) + "\n";
		}
		if (armorPropertyBundle.healthRegen > 0) {
			description += "Life Regeneration: "+(armorPropertyBundle.healthRegen) + "\n";
		}
		if (armorPropertyBundle.manaRegen > 0) {
			description += "Mana Regeneration: "+(armorPropertyBundle.manaRegen) + "\n";
		}
		if (armorPropertyBundle.bonusHealth > 0) {
			description += "Health increased by "+(armorPropertyBundle.bonusHealth) + "\n";
		}
		if (armorPropertyBundle.bonusMana > 0) {
			description += "Mana increased by "+(armorPropertyBundle.bonusMana) + "\n";
		}
	}

	//random 
	public Armor(int width, int height, Texture2D image, string name, float baseArmor, ArmorType armorType){
		this.width = width;
		this.height = height;
		this.image = image;
		this.name = name;
		this.baseArmor = baseArmor;
		this.armorType = armorType;
		
		//only for equippable

		//no equipped model for anything other than weapons and rings! :D
		//TODO: maybe add particle effects later
		if(armorType == ArmorType.Ring || armorType == ArmorType.Belt || armorType == ArmorType.Gloves){ //TODO: make models using teh hands that already exist in blender
																			//USE SAME ANIMATIONS
			Debug.Log (name + " getting equippedmodel");
			equippedModel = Global.equippedItemIndex.getEquippedModel (name);
		}
	}
	
	public override void Equip(){
		//Global.combatSystem.notifyEquippedArmor (this);
	}

	//always call UNEQUIP before EQUIP
	public override void Unequip(){
		//Global.combatSystem.notifyEquippedWeapon (null, null); //TODO Create an Unarmed weapon!
	}
	
	public override void Sell(){
		
	}

	public class ArmorPropertyBundle : ItemPropertyBundle{
		public float bonusArmor = 0; 
		public float bonusMoveSpeed = 0; //BOOTS ONLY
		public float bonusAttackSpeed = 0; //GLOVES ONLY
		public float lightningResist = 0; 
		public float iceResist = 0;
		public float fireResist = 0;
		public float chaosResist = 0; //rare only / amulet only
		public float healthRegen = 0; //amulets, rings
		public float manaRegen = 0;
		public int bonusHealth=0;
		public int bonusMana=0;
		public int bonusDEX=0;
		public int bonusINT=0;
		public int bonusSTR=0;
		public int quality = 0; //% between 0 and 20, improves baseDamage
		public int spellPower = 0;

		//randomized constructors
		//input item rarity + item level
		//the constructor does not set this property but the item factory does
		public ArmorPropertyBundle(ItemPropertyBundle.Rarity rarity, int itemLevel, ArmorType armorType){
			int multiplier = 1;
			this.rarity = rarity;
			quality = Random.Range(0,21); //0-5 normal, 6-10 superior, 10-15 remarkable, 15-20 masterwork

			switch(rarity){
				case Rarity.Common:
					break;

				//BEGIN MAGIC ITEM GENERATION
				case Rarity.Magic:
				multiplier = 1;
				itemLevel*=multiplier;
				//physical armor and speed
				switch(Random.Range(0,5)){
				case 0:
					if(armorType == ArmorType.Headgear || armorType == ArmorType.Chest || armorType == ArmorType.Gloves ||  armorType == ArmorType.Boots)
						bonusArmor = Random.Range(0, 11) * itemLevel;
					break;
				case 1:
					if(armorType == ArmorType.Ring || armorType == ArmorType.Belt || armorType == ArmorType.Amulet || armorType == ArmorType.Gloves)
						bonusAttackSpeed = Random.Range(0, 0.1f) * itemLevel;
					break;
				case 2:
					if(armorType == ArmorType.Ring || armorType == ArmorType.Belt || armorType == ArmorType.Amulet || armorType == ArmorType.Boots)
						bonusMoveSpeed = Random.Range(0, 0.01f) * itemLevel; 
					break;
				}
				//elemental resistance
				switch(Random.Range(0,5)){
				case 0:
					lightningResist = Random.Range(0, 11) * itemLevel;
					break;
				case 1:
					iceResist = Random.Range(0, 16) * itemLevel;
					break;
				case 2:
					fireResist = Random.Range(0, 16) * itemLevel;
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
				//chaos resist
				switch(Random.Range(0,5)){
				case 0:
					if(armorType == ArmorType.Ring || armorType == ArmorType.Amulet)
						chaosResist = Random.Range(0, 4) * itemLevel;
					break;
				case 1:
					if(armorType == ArmorType.Ring || armorType == ArmorType.Amulet)
						spellPower = Random.Range(0, 4) * itemLevel;
					break;
				}
				//bonus health/mana
				switch(Random.Range(0,5)){
				case 0:
					if(armorType == ArmorType.Ring || armorType == ArmorType.Belt || armorType == ArmorType.Amulet || armorType == ArmorType.Chest || armorType == ArmorType.Headgear )
						bonusHealth = Random.Range(0, 11) * itemLevel;
					break;
				case 1:
					if(armorType == ArmorType.Ring || armorType == ArmorType.Belt || armorType == ArmorType.Amulet || armorType == ArmorType.Chest || armorType == ArmorType.Headgear )
						bonusMana = Random.Range(0, 11) * itemLevel;
					break;
				}
				//health/mana regen
				switch(Random.Range(0,5)) {
				case 0:
					if(armorType == ArmorType.Ring || armorType == ArmorType.Belt || armorType == ArmorType.Amulet || armorType == ArmorType.Chest || armorType == ArmorType.Headgear )
						healthRegen = Random.Range(0, 4) * itemLevel;
					break;
				case 1:
					if(armorType == ArmorType.Ring || armorType == ArmorType.Belt || armorType == ArmorType.Amulet || armorType == ArmorType.Chest || armorType == ArmorType.Headgear )
						manaRegen = Random.Range(0, 4) * itemLevel;
					break;
				}
				break; //end of magic	



				/*
				 * BEGIN RARE ITEM GENERATION
				 */
				case Rarity.Rare:
					multiplier = 2;
					itemLevel*=multiplier;
					//physical armor and speed
					//always give bonus armor for rare item
					if(armorType == ArmorType.Headgear || armorType == ArmorType.Chest || armorType == ArmorType.Gloves ||  armorType == ArmorType.Boots)
						bonusArmor = Random.Range(0, 11) * itemLevel;

					switch(Random.Range(0,3)){
					case 0:
						if(armorType == ArmorType.Ring || armorType == ArmorType.Belt || armorType == ArmorType.Amulet || armorType == ArmorType.Gloves)
							bonusAttackSpeed = Random.Range(0, 0.1f) * itemLevel;
						break;
					case 1:
						if(armorType == ArmorType.Ring || armorType == ArmorType.Belt || armorType == ArmorType.Amulet || armorType == ArmorType.Boots)
							bonusMoveSpeed = Random.Range(0, 0.01f) * itemLevel; 
						break;
					}
					//elemental resistance //GROUPS OF TWO
					switch(Random.Range(0,4)){
					case 0:
						lightningResist = Random.Range(0, 11) * itemLevel;
						iceResist = Random.Range(0, 16) * itemLevel;
						break;
					case 1:
						iceResist = Random.Range(0, 16) * itemLevel;
						fireResist = Random.Range(0, 16) * itemLevel;
						break;
					case 2:
						lightningResist = Random.Range(0, 11) * itemLevel;
						fireResist = Random.Range(0, 16) * itemLevel;
						break;
					}
					//stat bonuses
					switch(Random.Range(0,4)){
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
					//chaos resist & spell power
					switch(Random.Range(0,5)){
					case 0:
						if(armorType == ArmorType.Ring || armorType == ArmorType.Belt || armorType == ArmorType.Amulet)
							chaosResist = Random.Range(0, 4) * itemLevel;
						break;
					case 1:
						if(armorType == ArmorType.Ring || armorType == ArmorType.Amulet)
							spellPower = Random.Range(0, 4) * itemLevel;
						break;
					}
					//bonus health/mana
					switch(Random.Range(0,4)){
					case 0:
						if(armorType == ArmorType.Ring || armorType == ArmorType.Belt || armorType == ArmorType.Amulet || armorType == ArmorType.Chest || armorType == ArmorType.Headgear )
							bonusHealth = Random.Range(0, 11) * itemLevel;
						break;
					case 1:
						if(armorType == ArmorType.Ring || armorType == ArmorType.Belt || armorType == ArmorType.Amulet || armorType == ArmorType.Chest || armorType == ArmorType.Headgear )
							bonusMana = Random.Range(0, 11) * itemLevel;
						break;
					}
					//health/mana regen
					switch(Random.Range(0,4)) {
					case 0:
						if(armorType == ArmorType.Ring || armorType == ArmorType.Belt || armorType == ArmorType.Amulet || armorType == ArmorType.Chest || armorType == ArmorType.Headgear )
							healthRegen = Random.Range(0, 4) * itemLevel;
						break;
					case 1:
						if(armorType == ArmorType.Ring || armorType == ArmorType.Belt || armorType == ArmorType.Amulet || armorType == ArmorType.Chest || armorType == ArmorType.Headgear )
							manaRegen = Random.Range(0, 4) * itemLevel;
						break;
					}
				break; //end of rare
			}
			//make sure we don't have magic items with no attributes...
			if(rarity!=Rarity.Common){
				if(bonusAttackSpeed==0 &&
				   bonusArmor == 0 &&
				   bonusMoveSpeed == 0 &&
				   bonusDEX == 0 &&
				   bonusINT == 0 &&
				   bonusSTR == 0 &&
				   iceResist == 0 &&
				   lightningResist == 0 &&
				   fireResist == 0 &&
				   healthRegen == 0 &&
				   manaRegen == 0 &&
				   chaosResist == 0 &&
				   spellPower == 0 &&
				   bonusHealth == 0&&
				   bonusMana == 0){
					Debug.Log ("Made a magic item with no affixes. Giving one random magical property");

					switch(Random.Range(0,7)){
					case 0:
						bonusArmor = Random.Range(1,11) * itemLevel;
						break;
					case 1:
						bonusHealth = Random.Range(1, 11) * itemLevel;
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
						iceResist = Random.Range(1, 11) * itemLevel;
						break;
					case 6:
						fireResist = Random.Range(1, 11) * itemLevel;
						break;
					}

					Debug.Log("Rarity is now" + rarity);
				}
			}

			calculateSuffixesAndPrefixes();
			calculateRequirements();
		}

		public override void calculateRequirements(){
			requirements = new Requirements ();
			requirements.strReq=0;
			requirements.intReq=0;
			requirements.dexReq=0;

			requirements.dexReq+=(int) (bonusMoveSpeed* 100); //12% movespeed = 12 dex

			requirements.dexReq+=(int) (bonusAttackSpeed * 25); //10% attack speed = 2.5 dex?

			requirements.intReq += (int) (lightningResist/10); //100 lightning = 10 dex
			requirements.dexReq += (int) (iceResist / 10);
			requirements.strReq += (int) (fireResist / 10);

			requirements.strReq += (int) (chaosResist / 10);
			requirements.intReq += (int) (chaosResist / 10);
			requirements.dexReq += (int) (chaosResist / 10);

			requirements.strReq += (int) (healthRegen/2); //amulets, rings
			requirements.intReq += (int) (manaRegen/2);

			requirements.strReq += bonusHealth/10;
			requirements.intReq += bonusMana/10;

			requirements.intReq += spellPower;
		}
		
		
		public override void calculateValue(){
			//TODO: FIGURE OUT HOW TO GET BASE ITEM VALUE AS STATIC VAR
			Debug.LogError ("SHOULD CALCULATE VALUE IN STORE OR OUTSIDE OF PROPERTYBUNDLE");
			value = 0;
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
			
			//bonus armor
			if (bonusArmor > 0 && bonusArmor <= 10) {
				prefixes+="Hard ";
			}
			if (bonusArmor > 10 && bonusArmor <= 40) {
				prefixes+="Heavy ";
			}
			if (bonusArmor > 40 && bonusArmor <= 80) {
				prefixes+="Fortified ";
			}
			if (bonusArmor > 80 && bonusArmor <= 140) {
				prefixes+="Plated ";
			}
			if (bonusArmor > 140) {
				prefixes+="Diamond ";
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
			
			//lightnig resist
			if (lightningResist > 0 && lightningResist <= 10) {
				prefixes+="Grounded ";
			}
			if (lightningResist > 10 && lightningResist <= 40) {
				prefixes+="Static ";
			}
			if (lightningResist > 40 && lightningResist <= 80) {
				prefixes+="Charged ";
			}
			if (lightningResist > 80 && lightningResist <= 140) {
				prefixes+="Infused ";
			}
			if (lightningResist > 140) {
				prefixes+="Voltaic ";
			}
			
			//fire resist
			if (fireResist > 0 && fireResist <= 10) {
				prefixes+="Ember ";
			}
			if (fireResist > 10 && fireResist <= 40) {
				prefixes+="Coal ";
			}
			if (fireResist > 40 && fireResist <= 80) {
				prefixes+="Charred ";
			}
			if (fireResist > 80 && fireResist <= 140) {
				prefixes+="Burned ";
			}
			if (fireResist > 140) {
				prefixes+="Scorched ";
			}
			
			//ice resist
			if (iceResist > 0 && iceResist <= 10) {
				prefixes+="Frosted ";
			}
			if (iceResist > 10 && iceResist <= 40) {
				prefixes+="Frozen ";
			}
			if (iceResist > 40 && iceResist <= 80) {
				prefixes+="Icy ";
			}
			if (iceResist > 80 && iceResist <= 140) {
				prefixes+="Arctic ";
			}
			if (iceResist > 140) {
				prefixes+="Glacial ";
			}
			
			//chaos resist
			if (chaosResist > 0 && chaosResist <= 10) {
				prefixes+="Calming ";
			}
			if (chaosResist > 10 && chaosResist <= 40) {
				prefixes+="Soothing ";
			}
			if (chaosResist > 40 && chaosResist <= 80) {
				prefixes+="Serene ";
			}
			if (chaosResist > 80 && chaosResist <= 140) {
				prefixes+="Tranquil ";
			}
			if (chaosResist > 140) {
				prefixes+="Harmonious ";
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
			
			//life 
			if (bonusHealth > 0 && bonusHealth <= 10) {
				prefixes+="Robust ";
			}
			if (bonusHealth > 10 && bonusHealth <= 40) {
				prefixes+="Firm ";
			}
			if (bonusHealth > 40 && bonusHealth <= 80) {
				prefixes+="Potent ";
			}
			if (bonusHealth > 80 && bonusHealth <= 140) {
				prefixes+="Virile ";
			}
			if (bonusHealth > 140) {
				prefixes+="Flourishing ";
			}
			
			//mana 
			if (bonusMana > 0 && bonusMana <= 10) {
				prefixes+="Thoughtful ";
			}
			if (bonusMana > 10 && bonusMana <= 40) {
				prefixes+="Deep ";
			}
			if (bonusMana > 40 && bonusMana <= 80) {
				prefixes+="Knowing ";
			}
			if (bonusMana > 80 && bonusMana <= 140) {
				prefixes+="Vivid ";
			}
			if (bonusMana > 140) {
				prefixes+="Vast ";
			}
			
			//life regen
			if (healthRegen > 0 && healthRegen <= 10) {
				suffixes+=" of Restoration";
			}
			if (healthRegen > 10 && healthRegen <= 40) {
				suffixes+=" of Healing";
			}
			if (healthRegen > 40 && healthRegen <= 80) {
				suffixes+=" of Regeneration";
			}
			if (healthRegen > 80 && healthRegen <= 140) {
				suffixes+=" of Troll's Blood";
			}
			if (bonusHealth > 140) {
				suffixes+=" of the Phoenix";
			}
			
			//mana regen
			if (manaRegen > 0 && manaRegen <= 10) {
				prefixes+="Therapeutic ";
			}
			if (manaRegen > 10 && manaRegen <= 40) {
				prefixes+="Restorative ";
			}
			if (manaRegen > 40 && manaRegen <= 80) {
				prefixes+="Introspective ";
			}
			if (bonusMana > 80 && bonusMana <= 140) {
				prefixes+="Reflective ";
			}
			if (manaRegen > 140) {
				prefixes+="Meditative ";
			}
		}
	}
	
	
	
	
}

