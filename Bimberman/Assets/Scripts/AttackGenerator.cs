using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using AttackEffects;

public class AttackGenerator : MonoBehaviour
{
	public GameObject explosionEffectPrefab;
	public GameObject bigExplosionEffectPrefab;
	public GameObject wallEffectPrefab;
	public GameObject glueEffectPrefab;

	private static AttackGenerator instance;

	private delegate void AttackFunction(GameObject attackObject, int strength);

	static Dictionary<Potion.PotionEffect, AttackFunction> attacks = new Dictionary<Potion.PotionEffect, AttackFunction>() {
		{ Potion.PotionEffect.Moonshine, GenerateRegularAttack },
		{ Potion.PotionEffect.Wall, GenerateWallAttack },
		{ Potion.PotionEffect.Boom, GenerateBoomAttack },
		{ Potion.PotionEffect.Glue, GenerateGlueAttack },
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

	private static void GenerateBoomAttack(GameObject attackObject, int strength)
	{
		var effectObject = Instantiate(instance.bigExplosionEffectPrefab);
		effectObject.transform.localScale = Vector3.one * (6 * strength);
		var effect = effectObject.GetComponent<ExplosionEffect>();
		effectObject.transform.position = attackObject.transform.position;
		effect.range = 3 * strength;
		effect.damage = 10 * strength;
	}

	private static void GenerateWallAttack(GameObject attackObject, int strength)
	{
		var effectObject = Instantiate(instance.wallEffectPrefab);
		effectObject.transform.localScale = Vector3.one * (2 * strength);
		effectObject.transform.position = attackObject.transform.position;

		Vector3 vel = attackObject.GetComponent<Rigidbody>().linearVelocity;

		vel.y = 0;

		effectObject.transform.rotation = Quaternion.LookRotation(vel);
	}

	private static void GenerateGlueAttack(GameObject attackObject, int strength)
	{
		var effectObject = Instantiate(instance.glueEffectPrefab);
		effectObject.transform.localScale = Vector3.one * (2 * strength);
		effectObject.transform.position = attackObject.transform.position;

		Vector3 vel = attackObject.GetComponent<Rigidbody>().linearVelocity;

		vel.y = 0;

		effectObject.transform.rotation = Quaternion.LookRotation(vel);
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