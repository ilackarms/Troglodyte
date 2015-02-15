using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PieceContent))]
public class Piece : MonoBehaviour {
	
	public MapGenerator.Cell[,,] cells;
	public Vector3 dimensions;
	public Vector3[] upConnectors;
	public Vector3[] downConnectors;
	public Vector3[] leftConnectors;
	public Vector3[] rightConnectors;

	public string debugCellContents;
	
	public Vector3 positionInGrid;
	public Vector3 specialShift;
	public Vector3 selectedConnector;
	public bool needsShift = false;

	public Dictionary<Vector3, Vector3> shiftDictionary;
	public Vector3[] keys;
	public Vector3[] values;

	private PieceContent contents;

	void Start(){
		//cells = new MapGenerator.Cell[(int) dimensions.x, (int) dimensions.y, (int) dimensions.z];
	}

	public void initialize(){
		cells = new MapGenerator.Cell[(int) dimensions.x, (int) dimensions.y, (int) dimensions.z];


		for(int x = 0; x< cells.GetLength(0); x++){
			for(int y = 0; y< cells.GetLength(1); y++){
				for(int z = 0; z< cells.GetLength(2); z++){
					cells[x,y,z] = new MapGenerator.Cell();
					cells[x,y,z].occupied = true;
					cells[x,y,z].nameOfPiece = this.gameObject.name;
					//set all connectors
					for(int i=0; i < upConnectors.Length; i++){
						if(upConnectors[i].x == x && upConnectors[i].y == y && upConnectors[i].z == z) {
							cells[x,y,z].occupiedByUpConnector = true;
						}
					}
					for(int i=0; i < downConnectors.Length; i++){
						if(downConnectors[i].x == x && downConnectors[i].y == y && downConnectors[i].z == z) {
							cells[x,y,z].occupiedByDownConnector = true;
						}
					}
					for(int i=0; i < leftConnectors.Length; i++){
						if(leftConnectors[i].x == x && leftConnectors[i].y == y && leftConnectors[i].z == z) {
							cells[x,y,z].occupiedByLeftConnector = true;
						}
					}
					for(int i=0; i < rightConnectors.Length; i++){
						if(rightConnectors[i].x == x && rightConnectors[i].y == y && rightConnectors[i].z == z) {
							cells[x,y,z].occupiedByRightConnector = true;
						}
					}
					
				}
			}
		}

		shiftDictionary = new Dictionary<Vector3, Vector3>();
		shiftDictionary.Clear();
		for(int i = 0; i < keys.Length; i++){
			shiftDictionary.Add(keys[i], values[i]);
		}

		contents = gameObject.GetComponent<PieceContent>();
		contents.initialize();
	}

	void Awake(){
		initialize();
	}

	/// <summary>
	/// Determines how to shift the piece (just the MODEL) based on which connector was selected
	/// This is calculated manually. 
	/// it works like this:
	/// input (0,1). (0,1) is in a dictionary, gives back another vector (-1, -1); this is our special shift
	/// if 
	/// </summary>
	/// <returns>The special shift.</returns>
	/// <param name="connectorPosition">Connector position.</param>
	public Vector3 calculateSpecialShift(Vector3 connectorPosition){
		foreach(Vector3 key in shiftDictionary.Keys){
			if(connectorPosition.x == key.x && connectorPosition.y == key.y && connectorPosition.z == key.z){
				//Debug.LogError("SHIFTING BY"+shiftDictionary[key]);
				return shiftDictionary[key];
			}
		}
		return new Vector3(0,0,0);
	}

	public bool matches(MapGenerator.Cell cell){
		bool match = true;
		if(cell.needsUpConnector && upConnectors.Length<1) match = false;
		if(cell.needsDownConnector && downConnectors.Length<1) match = false;
		if(cell.needsLeftConnector && leftConnectors.Length<1) match = false;
		if(cell.needsRightConnector && rightConnectors.Length<1) match = false;
		return match;
	}

	public void populate(string regionType){
		contents.populate(regionType);
	}
}
