using UnityEngine;

public class AimingAid : MonoBehaviour
{
	public Vector3 forwardAxis;
	public Vector3 upAxis;

	public SkinnedMeshRenderer stretchPart;

	public void PointAt(Vector3 point)
	{
		this.transform.rotation = Quaternion.LookRotation(point - this.transform.position, Vector3.up) * Quaternion.LookRotation(forwardAxis, upAxis);
	}

	public void SetStretch(float amount)
	{
		if (amount < 0)
		{
			stretchPart.enabled = false;
		}
		else
		{
			stretchPart.enabled = true;
			stretchPart.SetBlendShapeWeight(0, amount * 100);
		}
	}
}