using UnityEngine;
using System.Collections;

public class CustomCursor {

	public static Texture2D yourCursor = (Texture2D) Resources.Load("Sprites/Cursor/metalCursor");  // Your cursor texture
	static int cursorSizeX = (int) (20* Inventory.scaleFactor);  // Your cursor size x
	static int cursorSizeY = (int) (20 * Inventory.scaleFactor);  // Your cursor size y


	public static void drawCursor(){
 			GUI.DrawTexture (new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, cursorSizeX, cursorSizeY), yourCursor);
	}
}
