using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour {
	
	public Shader defaultShader;
	public Shader outlineShader;

	public string defaultShaderName;

	public RaycastHit hit;

	public Color outlineColor;
	public float outlineThickness;

	public GameObject renderObject;

	// Use this for initialization
	void Awake () {
		outlineColor = Color.green;
		outlineThickness = 0.005f;
		//defaultShader = Shader.Find (defaultShaderName);
		//outlineShader = Shader.Find ("Outlined/Diffuse");
		renderObject = gameObject;
		if (renderObject.GetComponent<Renderer>() == null) {
//			Debug.Log(name+" searching children for mesh renderer");
			if(GetComponentInChildren<MeshRenderer>() != null)
				renderObject = GetComponentInChildren<MeshRenderer>().gameObject;
			else{
                if (GetComponentInChildren<SkinnedMeshRenderer>() != null)
                {
                    renderObject = GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
                }
                else if (renderObject == null)
                {
                    renderObject = gameObject.AddComponent<SkinnedMeshRenderer>().gameObject;
                }
			}
//			Debug.Log(renderObject.name+" found");
		}
		
		defaultShaderName = renderObject.GetComponent<Renderer>().material.shader.name;
	}
	
	// Update is called once per frame
	void Update () {
		RemoveOutline ();
	}

	public virtual void Outline(){
		//if(defaultShader.name.Contains("Bumped"))
		//	gameObject.renderer.material.shader = Shader.Find ("Outlined/Bumped Diffuse");
		//else
		foreach(Material material in renderObject.GetComponent<Renderer>().materials){
			material.shader = Shader.Find ("Outlined/Diffuse");
			material.SetColor ("_OutlineColor", outlineColor);
			material.SetFloat("_Outline", outlineThickness);
		}
	}

	public virtual void RemoveOutline(){
		//gameObject.renderer.material.shader = defaultShader;
		
		foreach(Material material in renderObject.GetComponent<Renderer>().materials){
			material.shader = Shader.Find (defaultShaderName);
			material.SetColor ("_OutlineColor", outlineColor);
		}

	}

	public void SetRaycastHit(RaycastHit hit){
		this.hit = hit;
	}
}
