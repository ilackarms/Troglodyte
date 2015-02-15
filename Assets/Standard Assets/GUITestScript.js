//#pragma strict
/*
Necromancer GUI Demo Script
Author: Jason Wentzel
jc_wentzel@ironboundstudios.com

In this script you'll find some handy little functions for some of the 
Custom elements in the skin, these should help you create your own;

AddSpikes (not perfect but works well enough if you’re careful with your window widths)
FancyTop (just an example of using the elements to do a centered header graphic)
WaxSeal (adds the waxseal and ribbon to the right of the window)
DeathBadge (adds the iconFrame, skull, and ribbon elements properly aligned)

*/

var doWindow2 = false;

private var leafOffset;
private var frameOffset;
private var skullOffset;

private var RibbonOffsetX;
private var FrameOffsetX;
private var SkullOffsetX;
private var RibbonOffsetY;
private var FrameOffsetY;
private var SkullOffsetY;

private var WSwaxOffsetX;
private var WSwaxOffsetY;
private var WSribbonOffsetX;
private var WSribbonOffsetY;
	
private var spikeCount;

var spell1Content : GUIContent;
var spell2Content : GUIContent;
var spell3Content : GUIContent;
var spell4Content : GUIContent;

var drawSpell1;
var drawSpell2;
var drawSpell3;
var drawSpell4;

// This script will only work with the Necromancer skin
var mySkin : GUISkin;

//if you're using the spikes you'll need to find sizes that work well with them these are a few...
private var windowRect0 = Rect (500, 140, 350, 510);
private var windowRect1 = Rect (380, 40, 262, 420);
private var windowRect2 = Rect (700, 40, 600, 620);
private var windowRect3 = Rect (0, 40, 350, 500);

private var scrollPosition : Vector2;
private var HroizSliderValue = 0.5;
private var VertSliderValue = 0.5;
private var ToggleBTN = false;

public var draw = true;

//spell info ARRAY OF SPELLS
var spellList = new GUISpell[20];

var fSpell : GUISpell; // only 4 long at any given time!
var gSpell : GUISpell;
var vSpell : GUISpell;
var bSpell : GUISpell;

/*class Spell{
	var name: String;
	var stats: String;
	var description: String;
	var circle: String;
	var image: String;
	function Spell(name : String, stats : String, description : String, circle : String, image : String){
		this.name = name;
		this.stats = stats;
		this.description = description;
		this.circle = circle;
		this.image = image;
	}
}*/

class GUISpell{
	var name;
	var stats;
	var description;
	var circle;
	var image : Texture2D;
	
function GUISpell(name, stats, description, circle, image){
		this.name = name;
		this.stats = stats;
		this.description = description;
		this.circle = circle;
		this.image =  Resources.Load(image);
	}
}

//have a 'queued spell' always waiting. set its properties one by one with sendmessage, then call addSpellToList
var numberOfSpells = 0;
var queuedSpellName = "";
var queuedSpellStats = "";
var queuedSpellDescription = "";
var queuedSpellCircle = "";
var queuedSpellImagePath = "";
/*
function queueNewSpell(name){
	Debug.Log("Added spell"+ queuedSpell.name);
	queuedSpell.name = name;
}
function queueSpellStats(stats){
	Debug.Log("Added spell"+ queuedSpell.stats);
	queuedSpell.stats = stats;
}
function queueSpellDescription(description){
	queuedSpell.description = description;
}
function queueSpellCircle(circle){
	queuedSpell.circle = circle;
}
function queueSpellImage(image){
	queuedSpell.image = Resources.Load(image);
}*/

function addQueuedSpellToList(){
	var spell : GUISpell = new GUISpell(queuedSpellName, queuedSpellStats, queuedSpellDescription, queuedSpellCircle, queuedSpellImagePath);
	Debug.Log("Added spell"+ spell.name);
	spellList[numberOfSpells++] = spell;
	queuedSpellName = "";
	queuedSpellStats = "";
	queuedSpellDescription = "";
	queuedSpellCircle = "";
	queuedSpellImagePath = "";
}

/*function addSpellToList(name, stats, description, circle, image){
	//var spell = [name, stats, description, circle, image];	
	//Debug.Log(name + stats + description + circle + image);
	//spellList[numberOfSpells] = spell;
	//numberOfSpells+=1;
	Debug.Log("Added spell"+ name);
	var spell : Spell = new Spell(name, stats, description, circle, image);
	spellList[numberOfSpells++] = spell;
}*/

function Start(){ 
	
	drawSpell1 = false;
	drawSpell2 = false;
	drawSpell3 = false;
	drawSpell4 = false;
	
	fSpell.name = "";
	gSpell.name = "";
	vSpell.name = "";
	bSpell.name = "";
	
	queuedSpellName = "";
}

function incrementSpellLevel(name){
	for(i = 0; i<numberOfSpells; i++){
		if(spellList[i].name == name){
			spellList[i].stats = (parseInt(spellList[i].stats) + 1).ToString();
		}
	}
}

function AddSpikes(winX)
{
	spikeCount = Mathf.Floor(winX - 152)/22;
	GUILayout.BeginHorizontal();
	GUILayout.Label ("", "SpikeLeft");//-------------------------------- custom
	for (i = 0; i < spikeCount; i++)
        {
			GUILayout.Label ("", "SpikeMid");//-------------------------------- custom
        }
	GUILayout.Label ("", "SpikeRight");//-------------------------------- custom
	GUILayout.EndHorizontal();
}

function FancyTop(topX)
{
	leafOffset = (topX/2)-64;
	frameOffset = (topX/2)-27;
	skullOffset = (topX/2)-20;
	GUI.Label(new Rect(leafOffset, 18, 0, 0), "", "GoldLeaf");//-------------------------------- custom	
	GUI.Label(new Rect(frameOffset, 3, 0, 0), "", "IconFrame");//-------------------------------- custom	
	GUI.Label(new Rect(skullOffset, 12, 0, 0), "", "Skull");//-------------------------------- custom	
}

function WaxSeal(x,y)
{
	WSwaxOffsetX = x - 120;
	WSwaxOffsetY = y - 115;
	WSribbonOffsetX = x - 114;
	WSribbonOffsetY = y - 83;
	
	GUI.Label(new Rect(WSribbonOffsetX, WSribbonOffsetY, 0, 0), "", "RibbonBlue");//-------------------------------- custom	
	GUI.Label(new Rect(WSwaxOffsetX, WSwaxOffsetY, 0, 0), "", "WaxSeal");//-------------------------------- custom	
}

function DeathBadge(x,y)
{
	RibbonOffsetX = x;
	FrameOffsetX = x+3;
	SkullOffsetX = x+10;
	RibbonOffsetY = y+22;
	FrameOffsetY = y;
	SkullOffsetY = y+9;
	
	GUI.Label(new Rect(RibbonOffsetX, RibbonOffsetY, 0, 0), "", "RibbonRed");//-------------------------------- custom	
	GUI.Label(new Rect(FrameOffsetX, FrameOffsetY, 0, 0), "", "IconFrame");//-------------------------------- custom	
	GUI.Label(new Rect(SkullOffsetX, SkullOffsetY, 0, 0), "", "Skull");//-------------------------------- custom	
}

function DoMyWindow2 (windowID : int) 
{
	Debug.Log(queuedSpellName);
	if(queuedSpellName!="") addQueuedSpellToList();
	
		// use the spike function to add the spikes
		AddSpikes(windowRect2.width);

		GUILayout.Space(8);
		GUILayout.BeginVertical();
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true);
		        GUILayout.Label ("Spells");
				GUILayout.Label ("-", "Divider");		
				//draw Fire Spells TODO: DO IT IN ORDER OF SPELL STRENGTH / LAST ADDED!
				var testSpellButton  : GUIContent;
				testSpellButton = new GUIContent();
				var spell : GUISpell;	
				//fire loop
				GUILayout.Label("Circle: Fire","ShortLabel");
				for(i=0; i<numberOfSpells; i++){
					spell = spellList[i];
					if(spell!=null && spell.circle=="Fire"){
						GUILayout.BeginHorizontal();
							Debug.Log(spell.circle + "iteration "+i);
							Debug.Log(spell.image);
							testSpellButton.text = "\t"+spell.name+"\n\t"+spell.stats;
							testSpellButton.tooltip = "\t"+spell.description;
							testSpellButton.image = spell.image;
					        if(GUILayout.Button(testSpellButton,"ShortButton")){
					        	if(!drawSpell1){
					        		drawSpell1 = true;
					        		spell1Content.image = testSpellButton.image;
					        		fSpell = spell;
					        	}else if(!drawSpell2){
					        		drawSpell2 = true;
					        		spell2Content.image = testSpellButton.image;
					        		gSpell = spell;
					        	}else if(!drawSpell3){
					        		drawSpell3 = true;
					        		spell3Content.image = testSpellButton.image;
					        		vSpell = spell;
					        	}else if(!drawSpell4){
					        		drawSpell4 = true;
					        		spell4Content.image = testSpellButton.image;
					        		bSpell = spell;
					        	}
					        	//if we are full, push everything forward one, pop the last one)
					        	else{
					        		spell4Content.image = spell3Content.image;
					        		bSpell = vSpell;
					        		spell3Content.image = spell2Content.image;
					        		vSpell = gSpell;
					        		spell2Content.image = spell1Content.image;
					        		gSpell = fSpell;
					        		spell1Content.image = testSpellButton.image;
					        		fSpell = spell;
					        	}
					        }
					        GUILayout.Space(32);
					        GUILayout.Box(testSpellButton.tooltip,"PlainText");
				        GUILayout.EndHorizontal();
			        }
				}
				//ice loop
				GUILayout.Label("Circle: Ice","ShortLabel");
				for(i=0; i<numberOfSpells; i++){
					spell = spellList[i];
					if(spell!=null && spell.circle=="Ice"){
						GUILayout.BeginHorizontal();
							Debug.Log(spell.circle + "iteration "+i);
							Debug.Log(spell.image);
							testSpellButton.text = "\t"+spell.name+"\n\t"+spell.stats;
							testSpellButton.tooltip = "\t"+spell.description;
							testSpellButton.image = spell.image;
					        if(GUILayout.Button(testSpellButton,"ShortButton")){
					        	if(!drawSpell1){
					        		drawSpell1 = true;
					        		spell1Content.image = testSpellButton.image;
					        		fSpell = spell;
					        	}else if(!drawSpell2){
					        		drawSpell2 = true;
					        		spell2Content.image = testSpellButton.image;
					        		gSpell = spell;
					        	}else if(!drawSpell3){
					        		drawSpell3 = true;
					        		spell3Content.image = testSpellButton.image;
					        		vSpell = spell;
					        	}else if(!drawSpell4){
					        		drawSpell4 = true;
					        		spell4Content.image = testSpellButton.image;
					        		bSpell = spell;
					        	}
					        	//if we are full, push everything forward one, pop the last one)
					        	else{
					        		spell4Content.image = spell3Content.image;
					        		bSpell = vSpell;
					        		spell3Content.image = spell2Content.image;
					        		vSpell = gSpell;
					        		spell2Content.image = spell1Content.image;
					        		gSpell = fSpell;
					        		spell1Content.image = testSpellButton.image;
					        		fSpell = spell;
					        	}
					        }
					        GUILayout.Space(32);
					        GUILayout.Box(testSpellButton.tooltip,"PlainText");
				        GUILayout.EndHorizontal();
			        }
				}
				
				
	        GUILayout.EndScrollView();
			GUILayout.Space(8);
			
			//HroizSliderValue = GUILayout.HorizontalSlider(HroizSliderValue, 0.0, 1.1);
	        //VertSliderValue = GUILayout.VerticalSlider(VertSliderValue, 0.0, 1.1, GUILayout.Height(70));
	        
	        GUILayout.BeginHorizontal();
				GUILayout.Space(8);
				GUILayout.Box("F","OutlineText");
				GUILayout.Space(8);
				GUILayout.Box("G","OutlineText");
				GUILayout.Space(8);
				GUILayout.Box("V","OutlineText");
				GUILayout.Space(8);
				GUILayout.Box("B","OutlineText");
				GUILayout.Space(8);
			GUILayout.EndHorizontal();
			
			GUILayout.Space(8);
	        GUILayout.BeginHorizontal();
		                
				GUILayout.Space(8);
				
		        //draw selected spells
		        if(drawSpell1){
		        	GUILayout.Box(spell1Content.image,"IconFrame");
		        	GUILayout.Space(8);
		        }
		        if(drawSpell2){
		        	GUILayout.Box(spell2Content.image,"IconFrame");
		        	GUILayout.Space(8);
		        }
		        if(drawSpell3){
		        	GUILayout.Box(spell3Content.image,"IconFrame");
		        	GUILayout.Space(8);
		        }
		        if(drawSpell4){
		        	GUILayout.Box(spell4Content.image,"IconFrame");
		        	GUILayout.Space(8);
		        }
	        GUILayout.EndHorizontal();
	        
			DeathBadge(510,530);
        GUILayout.EndVertical();
		GUI.DragWindow (Rect (0,0,10000,10000));
		
		drawCursor();
	
}

function OnGUI ()
{
GUI.skin = mySkin;
	
if (doWindow2)
	windowRect2 = GUI.Window (2, windowRect2, DoMyWindow2, "");
	//now adjust to the group. (0,0) is the topleft corner of the group.
	GUI.BeginGroup (Rect (0,0,100,100));
	// End the group we started above. This is very important to remember!
	GUI.EndGroup ();
	
}

function drawCursor(){
		//draw custom cursor:
		var scaleFactor = 1.35f;
		var yourCursor = Resources.Load("Sprites/Cursor/metalCursor");  // Your cursor texture
		var cursorSizeX = 20* scaleFactor;  // Your cursor size x
		var cursorSizeY = 20 * scaleFactor;  // Your cursor size y

		GUI.DrawTexture (Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, cursorSizeX, cursorSizeY), yourCursor);
}