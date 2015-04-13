using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using Pathfinding;

public class MapGenerator : MonoBehaviour {

	public string regionType;
	public List<Piece> instantiatedPieces;

	Cell [ , , ] cell3D;
	public GameObject[] PieceObjects;
	public GameObject[] DeadEndObjects;
	private Piece[] Pieces;
	private Piece[] DeadEnds;
	private Queue<Vector3> connectorNeededQueue;
	private AstarPath Pathfinding;
	
	public GameObject DebugCell;
	
	private float cellXDimension = 28.5f;
	private float cellYDimension = 30f;
	private float cellZDimension = 28.5f;
	
	// Use this for initialization
	void Start () {
		Pathfinding = GetComponent<AstarPath>();

		cell3D = new Cell[8,8,1];
		
		for(int x = 0; x< cell3D.GetLength(0); x++){
			for(int y = 0; y< cell3D.GetLength(1); y++){
				for(int z = 0; z< cell3D.GetLength(2); z++){
					cell3D[x,y,z] = new Cell();
				}
			}
		}
		Pieces = new Piece[PieceObjects.Length];
		for(int i = 0; i < PieceObjects.Length; i++){
			Pieces[i] = PieceObjects[i].GetComponent<Piece>();
			Pieces[i].initialize();
			//Pieces[i].gameObject.AddComponent<PieceContent>();
		}
		
		DeadEnds = new Piece[DeadEndObjects.Length];
		for(int i = 0; i < DeadEndObjects.Length; i++){
			DeadEnds[i] = DeadEndObjects[i].GetComponent<Piece>();
			DeadEnds[i].initialize();
		}
		
		connectorNeededQueue = new Queue<Vector3>();
		int startX = (int) cell3D.GetLength(0)/2;
		int startY = (int) cell3D.GetLength(1)/2;
		int startZ = (int) cell3D.GetLength(2)/2;
		connectorNeededQueue.Enqueue(new Vector3(startX,startY,startZ)); //first connector at 0,0,0! should be a bend or dead end
		cell3D[startX,startY,startZ].needsUpConnector = true;
		cell3D[startX,startY,startZ].needsRightConnector = true;
		cell3D[startX,startY,startZ].needsLeftConnector = true;
		cell3D[startX,startY,startZ].needsDownConnector = true;

		StartCoroutine(generateMap());
		instantiatedPieces = new List<Piece>();
	}
	
	IEnumerator generateMap(){
		while(connectorNeededQueue.Count > 0){
			yield return new WaitForSeconds(0.05f);
			Vector3 addConnectorHere = connectorNeededQueue.Dequeue();
			Debug.Log ("Trying to add a piece here:"+addConnectorHere);
			Cell cell = cell3D[(int) addConnectorHere.x, (int) addConnectorHere.y, (int) addConnectorHere.z];
			if(cell.needsConnector())attachPiece(addConnectorHere);	
			
			foreach(Vector3 v in connectorNeededQueue){
				Debug.Log(v);
			}
		}

		configureAstar();
		populateMap();

        GameObject.FindGameObjectWithTag("Pathfinder").SetActive(true);

		Debug.Log ("Finished!");
				
		string bigAssLogMessage = "";
		for(int x = 0; x < cell3D.GetLength(0); x++){
			for(int y = 0; y < cell3D.GetLength(1); y++){
				for(int z = 0; z < cell3D.GetLength(2); z++){
					bigAssLogMessage+="Piece in ["+x+","+y+","+z+"] is " + cell3D[x,y,z].nameOfPiece + "\n";
					//GameObject debugCell = (GameObject) Instantiate(DebugCell, new Vector3((x - startX) * cellXDimension, 
					//                                                                       (z - startZ) * cellYDimension,
					//                                                                       (y - startY) * cellZDimension), DebugCell.gameObject.transform.rotation);
					//debugCell.GetComponent<Piece>().positionInGrid = new Vector3(x,y,z);
					//debugCell.GetComponent<Piece>().debugCellContents = cell3D[x,y,z].nameOfPiece+" "+cell3D[x,y,z].occupied;
				}
			}
		}
		
		Debug.Log (bigAssLogMessage);

	}
	
	void attachPiece(Vector3 connectorNeededPosition){
		Cell cell = cell3D[(int) connectorNeededPosition.x,
		                   (int) connectorNeededPosition.y,
		                   (int) connectorNeededPosition.z];
		List<Piece> possibleMatches = findMatchingPieces(cell);
		if(possibleMatches.Count<1){
			throw new UnityException("No possible matches for this cell! (Why wouldn't a dead end work?)");
		}
		
		//shuffle list
		List<Piece> randomizedPieceList = new List<Piece>();
		
		while(possibleMatches.Count > 0){
			Piece piece = possibleMatches[Random.Range(0, possibleMatches.Count)];
			randomizedPieceList.Add(piece);
			possibleMatches.Remove(piece);
		}
		/*
		//sort list by priority
		List<Piece> prioritizedList = new List<Piece>();
		
		for(int priority = 10; priority >= 0; priority--){
			foreach(Piece p in randomizedPieceList){
				if(p.priority >= priority && !prioritizedList.Contains(p)){
					prioritizedList.Add(p);
				}
			}
		}
		if(randomizedPieceList.Count!=prioritizedList.Count) Debug.LogError("Not all pieces were moved to prioritized random list!");
		*/
		
		Debug.Log("Searching for the right piece for "+connectorNeededPosition);
		
		//todo: set priority list to read in order
		//foreach(Piece piece in prioritizedList){
		for(int i = 0; i < randomizedPieceList.Count; i++){
			Piece piece = randomizedPieceList[i];
			if(piece.matches(cell)){
				Vector3 position = translateOriginOfPiece(connectorNeededPosition, piece);
				//position = connectorNeededPosition;

				if(canAddPiece(position, piece)){
					piece.specialShift = piece.calculateSpecialShift(piece.selectedConnector);					
					int startX = (int) cell3D.GetLength(0)/2;
					int startY = (int) cell3D.GetLength(1)/2;
					int startZ = (int) cell3D.GetLength(2)/2;
					GameObject pieceCopy = (GameObject) Instantiate(piece.gameObject, 
                                new Vector3((connectorNeededPosition.x - startX + piece.specialShift.x)  * cellXDimension, 
					            (connectorNeededPosition.z - startZ + piece.specialShift.z) * cellYDimension,
					            (connectorNeededPosition.y - startY + piece.specialShift.y  )* cellZDimension)
					                                                , piece.gameObject.transform.rotation);
					addPiece(position, piece);
					Piece pieceCopyScript = pieceCopy.GetComponent<Piece>();
					pieceCopyScript.positionInGrid = position;
					instantiatedPieces.Add(pieceCopyScript);
					Debug.Log("Attaching piece: "+piece.gameObject.name+" "+ connectorNeededPosition + "\n Number of possible matches: " + possibleMatches.Count);
					return;
				}
			}
		}
		
		Piece deadEnd = findDeadEnd(connectorNeededPosition);
		if(canAddPiece(connectorNeededPosition, deadEnd)){		
			int startX = (int) cell3D.GetLength(0)/2;
			int startY = (int) cell3D.GetLength(1)/2;
			int startZ = (int) cell3D.GetLength(2)/2;
			GameObject pieceCopy =  (GameObject)Instantiate(deadEnd.gameObject, 
			                                                new Vector3((connectorNeededPosition.x - startX) * cellXDimension, 
			            (connectorNeededPosition.z - startZ) * cellYDimension,
			            (connectorNeededPosition.y- startY) * cellZDimension)
			                                                , deadEnd.gameObject.transform.rotation);
			addPiece(connectorNeededPosition, deadEnd);
			Piece pieceCopyScript = pieceCopy.GetComponent<Piece>();
			pieceCopyScript.positionInGrid = connectorNeededPosition;
			instantiatedPieces.Add(pieceCopyScript);
			return;
		}
		
		foreach(Piece piece in DeadEnds){
			Vector3 position = connectorNeededPosition;
			if(piece.matches(cell) && !cell.occupied){	
				int startX = (int) cell3D.GetLength(0)/2;
				int startY = (int) cell3D.GetLength(1)/2;
				int startZ = (int) cell3D.GetLength(2)/2;
				GameObject pieceCopy = (GameObject) Instantiate(piece.gameObject, 
				                                                new Vector3((connectorNeededPosition.x - startX) * cellXDimension, 
				            (connectorNeededPosition.z - startZ) * cellYDimension,
				            (connectorNeededPosition.y- startY) * cellZDimension)
				                                                , piece.gameObject.transform.rotation);
				addPiece(position, piece);
				Piece pieceCopyScript = pieceCopy.GetComponent<Piece>();
				pieceCopyScript.positionInGrid = connectorNeededPosition;
				instantiatedPieces.Add(pieceCopyScript);
				Debug.Log("Attaching piece: "+ connectorNeededPosition + "\n Number of possible matches: " + possibleMatches.Count);
				return;
			}
		}
		//Debug.LogError("Cannot find a suitable piece at "+ connectorNeededPosition + "\n Cell occupied = "+cell.occupied+" NeedsUp/Down/Left/Right:"+cell.needsUpConnector+cell.needsDownConnector+cell.needsLeftConnector+cell.needsRightConnector);
	}
	
	//assumes we have eliminated all pieces that don't contain every single connector we need
	//this means for 2 needed connectors, we get tunnel
	//3 needed connectors, fork
	//4 needed, cross
	public Vector3 translateOriginOfPiece(Vector3 positionOfConnector, Piece piece){
		Cell cell = cell3D[(int) positionOfConnector.x, (int) positionOfConnector.y, (int) positionOfConnector.z];
		//Debug.Log (positionOfConnector + " " + cell.needsUpConnector + cell.needsDownConnector + cell.needsLeftConnector + cell.needsRightConnector);
		if(cell.needsUpConnector){
			//if piece has more than 1 of the necessary connector, randomly pick one to translate to
			int i = Random.Range(0, piece.upConnectors.Length);
			if(i > piece.upConnectors.Length - 1) i = piece.upConnectors.Length-1;
			piece.selectedConnector = piece.upConnectors[i];
			//Debug.LogWarning("Piece "+ piece.gameObject.name +" shifted from "+positionOfConnector+" to "+(positionOfConnector - piece.upConnectors[i]));
			return (positionOfConnector - piece.upConnectors[i]);
		}
		if(cell.needsDownConnector){
			//if piece has more than 1 of the necessary connector, randomly pick one to translate to
			int i = Random.Range(0, piece.downConnectors.Length);
			if(i > piece.downConnectors.Length - 1) i = piece.downConnectors.Length-1;
			piece.selectedConnector = piece.downConnectors[i];
			//Debug.LogWarning("Piece "+ piece.gameObject.name +" shifted from "+positionOfConnector+" to "+(positionOfConnector - piece.downConnectors[i]));
			return (positionOfConnector - piece.downConnectors[i]);
		}
		if(cell.needsLeftConnector){
			//if piece has more than 1 of the necessary connector, randomly pick one to translate to
			int i = Random.Range(0, piece.leftConnectors.Length);
			if(i > piece.leftConnectors.Length - 1) i = piece.leftConnectors.Length-1;
			piece.selectedConnector = piece.leftConnectors[i];
			//Debug.LogWarning("Piece "+ piece.gameObject.name +" shifted from "+positionOfConnector+" to "+(positionOfConnector - piece.leftConnectors[i]));
			return (positionOfConnector - piece.leftConnectors[i]);
		}
		if(cell.needsRightConnector){
			//if piece has more than 1 of the necessary connector, randomly pick one to translate to
			int i = Random.Range(0, piece.rightConnectors.Length);
			if(i > piece.rightConnectors.Length - 1) i = piece.rightConnectors.Length-1;
			piece.selectedConnector = piece.rightConnectors[i];
			//Debug.LogWarning("Piece "+ piece.gameObject.name +" shifted from "+positionOfConnector+" to "+(positionOfConnector - piece.rightConnectors[i]));
			return (positionOfConnector - piece.rightConnectors[i]);
		}
		else throw new UnityException(piece.gameObject.name + " was selected which does not have any of the necessary connectors\nCell needs U:" + cell.needsUpConnector + " D:"+cell.needsDownConnector+" L:"+cell.needsLeftConnector+" R:"+cell.needsRightConnector);
	}
	
	public List<Piece> findMatchingPieces(Cell cell){
		List<Piece> possibleMatches = new List<Piece>(Pieces);
		List<Piece> finalResult = new List<Piece>(Pieces);
		if(cell.needsUpConnector){
			foreach(Piece piece in possibleMatches){
				if(piece.upConnectors.Length==0 && finalResult.Contains(piece)){
					finalResult.Remove(piece);
				}
			}
		}
		if(cell.needsDownConnector){
			foreach(Piece piece in possibleMatches){
				if(piece.downConnectors.Length==0 && finalResult.Contains(piece)){
					finalResult.Remove(piece);
				}
			}
		}
		if(cell.needsLeftConnector){
			foreach(Piece piece in possibleMatches){
				if(piece.leftConnectors.Length==0 && finalResult.Contains(piece)){
					finalResult.Remove(piece);
				}
			}
		}
		if(cell.needsRightConnector){
			foreach(Piece piece in possibleMatches){
				if(piece.rightConnectors.Length==0 && finalResult.Contains(piece)){
					finalResult.Remove(piece);
				}
			}
		}
		return finalResult;
	}
	
	public Piece findDeadEnd(Vector3 position){
		Cell cell = cell3D[(int) position.x, (int) position.y, (int) position.z];
		if(cell.needsRightConnector){
			foreach(Piece piece in DeadEnds){
				if(piece.rightConnectors.Length > 0){
					return piece;
				}
			}
		}
		if(cell.needsLeftConnector){
			foreach(Piece piece in DeadEnds){
				if(piece.leftConnectors.Length > 0){
					return piece;
				}
			}
		}
		if(cell.needsUpConnector){
			foreach(Piece piece in DeadEnds){
				if(piece.upConnectors.Length > 0){
					return piece;
				}
			}
		}
		if(cell.needsDownConnector){
			foreach(Piece piece in DeadEnds){
				if(piece.downConnectors.Length > 0){
					return piece;
				}
			}
		}
		throw new UnityException("No dead end fits that space! wtf?");
	}
	
	public void addPiece(Vector3 position, Piece piece) { 
		if(canAddPiece(position, piece)){
			for(int x = 0; x < piece.dimensions.x ; x++){
				for(int y = 0; y < piece.dimensions.y; y++){
					for(int z = 0; z < piece.dimensions.z; z++){
						cell3D[x + (int) position.x, y + (int) position.y, z + (int) position.z] = piece.cells[x,y,z].copy();
					}
				}
			}
			foreach(Vector3 upConnector in piece.upConnectors){
				int x = (int) position.x + (int) upConnector.x;
				int y = (int) position.y + (int) upConnector.y + 1;
				int z = (int) position.z + (int) upConnector.z;
				if(!cell3D[x,y,z].occupiedByDownConnector){
					cell3D[x,y,z].needsDownConnector = true;
					bool addToQueue = true;
					foreach(Vector3 v in connectorNeededQueue){
						if( (int) v.x == x && (int) v.y == y && (int) v.z == z){
							addToQueue = false;
							break;
						}
					}
					if(addToQueue){
						connectorNeededQueue.Enqueue(new Vector3(x,y,z));
					}
					Debug.Log(x+","+y+","+z+" enqueued");
				}
			}
			
			foreach(Vector3 downConnector in piece.downConnectors){
				int x = (int) position.x + (int) downConnector.x;
				int y = (int) position.y + (int) downConnector.y - 1;
				int z = (int) position.z + (int) downConnector.z;
				if(!cell3D[x,y,z].occupiedByUpConnector){
					cell3D[x,y,z].needsUpConnector = true;
					bool addToQueue = true;
					foreach(Vector3 v in connectorNeededQueue){
						if( (int) v.x == x && (int) v.y == y && (int) v.z == z){
							addToQueue = false;
							break;
						}
					}
					if(addToQueue){
						connectorNeededQueue.Enqueue(new Vector3(x,y,z));
					}
				}
			}
			
			foreach(Vector3 leftConnector in piece.leftConnectors){
				int x = (int) position.x + (int) leftConnector.x - 1;
				int y = (int) position.y + (int) leftConnector.y;
				int z = (int) position.z + (int) leftConnector.z;
				if(!cell3D[x,y,z].occupiedByRightConnector){
					cell3D[x,y,z].needsRightConnector = true;
					bool addToQueue = true;
					foreach(Vector3 v in connectorNeededQueue){
						if( (int) v.x == x && (int) v.y == y && (int) v.z == z){
							addToQueue = false;
							break;
						}
					}
					if(addToQueue){
						connectorNeededQueue.Enqueue(new Vector3(x,y,z));
					}
				}
			}
			
			foreach(Vector3 rightConnector in piece.rightConnectors){
				int x = (int) position.x + (int) rightConnector.x + 1;
				int y = (int) position.y + (int) rightConnector.y;
				int z = (int) position.z + (int) rightConnector.z;
				if(!cell3D[x,y,z].occupiedByLeftConnector){
					cell3D[x,y,z].needsLeftConnector = true;
					bool addToQueue = true;
					foreach(Vector3 v in connectorNeededQueue){
						if( (int) v.x == x && (int) v.y == y && (int) v.z == z){
							addToQueue = false;
							break;
						}
					}
					if(addToQueue){
						connectorNeededQueue.Enqueue(new Vector3(x,y,z));
					}
				}
			}
		}
		
		Debug.Log("After piece "+piece+" was added, current queue: "+connectorNeededQueue+ " of length "+ connectorNeededQueue.Count);
		
	}
	
	//position will be the 'origin' of where the piece will be placed
	//connector will be ligned up before
	public bool canAddPiece(Vector3 position, Piece piece) {
		//detect whether piece is outside bounds of cell3D
		if(position.x < 0){
			//			Debug.LogError("to the left of 0");
			return false; 
		}
		if(position.y < 0) {
			//			Debug.LogError("beneath 0");
			return false; 
		}
		if(position.z < 0) {
			//			Debug.LogError("under 0");
			return false; 
		}
		if(position.x + piece.dimensions.x > cell3D.GetLength(0)){ 
			//			Debug.LogError("if size of piece is beyond x dimension");
			return false; 
		}
		if(position.y + piece.dimensions.y > cell3D.GetLength(1)){ 
			//			Debug.LogError("if size of piece is beyond y dimension");
			return false; 
		}
		if(position.z + piece.dimensions.z > cell3D.GetLength(2)){
			//			Debug.LogError("if size of piece is beyond z dimension");
			return false; 
		}
		
		//detect whether any cell is occupied
		for(int x = 0; x < piece.dimensions.x ; x++){
			for(int y = 0; y < piece.dimensions.y; y++){
				for(int z = 0; z < piece.dimensions.z; z++) {
					if(cell3D[(int) position.x + x, (int) position.y + y,(int) position.z + z].occupied) {
						//						Debug.LogError("cell is occupied");
						return false;
					}
				}
			}
		}
		
		//detect whether any cell requires a connector which this piece does not have!
		for(int x = 0; x < piece.dimensions.x ; x++){
			for(int y = 0; y < piece.dimensions.y; y++){
				for(int z = 0; z < piece.dimensions.z; z++) {
					int needConnectorX = (int) position.x + x;
					int needConnectorY = (int) position.y + y;
					int needConnectorZ = (int) position.z + z;
					Cell cell = cell3D[needConnectorX, needConnectorY, needConnectorZ];
					if(!piece.matches(cell)) return false;
					
					if(cell.needsUpConnector) {
						bool matchingConnector = false;
						//loop through all up connector positions in piece; return false if none match
						foreach(Vector3 connectorLocation in piece.upConnectors){
							int connectorX = (int) position.x + (int) connectorLocation.x;
							int connectorY = (int) position.y + (int) connectorLocation.y;
							int connectorZ = (int) position.z + (int) connectorLocation.z;
							//does absolute position of connector matchposition of connectorneeded?
							if(connectorX == needConnectorX && connectorY == needConnectorY && connectorZ == needConnectorZ){
								matchingConnector = true;
								break;
							}
						}
						if(!matchingConnector){
							//						Debug.`("cell has a connector requirement that does not match");
							return false;
						}
					}
					
					if(cell.needsDownConnector) {
						bool matchingConnector = false;
						//loop through all up connector positions in piece; return false if none match
						foreach(Vector3 connectorLocation in piece.downConnectors){
							int connectorX = (int) position.x + (int) connectorLocation.x;
							int connectorY = (int) position.y + (int) connectorLocation.y;
							int connectorZ = (int) position.z + (int) connectorLocation.z;
							//does absolute position of connector matchposition of connectorneeded?
							if(connectorX == needConnectorX && connectorY == needConnectorY && connectorZ == needConnectorZ){
								matchingConnector = true;
								break;
							}
						}
						if(!matchingConnector){
							//						Debug.LogError("cell has a connector requirement that does not match");
							return false;
						}
					}
					
					if(cell.needsLeftConnector) {
						bool matchingConnector = false;
						//loop through all up connector positions in piece; return false if none match
						foreach(Vector3 connectorLocation in piece.leftConnectors){
							int connectorX = (int) position.x + (int) connectorLocation.x;
							int connectorY = (int) position.y + (int) connectorLocation.y;
							int connectorZ = (int) position.z + (int) connectorLocation.z;
							//does absolute position of connector matchposition of connectorneeded?
							if(connectorX == needConnectorX && connectorY == needConnectorY && connectorZ == needConnectorZ){
								matchingConnector = true;
								break;
							}
						}
						if(!matchingConnector){
							//						Debug.LogError("cell has a connector requirement that does not match");
							return false;
						}
					}
					
					if(cell.needsRightConnector) {
						bool matchingConnector = false;
						//loop through all up connector positions in piece; return false if none match
						foreach(Vector3 connectorLocation in piece.rightConnectors){
							int connectorX = (int) position.x + (int) connectorLocation.x;
							int connectorY = (int) position.y + (int) connectorLocation.y;
							int connectorZ = (int) position.z + (int) connectorLocation.z;
							//does absolute position of connector matchposition of connectorneeded?
							if(connectorX == needConnectorX && connectorY == needConnectorY && connectorZ == needConnectorZ){
								matchingConnector = true;
								break;
							}
						}
						if(!matchingConnector){
							//						Debug.LogError("cell has a connector requirement that does not match");
							return false;
						}
					}
				}
			}
		}
		
		//detect whether the spot adjacent to a connector is occupied
		foreach(Vector3 upConnector in piece.upConnectors){
			//connector connects off of grid space
			if((int) position.x + (int) upConnector.x >= cell3D.GetLength(0)){
				//Debug.LogError("upconnector is off grid");
				return false;
			}
			
			if((int) position.y + (int) upConnector.y + 1 >= cell3D.GetLength(1)){
				//Debug.LogError("upconnector is off grid");
				return false;
			}
			
			if((int) position.z + (int) upConnector.z >= cell3D.GetLength(2)){
				//Debug.LogError("upconnector is off grid");
				return false;
			}
			
			//space next to connector is occupied and NOT a down connector
			if(cell3D[(int) position.x + (int) upConnector.x, 
			          (int) position.y + (int) upConnector.y + 1,
			          (int) position.z + (int) upConnector.z].occupied &&
			   !cell3D[(int) position.x + (int) upConnector.x, 
			        (int) position.y + (int) upConnector.y + 1, 
			        (int) position.z + (int) upConnector.z].occupiedByDownConnector)
			{
				
				//				Debug.LogError("space next to connector is occupied and NOT a down connector");
				return false;
			}
		}
		
		foreach(Vector3 downConnector in piece.downConnectors){
			//connector connects off of grid space
			if((int) position.y + (int) downConnector.y - 1< 0) 
			{
				//				Debug.LogError("if piece connects to spot below 0");
				return false; //if piece connects to spot below 0
			}

			if (((int) position.x + (int) downConnector.x)>=cell3D.GetLength(0)) return false;
			if (((int) position.y + (int) downConnector.y - 1)>=cell3D.GetLength(1)) return false;
			if (((int) position.z + (int) downConnector.z)>=cell3D.GetLength(2)) return false;
			
			//space next to connector is occupied and not an UP connector
			if(cell3D[((int) position.x + (int) downConnector.x), ((int) position.y + (int) downConnector.y - 1),((int)  position.z + (int) downConnector.z)].occupied &&
			   !cell3D[((int) position.x + (int) downConnector.x), ((int) position.y + (int) downConnector.y - 1),((int)  position.z + (int) downConnector.z)].occupiedByUpConnector)
			{
				//				Debug.LogError("space next to connector is occupied and not an UP connector");
				return false;
			}
		}
		
		foreach(Vector3 leftConnector in piece.leftConnectors){
			//connector connects off of grid space
			if((int) position.x + (int) leftConnector.x - 1< 0){
				//Debug.LogError("connector connects off of grid space");
				return false; //if piece connects to spot to the left of 0
			}

			
			if (((int) position.x + (int) leftConnector.x-1)>=cell3D.GetLength(0)) return false;
			if (((int) position.y + (int) leftConnector.y)>=cell3D.GetLength(1)) return false;
			if (((int) position.z + (int) leftConnector.z)>=cell3D.GetLength(2)) return false;
			
			//space next to connector is occupied and not a right connector
			if(cell3D[(int) position.x + (int) leftConnector.x - 1, (int) position.y + (int) leftConnector.y, (int) position.z + (int) leftConnector.z].occupied &&
			   !cell3D[(int) position.x + (int) leftConnector.x -1 , (int) position.y + (int) leftConnector.y,(int)  position.z + (int) leftConnector.z].occupiedByRightConnector)
			{
				//Debug.LogError("space next to connector is occupied and not a right connector");
				return false;
			}
		}
		
		foreach(Vector3 rigtConnector in piece.rightConnectors){
			//connector connects off of grid space
			if((position.x + rigtConnector.x + 1>= cell3D.GetLength(0)) || (position.y + rigtConnector.y >= cell3D.GetLength(1)) ||
			   (position.z + rigtConnector.z>= cell3D.GetLength(2))){
				//Debug.LogError("connector connects off of grid space");
				return false; //if piece connects to spot to the right of grid x
			}
			
//						Debug.LogWarning(position.x + rigtConnector.x + 1 +","+position.y + rigtConnector.y+","+position.z + rigtConnector.z);
			
			if (((int) position.x + (int) rigtConnector.x+1)>=cell3D.GetLength(0)) return false;
			if (((int) position.y + (int) rigtConnector.y)>=cell3D.GetLength(1)) return false;
			if (((int) position.z + (int) rigtConnector.z)>=cell3D.GetLength(2)) return false;
			//space next to connector is occupied and not a right connector
			if(cell3D[(int) position.x + (int) rigtConnector.x + 1, (int) position.y + (int) rigtConnector.y, (int) position.z + (int) rigtConnector.z].occupied &&
			   !cell3D[(int) position.x + (int) rigtConnector.x +1 , (int) position.y + (int) rigtConnector.y, (int) position.z + (int) rigtConnector.z].occupiedByLeftConnector)
			{
				//Debug.LogError("space next to connector is occupied and not a right connector");
				return false;
			}
		}
		
		return true;
	}
	
	public void finalPass(){
		
	}
	
	
	public class Cell{
		public bool occupied;
		public bool needsDownConnector;
		public bool needsUpConnector;
		public bool needsLeftConnector;
		public bool needsRightConnector;
		public bool occupiedByUpConnector;
		public bool occupiedByDownConnector;
		public bool occupiedByLeftConnector;
		public bool occupiedByRightConnector;
		
		public string nameOfPiece;
		
		public Cell(){
			nameOfPiece = "";
			occupied = false;
			needsUpConnector = false;
			needsDownConnector = false;
			needsLeftConnector = false;
			needsRightConnector = false;
			occupiedByDownConnector = false;
			occupiedByLeftConnector = false;
			occupiedByUpConnector = false;
			occupiedByRightConnector = false;
		}
		
		private Cell(Cell cell){
			occupied = cell.occupied;
			needsUpConnector = cell.needsUpConnector;
			needsDownConnector = cell.needsDownConnector;
			needsLeftConnector = cell.needsLeftConnector;
			needsRightConnector = cell.needsRightConnector;
			occupiedByDownConnector = cell.occupiedByDownConnector;
			occupiedByLeftConnector = cell.occupiedByLeftConnector;
			occupiedByUpConnector = cell.occupiedByUpConnector;
			occupiedByRightConnector = cell.occupiedByRightConnector;
			nameOfPiece = cell.nameOfPiece;
		}
		
		public Cell copy(){
			return new Cell(this);
		}

		public bool needsConnector(){
			if(needsDownConnector || needsUpConnector || needsLeftConnector || needsRightConnector) return true;
			else return false;
		}
	}


    void configureAstar(){
        AstarData data = Pathfinding.astarData;

        int graphWidth = (int) (2000 / (cellXDimension/2) * cell3D.GetLength(0));
        int graphLength = (int) (2000 / (cellZDimension/2) * cell3D.GetLength(1));
        int numberOfGraphs = cell3D.GetLength(2); //one graph for each z level
        //2 per?
        for(int i = 0; i < numberOfGraphs; i++){
            int graphHeight = (int) ((i-numberOfGraphs/2) * cellYDimension - 15);
            GridGraph gg = data.AddGraph(typeof(GridGraph)) as GridGraph;
            gg.width = graphWidth;
            gg.depth = graphLength;
            gg.nodeSize = 0.25f;
            gg.center = new Vector3(0,graphHeight,0);
            gg.maxClimb = cellYDimension;
            //gg.autoLinkGrids = true;
            //gg.autoLinkDistLimit = 10; //??
            gg.collision.fromHeight = cellYDimension/2;
            gg.collision.heightMask = LayerMask.GetMask("Terrain"); //Terrain
            gg.collision.thickRaycast = true;
            gg.collision.thickRaycastDiameter = 2.0f;
            //gg.cutCorners = false;
            gg.erodeIterations = 1;
            gg.maxSlope = 80;
            //gg.collision.collisionCheck = false;
            //gg.maxClimb = 2.5f;
            gg.collision.diameter = 4.5f;
            gg.UpdateSizeFromWidthDepth();
        }
        AstarPath.active.Scan();
        foreach(GridGraph gg in AstarPath.active.graphs){
            gg.ErodeWalkableArea();
        }
    }

	void populateMap(){
		foreach(Piece piece in instantiatedPieces){
			piece.populate(regionType);
		}
	}
}
