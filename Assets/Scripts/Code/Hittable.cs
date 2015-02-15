using UnityEngine;
using System.Collections;

abstract public class Hittable : MonoBehaviour
{
	public abstract void GetHit(DamageBundle damageBundle);
}

