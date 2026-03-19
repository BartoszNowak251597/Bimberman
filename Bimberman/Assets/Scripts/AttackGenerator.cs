using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AttackGenerator : MonoBehaviour
{
	public GameObject moonshineAttackPrefab;

	private static AttackGenerator instance;

	private delegate void AttackFunction(Potion potion, int strength);

	static Dictionary<Potion.PotionEffect, AttackFunction> attacks = new Dictionary<Potion.PotionEffect, AttackFunction>() {
		{ Potion.PotionEffect.Moonshine, GenerateRegularAttack }
	};

	private void Awake()
	{
		instance = this;
	}

	private static void GenerateRegularAttack(Potion potion, int strength)
	{
		Vector3 hitPoint = potion.transform.position;
        hitPoint.y = 0;

		Instantiate(instance.moonshineAttackPrefab, hitPoint, Quaternion.identity);
	}

	public static void GenerateAttack(Potion potion)
	{
		var effects = potion.effects.GroupBy(e => e).Select( e => new Tuple<Potion.PotionEffect, int>(e.First(), e.Count()) );

		foreach (var e in effects)
		{
			if (attacks.ContainsKey(e.Item1))
			{
				attacks[e.Item1](potion, e.Item2);
			}
		}
	}
}