using UnityEngine;
using System.Collections;

public class Slot {

	public Item item;
	public bool occupied;
	public Rect position;


	public Slot(Rect position){
		this.position = position;
		//Debug.Log (this+": my position is "+position);
	}

	public void draw(float frameX, float frameY) {
		if(item!=null && item.image!=null){
			GUI.DrawTexture (new Rect(frameX + position.x, frameY + position.y, position.width, position.height),item.image);
		}
	}
}
