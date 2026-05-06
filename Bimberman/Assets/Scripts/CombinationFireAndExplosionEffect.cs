using UnityEngine;

public class CombinationFireAndExplosionEffect : MonoBehaviour
{
    // Wartość od 0 do 1 w zależności od proporcji składniku eksplozji w potce
    [Range(0, 1)]
    public float explosionStrength;
    [Range(0, 1)]
    public float fireStrength;
    public float maxExplosionRange;
    public float maxExplosionDamage;
    private float lifetime;
    public MeshRenderer explosionRenderer;
    public GameObject flamingBitPrefab;
    public int minFlamingBits;
    public int maxFlamingBits;
    public float minFlamingBitSpeed;
    public float maxFlamingBitSpeed;

    private float GetRange() {
        return explosionStrength * maxExplosionRange;
    }

    private float GetDamage() {
        return explosionStrength * maxExplosionDamage;
    }

    private int GetNumFlamingBits() {
        return Mathf.RoundToInt(Mathf.Lerp(minFlamingBits, maxFlamingBits, explosionStrength)) + Random.Range(-1, 2);
    }

    private float GetFlamingBitSpeed() {
        return Mathf.Lerp(minFlamingBitSpeed, maxFlamingBitSpeed, explosionStrength) * Random.Range(0.8f, 1.2f);
    }

    public void Awake() {
        this.explosionRenderer.transform.localScale = Vector3.one * GetRange();

		foreach (var enemy in GameObject.FindObjectsByType<Skeleton>(FindObjectsSortMode.None)) {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= GetRange()) {
                enemy.TakeDamage(GetDamage());

				if (enemy.IsDead()) {
					foreach (var body in enemy.GetComponentsInChildren<Rigidbody>(true))
					{
						body.AddExplosionForce(10, transform.position, GetRange() * 2, 1, ForceMode.Impulse);
					}
				}
            }
        }

        int numBits = GetNumFlamingBits();

        for (int i = 0; i < numBits; i++) {
            Vector3 direction = Random.onUnitSphere;
            direction.y = Mathf.Abs(direction.y);

            GameObject spawnedBit = Instantiate(this.flamingBitPrefab, this.transform.position + direction, Quaternion.identity);

            spawnedBit.GetComponent<Rigidbody>().linearVelocity = direction * GetFlamingBitSpeed();

            (spawnedBit.GetComponent<DebugThrowable>().effect.GetComponent<FireEffect2>()).strength = fireStrength;

            spawnedBit.SetActive(true);
        }
    }

    public void Update() {
        float factor = lifetime / GetRange();

        factor = Mathf.Sin(factor * Mathf.PI / 2);

        explosionRenderer.material.SetFloat("_ExplosionTime", factor * GetRange());

        lifetime += Time.deltaTime * 30;

        if (lifetime > GetRange()) {
            Destroy(this.gameObject);
        }
    }
}
