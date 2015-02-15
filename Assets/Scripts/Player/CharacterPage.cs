using UnityEngine;
using System.Collections;

public class CharacterPage : MonoBehaviour
{
	
	public Texture2D image;
	public Rect position; 

	public Texture2D plusButtonImage;
	public Texture2D skillPointsBox;

	Rect plusButtonPositionSTR;
	Rect plusButtonPositionDEX;
	Rect plusButtonPositionINT;
	Rect skillPointsBoxPosition;
	DrawBox skillPointsLabel;
	DrawBox skillPointsValue;

	//38 boxes
	DrawBox[] drawLabels = new DrawBox[20];
	DrawBox[] drawValues = new DrawBox[17];

	Rect closeBox;

	public bool draw = false;
	public static float scaleFactor = 1.35f;
	public bool changed = false;

	//TODO STR/DEX/INT +: ONLY DRAW THESE IF WE ARE READY TO LEVEL UP

	void Awake(){
		
		image = (Texture2D) Resources.Load ("Sprites/GUI/CharacterSheet");
		plusButtonImage = (Texture2D) Resources.Load ("Sprites/GUI/LevelUpButton");
		skillPointsBox = (Texture2D) Resources.Load ("Sprites/GUI/SkillPointsBox");
	}

	//dictionary: labels (string), rect
	//dictionary: values, rect
		// Use this for initialization
		void Start ()
		{

		initializeDrawBoxes ();
		}
	
		// Update is called once per frame
		void Update ()
		{
			if(Input.GetButtonDown("CharacterSheet") && !draw){
					initializeDrawBoxes();
					draw=true;
					Screen.lockCursor = false;
					Time.timeScale = 0;
					Global.OpenGUICount++;
					//Debug.Log(Global.OpenGUICount);
				}	
			else if(Input.GetButtonDown("CharacterSheet") && draw){
					draw=false;
					Global.OpenGUICount--;
					if (Global.OpenGUICount<=0){
						Global.OpenGUICount=0;
						Screen.lockCursor = true;
						Time.timeScale = 1;
					}
					//Debug.Log(Global.OpenGUICount);
				}
			}

	void initializeDrawBoxes(){
		
		int i = 0;
		int j = 0;
		DrawBox name = new DrawBox (CombatSystem.Statistics.Name, new Rect (12, 6, 169, 15));
		drawLabels [i++] = name;
		DrawBox level = new DrawBox("Level\n"+CombatSystem.Statistics.Level, new Rect(12, 29, 41, 33));
		drawLabels [i++] = level;
		DrawBox XP = new DrawBox("Experience\n"+CombatSystem.Statistics.XP, new Rect(67, 29, 114 ,33));
		drawLabels [i++] = XP;
		DrawBox transformationState = new DrawBox(CombatSystem.Statistics.TransformationState(CombatSystem.Statistics.Evil), new Rect(195, 6, 115, 15));
		drawLabels [i++] = transformationState;
		DrawBox nextLevelXP = new DrawBox("Next Level\n"+CombatSystem.Statistics.NextLevelXP, new Rect(195, 29, 115, 33));
		drawLabels [i++] = nextLevelXP;
		DrawBox STRLabel = new DrawBox("Strength",new Rect(12, 79, 65, 18));
		drawLabels [i++] = STRLabel;
		DrawBox STRValue = new DrawBox(CombatSystem.Statistics.TotalSTR, new Rect(77, 79, 38, 16));
		drawValues [j++] = STRValue;
		DrawBox DEXLabel = new DrawBox("Dexterity", new Rect(12, 141, 65, 18));
		drawLabels [i++] = DEXLabel;
		DrawBox DEXValue = new DrawBox(CombatSystem.Statistics.TotalDEX, new Rect(77, 141, 38, 16));
		drawValues [j++] = DEXValue;
		DrawBox INTLabel = new DrawBox("Intelligence", new Rect(11, 230, 65, 18));
		drawLabels [i++] = INTLabel;
		DrawBox INTValue = new DrawBox(CombatSystem.Statistics.TotalINT, new Rect(77, 230, 38, 16));
		drawValues [j++] = INTValue;
		DrawBox HungerLabel = new DrawBox("Hunger", new Rect(203, 293,68, 18)); //hunger? 
		drawLabels [i++] = HungerLabel;
		DrawBox HungerValue = new DrawBox(CombatSystem.Statistics.Hunger, new Rect(273, 293, 39, 17)) ; //hunger? 
		drawValues [j++] = HungerValue;
		DrawBox AttackDamageLabel = new DrawBox("Attack Damage", new Rect(161, 79, 98, 19));
		drawLabels [i++] = AttackDamageLabel;
		DrawBox AttackDamageValue = new DrawBox(CombatSystem.Statistics.MinTotalDamage+"-"+CombatSystem.Statistics.MaxTotalDamage, new Rect(262, 79, 47, 16));
		drawValues [j++] = AttackDamageValue;
		DrawBox ArmorLabel = new DrawBox("Armor", new Rect(161, 103, 98, 18));
		drawLabels [i++] = ArmorLabel;
		DrawBox ArmorValue = new DrawBox(CombatSystem.Statistics.TotalArmor, new Rect(262, 103, 47, 16));
		drawValues [j++] = ArmorValue;
		DrawBox MovementSpeedLabel = new DrawBox("Movement Speed", new Rect(162, 141, 108, 18));
		drawLabels [i++] = MovementSpeedLabel;
		DrawBox MovementSpeedValue = new DrawBox(CombatSystem.Statistics.MoveSpeed.ToString(".0#"), new Rect(272, 141, 37, 15));
		drawValues [j++] = MovementSpeedValue;
		DrawBox AttackSpeedLabel = new DrawBox("Attack Speed", new Rect(162, 165, 108, 18));
		drawLabels [i++] = AttackSpeedLabel;
		DrawBox AttackSpeedValue = new DrawBox(CombatSystem.Statistics.AttackSpeed.ToString(".0#"), new Rect(272, 165, 37, 15));
		drawValues [j++] = AttackSpeedValue;
		DrawBox SpellPowerLabel = new DrawBox("Spell Power", new Rect(162, 192, 108, 18));
		drawLabels [i++] = SpellPowerLabel;
		DrawBox SpellPowerValue =  new DrawBox(CombatSystem.Statistics.SpellPower, new Rect(272, 192, 37, 15));
		drawValues [j++] = SpellPowerValue;
		DrawBox LifeLabel = new DrawBox("Life", new Rect(162, 230, 67, 18));
		drawLabels [i++] = LifeLabel;
		DrawBox LifeCurrent = new DrawBox(CombatSystem.Statistics.CurrentHP, new Rect(232, 230, 37, 17));
		drawValues [j++] = LifeCurrent;
		DrawBox LifeMax= new DrawBox(CombatSystem.Statistics.MaxHP, new Rect(273, 230, 37, 17));
		drawValues [j++] = LifeMax;
		DrawBox ManaLabel = new DrawBox("Mana", new Rect(162, 254, 67, 18));
		drawLabels [i++] = ManaLabel;
		DrawBox ManaCurrent = new DrawBox(CombatSystem.Statistics.CurrentMana, new Rect(232, 254, 37, 17));
		drawValues [j++] = ManaCurrent;
		DrawBox ManaMax = new DrawBox(CombatSystem.Statistics.MaxMana, new Rect(273, 254, 37, 17));
		drawValues [j++] = ManaMax;
		DrawBox FireResistLabel = new DrawBox("Fire Resistance", new Rect(175, 332, 95, 17));
		drawLabels [i++] = FireResistLabel;
		DrawBox FireResistValue = new DrawBox(CombatSystem.Statistics.FireResistance, new Rect(272, 332, 37, 17));
		drawValues [j++] = FireResistValue;
		DrawBox LightningResistLabel = new DrawBox("Lightning Resistance", new Rect(175, 360, 95, 17));
		drawLabels [i++] = LightningResistLabel;
		DrawBox LightningResistValue = new DrawBox(CombatSystem.Statistics.FireResistance, new Rect(272, 360, 37, 17));
		drawValues [j++] = LightningResistValue;
		DrawBox IceResistLabel = new DrawBox("Ice Resistance", new Rect(175, 382, 95, 17));
		drawLabels [i++] = IceResistLabel;
		DrawBox IceResistValue = new DrawBox(CombatSystem.Statistics.FireResistance, new Rect(272, 382, 37, 17));
		drawValues [j++] = IceResistValue;
		DrawBox ChaosResistLabel = new DrawBox("Chaos Resistance", new Rect(175, 406, 95, 17));
		drawLabels [i++] = ChaosResistLabel;
		DrawBox ChaosResistValue = new DrawBox(CombatSystem.Statistics.FireResistance, new Rect(272, 406, 37, 17));
		drawValues [j++] = ChaosResistValue;

		
		plusButtonPositionSTR = new Rect(127, 79, 21, 21);//125 79
		plusButtonPositionDEX = new Rect(127, 141, 21, 21);
		plusButtonPositionINT = new Rect(127, 231, 21, 21);
		skillPointsBoxPosition = new Rect(13, 282, skillPointsBox.width, skillPointsBox.height+15);
		skillPointsLabel = new DrawBox ("Skill Points Remaining", new Rect (skillPointsBoxPosition.x + 3, skillPointsBoxPosition.y + 3, 70, 17)) ;
		skillPointsValue = new DrawBox(CombatSystem.Statistics.SkillPointsToAssign, new Rect(skillPointsBoxPosition.x + 76 , skillPointsBoxPosition.y + 3, 38, 15));

		closeBox = new Rect (133, 395, 31, 31);
	}
	//128 385
	void OnGUI(){
		if(draw){
			drawCharacterSheet();
			drawBoxes();
			detectCloseButton();
			if(CombatSystem.Statistics.SkillPointsToAssign>0){
				drawLevelUpButtons();
			}
			CustomCursor.drawCursor();
		}
	}

	void drawCharacterSheet(){
		scaleFactor = Inventory.scaleFactor;
		position.width = Global.inventory.position.width;
		position.height = Global.inventory.position.height;
		position.x = 0;
		position.y = 0;
		GUI.DrawTexture(position, image);
		
		//GUI.Box (weaponSlot1.position,"Nan");
	}

	void drawLevelUpButtons(){
		//draw skill points box
		GUI.DrawTexture(new Rect(skillPointsBoxPosition.x * scaleFactor + position.x,
		                         skillPointsBoxPosition.y * scaleFactor + position.y,
		                         skillPointsBoxPosition.width * scaleFactor,
		                         skillPointsBoxPosition.height * scaleFactor),skillPointsBox);

		//draw skill point plus boxes
		GUI.DrawTexture(new Rect(plusButtonPositionSTR.x * scaleFactor + position.x,
		                         plusButtonPositionSTR.y * scaleFactor + position.y,
		                         plusButtonPositionSTR.width * scaleFactor,
		                         plusButtonPositionSTR.height * scaleFactor),plusButtonImage);

		if (GUI.Button (new Rect (plusButtonPositionSTR.x * scaleFactor + position.x,
		                       plusButtonPositionSTR.y * scaleFactor + position.y,
		                       plusButtonPositionSTR.width * scaleFactor,
		                       plusButtonPositionSTR.height * scaleFactor), "")) {
			CombatSystem.Statistics.levelUpSTR+=1;
			CombatSystem.Statistics.SkillPointsToAssign-=1;
			CombatSystem.Statistics.calculateStatistics();
			initializeDrawBoxes();
		}

		GUI.DrawTexture(new Rect(plusButtonPositionDEX.x * scaleFactor + position.x,
		                         plusButtonPositionDEX.y * scaleFactor + position.y,
		                         plusButtonPositionDEX.width * scaleFactor,
		                         plusButtonPositionDEX.height * scaleFactor),plusButtonImage);
		
		if (GUI.Button (new Rect (plusButtonPositionDEX.x * scaleFactor + position.x,
		                          plusButtonPositionDEX.y * scaleFactor + position.y,
		                          plusButtonPositionDEX.width * scaleFactor,
		                          plusButtonPositionDEX.height * scaleFactor), "")) {
			CombatSystem.Statistics.levelUpDEX+=1;
			CombatSystem.Statistics.SkillPointsToAssign-=1;
			CombatSystem.Statistics.calculateStatistics();
			initializeDrawBoxes();
		}
		GUI.DrawTexture(new Rect(plusButtonPositionINT.x * scaleFactor + position.x,
		                         plusButtonPositionINT.y * scaleFactor + position.y,
		                         plusButtonPositionINT.width * scaleFactor,
		                         plusButtonPositionINT.height * scaleFactor),plusButtonImage);
		
		if (GUI.Button (new Rect (plusButtonPositionINT.x * scaleFactor + position.x,
		                          plusButtonPositionINT.y * scaleFactor + position.y,
		                          plusButtonPositionINT.width * scaleFactor,
		                          plusButtonPositionINT.height * scaleFactor), "")) {
			CombatSystem.Statistics.levelUpINT+=1;
			CombatSystem.Statistics.SkillPointsToAssign-=1;
			CombatSystem.Statistics.calculateStatistics();
			initializeDrawBoxes();
		}

		GUIStyle labelGUIStyle;
		Font newFont = (Font)Resources.Load ("Fonts/diablo_h");
		labelGUIStyle = new GUIStyle ();
		labelGUIStyle.font = newFont;
		labelGUIStyle.fontSize = 11;
		labelGUIStyle.wordWrap = true;
		labelGUIStyle.alignment = TextAnchor.MiddleCenter;
		labelGUIStyle.normal.textColor = Color.white;

		GUI.Box (new Rect(skillPointsLabel.rect.x * scaleFactor +position.x,
		                  skillPointsLabel.rect.y * scaleFactor +position.y,
		                  skillPointsLabel.rect.width  * scaleFactor,
		                  skillPointsLabel.rect.height * scaleFactor), skillPointsLabel.text, labelGUIStyle);

		labelGUIStyle.normal.textColor = Color.magenta	;
		
		GUI.Box (new Rect(skillPointsValue.rect.x * scaleFactor +position.x,
		                  skillPointsValue.rect.y * scaleFactor +position.y,
		                  skillPointsValue.rect.width  * scaleFactor,
		                  skillPointsValue.rect.height * scaleFactor), skillPointsValue.text, labelGUIStyle);

	}

	
	void detectCloseButton(){
		if (GUI.Button(new Rect(closeBox.x * scaleFactor + position.x,
		                        closeBox.y * scaleFactor + position.y,
		                        closeBox.width * scaleFactor,
		                        closeBox.height * scaleFactor),"")){
			draw=false;
			Global.OpenGUICount--;
			if (Global.OpenGUICount<=0){
				Global.OpenGUICount=0;
				Screen.lockCursor = true;
				Time.timeScale = 1;
			}
			//Debug.Log(Global.OpenGUICount);
		}
	}

	void drawBoxes(){
		if (changed) {
			initializeDrawBoxes();
			Debug.Log("Initialize");
			changed=false;
		}
		GUIStyle labelGUIStyle;
		Font newFont = (Font)Resources.Load ("Fonts/diablo_h");
		labelGUIStyle = new GUIStyle ();
		labelGUIStyle.font = newFont;
		labelGUIStyle.fontSize = 11;
		labelGUIStyle.wordWrap = true;
		labelGUIStyle.alignment = TextAnchor.MiddleCenter;
		labelGUIStyle.normal.textColor = Color.white;
		for(int i=0; i<drawLabels.Length; i++){
			GUI.Box (new Rect(drawLabels[i].rect.x * scaleFactor +position.x,
			                  drawLabels[i].rect.y * scaleFactor +position.y,
			                  drawLabels[i].rect.width  * scaleFactor,
			                  drawLabels[i].rect.height * scaleFactor), drawLabels[i].text, labelGUIStyle);
		}
		labelGUIStyle.normal.textColor = Color.green;
		labelGUIStyle.fontSize = 14;
		for(int i=0; i<drawValues.Length; i++){
			GUI.Box (new Rect(drawValues[i].rect.x * scaleFactor +position.x,
			                  drawValues[i].rect.y * scaleFactor +position.y,
			                  drawValues[i].rect.width  * scaleFactor,
			                  drawValues[i].rect.height * scaleFactor), drawValues[i].text, labelGUIStyle);
		}
	}

	public void notifyChangesToStats(){
		//Debug.Log("Initialize1");
		initializeDrawBoxes (); 
		changed = true;
	}

	private class DrawBox{
		public string text;
		public Rect rect;
		
		public DrawBox(string text, Rect rect){
			this.text = text;
			this.rect = rect;
		}
		
		public DrawBox(int text, Rect rect){
			this.text = text.ToString();
			this.rect = rect;
		}
		
		public DrawBox(float text, Rect rect){
			this.text = ((int) text).ToString();
			this.rect = rect;
		}
	}
}

