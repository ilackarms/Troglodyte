using UnityEngine;
using System.Collections;
using AssemblyCSharp;

[System.Serializable]
public abstract class Item : Equippable, Sellable {

	public Texture2D image;
	public int width;
	public int height;

	public int drawPositionX;
	public int drawPositionY;

	public Slot occupiedSlot;

	public string name;
	public int itemID;
	public string description;
	public string requirementsText;

	public ItemPropertyBundle propertyBundle;

	//equipped model automatically set by item name!
	public GameObject equippedModel;

	public abstract void performAction ();
	
	public abstract void Equip ();
	
	public abstract void Unequip ();

	public abstract void Sell ();


	//variable/randomizeable values on item properties
	public abstract class ItemPropertyBundle{
		public int value;
		public Rarity rarity = Rarity.Common;
		public string suffixes="";
		public string prefixes="";
		public Requirements requirements;

		public enum Rarity {
			Common, Magic, Rare, Unique
		}

		abstract public void calculateValue ();
		
		public abstract void calculateRequirements();

	}

	
	public int tooltipWidth = 350;
	public int tooltipHeight = 450;
	public GUIStyle itemNameGUIStyle;
	public GUIStyle itemInfoGUIStyle;
	public GUIStyle itemReqsGUIStyle;
	
	public void drawItemTooltip(Vector2 position){
		
		//position.x -= tooltipWidth / 2;
		//position.y -= tooltipHeight / 2;
		
		//define guistyle and guiskin for tooltop
		Font newFont = (Font)Resources.Load ("Fonts/diablo_h");
		itemNameGUIStyle = new GUIStyle ();
		itemNameGUIStyle.font = newFont;
		itemNameGUIStyle.wordWrap = true;
		itemNameGUIStyle.alignment = TextAnchor.UpperCenter;
		//itemNameGUIStyle.normal.background.alphaIsTransparency = true;
		switch(propertyBundle.rarity){
		case ItemPropertyBundle.Rarity.Common:
			itemNameGUIStyle.normal.textColor = new Color(230/255f, 230/255f, 230/255f);
			break;
		case ItemPropertyBundle.Rarity.Magic:
			itemNameGUIStyle.normal.textColor = new Color(129/255f, 218/255f, 255/255f);
			break;
		case ItemPropertyBundle.Rarity.Rare:
			itemNameGUIStyle.normal.textColor = new Color(252/255f, 255/255f, 129/255f);
			break;
		}
		
		
		itemInfoGUIStyle = new GUIStyle ();
		Font descriptionFont = (Font)Resources.Load ("Fonts/diablo_h");
		itemInfoGUIStyle = new GUIStyle ();
		itemInfoGUIStyle.font = descriptionFont;
		itemInfoGUIStyle.wordWrap = true;
		itemInfoGUIStyle.alignment = TextAnchor.UpperCenter;
		//itemNameGUIStyle.normal.background.alphaIsTransparency = true;
		itemInfoGUIStyle.normal.textColor = new Color(101/255f, 240/255f, 139/255f);
		
		//make sure tooltip is always inside screen
		if(tooltipWidth>Screen.width){
			Debug.LogError("Tool Tip width is too large for screen!");
			return;
		}
		if(tooltipHeight>Screen.height){
			Debug.LogError("Tool Tip width is too large for screen!");
			return;
		}
		while (position.x+tooltipWidth>=Screen.width) {
			position.x--;
		}
		
		calculateDescription ();
		description = description.ToLower ();

		/*calculate requirements text*/
			requirementsText = "\n";
			if (propertyBundle.requirements.strReq > 0) {
				requirementsText+="Strength Required: "+ propertyBundle.requirements.strReq+"\n";
			}
			if (propertyBundle.requirements.dexReq > 0) {
				requirementsText+="Dexterity Required: "+ propertyBundle.requirements.dexReq+"\n";
			}
			if (propertyBundle.requirements.intReq > 0) {
				requirementsText+="Intelligence Required: "+ propertyBundle.requirements.intReq+"\n";
			}
		///////////////////////////////////////
		
		itemReqsGUIStyle = new GUIStyle ();
		itemReqsGUIStyle.font = descriptionFont;
		itemReqsGUIStyle.wordWrap = true;
		itemReqsGUIStyle.alignment = TextAnchor.UpperCenter;
		//itemNameGUIStyle.normal.background.alphaIsTransparency = true;
		itemReqsGUIStyle.normal.textColor = Color.white;
		if(!Global.combatSystem.canUseItem(this)) itemReqsGUIStyle.normal.textColor = Color.red;
		
		Vector2 tempSize = GUI.skin.GetStyle ("Box").CalcSize (new GUIContent ((propertyBundle.prefixes + name + propertyBundle.suffixes)));
		Vector2 tempSize2 = GUI.skin.GetStyle ("Box").CalcSize (new GUIContent ((description+requirementsText)));
		
		position.y = position.y - tempSize.y - tempSize2.y;
		
		while (position.y>=Screen.height) {
			position.y--;
		}
		while (position.y<=0) {
			position.y++;
		}
		
		GUI.Box (new Rect (position.x, position.y, tooltipWidth, tooltipHeight),
		         (propertyBundle.prefixes + name + propertyBundle.suffixes),itemNameGUIStyle);
				
		GUI.Box (new Rect (position.x, position.y+tempSize.y*1.5f, tooltipWidth, tooltipHeight-tempSize.y),
		         (description),itemInfoGUIStyle);


		GUI.Box (new Rect (position.x, position.y+tempSize2.y, tooltipWidth, tooltipHeight-tempSize.y),
		         (requirementsText),itemReqsGUIStyle);

		//GUIStyle boxstyle = new GUIStyle ();
		//boxstyle.normal.
		GUI.Box (new Rect (position.x, position.y, tooltipWidth, tempSize.y*1.5f+tempSize2.y),"");

	}

	public class Requirements{
		public int strReq;
		public int intReq;
		public int dexReq;
	}

	public abstract void calculateDescription();
}
