using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public float height = 5f;
    public float distance = 5f;
    public float angle = 0f;

    private bool stationMode = false;
    private Transform stationPoint;

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

        transform.position = target.position + offset + Vector3.up * height;
        transform.LookAt(target);
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