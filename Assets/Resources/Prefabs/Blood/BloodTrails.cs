using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BloodTrails : MonoBehaviour {

	public GameObject TrailPrefab;
	private const int NUM = 5;

	List<Object>  trails = new List<Object> ();
	List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();

	// Use this for initialization
	void Start () {
	
	}

	IEnumerator FollowParticle(ParticleSystem.Particle p){
		yield return new WaitForSeconds (0.05f);
		int index = -1;
		//Check for an existing particle
		for (int i = 0; i < particles.Count; i++)
		{
			if(particles[i].Equals(p))
			{
				index = particles.IndexOf(p);
			}
		}
		//If the particle trail doesn't exist, create one
		if(index == -1)
		{
			particles.Add (p);
			index = particles.IndexOf(p);
			trails.Add (Instantiate(TrailPrefab));
		}
		//Update position
		((GameObject)trails [index]).transform.position = ((ParticleSystem.Particle)particles [index]).position;
	}

	// Update is called once per frame
	void Update () {
		ParticleSystem.Particle[] parts = new ParticleSystem.Particle[NUM];
		int iPart = particleSystem.GetParticles (parts);
		if (iPart == 0){
			return;
		}

		for(int i = 0; i<iPart; i++){
			StartCoroutine(FollowParticle(parts[i]));
		}
	}
}
