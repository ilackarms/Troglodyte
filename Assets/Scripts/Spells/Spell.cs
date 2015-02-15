using UnityEngine;
using System.Collections;


abstract public class Spell {
	public string name;
	public int level;
	public string description;
	public string circle;
	public int manaCost;
	public float coolDown;
	public string iconPath;
	
	public bool onCoolDown = false;
	
	public Spell(string name, int level, string description, string circle, int manaCost, float coolDown, string iconPath){
		this.name = name;
		this.level = level;
		this.description = description;
		this.circle = circle;
		this.manaCost = manaCost;
		this.coolDown = coolDown;
		this.iconPath = iconPath;
	}

	
	//called all the time by spellSystem
	public void update(){
		if (coolDown <= 0)
			coolDown = 0;
		if (coolDown >= 0) {
			coolDown-=Time.deltaTime;
		}
	}
	
	abstract public void cast ();
	abstract public void notifyCollision(RaycastHit hit);
	abstract public bool canCast ();
}

public class FlameArrow : Spell {
	int damage;
	int speed;

	//level 1 spell
	public FlameArrow() : base("Flame Arrow", 1, "Fires an explosive missile in a straight line", "Fire", 3 * 1, 3, "SpellIcons/FireArrows1"){
		//anything else we need?
	}

	override public void cast(){
		GameObject player = Global.combatSystem.gameObject;
		GameObject target = new GameObject ();
		target.transform.position = player.transform.position + Global.arms.transform.forward * 1000; //just keep going straight ya dingus
		//set these every time we cast because level can change
		damage = 5 * level;
		speed = 6 + level;
		CombatSystem.Statistics.CurrentMana -= manaCost;
		EffectSettings fireArrowSettings = SpellParticleFactory.InstantiateParticleFromPrefab (this, "Prefabs/Spells/FireArrow", player, target).GetComponent<EffectSettings>();
		fireArrowSettings.ColliderRadius = 0.2f;
		fireArrowSettings.EffectRadius = 0;
		fireArrowSettings.MoveSpeed = speed; //overwritten later
		fireArrowSettings.MoveDistance = 20;
		fireArrowSettings.DeactivateAfterCollision = true;
		fireArrowSettings.DeactivateTimeDelay = 3;
		fireArrowSettings.IsHomingMove=false;
		fireArrowSettings.MoveSpeed = speed;
	}

	override public void notifyCollision(RaycastHit hit){
		Debug.Log ("Hit!!!!!");
		if (!(hit.transform == null)) {
			hit.transform.SendMessage ("GetHit", damage, SendMessageOptions.DontRequireReceiver);
		}
	}

	override public bool canCast(){
		manaCost = 3 * level;
		return (CombatSystem.Statistics.CurrentMana >= this.manaCost);
	}
}