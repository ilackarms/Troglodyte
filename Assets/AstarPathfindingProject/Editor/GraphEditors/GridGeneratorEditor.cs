#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3
#define UNITY_LE_4_3
#endif

using UnityEngine;
using UnityEditor;
using Pathfinding;
using System.Collections;
using Pathfinding.Serialization.JsonFx;

namespace Pathfinding {
	/*
	#if !AstarRelease
	[CustomGraphEditor (typeof(CustomGridGraph),"CustomGrid Graph")]
	//[CustomGraphEditor (typeof(LineTraceGraph),"Grid Tracing Graph")]
	#endif
	*/
	[CustomGraphEditor (typeof(GridGraph),"Grid Graph")]
	public class GridGraphEditor : GraphEditor {
		
		[JsonMember]
		public bool locked = true;
		
		float newNodeSize;
		
		[JsonMember]
		public bool showExtra = false;
		
		/** Should textures be allowed to be used.
		  * This can be set to false by inheriting graphs not implemeting that feature */
		[JsonMember]
		public bool textureVisible = true;
		
		Matrix4x4 savedMatrix;
		
		Vector3 savedCenter;
		
		public bool isMouseDown = false;
		
		[JsonMember]
		public GridPivot pivot;


		/** Rounds a vector's components to whole numbers if very close to them */
		public static Vector3 RoundVector3 ( Vector3 v ) {
			if (Mathf.Abs ( v.x - Mathf.Round(v.x)) < 0.001f ) v.x = Mathf.Round ( v.x );
			if (Mathf.Abs ( v.y - Mathf.Round(v.y)) < 0.001f ) v.y = Mathf.Round ( v.y );
			if (Mathf.Abs ( v.z - Mathf.Round(v.z)) < 0.001f ) v.z = Mathf.Round ( v.z );
			return v;
		}
	
		public override void OnInspectorGUI (NavGraph target) {

			GridGraph graph = target as GridGraph;

			GUIStyle lockStyle = AstarPathEditor.astarSkin.FindStyle ("GridSizeLock");
			if (lockStyle == null) {
				lockStyle = new GUIStyle ();
			}
	
			GUILayout.BeginHorizontal ();
			GUILayout.BeginVertical ();
			int newWidth = EditorGUILayout.IntField (new GUIContent ("Width (nodes)","Width of the graph in nodes"), graph.width);
			int newDepth = EditorGUILayout.IntField (new GUIContent ("Depth (nodes)","Depth (or height you might also call it) of the graph in nodes"), graph.depth);
			GUILayout.EndVertical ();
	
			Rect lockRect = GUILayoutUtility.GetRect (lockStyle.fixedWidth,lockStyle.fixedHeight);

			GUILayout.EndHorizontal ();
	
			// All the layouts mess up the margin to the next control, so add it manually
			GUILayout.Space (2);

			// Add a small offset to make it better centred around the controls
			lockRect.y += 3;
			lockRect.width = lockStyle.fixedWidth;
			lockRect.height = lockStyle.fixedHeight;
			lockRect.x += lockStyle.margin.left;
			lockRect.y += lockStyle.margin.top;
			
			locked = GUI.Toggle (lockRect,locked,new GUIContent ("","If the width and depth values are locked, changing the node size will scale the grid which keeping the number of nodes consistent instead of keeping the size the same and changing the number of nodes in the graph"),lockStyle);
			
			if (newWidth != graph.width || newDepth != graph.depth) {
				SnapSizeToNodes (newWidth,newDepth,graph);
			}
			
			GUI.SetNextControlName ("NodeSize");
			newNodeSize = EditorGUILayout.FloatField (new GUIContent ("Node size","The size of a single node. The size is the side of the node square in world units"),graph.nodeSize);
			
			newNodeSize = newNodeSize <= 0.01F ? 0.01F : newNodeSize;
			
			float prevRatio = graph.aspectRatio;
			graph.aspectRatio = EditorGUILayout.FloatField (new GUIContent ("Aspect Ratio","Scaling of the nodes width/depth ratio. Good for isometric games"),graph.aspectRatio);

			float standardIsometric = 90-Mathf.Atan (1/Mathf.Sqrt(2))*Mathf.Rad2Deg;

			var isometricGUIContent = new GUIContent ("Isometric Angle", "For an isometric 2D game, you can use this parameter to scale the graph correctly.\nIt can also be used to create a hexagon grid.");
			var isometricOptions = new GUIContent[] {new GUIContent ("None (0°)"), new GUIContent ("Isometric (≈54.74°)"), new GUIContent("Custom")};
			var isometricValues = new float[] {0, standardIsometric};
			var isometricOption = 2;
			for (int i = 0; i < isometricValues.Length; i++) {
				if (Mathf.Approximately (graph.isometricAngle, isometricValues[i])) {
					isometricOption = i;
				}
			}

			var prevIsometricOption = isometricOption;
			isometricOption = EditorGUILayout.IntPopup (isometricGUIContent, isometricOption, isometricOptions, new int[] {0, 1, 2});
			if (prevIsometricOption != isometricOption) {
				// Change to something that will not match the predefined values above
				graph.isometricAngle = 45;
			}

			if (isometricOption < 2) {
				graph.isometricAngle = isometricValues[isometricOption];
			} else {
				graph.isometricAngle = EditorGUILayout.FloatField (isometricGUIContent, graph.isometricAngle);
			}

			if (graph.nodeSize != newNodeSize || prevRatio != graph.aspectRatio) {
				if (!locked) {
					graph.nodeSize = newNodeSize;
					Matrix4x4 oldMatrix = graph.matrix;
					graph.GenerateMatrix ();
					if (graph.matrix != oldMatrix) {
						//Rescann the graphs
						//AstarPath.active.AutoScan ();
						GUI.changed = true;
					}
				} else {
					float delta = newNodeSize / graph.nodeSize;
					graph.nodeSize = newNodeSize;
					graph.unclampedSize = RoundVector3 (new Vector2 (newWidth*graph.nodeSize,newDepth*graph.nodeSize));
					Vector3 newCenter = graph.matrix.MultiplyPoint3x4 (new Vector3 ((newWidth/2F)*delta,0,(newDepth/2F)*delta));
					graph.center = RoundVector3 (newCenter);

					graph.GenerateMatrix ();
					
					//Make sure the width & depths stay the same
					graph.width = newWidth;
					graph.depth = newDepth;
					AutoScan ();
				}
			}
			
			Vector3 pivotPoint;
			Vector3 diff;
	
			GUILayout.BeginHorizontal ();
			
			switch (pivot) {
				case GridPivot.Center:
					graph.center = RoundVector3 ( graph.center );
					graph.center = EditorGUILayout.Vector3Field ("Center",graph.center);
					break;
				case GridPivot.TopLeft:
					pivotPoint = graph.matrix.MultiplyPoint3x4 (new Vector3 (0,0,graph.depth));
					pivotPoint = RoundVector3 ( pivotPoint );
					diff = pivotPoint-graph.center;
					pivotPoint = EditorGUILayout.Vector3Field ("Top-Left",pivotPoint);
					graph.center = pivotPoint-diff;
					break;
				case GridPivot.TopRight:
					pivotPoint = graph.matrix.MultiplyPoint3x4 (new Vector3 (graph.width,0,graph.depth));
					pivotPoint = RoundVector3 ( pivotPoint );
					diff = pivotPoint-graph.center;
					pivotPoint = EditorGUILayout.Vector3Field ("Top-Right",pivotPoint);
					graph.center = pivotPoint-diff;
					break;
				case GridPivot.BottomLeft:
					pivotPoint = graph.matrix.MultiplyPoint3x4 (new Vector3 (0,0,0));
					pivotPoint = RoundVector3 ( pivotPoint );
					diff = pivotPoint-graph.center;
					pivotPoint = EditorGUILayout.Vector3Field ("Bottom-Left",pivotPoint);
					graph.center = pivotPoint-diff;
					break;
				case GridPivot.BottomRight:
					pivotPoint = graph.matrix.MultiplyPoint3x4 (new Vector3 (graph.width,0,0));
					pivotPoint = RoundVector3 ( pivotPoint );
					diff = pivotPoint-graph.center;
					pivotPoint = EditorGUILayout.Vector3Field ("Bottom-Right",pivotPoint);
					graph.center = pivotPoint-diff;
					break;
			}

			graph.GenerateMatrix ();
			
			pivot = PivotPointSelector (pivot);

			GUILayout.EndHorizontal ();
			
			graph.rotation = EditorGUILayout.Vector3Field ("Rotation",graph.rotation);
	
			if (GUILayout.Button (new GUIContent ("Snap Size","Snap the size to exactly fit nodes"),GUILayout.MaxWidth (100),GUILayout.MaxHeight (16))) {
				SnapSizeToNodes (newWidth,newDepth,graph);
			}
			
			Separator ();
			
			graph.cutCorners = EditorGUILayout.Toggle (new GUIContent ("Cut Corners","Enables or disables cutting corners. See docs for image example"),graph.cutCorners);
			graph.neighbours = (NumNeighbours)EditorGUILayout.EnumPopup (new GUIContent ("Connections","Sets how many connections a node should have to it's neighbour nodes."),graph.neighbours);

			if (graph.neighbours == NumNeighbours.Six) {
				if ((isometricOption != 1 || !graph.uniformEdgeCosts) && GUILayout.Button ("Configure as hexagon graph")) {
					graph.isometricAngle = standardIsometric;
					graph.uniformEdgeCosts = true;
				}
				graph.uniformEdgeCosts = EditorGUILayout.Toggle (new GUIContent ("Hexagon connection costs", "Tweak the edge costs in the graph to be more suitable for hexagon graphs"), graph.uniformEdgeCosts);
			} else {
				graph.uniformEdgeCosts = false;
			}


			graph.maxClimb = EditorGUILayout.FloatField (new GUIContent ("Max Climb","How high, relative to the graph, should a climbable level be. A zero (0) indicates infinity"),graph.maxClimb);
			if ( graph.maxClimb < 0 ) graph.maxClimb = 0;
			EditorGUI.indentLevel++;
			graph.maxClimbAxis = EditorGUILayout.IntPopup (new GUIContent ("Climb Axis","Determines which axis the above setting should test on"),graph.maxClimbAxis,new GUIContent[3] {new GUIContent ("X"),new GUIContent ("Y"),new GUIContent ("Z")},new int[3] {0,1,2});
			EditorGUI.indentLevel--;
			
			if ( graph.maxClimb > 0 && Mathf.Abs((Quaternion.Euler (graph.rotation) * new Vector3 (graph.nodeSize,0,graph.nodeSize))[graph.maxClimbAxis]) > graph.maxClimb ) {
				EditorGUILayout.HelpBox ("Nodes are spaced further apart than this in the grid. You might want to increase this value or change the axis", MessageType.Warning );
			}
			
			//GUILayout.EndHorizontal ();
			
			graph.maxSlope = EditorGUILayout.Slider (new GUIContent ("Max Slope","Sets the max slope in degrees for a point to be walkable. Only enabled if Height Testing is enabled."),graph.maxSlope,0,90F);
			
			graph.erodeIterations = EditorGUILayout.IntField (new GUIContent ("Erosion iterations","Sets how many times the graph should be eroded. This adds extra margin to objects. This will not work when using Graph Updates, so if you can, use the Diameter setting in collision settings instead"),graph.erodeIterations);
			graph.erodeIterations = graph.erodeIterations < 0 ? 0 : (graph.erodeIterations > 16 ? 16 : graph.erodeIterations); //Clamp iterations to [0,16]
	
			if ( graph.erodeIterations > 0 ) {
				EditorGUI.indentLevel++;
				graph.erosionUseTags = EditorGUILayout.Toggle (new GUIContent ("Erosion Uses Tags","Instead of making nodes unwalkable, " +
				"nodes will have their tag set to a value corresponding to their erosion level, " +
				"which is a quite good measurement of their distance to the closest wall.\nSee online documentation for more info."),
			                                               graph.erosionUseTags);
				if (graph.erosionUseTags) {
					EditorGUI.indentLevel++;
					graph.erosionFirstTag = EditorGUILayoutx.SingleTagField ("First Tag",graph.erosionFirstTag);
					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
			}
			DrawCollisionEditor (graph.collision);
			
			if ( graph.collision.use2D ) {
				if ( Mathf.Abs ( Vector3.Dot ( Vector3.forward, Quaternion.Euler (graph.rotation) * Vector3.up ) ) < 0.9f ) {
					EditorGUILayout.HelpBox ("When using 2D it is recommended to rotate the graph so that it aligns with the 2D plane.", MessageType.Warning );
				}
			}

			Separator ();
			GUILayout.Label (new GUIContent ("Advanced"), EditorStyles.boldLabel);
			
			showExtra = EditorGUILayout.Foldout (showExtra, "Penalty Modifications");
			
			if (showExtra) {
				EditorGUI.indentLevel+=2;
				
				graph.penaltyAngle = ToggleGroup (new GUIContent ("Angle Penalty","Adds a penalty based on the slope of the node"),graph.penaltyAngle);
				//bool preGUI = GUI.enabled;
				//GUI.enabled = graph.penaltyAngle && GUI.enabled;
				if (graph.penaltyAngle) {
					EditorGUI.indentLevel++;
					graph.penaltyAngleFactor = EditorGUILayout.FloatField (new GUIContent ("Factor","Scale of the penalty. A negative value should not be used"),graph.penaltyAngleFactor);
					graph.penaltyAnglePower = EditorGUILayout.Slider ("Power", graph.penaltyAnglePower, 0.1f, 10f);
					//GUI.enabled = preGUI;
					HelpBox ("Applies penalty to nodes based on the angle of the hit surface during the Height Testing\nPenalty applied is: P=(1-cos(angle)^power)*factor.");
					
					EditorGUI.indentLevel--;
				}
				
				graph.penaltyPosition = ToggleGroup ("Position Penalty",graph.penaltyPosition);
					//EditorGUILayout.Toggle ("Position Penalty",graph.penaltyPosition);
				//preGUI = GUI.enabled;
				//GUI.enabled = graph.penaltyPosition && GUI.enabled;
				if (graph.penaltyPosition) {
					EditorGUI.indentLevel++;
					graph.penaltyPositionOffset = EditorGUILayout.FloatField ("Offset",graph.penaltyPositionOffset);
					graph.penaltyPositionFactor = EditorGUILayout.FloatField ("Factor",graph.penaltyPositionFactor);
					HelpBox ("Applies penalty to nodes based on their Y coordinate\nSampled in Int3 space, i.e it is multiplied with Int3.Precision first ("+Int3.Precision+")\n" +
						"Be very careful when using negative values since a negative penalty will underflow and instead get really high");
					//GUI.enabled = preGUI;
					EditorGUI.indentLevel--;
				}
				
				GUI.enabled = false;
				ToggleGroup (new GUIContent ("Use Texture",AstarPathEditor.AstarProTooltip),false);
				GUI.enabled = true;
				EditorGUI.indentLevel-=2;
			}

		}
	
		
		/** Displays an object field for objects which must be in the 'Resources' folder.
		 * If the selected object is not in the resources folder, a warning message with a Fix button will be shown
		 */
		[System.Obsolete("Use ObjectField instead")]
		public UnityEngine.Object ResourcesField (string label, UnityEngine.Object obj, System.Type type) {

			obj = EditorGUILayout.ObjectField (label,obj,type,false);
			
			if (obj != null) {
				string path = AssetDatabase.GetAssetPath (obj);
				
				if (!path.Contains ("Resources/")) {
					if (FixLabel ("Object must be in the 'Resources' folder")) {
						if (!System.IO.Directory.Exists (Application.dataPath+"/Resources")) {
							System.IO.Directory.CreateDirectory (Application.dataPath+"/Resources");
							AssetDatabase.Refresh ();
						}
						string ext = System.IO.Path.GetExtension(path);
						
						string error = AssetDatabase.MoveAsset	(path,"Assets/Resources/"+obj.name+ext);
						
						if (error == "") {
							//Debug.Log ("Successful move");
						} else {
							Debug.LogError ("Couldn't move asset - "+error);
						}
					}
				}
			}
			return obj;
		}
		
		
		public void SnapSizeToNodes (int newWidth, int newDepth, GridGraph graph) {
			//Vector2 preSize = graph.unclampedSize;
			
			/*if (locked) {
				graph.unclampedSize = new Vector2 (newWidth*newNodeSize,newDepth*newNodeSize);
				graph.nodeSize = newNodeSize;
				graph.GenerateMatrix ();
				Vector3 newCenter = graph.matrix.MultiplyPoint3x4 (new Vector3 (newWidth/2F,0,newDepth/2F));
				graph.center = newCenter;
				AstarPath.active.AutoScan ();
			} else {*/
				graph.unclampedSize = new Vector2 (newWidth*graph.nodeSize,newDepth*graph.nodeSize);
				Vector3 newCenter = graph.matrix.MultiplyPoint3x4 (new Vector3 (newWidth/2F,0,newDepth/2F));
				graph.center = newCenter;
				graph.GenerateMatrix ();
				AutoScan ();
			//}
			
			GUI.changed = true;
		}
		
		public static GridPivot PivotPointSelector (GridPivot pivot) {
			
			GUISkin skin = AstarPathEditor.astarSkin;
			
			GUIStyle background = skin.FindStyle ("GridPivotSelectBackground");
			
			Rect r = GUILayoutUtility.GetRect (19,19,background);
	
	#if !UNITY_LE_4_3
			// I have no idea... but it is required for it to work well
			r.y -= 14;
	#endif
	
			r.width = 19;
			r.height = 19;
			
			if (background == null) {
				return pivot;
			}
			
			if (Event.current.type == EventType.Repaint) {
				background.Draw (r,false,false,false,false);
			}
			
			if (GUI.Toggle (new Rect (r.x,r.y,7,7),pivot == GridPivot.TopLeft, "",skin.FindStyle ("GridPivotSelectButton")))
				pivot = GridPivot.TopLeft;
				
			if (GUI.Toggle (new Rect (r.x+12,r.y,7,7),pivot == GridPivot.TopRight,"",skin.FindStyle ("GridPivotSelectButton")))
				pivot = GridPivot.TopRight;
			
			if (GUI.Toggle (new Rect (r.x+12,r.y+12,7,7),pivot == GridPivot.BottomRight,"",skin.FindStyle ("GridPivotSelectButton")))
				pivot = GridPivot.BottomRight;
				
			if (GUI.Toggle (new Rect (r.x,r.y+12,7,7),pivot == GridPivot.BottomLeft,"",skin.FindStyle ("GridPivotSelectButton")))
				pivot = GridPivot.BottomLeft;	
			
			if (GUI.Toggle (new Rect (r.x+6,r.y+6,7,7),pivot == GridPivot.Center,"",skin.FindStyle ("GridPivotSelectButton")))
				pivot = GridPivot.Center;	
					
			return pivot;
		}

		public override void OnSceneGUI (NavGraph target) {
			
			Event e = Event.current;

			GridGraph graph = target as GridGraph;
			
			Matrix4x4 matrixPre = graph.matrix;
			
			graph.GenerateMatrix ();
			
			if (e.type == EventType.MouseDown) {
				isMouseDown = true;
			} else if (e.type == EventType.MouseUp) {
				isMouseDown = false;
			}
			
			if (!isMouseDown) {
				savedMatrix = graph.boundsMatrix;
			}
			
			Handles.matrix = savedMatrix;
			
			if ((graph.GetType() == typeof(GridGraph) && graph.nodes == null) || (graph.uniformWidthDepthGrid && graph.depth*graph.width != graph.nodes.Length) || graph.matrix != matrixPre) {
				//Rescan the graphs
				if (AutoScan ()) {
					GUI.changed = true;
				}
			}
			
			Matrix4x4 inversed = savedMatrix.inverse;
			
			Handles.color = AstarColor.BoundsHandles;
			
			Handles.DrawCapFunction cap = Handles.CylinderCap;
			
			Vector2 extents = graph.unclampedSize*0.5F;

			Vector3 center = inversed.MultiplyPoint3x4 (graph.center);


			if (Tools.current == Tool.Scale) {
				const float HandleScale = 0.1f;

				EditorGUI.BeginChangeCheck ();

				Vector3 p1 = Handles.Slider (center+new Vector3 (extents.x,0,0),	Vector3.right,		HandleScale*HandleUtility.GetHandleSize (center+new Vector3 (extents.x,0,0)),cap,0);
				Vector3 p2 = Handles.Slider (center+new Vector3 (0,0,extents.y),	Vector3.forward,	HandleScale*HandleUtility.GetHandleSize (center+new Vector3 (0,0,extents.y)),cap,0);
				
				Vector3 p4 = Handles.Slider (center+new Vector3 (-extents.x,0,0),	-Vector3.right,		HandleScale*HandleUtility.GetHandleSize (center+new Vector3 (-extents.x,0,0)),cap,0);
				Vector3 p5 = Handles.Slider (center+new Vector3 (0,0,-extents.y),	-Vector3.forward,	HandleScale*HandleUtility.GetHandleSize (center+new Vector3 (0,0,-extents.y)),cap,0);
				
				Vector3 p6 = Handles.Slider (center, Vector3.up, HandleScale*HandleUtility.GetHandleSize (center),cap,0);
				
				Vector3 r1 = new Vector3 (p1.x,p6.y,p2.z);
				Vector3 r2 = new Vector3 (p4.x,p6.y,p5.z);

				if (EditorGUI.EndChangeCheck ()) {
					graph.center = savedMatrix.MultiplyPoint3x4 ((r1+r2)/2F);
				
					Vector3 tmp = r1-r2;
					graph.unclampedSize = new Vector2(tmp.x,tmp.z);
				}

			} else if (Tools.current == Tool.Move) {
				
				if (Tools.pivotRotation == PivotRotation.Local) {
					EditorGUI.BeginChangeCheck ();
					center = Handles.PositionHandle (center,Quaternion.identity);
					
					if (EditorGUI.EndChangeCheck () && Tools.viewTool != ViewTool.Orbit) {
						graph.center = savedMatrix.MultiplyPoint3x4 (center);
						Debug.Log (graph.center.ToString("R"));
					}
				} else {
					Handles.matrix = Matrix4x4.identity;

					EditorGUI.BeginChangeCheck ();
					center = Handles.PositionHandle (graph.center,Quaternion.identity);
					
					if (EditorGUI.EndChangeCheck () && Tools.viewTool != ViewTool.Orbit) {
						graph.center = center;
					}
				}
			} else if (Tools.current == Tool.Rotate) {
				//The rotation handle doesn't seem to be able to handle different matrixes of some reason
				Handles.matrix = Matrix4x4.identity;

				EditorGUI.BeginChangeCheck ();
				var rot = Handles.RotationHandle (Quaternion.Euler (graph.rotation),graph.center);
				
				if (EditorGUI.EndChangeCheck () && Tools.viewTool != ViewTool.Orbit) {
					graph.rotation = rot.eulerAngles;
				}
			}
			
			Handles.matrix = Matrix4x4.identity;
			
			
		}
		
		public enum GridPivot {
			Center,
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight
		}
	}
}