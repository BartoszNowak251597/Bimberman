using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace AttackEffects
{
	public class ExplosionEffect : MonoBehaviour
	{
		public int damage = 5;
		public float range = 1f;

		float t = 0;

		void Update()
		{
			t += Time.deltaTime;

			if (t > 0.1)
			{
				Destroy(gameObject);
			}
		}

		private void Start()
		{
			// var hits = Physics.CapsuleCastAll(transform.position, transform.position + Vector3.up, 0.5f * range, Vector3.up);
			var hits = Physics.OverlapSphere(transform.position, range);

			foreach (var hit in hits)
			{
				Debug.Log(hit);

				if (hit.gameObject.TryGetComponent<Enemy>(out var enemyComponent))
				{
					enemyComponent.TakeDamage(damage);
				}
			}
		}
		
	}
}
