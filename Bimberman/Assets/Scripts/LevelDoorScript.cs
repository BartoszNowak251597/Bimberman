using UnityEngine;

public class LevelDoorScript : InteractiveItem
{
	public LevelLoader levelLoader;

	public override void Interact() {
		if (this.levelLoader)
		{
			this.levelLoader.LoadLevel();
		}
	}
}