using System;
using UnityEngine;
public class Potion : Item, Consumable
{
	public static float POTION_DURATION = 5;
	
	public enum PotionType{
		Healing, Mana, Rejuvination, Buff
	}

	public PotionType potionType;

	public Potion(int width, int height, Texture2D image, string name,  PotionType potionType){
		this.width = width;
		this.height = height;
		this.image = image;
		this.name = name;
		this.potionType = potionType;
	}

	public void Consume(){
		Debug.Log ("Consumed!");
		//Global.combatSystem
		/*
		if (potionType == PotionType.Healing) {
			Global.combatSystem.notifyHealthPotion(((PotionPropertyBundle)propertyBundle).healthRestored);
		}
		if(potionType == PotionType.Mana){
			Global.combatSystem.notifyManaPotion(((PotionPropertyBundle)propertyBundle).manaRestored);
		}
		if(potionType == PotionType.Buff){
			CombatSystem.Statistics.notifyBuff(new Buff((PotionPropertyBundle)propertyBundle, POTION_DURATION));
		}*/
		CombatSystem.Statistics.notifyBuff(new Buff((PotionPropertyBundle)propertyBundle, POTION_DURATION));
	}

	public override void performAction(){
		Consume ();
	}
	
	public override void Equip(){
		return;
	}
	
	public override void Unequip(){
		return;
	}
	
	public override void Sell(){
		return;
	}

	public override void calculateDescription ()
	{
		description = ((PotionPropertyBundle)propertyBundle).description;
	}

	public class PotionPropertyBundle : ItemPropertyBundle{
		public int healthRestored=0;
		public int manaRestored = 0;
		public int damageBuff=0;
		public float attackSpeedBuff = 0;
		public int movementSpeedBuff = 0;
		public float armorBuff = 0;
		public int fireResistBuff = 0;
		public int iceResistBuff = 0;
		public int lightningResistBuff = 0;
		public int chaosResistBuff = 0;

		public string description="";


		public PotionPropertyBundle(int itemLevel, PotionType potionType){
			prefixes="";
			requirements = new Requirements();
			requirements.dexReq=0;
			requirements.strReq=0;
			requirements.intReq=0;
			switch(itemLevel){
				case 1:
					prefixes="Feeble ";
					break;
				case 2:
					prefixes="Minor ";
					break;
				case 3:
					prefixes="Lesser ";
					break;
				case 4:
					prefixes=""; //normal
					break;
				case 5:
					prefixes="Potent ";
					break;
				case 6:
					prefixes="Greater ";
					break;
				case 7:
					prefixes="Incredible ";
					break;
				//todo: add more
			}
			rarity = Rarity.Common;
			if(potionType == PotionType.Healing){
				healthRestored=10 * itemLevel;
				description+="Health Restored: "+healthRestored+"\n";
			}
			if(potionType == PotionType.Mana){
				manaRestored=10 * itemLevel;
				description+="Mana Restored: "+manaRestored+"\n";
			}
			//call buff potions "Potion"
			if(potionType == PotionType.Buff){
				switch(UnityEngine.Random.Range(0,6)){
				case 0:
					attackSpeedBuff = UnityEngine.Random.Range(0, 0.1f) * itemLevel;
					prefixes+="Lightning Strike ";
					description+="Attack Speed Increased By: "+attackSpeedBuff+"\n";
					break;
				case 1:
					damageBuff = UnityEngine.Random.Range(1, 10) * itemLevel;
					attackSpeedBuff = UnityEngine.Random.Range(0, 0.1f) * itemLevel;
					prefixes+="Assasssin's ";
					description+="Attack Speed Increased By: "+attackSpeedBuff+"\n";
					description+="Attack Damage Increased By: "+damageBuff+"\n";
					break;
				case 2:
					damageBuff = UnityEngine.Random.Range(1, 10) * itemLevel;
					armorBuff = UnityEngine.Random.Range(1, 10) * itemLevel;
					prefixes+="Brute's ";
					description+="Attack Damage Increased By: "+damageBuff+"\n";
					description+="Armor Increased By: "+armorBuff+"\n";
					break;
				case 3:
					armorBuff = UnityEngine.Random.Range(1, 10) * itemLevel;
					description+="Armor Increased By: "+armorBuff+"\n";
					switch(UnityEngine.Random.Range(0,3)){
					case 0:
						fireResistBuff = UnityEngine.Random.Range(1, 10) * itemLevel;
						description+="Fire Resistance Increased By: "+armorBuff+"\n";
						iceResistBuff = UnityEngine.Random.Range(1, 10) * itemLevel;
						description+="Ice Resistance Increased By: "+lightningResistBuff+"\n";
						break;
					case 1:
						iceResistBuff = UnityEngine.Random.Range(1, 10) * itemLevel;
						description+="Ice Resistance Increased By: "+armorBuff+"\n";
						lightningResistBuff = UnityEngine.Random.Range(1, 10) * itemLevel;
						description+="Lightning Resistance Increased By: "+lightningResistBuff+"\n";
						break;
					case 2:
						fireResistBuff = UnityEngine.Random.Range(1, 10) * itemLevel;
						description+="Fire Resistance Increased By: "+armorBuff+"\n";
						lightningResistBuff = UnityEngine.Random.Range(1, 10) * itemLevel;
						description+="Lightning Resistance Increased By: "+lightningResistBuff+"\n";
						break;
					}
					prefixes+="Dwarf's ";
					break;
				case 4:
					fireResistBuff = UnityEngine.Random.Range(1, 10) * itemLevel;
					description+="Fire Resistance Increased By: "+armorBuff+"\n";
					iceResistBuff = UnityEngine.Random.Range(1, 10) * itemLevel;
					description+="Ice Resistance Increased By: "+armorBuff+"\n";
					lightningResistBuff = UnityEngine.Random.Range(1, 10) * itemLevel;
					description+="Lightning Resistance Increased By: "+lightningResistBuff+"\n";
					prefixes+="Prismatic ";
					break;
					
				case 5:
					chaosResistBuff = UnityEngine.Random.Range(1, 5) * itemLevel;
					description+="Chaos Resistance Increased By: "+chaosResistBuff+"\n";
					prefixes+="Divine Fortitude ";
					break;
				}
			}
		}

		//potions have no requirements
		public override void calculateRequirements ()
		{
			requirements.strReq = 0;
			requirements.intReq = 0;
			requirements.dexReq = 0;
		}

		public override void calculateValue ()
		{
			throw new NotImplementedException ();
		}
	}
}

