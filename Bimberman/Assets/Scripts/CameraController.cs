using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public float height = 5f;
    public float distance = 5f;
    public float angle = 0f;

    private bool stationMode = false;
    private Transform stationPoint;

    public float smoothSpeed = 5f;
    private Vector3 velocity = Vector3.zero;

    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;

    private bool isTransitioning = false;
    public float transitionSpeed = 2f;


    void LateUpdate()
    {
        if (isTransitioning) return;

        if (stationMode && stationPoint != null)
        {
            transform.position = stationPoint.position;
            transform.rotation = stationPoint.rotation;
            return;
        }

        if (target == null) return;

        Quaternion rotation = Quaternion.Euler(angle, 45f, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        Vector3 desiredPosition = target.position + offset + Vector3.up * height;
        transform.position = desiredPosition;
        transform.LookAt(target);
    }

    //public void EnterStationMode(Transform point)
    //{
    //    stationMode = true;
    //    stationPoint = point;
    //}

    //public void ExitStationMode()
    //{
    //    stationMode = false;
    //    stationPoint = null;
    //}
    public void EnterStationMode(Transform point)
    {
        if (isTransitioning) return;
        stationPoint = point;
        StartCoroutine(TransitionToStation());
    }

    public void ExitStationMode()
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionFromStation());
    }

    private IEnumerator TransitionToStation()
    {
        isTransitioning = true;
        float t = 0;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            transform.position = Vector3.Lerp(startPos, stationPoint.position, t);
            transform.rotation = Quaternion.Slerp(startRot, stationPoint.rotation, t);
            yield return null;
        }

        stationMode = true;
        isTransitioning = false;
    }

    private IEnumerator TransitionFromStation()
    {
        isTransitioning = true;
        float t = 0;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Quaternion rotation = Quaternion.Euler(angle, 45f, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        Vector3 targetPos = target.position + offset + Vector3.up * height;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            Vector3 currentTargetPos = target.position + offset + Vector3.up * height;
            transform.position = Vector3.Lerp(startPos, currentTargetPos, t);
            Quaternion lookRot = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(startRot, lookRot, t);
            yield return null;
        }

        stationMode = false;
        isTransitioning = false;
    }

    public void ShakeCamera()
    {
        Debug.Log("Camera shake triggered.");
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}