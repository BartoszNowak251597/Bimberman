using System.Collections.Generic;
using UnityEngine;

namespace AttackEffects
{
    public class TeleportEffect : MonoBehaviour
    {
        private static TeleportEffect waitingPortal;

        [Header("Teleport")]
        public float radius = 4f;
        public float heightOffset = 0.1f;
        public bool resetVelocityAfterTeleport = true;

        [Header("Visual")]
        public GameObject visual;
        public float visualHeightOffset = 0.05f;
        public float visualLifetimeAfterTeleport = 0.6f;

        [Header("First portal")]
        public float maxWaitTimeForSecondPortal = 8f;

        [Header("Debug")]
        public bool debugLogs = true;

        private readonly List<GameObject> spawnedVisuals = new List<GameObject>();

        private bool isWaitingForSecondPortal;
        private bool finished;

        private float waitTimer;
        private float destroyTimer = -1f;

        public void Awake()
        {
            FindVisualIfNeeded();

            if (waitingPortal == null)
            {
                CreateFirstPortal();
                return;
            }

            if (waitingPortal == this)
            {
                return;
            }

            CreateSecondPortal();
        }

        public void Update()
        {
            if (isWaitingForSecondPortal)
            {
                waitTimer -= Time.deltaTime;

                if (waitTimer <= 0)
                {
                    if (debugLogs)
                    {
                        Debug.Log("Teleport potion: first portal expired.");
                    }

                    if (waitingPortal == this)
                    {
                        waitingPortal = null;
                    }

                    DestroySpawnedVisuals();
                    Destroy(gameObject);
                }

                return;
            }

            if (!finished)
            {
                return;
            }

            destroyTimer -= Time.deltaTime;

            if (destroyTimer <= 0)
            {
                DestroySpawnedVisuals();
                Destroy(gameObject);
            }
        }

        private void CreateFirstPortal()
        {
            waitingPortal = this;
            isWaitingForSecondPortal = true;
            finished = false;
            waitTimer = maxWaitTimeForSecondPortal;

            SpawnPortalVisual(transform.position);

            if (debugLogs)
            {
                Debug.Log("Teleport potion: first portal created.");
            }
        }

        private void CreateSecondPortal()
        {
            TeleportEffect firstPortal = waitingPortal;
            waitingPortal = null;

            if (firstPortal == null)
            {
                CreateFirstPortal();
                return;
            }

            isWaitingForSecondPortal = false;
            finished = false;

            SpawnPortalVisual(transform.position);

            if (debugLogs)
            {
                Debug.Log("Teleport potion: second portal created. Teleporting entities.");
            }

            TeleportBetween(firstPortal.transform.position, transform.position);

            firstPortal.FinishAfterTeleport();
            FinishAfterTeleport();
        }

        private void TeleportBetween(Vector3 portalA, Vector3 portalB)
        {
            List<Transform> entitiesNearA = new List<Transform>();
            List<Transform> entitiesNearB = new List<Transform>();

            CollectTeleportTargets(portalA, portalB, entitiesNearA, entitiesNearB);

            foreach (Transform target in entitiesNearA)
            {
                TeleportTarget(target, portalB);
            }

            foreach (Transform target in entitiesNearB)
            {
                TeleportTarget(target, portalA);
            }

            if (debugLogs)
            {
                Debug.Log(
                    $"Teleport potion: moved {entitiesNearA.Count} entities from A to B and {entitiesNearB.Count} entities from B to A."
                );
            }
        }

        private void CollectTeleportTargets(
            Vector3 portalA,
            Vector3 portalB,
            List<Transform> entitiesNearA,
            List<Transform> entitiesNearB
        )
        {
            foreach (PlayerController player in GameObject.FindObjectsByType<PlayerController>(FindObjectsSortMode.None))
            {
                AddTargetToCorrectPortal(player.transform, portalA, portalB, entitiesNearA, entitiesNearB);
            }

            foreach (Skeleton skeleton in GameObject.FindObjectsByType<Skeleton>(FindObjectsSortMode.None))
            {
                if (skeleton == null || !skeleton.IsAlive())
                {
                    continue;
                }

                AddTargetToCorrectPortal(skeleton.transform, portalA, portalB, entitiesNearA, entitiesNearB);
            }
        }

        private void AddTargetToCorrectPortal(
            Transform target,
            Vector3 portalA,
            Vector3 portalB,
            List<Transform> entitiesNearA,
            List<Transform> entitiesNearB
        )
        {
            if (target == null)
            {
                return;
            }

            float distanceToA = Vector3.Distance(target.position, portalA);
            float distanceToB = Vector3.Distance(target.position, portalB);

            bool isNearA = distanceToA <= radius;
            bool isNearB = distanceToB <= radius;

            if (!isNearA && !isNearB)
            {
                return;
            }

            if (isNearA && (!isNearB || distanceToA <= distanceToB))
            {
                entitiesNearA.Add(target);
            }
            else if (isNearB)
            {
                entitiesNearB.Add(target);
            }
        }

        private void TeleportTarget(Transform target, Vector3 destination)
        {
            if (target == null)
            {
                return;
            }

            Vector3 newPosition = destination + Vector3.up * heightOffset;
            target.position = newPosition;

            if (resetVelocityAfterTeleport)
            {
                Rigidbody rb = target.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }

        private void SpawnPortalVisual(Vector3 position)
        {
            FindVisualIfNeeded();

            if (visual == null)
            {
                if (debugLogs)
                {
                    Debug.LogWarning("TeleportEffect: visual is not assigned and could not be found in children.");
                }

                return;
            }

            position += Vector3.up * visualHeightOffset;

            GameObject spawnedVisual = Instantiate(visual, position, Quaternion.identity, null);

            spawnedVisual.transform.localScale = new Vector3(
                radius * 2f,
                0.03f,
                radius * 2f
            );

            spawnedVisual.SetActive(true);
            spawnedVisuals.Add(spawnedVisual);

            if (debugLogs)
            {
                Debug.Log($"TeleportEffect: visual spawned at {position}.");
            }
        }

        private void FindVisualIfNeeded()
        {
            if (visual != null)
            {
                visual.SetActive(false);
                return;
            }

            Transform visualTransform = transform.Find("GameObject/visual");

            if (visualTransform == null)
            {
                visualTransform = transform.Find("visual");
            }

            if (visualTransform == null)
            {
                visualTransform = GetChildByName(transform, "visual");
            }

            if (visualTransform != null)
            {
                visual = visualTransform.gameObject;
                visual.SetActive(false);
            }
        }

        private Transform GetChildByName(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    return child;
                }

                Transform result = GetChildByName(child, childName);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private void FinishAfterTeleport()
        {
            finished = true;
            isWaitingForSecondPortal = false;
            destroyTimer = visualLifetimeAfterTeleport;
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

        public void OnDestroy()
        {
            if (waitingPortal == this)
            {
                waitingPortal = null;
            }

            DestroySpawnedVisuals();
        }
    }
}