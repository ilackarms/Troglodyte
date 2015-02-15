using UnityEngine;
using System.Collections;

public class ParticleDestroyer : MonoBehaviour {

	//private ParticleSystem ps;
	public float duration;

	IEnumerator destroy(){
		yield return new WaitForSeconds(duration);
		Destroy (gameObject);

	}

	// Use this for initialization
	void Start () {
		//ps = GetComponent<ParticleSystem>();
		StartCoroutine(destroy ());
	}
	
	// Update is called once per frame
	void Update () {
		/*if(ps){
			if(ps.IsAlive()){
				Destroy (gameObject);
			}
		}*/
	}
}
