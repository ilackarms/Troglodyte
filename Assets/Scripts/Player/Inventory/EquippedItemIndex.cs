using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquippedItemIndex : MonoBehaviour {
	static public Dictionary<string, GameObject> inputItemDatabase = new Dictionary<string, GameObject>();

	public GameObject[] weaponArray;// = new GameObject[publicArrayLength];

	public static GameObject unarmed;

	public void Awake() {

		for(int i = 0; i< weaponArray.Length; i++){
//			Debug.Log(weaponArray.Length);
			if(weaponArray[i]!=null){
	//			Debug.Log(weaponArray[i].name);
				GameObject equippedModel = weaponArray[i];
	//			Debug.Log("adding item to equipped model index: "+equippedModel.name);
				inputItemDatabase.Add(equippedModel.name, equippedModel);
				equippedModel.SetActive(false);
			}
		}
		
		unarmed = inputItemDatabase["Unarmed"];
		unarmed.SetActive(true);
	}


	public GameObject getEquippedModel(string weaponName){
		if(!inputItemDatabase.ContainsKey(weaponName)) return null;
		return inputItemDatabase [weaponName];
	}
}