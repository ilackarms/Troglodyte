var mySkin : GUISkin;

function OnGUI ()
{
	GUI.skin = mySkin;
	
	GUI.Label (Rect (40,10,200,20), "Camera rotate on/off");
	
	GUI.Label (Rect (210,10,200,20), "Background visibility");

}