using UnityEngine;

public class PotionCollectible : InteractiveItem
{
    public Potion potion;

	public override void Interact()
	{
		PlayerController.playerInstance.inventory.Collect(potion);

        potion = null;
	}

	public override bool CanInteract()
	{
		return potion != null;
	}
}