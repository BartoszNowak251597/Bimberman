using UnityEngine;

public class TargetPractice : MonoBehaviour {
	public GameObject throwTargetPrefab;
	public TMPro.TMP_Text scoreText;
	
	public float minDist;
	public float maxDist;
	public int maxTargets;
	public float minTargetSpawnTime;
	public float maxTargetSpawnTime;
	
	private int score = 0;
	private int readyTargets;
	private float targetSpawnTime;

	private void OnTargetHit(TargetHitEvent e) {
		Destroy(e.target.gameObject);

		readyTargets--;

		targetSpawnTime = Mathf.Lerp(minTargetSpawnTime, maxTargetSpawnTime, Random.value);

		score++;

		scoreText.text = string.Format("Score: {0}", score);
	}

	public void Update() {
		if (readyTargets < maxTargets) {
			targetSpawnTime -= Time.deltaTime;

			if (targetSpawnTime < 0) {
				GameObject newTarget = Instantiate(throwTargetPrefab);
				Vector3 targetPos = Random.onUnitSphere;
				targetPos.y = 0;
				targetPos = targetPos.normalized * Mathf.Lerp(minDist, maxDist, Random.value);

				newTarget.transform.position = targetPos;
				newTarget.SetActive(true);

				this.readyTargets++;
				this.targetSpawnTime = Mathf.Lerp(minTargetSpawnTime, maxTargetSpawnTime, Random.value);
			}
		}

	}

	public void OnEnable() {
		EventManager.Subscribe(OnTargetHit);
	}

	public void OnDisable() {
		EventManager.Unsubscribe(OnTargetHit);
	}
}