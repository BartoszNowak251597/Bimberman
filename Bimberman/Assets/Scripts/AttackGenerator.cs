using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using AttackEffects;

public class AttackGenerator : MonoBehaviour
{
	public GameObject explosionEffectPrefab;
	public GameObject wallEffectPrefab;

	private static AttackGenerator instance;

	private delegate void AttackFunction(GameObject attackObject, int strength);

	static Dictionary<Potion.PotionEffect, AttackFunction> attacks = new Dictionary<Potion.PotionEffect, AttackFunction>() {
		{ Potion.PotionEffect.Moonshine, GenerateRegularAttack }
	};

	private void Awake()
	{
		instance = this;
	}

	private static void GenerateRegularAttack(GameObject attackObject, int strength)
	{
		var effectObject = Instantiate(instance.explosionEffectPrefab);
		effectObject.transform.localScale = Vector3.one * (2 * strength);
		var effect = effectObject.GetComponent<ExplosionEffect>();
		effectObject.transform.position = attackObject.transform.position;
		effect.range = strength;
		effect.damage = 5 * strength;
	}

	public static void GenerateAttack(Potion potion)
	{
		var effects = potion.effects.GroupBy(e => e).Select( e => new Tuple<Potion.PotionEffect, int>(e.First(), e.Count()) );

		foreach (var e in effects)
		{
			if (attacks.ContainsKey(e.Item1))
			{
				attacks[e.Item1](potion.gameObject, e.Item2);
			}
		}
	}
}