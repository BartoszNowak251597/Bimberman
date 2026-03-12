using UnityEngine;

public class Collectible : InteractiveItem {
	public void Update() {
		this.interactPrompt.position = this.transform.position + Vector3.up;
	}

	public override void Interact()
	{
		PlayerController.playerInstance.inventory.Collect(this);
	}
}