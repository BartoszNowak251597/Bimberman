using UnityEngine;

public class Potion : MonoBehaviour
{
	public enum PotionEffect
	{
		Moonshine,
		Wall,
		Boom,
		Glue
	}

	public PotionEffect[] effects;
}