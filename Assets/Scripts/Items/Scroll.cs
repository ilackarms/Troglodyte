using UnityEngine;
using System.Collections;

public class Scroll : Item, Consumable
{
	public Spell spell;

	public Scroll(Spell spell){
		this.width = 1;
		this.height = 1;
		this.image = (Texture2D) Resources.Load("Sprites/Scrolls/Scroll");
		this.name = "Scroll of "+ spell.name;
		this.spell = spell;
		description = spell.description;
		propertyBundle = new ScrollPropertyBundle ();
		((ScrollPropertyBundle)propertyBundle).spell = spell;
		((ScrollPropertyBundle)propertyBundle).requirements = new Requirements ();
		propertyBundle.requirements.intReq = spell.level * SpellSystem.INT_PER_SPELL_LEVEL;
	}

	public void Consume(){
		Debug.Log ("Spell learned!!");
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
		description = spell.description;
	}
	public class ScrollPropertyBundle : ItemPropertyBundle {
		//potions have no requirements
		public Spell spell;
		public override void calculateRequirements ()
		{
			requirements.intReq = spell.level * SpellSystem.INT_PER_SPELL_LEVEL;
		}
		
		public override void calculateValue ()
		{
			throw new UnityException ();
		}

	}
}

