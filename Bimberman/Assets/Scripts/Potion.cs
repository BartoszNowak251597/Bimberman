using UnityEngine;

public class Potion : MonoBehaviour
{
	public enum PotionEffect
	{
		Moonshine,
		Wall,
		Boom
	}

	public PotionEffect[] effects;
}