using System.Collections;
using TMPro;
using UnityEngine;

public class CollectibleGiver : InteractiveItem {
    public GameObject thingPrefab;

    public TMP_Text pickupText;
    public string pickupMessage;

    private Coroutine messageCoroutine;
    private Vector3 startPosition;

    private void Start()
    {
        if (pickupText != null)
        {
            startPosition = pickupText.rectTransform.localPosition;
            pickupText.gameObject.SetActive(false);
        }
    }

    public override void Interact() {
        GiveThingToPlayer();
    }

    public void GiveThingToPlayer()
    {
        var inventory = PlayerController.playerInstance.inventory;
        inventory.Collect(Instantiate(thingPrefab).GetComponent<Collectible>());

        if (pickupText == null)
            return;

        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        pickupText.gameObject.SetActive(false);
        messageCoroutine = StartCoroutine(ShowPickupMessage());
    }

    private IEnumerator ShowPickupMessage()
    {
        RectTransform textTransform = pickupText.rectTransform;

        pickupText.text = pickupMessage;
        pickupText.gameObject.SetActive(true);

        Color color = pickupText.color;
        color.a = 1f;
        pickupText.color = color;

        textTransform.localPosition = startPosition;

        Vector3 endPosition = startPosition + new Vector3(0f, 2f, 0f);

        float time = 0f;
        float duration = 1.5f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            textTransform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
            color.a = Mathf.Lerp(1f, 0f, t);
            pickupText.color = color;

            yield return null;
        }

        textTransform.localPosition = startPosition;
        pickupText.gameObject.SetActive(false);
        messageCoroutine = null;
    }
}