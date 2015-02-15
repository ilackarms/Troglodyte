using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
public class AstarExtension
{
	//will return null if no random node is found
	public static Vector3 findRandomConnectedNode(Vector3 position){
		Vector3 randomNodePosition = position;
		GraphNode node = AstarPath.active.GetNearest(position, NNConstraint.Default).node;
		if(node!=null) {
			List<GraphNode> reachableNodes = PathUtilities.GetReachableNodes(node);
			randomNodePosition = PathUtilities.GetPointsOnNodes(reachableNodes, 1)[0]; //since we're only get ting one node, we should always store it at 0
		}
		return randomNodePosition;

	}

	public static Vector3 findRandomConnectedNodeFast(Vector3 position, float radius){
		//get random point
		Vector3 randomPoint = position + Random.insideUnitSphere * 100;
		//find the node the character is standing on
		GraphNode node = AstarPath.active.GetNearest(position, NNConstraint.Default).node;
		//Confingure an NNConstraint which makes sure the resulting node can be reached from your character
		NNConstraint constraint = NNConstraint.Default;
		constraint.constrainArea = true;
		constraint.area = (int) node.Area;
		NNInfo closestOnGraph = AstarPath.active.GetNearest(randomPoint, constraint);
		randomPoint = closestOnGraph.clampedPosition;
		return randomPoint;
	}
}

