using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PieceContent : MonoBehaviour {

	public float percentOfRandomElementsToDisplay = 0.25f;

	public enum RegionType { Basic, Fire, Ice, Lightning, Crystal, Poison, UndergroundForest, Shadow, Civilized};

	//make assumptions about region types, it's the only way
	public GameObject[] basicRegionContents;
	public GameObject[] fireRegionContents;
	public GameObject[] iceRegionContents;
	public GameObject[] lightningRegionContents;
	public GameObject[] crystalRegionContents;
	public GameObject[] poisonRegionContents;
	public GameObject[] undergroundForestRegionContents;
	public GameObject[] shadowRegionContents;
	public GameObject[] civilizedRegionContents;

	public Dictionary<RegionType, GameObject[]> ContentDictionary;
	// Use this for initialization
	void Start () {
		ContentDictionary = new Dictionary<RegionType, GameObject[]>();
		ContentDictionary.Add(RegionType.Basic, basicRegionContents);
		ContentDictionary.Add(RegionType.Fire, fireRegionContents);
		ContentDictionary.Add(RegionType.Ice, iceRegionContents);
		ContentDictionary.Add(RegionType.Lightning, lightningRegionContents);
		ContentDictionary.Add(RegionType.Crystal, crystalRegionContents);
		ContentDictionary.Add(RegionType.Poison, poisonRegionContents);
		ContentDictionary.Add(RegionType.UndergroundForest, undergroundForestRegionContents);
		ContentDictionary.Add(RegionType.Shadow, undergroundForestRegionContents);
		ContentDictionary.Add(RegionType.Civilized, undergroundForestRegionContents);
		foreach(GameObject[] contents in ContentDictionary.Values){
			for(int i = 0; i < contents.Length; i++){
				contents[i].SetActive(false); //make sure everything is invisible on start
			}
		}

		//clamp range to 0-100%
		percentOfRandomElementsToDisplay = Mathf.Clamp(percentOfRandomElementsToDisplay, 0.0f, 1.0f);
	}

	public void initialize(){
		Start();
	}

	public void populate(string regionName){
		RegionType regionType = convertStringToRegion(regionName);
		List<GameObject> randomGameObjects = new List<GameObject>();
		List<GameObject> contents = new List<GameObject>();
		foreach(GameObject o in ContentDictionary[regionType]){
			contents.Add(o);
		}

		while(contents.Count > 0){
			GameObject o = contents[Random.Range(0, contents.Count)];
			randomGameObjects.Add(o);
			contents.Remove(o);
		}
		for(int i = 0; i < randomGameObjects.Count * percentOfRandomElementsToDisplay; i++){
			//SetActive method:
			////randomGameObjects[i].SetActive(true);

			//INSTANTIATE METHOD (copies) -> could be good for SPAWNING
			//position of instantiated objects = local position of object relative to piece + transform of piece
			//objects must all be children of piece... does that make sense? :/
			//think of possible uses.. when monsters die, piece gets altered in some way.
			GameObject obj = (GameObject) Instantiate(randomGameObjects[i], transform.position, randomGameObjects[i].transform.rotation );
			obj.SetActive(true); //in case the instantiated objects are still inactive
		}
	}

	static RegionType convertStringToRegion(string regionName){
		if(regionName == "Basic") return RegionType.Basic;
		if(regionName == "Civilized") return RegionType.Civilized;
		if(regionName == "Crystal") return RegionType.Crystal;
		if(regionName == "Fire") return RegionType.Fire;
		if(regionName == "Ice") return RegionType.Ice;
		if(regionName == "Lightning") return RegionType.Lightning;
		if(regionName == "Poison") return RegionType.Poison;
		if(regionName == "Shadow") return RegionType.Shadow;
		if(regionName == "UndergroundForest") return RegionType.UndergroundForest;
		//default
		return RegionType.Basic;
	}
}
