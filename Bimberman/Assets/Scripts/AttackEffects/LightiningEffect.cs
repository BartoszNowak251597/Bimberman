using System.Collections.Generic;
using UnityEngine;

namespace AttackEffects
{
    public class LightningEffect : MonoBehaviour
    {
        // Lightning sam w sobie robi chain attack.
        [Range(0, 1)]
        public float strength;

        [Header("Chain attack")]
        public float maxStartRange = 6f;
        public float maxJumpRange = 5f;
        public float maxDamage = 40f;

        public int minTargets = 1;
        public int maxTargets = 5;

        [Header("Visual")]
        public GameObject visual;
        public float visualHeightOffset = 0.5f;
        public float minVisualThickness = 0.08f;
        public float maxVisualThickness = 0.25f;
        public float lifetime = 0.4f;

        private readonly List<Skeleton> hitEnemies = new List<Skeleton>();
        private readonly List<GameObject> spawnedVisuals = new List<GameObject>();

        private float GetStartRange()
        {
            return Mathf.Max(1f, strength * maxStartRange);
        }

        private float GetJumpRange()
        {
            return Mathf.Max(1f, strength * maxJumpRange);
        }

        private float GetDamage()
        {
            return strength * maxDamage;
        }

        private int GetTargetsCount()
        {
            return Mathf.Max(1, Mathf.RoundToInt(Mathf.Lerp(minTargets, maxTargets, strength)));
        }

        public void Awake()
        {
            DoChainAttack();
        }

        public void Update()
        {
            lifetime -= Time.deltaTime;

            if (lifetime <= 0)
            {
                DestroySpawnedVisuals();
                Destroy(gameObject);
            }
        }

        private void DoChainAttack()
        {
            Skeleton currentEnemy = FindClosestEnemy(transform.position, GetStartRange());

            Vector3 previousPosition = transform.position;
            int targetsLeft = GetTargetsCount();

            while (currentEnemy != null && targetsLeft > 0)
            {
                hitEnemies.Add(currentEnemy);

                currentEnemy.TakeDamage(GetDamage());

                SpawnVisual(previousPosition, currentEnemy.transform.position);

                previousPosition = currentEnemy.transform.position;
                currentEnemy = FindClosestEnemy(previousPosition, GetJumpRange());

                targetsLeft--;
            }
        }

        private Skeleton FindClosestEnemy(Vector3 position, float range)
        {
            Skeleton closestEnemy = null;
            float closestDistance = range;

            foreach (Skeleton enemy in GameObject.FindObjectsByType<Skeleton>(FindObjectsSortMode.None))
            {
                if (enemy == null || !enemy.IsAlive() || hitEnemies.Contains(enemy))
                {
                    continue;
                }

                float distance = Vector3.Distance(position, enemy.transform.position);

                if (distance <= closestDistance)
                {
                    closestEnemy = enemy;
                    closestDistance = distance;
                }
            }

            return closestEnemy;
        }

        private void SpawnVisual(Vector3 startPosition, Vector3 endPosition)
        {
            if (visual == null)
            {
                return;
            }

            startPosition += Vector3.up * visualHeightOffset;
            endPosition += Vector3.up * visualHeightOffset;

            Vector3 direction = endPosition - startPosition;
            float distance = direction.magnitude;

            if (distance <= 0.01f)
            {
                return;
            }

            Vector3 middlePosition = startPosition + direction * 0.5f;

            GameObject spawnedVisual = Instantiate(visual, middlePosition, Quaternion.identity, null);
            spawnedVisual.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
            float thickness = Mathf.Lerp(minVisualThickness, maxVisualThickness, strength);

            spawnedVisual.transform.localScale = new Vector3(
                thickness,
                distance * 0.5f,
                thickness
            );

            spawnedVisual.SetActive(true);
            spawnedVisuals.Add(spawnedVisual);
        }

        private void DestroySpawnedVisuals()
        {
            foreach (GameObject spawnedVisual in spawnedVisuals)
            {
                if (spawnedVisual != null)
                {
                    Destroy(spawnedVisual);
                }
            }

            spawnedVisuals.Clear();
        }
    }
}