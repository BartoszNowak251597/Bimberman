using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace AttackEffects
{
	public class WallEffect : MonoBehaviour
	{
		float t = 0;

		void Update()
		{
			t += Time.deltaTime;

			Vector3 scale = this.transform.localScale;

			scale.y = Mathf.Lerp(0.01f, 1.0f, (t - 0.5f) / 0.2f);

			this.transform.localScale = scale;

			if (t > 5)
			{
				Destroy(gameObject);
			}
		}

		private void Start()
		{
			Vector3 scale = this.transform.localScale;

			scale.y = 0.1f;

			this.transform.localScale = scale;
		}
	}
}
