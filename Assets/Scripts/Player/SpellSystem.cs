using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellSystem : MonoBehaviour
{
	public static int INT_PER_SPELL_LEVEL = 10;
	private GUITestScript spellGUI;

	private Dictionary<string, Spell> knownSpells;
	private Dictionary<string, Spell> spellDatabase; //base spells, all level one...

	private Spell fSpell=null, gSpell=null, vSpell=null, bSpell=null;
	
	void OpenOrCloseGUI(){		
		if(Input.GetButtonDown("SpellGUI") && !spellGUI.doWindow2){
			spellGUI.doWindow2=true;
			Screen.lockCursor = false;
			Time.timeScale = 0;
			Global.OpenGUICount++;
			
			setCurrentSpells();
		}	
		else if(Input.GetButtonDown("SpellGUI") && spellGUI.doWindow2){
			spellGUI.doWindow2=false;
			Global.OpenGUICount--;
			if (Global.OpenGUICount<=0){
				Global.OpenGUICount=0;
				Screen.lockCursor = true;
				Time.timeScale = 1;
			}
			
			setCurrentSpells();
		}
		if((Input.GetButtonDown("CloseGUI") && spellGUI.doWindow2)){
			spellGUI.doWindow2=false;
			Screen.lockCursor = Screen.lockCursor^true;
			Time.timeScale = (spellGUI.doWindow2) ? 0 : 1;
			Debug.Log(Global.OpenGUICount);

			setCurrentSpells();
		}
	}

	void setCurrentSpells(){
		string spellname1 = ((string) spellGUI.fSpell.name);
		string spellname2 = ((string) spellGUI.gSpell.name);
		string spellname3 = ((string) spellGUI.vSpell.name);
		string spellname4 = ((string) spellGUI.bSpell.name);
		//set fgvb spels
		fSpell = knownSpells[spellname1];
		gSpell = knownSpells[spellname2];
		vSpell = knownSpells[spellname3];
		bSpell = knownSpells[spellname4];
	}
	
	void Awake(){
		spellGUI = this.GetComponentInChildren<GUITestScript> ();
		knownSpells = new Dictionary<string, Spell> ();
	}

	// Use this for initialization
	void Start ()
	{
		spellGUI.doWindow2 = false;
		knownSpells.Add ("", null);
	}

	// Update is called once per frame
	void Update ()
	{
		foreach (Spell spell in knownSpells.Values) {
			if(spell!=null) spell.update();
		}
		OpenOrCloseGUI ();
		checkSpellInputs ();
	}

	void checkSpellInputs(){
		if(Input.GetButtonDown("Spell1")){
			Debug.Log("CAST SPELL 1!!!!"+fSpell);
			if(fSpell!=null && fSpell.canCast()) fSpell.cast();
		}
		if(Input.GetButtonDown("Spell2")){
			if(gSpell!=null && gSpell.canCast()) gSpell.cast();
		}
		if(Input.GetButtonDown("Spell3")){
			if(vSpell!=null && vSpell.canCast()) vSpell.cast();
		}
		if(Input.GetButtonDown("Spell4")){
			if(bSpell!=null && bSpell.canCast()) bSpell.cast();
		}
	}

	//learn spell for first time: base level
	//learn spell for 2nd time: add +1 to level
	//require 10 int per level!
	public bool learnSpell(Spell spell){
		if (knownSpells.ContainsKey (spell.name)) {
			if((knownSpells[spell.name].level+1)*INT_PER_SPELL_LEVEL <= CombatSystem.Statistics.TotalINT){
				knownSpells[spell.name].level+=1;
				spellGUI.incrementSpellLevel(spell.name);
				return true;
			}
			else{ 
				Debug.Log("Failed to learn spell");
			     return false;
			}
		}
		else{
			if(spell.level*INT_PER_SPELL_LEVEL <= CombatSystem.Statistics.TotalINT){
				knownSpells.Add(spell.name, spell);
				spellGUI.queuedSpellName = spell.name;
				Debug.Log("QUeued spell name "+spellGUI.queuedSpellName);
				spellGUI.queuedSpellStats = spell.level.ToString();
				spellGUI.queuedSpellDescription = spell.description;
				spellGUI.queuedSpellCircle = spell.circle;
				spellGUI.queuedSpellImagePath = spell.iconPath;
				/*spellGUI.transform.SendMessage("queueNewSpell",spell.name, SendMessageOptions.RequireReceiver);
				spellGUI.transform.SendMessage("queueSpellStats",spell.level.ToString(), SendMessageOptions.RequireReceiver);
				spellGUI.transform.SendMessage("queueSpellDescription",spell.description, SendMessageOptions.RequireReceiver);
				spellGUI.transform.SendMessage("queueSpellCircle",spell.circle, SendMessageOptions.RequireReceiver);
				spellGUI.transform.SendMessage("queueSpellImage",spell.iconPath, SendMessageOptions.RequireReceiver);
				spellGUI.transform.SendMessage("addQueuedSpellToList", SendMessageOptions.RequireReceiver);
				*/
				//spellGUI.addSpellToList(spell.name, spell.level.ToString(), spell.description, spell.circle, spell.iconPath);
				return true;
			}else{
				Debug.Log("Failed to learn spell");
				return false;
			}
		}
	}

	/////////////known spells///////////
	/// 
	/// 
	/// ///////////////////////////////

}

