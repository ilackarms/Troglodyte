using UnityEngine;
using System;
using System.Collections;

public class SubstanceGUI : MonoBehaviour {

	public Rect guiRect;
	public Vector2 scroll;
	public Texture2D colorPicker;
	public Transform creature;
	
	private ProceduralPropertyDescription[] creatureProperties;
	private ProceduralMaterial creatureSubstance;
	
	// Use this for initialization
	void Start () {
		creatureSubstance = creature.GetComponent<Renderer>().sharedMaterial as ProceduralMaterial;
		creatureProperties = creatureSubstance.GetProceduralPropertyDescriptions();
	
	}
	
	// Update is called once per frame
	void OnGUI() {
		guiRect = new Rect(guiRect.x, 40, Screen.width * 0.25f, Screen.height-40);
		if(creatureSubstance)
		{
			GUI.Window(0, guiRect, PropertiesGUI, "Creature Sample Demo");
		}
	
	}
	
	void PropertiesGUI(int guiID)	{
		scroll = GUILayout.BeginScrollView(scroll);
		
		int i = 0;
		while(i < creatureProperties.Length){
			ProceduralPropertyDescription creatureProperty = creatureProperties[i];
			ProceduralPropertyType creatureType = creatureProperties[i].type;
			
			if(creatureProperty.hasRange){
				if(creatureType == ProceduralPropertyType.Float){
					GUILayout.Label(creatureProperty.name);
					float creatureFloat = creatureSubstance.GetProceduralFloat(creatureProperty.name);
					float oldFloat = creatureFloat;
					creatureFloat = GUILayout.HorizontalSlider(creatureFloat, creatureProperty.minimum, creatureProperty.maximum);
					if(creatureFloat != oldFloat)
					{creatureSubstance.SetProceduralFloat(creatureProperty.name, creatureFloat);}
				}
			}
			else if(creatureType == ProceduralPropertyType.Color4){
				GUILayout.Label(creatureProperty.name);
				Color creatureColor = creatureSubstance.GetProceduralColor(creatureProperty.name);
				Color oldColor = creatureColor;
				
				Rect creatureRect = GUILayoutUtility.GetLastRect();
				
				if(GUILayout.RepeatButton(colorPicker)){
					Vector2 mousePosition = Event.current.mousePosition;
					float currentPickerPosX = mousePosition.x - creatureRect.x - 10.0f;
					float currentPickerPosY = (mousePosition.y - creatureRect.y + 5.0f)* (-1.0f);
					
					int x = Convert.ToInt32(currentPickerPosX);
					int y = Convert.ToInt32(currentPickerPosY);
					
					Color col = colorPicker.GetPixel(x,y);
					creatureColor = col;
				}
				
				if(creatureColor != oldColor){
					creatureSubstance.SetProceduralColor(creatureProperty.name, creatureColor);
				}
			}
					
			
			i++;
		}
		
		creatureSubstance.RebuildTextures();
		
		GUILayout.EndScrollView();
				
	}
}
