using UnityEngine;

public class CollectibleGiver : InteractiveItem {
	public GameObject thingPrefab;

	public override void Interact() {
		GiveThingToPlayer();
	}

	public void GiveThingToPlayer()
	{
		var inventory = PlayerController.playerInstance.inventory;

		inventory.Collect(Instantiate<GameObject>(this.thingPrefab).GetComponent<Collectible>());
	}
}