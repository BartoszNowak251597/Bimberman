using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace AttackEffects
{
	public class GlueEffect : MonoBehaviour
	{
		float t = 0;

		void Update()
		{
			t += Time.deltaTime;

			Vector3 scale = this.transform.localScale;

			if (t > 10)
			{
				Destroy(gameObject);
			}
		}

		private void OnTriggerEnter(Collider collision)
		{
			if (collision.TryGetComponent<Enemy>(out var enemy)) {
				enemy.m_Locked++;
			}
		}

		private void OnTriggerExit(Collider collision)
		{
			if (collision.TryGetComponent<Enemy>(out var enemy)) {
				enemy.m_Locked--;
			}
		}
	}
}
