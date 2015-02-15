using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class Inventory : MonoBehaviour {
	//dimensions of rect = 321/434
	public Texture2D image;
	public Rect position; 
	
	public List<Item> items;
	public int slotPixelWidth=29;
	public int slotPixelHeight=29;
	public int inventoryGridPositionX=17;
	public int inventoryGridPositionY=255;
	public Slot[,] slots;

	public Dictionary<string, Slot> equipmentSlots = new Dictionary<string, Slot>();
	public Slot weaponSlot;
	public Slot chestSlot, helmSlot, gloveSlot, ringSlot1, ringSlot2, bootSlot, beltSlot, amuletSlot;

	public Item clickedItem;

	//potion stuff
	public Texture2D potionBar;
	public Rect potionBarPosition;
	public Slot potion1;
	public Slot potion2;
	public Slot potion3;
	public Slot potion4;
	public Slot potion5;
	public Slot[] potionSlots = new Slot[5];


	public static float scaleFactor = 1.35f;

	public bool draw=false;

	// Use this for initialization
	void Start () {

		Time.timeScale = 1;

		image = (Texture2D) Resources.Load ("Sprites/GUI/InventoryScreen");
		potionBar = (Texture2D) Resources.Load ("Sprites/GUI/PotionSlots");

		slotPixelWidth = (int) (slotPixelWidth * scaleFactor);
		slotPixelHeight = (int) (slotPixelHeight * scaleFactor);
		inventoryGridPositionX = (int) (inventoryGridPositionX * scaleFactor);
		inventoryGridPositionY = (int) (inventoryGridPositionY * scaleFactor);

		items = new List<Item> ();

		position.width = image.width * scaleFactor;
		position.height = image.height * scaleFactor;
		position.x = Screen.width - position.width;
		position.y = 0;

		
		potionBarPosition.width = potionBar.width * scaleFactor;
		potionBarPosition.height = potionBar.height * scaleFactor;
		potionBarPosition.x = Screen.width/2 - potionBarPosition.width/2;
		potionBarPosition.y = Screen.height - potionBarPosition.height;
		potion1 = new Slot(new Rect(potionBarPosition.x, potionBarPosition.y, potionBarPosition.width/5, potionBarPosition.height));
		potion2 = new Slot(new Rect(potionBarPosition.x + potionBarPosition.width/5, potionBarPosition.y, potionBarPosition.width/5, potionBarPosition.height));
		potion3 = new Slot(new Rect(potionBarPosition.x + potionBarPosition.width*2/5, potionBarPosition.y, potionBarPosition.width/5, potionBarPosition.height));
		potion4 = new Slot(new Rect(potionBarPosition.x + potionBarPosition.width*3/5, potionBarPosition.y, potionBarPosition.width/5, potionBarPosition.height));
		potion5 = new Slot(new Rect(potionBarPosition.x + potionBarPosition.width*4/5, potionBarPosition.y, potionBarPosition.width/5, potionBarPosition.height));
		potionSlots [0] = potion1;
		potionSlots [1] = potion2;
		potionSlots [2] = potion3;
		potionSlots [3] = potion4;
		potionSlots [4] = potion5;

		initializeSlots ();


		//addItem (0,0,new Weapon (1,2,(Texture2D) Resources.Load("Sprites/Weapons/DrowSword"),"Drow Sword"));
		//addItem (1,1,new Weapon (1,2,(Texture2D) Resources.Load("Sprites/Weapons/DrowSword"),"Drow Sword"));
		//addItem (2,0,new Weapon (2,4,(Texture2D) Resources.Load("Sprites/Weapons/ShamanStaff"),"Shaman Staff"));
		
		//PickUpItem(Global.itemFactory.GenerateNewRandomItem("Drow Sword", 3));
		//PickUpItem(Global.itemFactory.GenerateNewRandomItem("Drow Sword", 4));
		PickUpItem(Global.itemFactory.GenerateNewRandomItem("Shaman Staff", 1));
		PickUpItem(Global.itemFactory.GenerateNewRandomItem("Compound Bow", 1));
		PickUpItem(Global.itemFactory.GenerateNewRandomItem("Drow Sword", 1));

	}

	void initializeSlots() { 
		slots = new Slot[10,4];
		for (int x=0; x<slots.GetLength(0); x++) {
			for (int y=0; y<slots.GetLength(1);y++){
				//Debug.Log("Slot length = "+slots.GetLength(0)+","+slots.GetLength(1));
				slots[x,y] = new Slot(new Rect(inventoryGridPositionX+slotPixelWidth*x,
				                               inventoryGridPositionY+slotPixelHeight*y,
				                               slotPixelWidth,
				                               slotPixelHeight));

			}
		}		
		weaponSlot = new Slot(new Rect (26 * scaleFactor + position.x , 217 * scaleFactor + position.y , 54 * scaleFactor, 114 * scaleFactor));
		chestSlot = new Slot(new Rect (134 * scaleFactor + position.x, 74 * scaleFactor + position.y, 57 * scaleFactor, 86 * scaleFactor));
		helmSlot = new Slot(new Rect (134 * scaleFactor + position.x, 3 * scaleFactor + position.y, 57 * scaleFactor, 57 * scaleFactor));
		gloveSlot = new Slot(new Rect (20 * scaleFactor + position.x , 176 * scaleFactor + position.y , 57 * scaleFactor, 57 * scaleFactor));
		ringSlot1 = new Slot(new Rect (93 * scaleFactor + position.x , 176 * scaleFactor + position.y , 27 * scaleFactor, 27 * scaleFactor));
		ringSlot2 = new Slot(new Rect (207 * scaleFactor + position.x , 176 * scaleFactor + position.y , 27 * scaleFactor, 27 * scaleFactor));
		bootSlot = new Slot(new Rect (250 * scaleFactor + position.x , 176 * scaleFactor + position.y , 57 * scaleFactor, 57 * scaleFactor));
		beltSlot = new Slot(new Rect (26 * scaleFactor + position.x , 176 * scaleFactor + position.y , 57 * scaleFactor, 27 * scaleFactor));
		amuletSlot = new Slot(new Rect (93 * scaleFactor + position.x , 30 * scaleFactor + position.y , 27 * scaleFactor, 27 * scaleFactor));

		equipmentSlots.Add ("Weapon Slot",weaponSlot);
		equipmentSlots.Add ("Chest Slot",chestSlot);
		equipmentSlots.Add ("Helm Slot",helmSlot);
		equipmentSlots.Add ("Glove Slot",gloveSlot);
		equipmentSlots.Add ("Ring Slot 1",ringSlot1);
		equipmentSlots.Add ("Ring Slot 2",ringSlot2);
		equipmentSlots.Add ("Boot Slot",bootSlot);
		equipmentSlots.Add ("Belt Slot",beltSlot);
		equipmentSlots.Add ("Amulet Slot",amuletSlot);
	}
	
	// Update is called once per frame
	void Update () {
		OpenOrCloseGUI ();
		detectPotionDrinking ();
	}

	void OnGUI() {
		//always draw potions
		drawPotions ();
		if(draw){
			drawInventory ();
			//drawDebugBoxes();
			drawItems ();
			CustomCursor.drawCursor();
			detectMouseAction();
			GUI.backgroundColor = Color.gray;
			detectCloseButton();
		}
	}

	void OpenOrCloseGUI(){		
		if(Input.GetButtonDown("Inventory") && !draw){
			draw=true;
			Screen.lockCursor = false;
			Time.timeScale = 0;
			Global.OpenGUICount++;
			//Debug.Log(Global.OpenGUICount);
		}	
		else if(Input.GetButtonDown("Inventory") && draw){
			draw=false;
			Global.OpenGUICount--;
			if (Global.OpenGUICount<=0){
				Global.OpenGUICount=0;
				Screen.lockCursor = true;
				Time.timeScale = 1;
			}
			//Debug.Log(Global.OpenGUICount);
		}
		if((Input.GetButtonDown("CloseGUI") && draw)){
			draw=draw^true;
			Screen.lockCursor = Screen.lockCursor^true;
			Time.timeScale = (draw) ? 0 : 1;
			Debug.Log(Global.OpenGUICount);
		}
	}

	void detectCloseButton(){
		if (GUI.Button(new Rect (17 * scaleFactor + position.x, 386 * scaleFactor + position.y, 32 * scaleFactor, 32 * scaleFactor),"") && clickedItem==null){
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

	void detectPotionDrinking(){
		if(Input.GetButtonDown("Potion1")){
			if(potion1.item==null) return;
			((Potion) potion1.item).Consume();
			potion1.occupied=false;
			potion1.item = null;
		}
		if(Input.GetButtonDown("Potion2")){
			if(potion2.item==null) return;
			((Potion) potion2.item).Consume();
			potion2.occupied=false;
			potion2.item = null;
		}
		if(Input.GetButtonDown("Potion3")){
			if(potion3.item==null) return;
			((Potion) potion3.item).Consume();
			potion3.occupied=false;
			potion3.item = null;
		}
		if(Input.GetButtonDown("Potion4")){
			if(potion4.item==null) return;
			((Potion) potion4.item).Consume();
			potion4.occupied=false;
			potion4.item = null;
		}
		if(Input.GetButtonDown("Potion5")){
			if(potion5.item==null) return;
			((Potion) potion5.item).Consume();
			potion5.occupied=false;
			potion5.item = null;
		}
	}

	void drawPotions(){
		GUI.DrawTexture (potionBarPosition, potionBar);
		for (int i=0; i<5; i++) {
			if(potionSlots[i].item!=null){
				GUI.DrawTexture(potionSlots[i].position, potionSlots[i].item.image);
			}
		}
	}

	void drawInventory(){
		position.width = image.width * scaleFactor;
		position.height = image.height * scaleFactor;
		position.x = Screen.width - position.width;
		position.y = 0;
		GUI.DrawTexture(position, image);
		weaponSlot.position = new Rect (20 * scaleFactor + position.x, 46 * scaleFactor + position.y, 54 * scaleFactor, 114 * scaleFactor);
		chestSlot.position = new Rect (134 * scaleFactor + position.x, 74 * scaleFactor + position.y, 57 * scaleFactor, 86 * scaleFactor);
		helmSlot.position = new Rect (134 * scaleFactor + position.x, 3 * scaleFactor + position.y, 57 * scaleFactor, 57 * scaleFactor);
		gloveSlot.position = new Rect (20 * scaleFactor + position.x , 176 * scaleFactor + position.y , 57 * scaleFactor, 57 * scaleFactor);
		ringSlot1.position = new Rect (93 * scaleFactor + position.x , 176 * scaleFactor + position.y , 27 * scaleFactor, 27 * scaleFactor);
		ringSlot2.position = new Rect (207 * scaleFactor + position.x , 176 * scaleFactor + position.y , 27 * scaleFactor, 27 * scaleFactor);
		bootSlot.position = new Rect (250 * scaleFactor + position.x , 176 * scaleFactor + position.y , 57 * scaleFactor, 57 * scaleFactor);
		beltSlot.position = new Rect (26 * scaleFactor + position.x , 176 * scaleFactor + position.y , 57 * scaleFactor, 27 * scaleFactor);
		amuletSlot.position = new Rect (93 * scaleFactor + position.x , 30 * scaleFactor + position.y , 27 * scaleFactor, 27 * scaleFactor);

		//GUI.Box (weaponSlot1.position,"Nan");
	}

	void drawItems(){
		for (int i=0; i<items.Count; i++) {
			GUI.DrawTexture(new Rect(items[i].drawPositionX*slotPixelWidth+inventoryGridPositionX+position.x + 4 * scaleFactor,
			                         items[i].drawPositionY*slotPixelHeight+inventoryGridPositionY+position.y + 4 * scaleFactor,
			                         items[i].width*slotPixelWidth - 8 * scaleFactor,
			                         items[i].height*slotPixelHeight - 8 * scaleFactor),
			                items[i].image);
		}
		foreach(Slot slot in equipmentSlots.Values){
			if(slot.item!=null){
				//Debug.Log("Drawing the item in weapon slot 1");
				GUI.DrawTexture(new Rect(slot.position.center.x-(slot.item.width*slotPixelWidth - 8 * scaleFactor)/2,
				                         slot.position.center.y-(slot.item.height*slotPixelHeight - 8 * scaleFactor)/2,
				                         slot.item.width*slotPixelWidth - 8 * scaleFactor,
				                         slot.item.height*slotPixelHeight - 8 * scaleFactor),
				                slot.item.image);
			}
		}
		if(clickedItem!=null){
			clickedItem.drawPositionX = (int) Input.mousePosition.x;
			clickedItem.drawPositionY = (int) (Screen.height - Input.mousePosition.y);
			GUI.DrawTexture(new Rect(Event.current.mousePosition.x + 4 * scaleFactor - (clickedItem.width*slotPixelWidth - 8 * scaleFactor)*2/3,
			                         Event.current.mousePosition.y + 4 * scaleFactor- (clickedItem.width*slotPixelHeight - 8 * scaleFactor)*2/3,
			                         clickedItem.width*slotPixelWidth - 8 * scaleFactor,
			                         clickedItem.height*slotPixelHeight - 8 * scaleFactor),
			                clickedItem.image);
		}
	}

	void detectMouseAction(){
		//if item is outside of rect, and holding an item... drop it!
		if(Input.GetMouseButtonDown(0)){
			if(clickedItem!=null && !(position.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y))) 
			   && !(potionBarPosition.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))){
				DropItem(clickedItem);
				return;
			}
		}

		Rect slot;
		for(int i = 0; i< slots.GetLength(0); i++){
			for (int j=0; j<slots.GetLength(1);j++){
				slot = slots[i,j].position;
				slot.x += position.x;
				slot.y += position.y;
				if(slot.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y))){
					if(slots[i,j].item!=null){
						slots[i,j].item.drawItemTooltip(new Vector2(Input.mousePosition.x+5, Screen.height - Input.mousePosition.y+5));
					}
					if(Event.current.isMouse && Input.GetMouseButtonDown(0)){
						//						if(slots[i,j].item!=null) Debug.Log("Clicked on top of "+slots[i,j].item.name);
						if(slots[i,j].item==null && clickedItem!=null){
							if(addItem(i,j,clickedItem)){
								clickedItem=null;
								//								Debug.Log("Item back into inventory "+slots[i,j].item.name);
							}
							return;
						}
						if(slots[i,j].item!=null && clickedItem!=null){
							if(addItem(i,j,clickedItem)){
								clickedItem=null;
								Debug.Log("Item back into inventory "+slots[i,j].item.name);
							}
							return;
						}
						if(slots[i,j].item!=null && clickedItem==null){
							//							Debug.Log("Item Clicked"+slots[i,j].item.name);
							clickedItem=slots[i,j].item;
							removeItem(slots[i,j].item);
							return;
						}
					}
					//drink potions from inventory
					if(Event.current.isMouse && Input.GetMouseButtonDown(1)){
						if(slots[i,j].item!=null && (slots[i,j].item is Potion) && clickedItem==null){
							Debug.Log("Drank a potion.");
							((Potion) slots[i,j].item).Consume();
							removeItem(slots[i,j].item);
							return;
						}
						if(slots[i,j].item!=null && (slots[i,j].item is Scroll) && clickedItem==null){
							if (Global.SpellSystem.learnSpell(((Scroll) slots[i,j].item).spell)){
								Debug.Log("Read a scroll.");
								removeItem(slots[i,j].item);
							}
							return;
						}
					}
				}
			}
		}
		//equipped item slots
		foreach(Slot equipmentSlot in equipmentSlots.Values){
			if(equipmentSlot.position.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y))){
				if(Event.current.isMouse && Input.GetMouseButtonDown(0)){
					if(clickedItem!=null){
						//DO THIS FOR EACH WEAPON SLOT / ITEM TYPE
						if(equipmentSlot.Equals(weaponSlot) && (clickedItem is Weapon) ){ //((Armor.WeaponPropertyBundle) clickedItem.propertyBundle)
							equipItem(equipmentSlot,clickedItem);
							return;
						}
						if(equipmentSlot.Equals(chestSlot) && (clickedItem is Armor) && ((Armor) clickedItem).armorType == Armor.ArmorType.Chest ){ //((Armor.WeaponPropertyBundle) clickedItem.propertyBundle)
							equipItem(equipmentSlot,clickedItem);
							return;
						}
						if(equipmentSlot.Equals(helmSlot) && (clickedItem is Armor) && ((Armor) clickedItem).armorType == Armor.ArmorType.Headgear ){ //((Armor.WeaponPropertyBundle) clickedItem.propertyBundle)
							equipItem(equipmentSlot,clickedItem);
							return;
						}
						if(equipmentSlot.Equals(gloveSlot) && (clickedItem is Armor) && ((Armor) clickedItem).armorType == Armor.ArmorType.Gloves ){ //((Armor.WeaponPropertyBundle) clickedItem.propertyBundle)
							equipItem(equipmentSlot,clickedItem);
							return;
						}
						if(equipmentSlot.Equals(ringSlot1) && (clickedItem is Armor) && ((Armor) clickedItem).armorType == Armor.ArmorType.Ring ){ //((Armor.WeaponPropertyBundle) clickedItem.propertyBundle)
							equipItem(equipmentSlot,clickedItem);
							return;
						}
						if(equipmentSlot.Equals(ringSlot2) && (clickedItem is Armor) && ((Armor) clickedItem).armorType == Armor.ArmorType.Ring ){ //((Armor.WeaponPropertyBundle) clickedItem.propertyBundle)
							equipItem(equipmentSlot,clickedItem);
							return;
						}
						if(equipmentSlot.Equals(bootSlot) && (clickedItem is Armor) && ((Armor) clickedItem).armorType == Armor.ArmorType.Boots ){ //((Armor.WeaponPropertyBundle) clickedItem.propertyBundle)
							equipItem(equipmentSlot,clickedItem);
							return;
						}
						if(equipmentSlot.Equals(beltSlot) && (clickedItem is Armor) && ((Armor) clickedItem).armorType == Armor.ArmorType.Belt ){ //((Armor.WeaponPropertyBundle) clickedItem.propertyBundle)
							equipItem(equipmentSlot,clickedItem);
							return;
						}
						if(equipmentSlot.Equals(amuletSlot) && (clickedItem is Armor) && ((Armor) clickedItem).armorType == Armor.ArmorType.Amulet ){ //((Armor.WeaponPropertyBundle) clickedItem.propertyBundle)
							equipItem(equipmentSlot,clickedItem);
							return;
						}
					}
					if(equipmentSlot.item!=null && clickedItem==null){
						//							Debug.Log("Item unequipped"+equipmentSlot.item.name);
						clickedItem=equipmentSlot.item;
						unequipItem(equipmentSlot,equipmentSlot.item);
						return;
					}
				}
				if(equipmentSlot.item!=null){
					equipmentSlot.item.drawItemTooltip(new Vector2(Input.mousePosition.x+5, Screen.height - Input.mousePosition.y+5));
					return;
				}
			}
		}
		//potions
		foreach(Slot potionSlot in potionSlots){
			if(potionSlot.position.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y))){
				//draw potion tooltip if slot contains potion
				if(potionSlot.item!=null){
					potionSlot.item.drawItemTooltip(new Vector2(Input.mousePosition.x+5, Screen.height - Input.mousePosition.y+5));
				}
				if(Event.current.isMouse && Input.GetMouseButtonDown(0)){
					if(clickedItem!=null && (clickedItem is Potion)){
						if(potionSlot.item!=null){
							Item temp = potionSlot.item;
							potionSlot.item = clickedItem;
							clickedItem = temp;
							return;
						}
						else{
							potionSlot.item = clickedItem;
							clickedItem = null;
							return;
						}
					}
					if(clickedItem==null && potionSlot.item!=null){
						clickedItem = potionSlot.item;
						potionSlot.occupied=false;
						potionSlot.item = null;
						return;
					}
				}
				//drink potion if we right click on it
				if(Event.current.isMouse && Input.GetMouseButtonDown(1)){
					((Potion) potionSlot.item).Consume();
					potionSlot.occupied=false;
					potionSlot.item=null;
					return;
				}
			}
		}
		/*
		 * for(int i = 0; i< slots.GetLength(0); i++){
			for (int j=0; j<slots.GetLength(1);j++){
				slot = slots[i,j].position;
				slot.x += position.x;
				slot.y += position.y;
				if(
					if(Event.current.isMouse && Input.GetMouseButtonDown(0)){
						//						if(slots[i,j].item!=null) Debug.Log("Clicked on top of "+slots[i,j].item.name);
						if(slots[i,j].item==null && clickedItem!=null){
							if(addItem(i,j,clickedItem)){
								clickedItem=null;
								//								Debug.Log("Item back into inventory "+slots[i,j].item.name);
							}
							return;
						}
						if(slots[i,j].item!=null && clickedItem!=null){
							if(addItem(i,j,clickedItem)){
								clickedItem=null;
								Debug.Log("Item back into inventory "+slots[i,j].item.name);
							}
							return;
						}
						if(slots[i,j].item!=null && clickedItem==null){
							//							Debug.Log("Item Clicked"+slots[i,j].item.name);
							clickedItem=slots[i,j].item;
							removeItem(slots[i,j].item);
							return;
						}
					}
					//drink potions from inventory
					if(Event.current.isMouse && Input.GetMouseButtonDown(1)){
						if(slots[i,j].item!=null && (slots[i,j].item is Potion) && clickedItem==null){
							((Potion) slots[i,j].item).Consume();
							removeItem(slots[i,j].item);
							return;
						}
					}
				}
			}
		}*/

	}

	public bool equipItem(Slot slot, Item item){
		if (item.propertyBundle != null && !Global.combatSystem.canUseItem (item))
						return false;
		if(item is Equippable){
			if(!slot.occupied && clickedItem!=null){
				slot.item = item;
				slot.occupied=true;
				item.drawPositionX = (int) slot.position.center.x;
				item.drawPositionY = (int) slot.position.center.y;

				//swap weapon models
				EquippedItemIndex.unarmed.SetActive(false);
				((Equippable) item).Equip();

				clickedItem=null;
				debugSlots.Add(slot);

				Global.combatSystem.notifyEquipmentChange();
				return true;
			}
			if (slot.occupied && clickedItem != null) {
				Item temp = clickedItem;
				clickedItem = slot.item;
				slot.item = temp;
//				Debug.Log ("swapped weapons "+slot.item+" in for "+clickedItem.name);

				
				//swap weapon models
				((Equippable) clickedItem).Unequip();
				((Equippable) slot.item).Equip();

				Global.combatSystem.notifyEquipmentChange();
				return true;
			}
			else{
				Debug.Log("Already have one equipped!");
				return false;
			}
		}
		else return false;
	}


	public void unequipItem(Slot slot, Item item){
		slot.occupied = false;
		slot.item = null;
		
		//swap weapon models
		EquippedItemIndex.unarmed.SetActive(true);
		((Equippable) item).Unequip();
		Global.combatSystem.notifyEquipmentChange();
	}
	
	public bool addItem(int x, int y, Item item){
//		Debug.Log ("Attempting to add " + item.name + " to slot " + x + "," + y);
		bool occupied = false;
		int conflictingItems=0;
		int conflictIndexX=0, conflictIndexY = 0;
		Item conflictingItem = null;

		if(x+item.width>slots.GetLength(0)){
			//Debug.Log("Item will not fit this far right in inventory");
			return(addItem(x-1,y,item));
		}
		if(y+item.height>slots.GetLength(1)){
			//Debug.Log("Item will not fit this far down in inventory");
			return(addItem(x,y-1,item));
		}

		if(!occupied){
			for (int i=0; i<item.width; i++) {
				for (int j=0; j<item.height;j++){
					if(slots[i+x,j+y].occupied){
//						Debug.Log("Slot "+(i+x)+","+(j+y)+" is occupied by item "+
//						          slots[i+x,j+y].item.name);
						if(conflictingItem!=slots[i+x,j+y].item){
							conflictingItem = slots[i+x,j+y].item;
							conflictingItems++;
							conflictIndexX = i+x;
							conflictIndexY = j+y;
//							Debug.Log("Conflicting item"+conflictingItem.name+" conflicting times:"+conflictingItems);
						}
						occupied=true;
					//Debug.Log("No room for item there.");
					}
				}
			}
		}
		if(conflictingItems==1 && clickedItem!=null){
//			Debug.Log("Swap one item!");
			clickedItem = slots[conflictIndexX,conflictIndexY].item;
			removeItem(slots[conflictIndexX,conflictIndexY].item);
			addItem(x, y, item);
		}
		if(!occupied) {
			//Debug.Log("Item added");
			items.Add (item);
			item.drawPositionX = x;
			item.drawPositionY = y;
			item.occupiedSlot = slots[x,y];
			for(int i=x; i<x+item.width; i++){
				for(int j=y; j<y+item.height; j++){
					slots[i,j].item = item;
					slots[i,j].occupied = true;
					debugSlots.Add(slots[i,j]);
				}
			}
		}
		return !occupied;
	}

	public void removeItem(Item item){
		int x=0, y=0;
		for (int i=0; i<slots.GetLength(0); i++) {
			for (int j=0; j<slots.GetLength(1); j++) {
				if(slots[i,j].Equals(item.occupiedSlot)){
					x = i;
					y = j;
					break;
				}
			}
		}
		if(items.Contains(item)){
			for (int i=0; i<item.width; i++) {
				for (int j=0; j<item.height;j++){
					slots[i+x,j+y].occupied = false;
					slots[i+x,j+y].item = null;
					debugSlots.Remove(slots[i+x,j+y]);
				}
			}
			items.Remove (item);
		}
		else{
			Debug.LogError("That item does not exist to be removed!");
		}

	}

	List<Slot> debugSlots = new List<Slot> ();
	public void drawDebugBoxes(){
		foreach(Slot slot in debugSlots){
			GUI.Box(new Rect(slot.position.x + position.x, (slot.position.y)* 1 + position.y, (slot.position.width)* 1, (slot.position.height)* 1),"");
		}
		foreach (Slot slot in equipmentSlots.Values) {
			GUI.Box(slot.position,"");
		}
		GUI.Box(chestSlot.position,"");
	}

	public bool PickUpItem(Item item){
//		Debug.Log ("Pickup!");
		//Debug.Log("Item width: "+item.width);
		//Debug.Log("Item height: "+item.height);
		for(int i=0; i<=slots.GetLength(0)-item.width; i++){
			for(int j=0; j<=slots.GetLength(1)-item.height; j++){
		//		Debug.Log("TRying slot "+i+" in range "+ (slots.GetLength(0)-item.width) + ", and "+j+" in range "+(slots.GetLength(1)-item.height));
				if(!slots[i,j].occupied){
					if(checkOccupiedSlots(i,j,item)){
						addItem(i,j,item);
						return true;
					}
				}
			}
		}
		Debug.Log ("Not a single space free.");
		return false;
	}

	private bool checkOccupiedSlots(int x, int y, Item item){
		for(int i=0; i<item.width; i++){
			for(int j=0; j<item.height; j++){
				if(slots[x+i,y+j].occupied) return false;
			}
		}
	    return true;
	}

	public void DropItem(Item item){
		Global.itemFactory.InstantiateItemFromInventory(transform, clickedItem);
		clickedItem=null;
	}
}
