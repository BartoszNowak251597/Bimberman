using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public float height = 5f;
    public float distance = 5f;
    public float angle = 0f;

    private bool stationMode = false;
    private Transform stationPoint;

    public Vector3 lookAtPos;

    public float cameraDrunk = 0.0f;

    void LateUpdate()
    {
        if (stationMode && stationPoint != null)
        {
            transform.position = stationPoint.position;
            transform.rotation = stationPoint.rotation;
            return;
        }

        if (target == null) return;

        Quaternion rotation = Quaternion.Euler(angle, 45f, 0);

        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        Vector3 targetPos = Vector3.Lerp(
            PlayerController.playerInstance.transform.position,
            PlayerController.playerInstance.targetPoint,
            0.6f
        );

        this.lookAtPos = Vector3.Lerp(this.lookAtPos, targetPos, 0.02f);

        transform.position = (this.lookAtPos * (1 - cameraDrunk)) + offset + Vector3.up * height;
        transform.LookAt(this.lookAtPos);
    }

    public void EnterStationMode(Transform point)
    {
        stationMode = true;
        stationPoint = point;
    }

    public void ExitStationMode()
    {
        stationMode = false;
        stationPoint = null;
    }
}