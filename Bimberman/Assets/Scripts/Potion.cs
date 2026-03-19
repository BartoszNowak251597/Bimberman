using UnityEngine;

public class Potion : MonoBehaviour
{
	public enum PotionEffect
	{
		Moonshine,
		Lifesteal
	}

	public PotionEffect[] effects;
}