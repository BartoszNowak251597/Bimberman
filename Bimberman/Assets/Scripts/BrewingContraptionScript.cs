using UnityEngine;

public class BrewingContraptionScript : InteractiveItem {
	public Canvas uiCanvas;

	public override void Interact() {
		uiCanvas.gameObject.SetActive(true);

		interactPrompt.gameObject.SetActive(false);
	}
}