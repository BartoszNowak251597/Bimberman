using UnityEngine;

public class Collectible : InteractiveItem {
	public void Update() {
		if (this.interactPrompt)
		{
			this.interactPrompt.position = this.transform.position + Vector3.up;
		}
	}

	public override void Interact()
	{
		PlayerController.playerInstance.inventory.Collect(this);
	}

	public virtual string DisplayName()
	{
		return "";
	}
}