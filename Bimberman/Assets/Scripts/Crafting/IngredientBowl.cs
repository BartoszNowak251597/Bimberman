using System.Collections.Generic;
using UnityEngine;

namespace Crafting
{
    public class IngredientBowl : MonoBehaviour
    {
        public IngredientData ingredientData;
        public IngredientWorldItem ingredientPrefab;

        [Header("Amount")]
        public int storedAmount = 3;
        public int maxVisibleItems = 10;

        [Header("Spawn")]
        public Transform visualRoot;
        public Transform spawnCenter;
        public float verticalSpacing = 0.015f;
        public bool randomYRotation = true;

        private readonly List<IngredientWorldItem> spawnedItems = new();

        private void Start()
        {
            RefreshVisuals();
        }

        public void RefreshVisuals()
        {
            ClearVisuals();

            if (ingredientData == null || ingredientPrefab == null || visualRoot == null || spawnCenter == null)
                return;

            int visibleCount = Mathf.Min(storedAmount, maxVisibleItems);

            for (int i = 0; i < visibleCount; i++)
            {
                Vector3 spawnPos = spawnCenter.position + Vector3.up * (verticalSpacing * i);
                Quaternion spawnRot = randomYRotation
                    ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
                    : spawnCenter.rotation;

                IngredientWorldItem item = Instantiate(
                    ingredientPrefab,
                    spawnPos,
                    spawnRot
                );

                item.transform.SetParent(visualRoot, true);
                SetWorldScale(item.transform, ingredientPrefab.transform.lossyScale);

                item.gameObject.layer = LayerMask.NameToLayer("Interaction");

                item.data = ingredientData;
                item.sourceBowl = this;
                item.ApplyColorFromData();

                Rigidbody rb = item.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                spawnedItems.Add(item);
            }
        }

        private void SetWorldScale(Transform target, Vector3 desiredWorldScale)
        {
            Vector3 parentScale = target.parent != null ? target.parent.lossyScale : Vector3.one;

            target.localScale = new Vector3(
                parentScale.x != 0 ? desiredWorldScale.x / parentScale.x : desiredWorldScale.x,
                parentScale.y != 0 ? desiredWorldScale.y / parentScale.y : desiredWorldScale.y,
                parentScale.z != 0 ? desiredWorldScale.z / parentScale.z : desiredWorldScale.z
            );
        }

        public void NotifyIngredientTaken(IngredientWorldItem item)
        {
            storedAmount = Mathf.Max(0, storedAmount - 1);
            spawnedItems.Remove(item);
        }

        private void ClearVisuals()
        {
            for (int i = spawnedItems.Count - 1; i >= 0; i--)
            {
                if (spawnedItems[i] != null)
                    Destroy(spawnedItems[i].gameObject);
            }

            spawnedItems.Clear();
        }
    }
}