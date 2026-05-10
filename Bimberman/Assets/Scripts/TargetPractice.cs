using UnityEngine;
using UnityEngine.AI;

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

	private bool readyToSpawnTargets = true;

	private Vector3 spawnPosition;

	private void OnTargetHit(TargetHitEvent e) {
		if (this.readyToSpawnTargets)
		{
			return;
		}

		readyTargets--;

		if (readyTargets == 0)
		{
			readyToSpawnTargets = true;

			spawnPosition = Random.onUnitSphere;
			spawnPosition.y = 0;
			spawnPosition = spawnPosition.normalized * Mathf.Lerp(minDist * 2, maxDist * 2, Random.value);

			spawnPosition += FindFirstObjectByType<PlayerController>().transform.position;
		}

		targetSpawnTime = Mathf.Lerp(minTargetSpawnTime, maxTargetSpawnTime, Random.value);

		score++;

		scoreText.text = string.Format("Score: {0}", score);
	}

	public void Update() {
		if (readyToSpawnTargets) {
			targetSpawnTime -= Time.deltaTime;

			if (targetSpawnTime < 0)
			{
				Vector3 targetPos = Random.onUnitSphere;
				targetPos.y = 0;
				targetPos = targetPos.normalized * Mathf.Lerp(minDist, maxDist, Random.value);
				targetPos += spawnPosition;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(targetPos, out hit, 5.0f, NavMesh.AllAreas))
                {
                    targetPos = hit.position;
                }
                else
                {
                    Debug.LogWarning("Spawn point not on NavMesh, using original fallback");
                }

                GameObject newTarget = Instantiate(throwTargetPrefab);
                newTarget.transform.position = targetPos;
				newTarget.SetActive(true);

				this.readyTargets++;
				this.targetSpawnTime = Mathf.Lerp(minTargetSpawnTime, maxTargetSpawnTime, Random.value);
			}
			
			if (readyTargets == maxTargets)
			{
				this.readyToSpawnTargets = false;
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