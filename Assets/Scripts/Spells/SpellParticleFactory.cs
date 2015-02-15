using UnityEngine;
using System.Collections;

public class SpellParticleFactory
{
	
	public static GameObject InstantiateParticleFromPrefab(Spell spell, string prefabPath, GameObject source, GameObject target){
		Vector3 position = new Vector3 (source.transform.position.x + UnityEngine.Random.Range (0, 1.5f),
		                                source.transform.position.y + UnityEngine.Random.Range (0, 1.5f),
		                                source.transform.position.z + UnityEngine.Random.Range (0, 1.5f));
		GameObject clone = (GameObject) MonoBehaviour.Instantiate(((GameObject) Resources .Load(prefabPath)),position,source.transform.rotation);
//		clone.AddComponent<EffectSettings> ();
		EffectSettings effectSettings = clone.GetComponentInChildren<EffectSettings>();
		effectSettings.spell = spell;
		effectSettings.Target = target;
		//extra things like movespeed taken care of in cast() function
		//effectSettings.MoveSpeed = spell.
		return clone;
	}
}

