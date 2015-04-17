using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {
	
	public float maxHealth;
	public float currentHealth;
	public float maxMana;
	public float currentMana;
	public float maxHunger;
	public float currentHunger;

	public Texture hpBarTexture;
	public Texture manaBarTexture;
	public Texture hungerBarTexture;
	public Texture frameTexture;
	
	public Rect hpBarPosition;
	public Rect manaBarPosition;
	public Rect hungerBarPosition;
	public Rect position;

	public float ratioHealthWidth;
	public float ratioManaWidth;
	public float ratioHungerWidth;

	public float scaleFactor = 1;

	// Use this for initialization
	void Start () {

		hpBarTexture = (Texture2D)Resources.Load ("Sprites/HealthStatus/healthbar");
		manaBarTexture = (Texture2D)Resources.Load ("Sprites/HealthStatus/manabar");
		hungerBarTexture = (Texture2D)Resources.Load ("Sprites/HealthStatus/hungerbar");
		frameTexture = (Texture2D)Resources.Load ("Sprites/HealthStatus/StatusBars");

		hpBarPosition = new Rect (106, 75, 811, 28);
		manaBarPosition = new Rect(106, 116, 811, 28);
		hungerBarPosition = new Rect(106, 156, 811, 28);


	}
	
	// Update is called once per frame
	void Update () {

		ratioHealthWidth = ((float) (currentHealth / maxHealth));
		ratioManaWidth = ((float) (currentMana / maxMana));
		ratioHungerWidth = ((float) (currentHunger / maxHunger));
	}

	void OnGUI () {
		currentMana = 10;
		maxMana = 20;
		currentHealth = 10;
		position = new Rect(0, Screen.height - frameTexture.height * scaleFactor, frameTexture.width * scaleFactor, frameTexture.height * scaleFactor);

		//background
		GUI.Box (new Rect(position.x+90*scaleFactor, position.y+70*scaleFactor, 830*scaleFactor, 127*scaleFactor), "");
		
		//health bar
		GUI.DrawTexture (new Rect (hpBarPosition.x * scaleFactor + position.x, hpBarPosition.y * scaleFactor + position.y, hpBarPosition.width * ratioHealthWidth * scaleFactor, hpBarPosition.height * scaleFactor), hpBarTexture, ScaleMode.ScaleAndCrop);
		//mana bar
		GUI.DrawTexture (new Rect (manaBarPosition.x * scaleFactor + position.x, manaBarPosition.y * scaleFactor + position.y, manaBarPosition.width * ratioManaWidth * scaleFactor, manaBarPosition.height * scaleFactor), manaBarTexture, ScaleMode.ScaleAndCrop);
		//hunger bar
		GUI.DrawTexture (new Rect (hungerBarPosition.x * scaleFactor + position.x, hungerBarPosition.y * scaleFactor + position.y, hungerBarPosition.width * ratioHungerWidth * scaleFactor, hungerBarPosition.height * scaleFactor), hungerBarTexture, ScaleMode.ScaleAndCrop);
		
		//frame
		GUI.DrawTexture(new Rect(position.x,position.y, position.width,position.height), frameTexture);//, ScaleMode.ScaleToFit, true, 0

	}

	public void notifyHealthChange(float currentHealth, float maxHealth, float currentMana, float maxMana, float currentHunger, float maxHunger){
		this.currentHealth = currentHealth;
		this.maxHealth = maxHealth;
		this.currentMana = currentMana;
		this.maxMana = maxMana;
		this.currentHunger = currentHunger;
		this.maxHunger = maxHunger;
	}
}
