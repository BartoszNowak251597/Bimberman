using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseViewController : MonoBehaviour
{
    [Header("Look Points")]
    [SerializeField] private Transform[] lookPoints;

    [Header("Base Camera")]
    [SerializeField] private Transform playerOrigin;
    [SerializeField] private Vector3 cameraLocalOffset = new Vector3(0f, 1.6f, 0f);

    [Header("Rotation")]
    [SerializeField] private float rotationDuration = 0.2f;

    [Header("UI Transition")]
    [SerializeField] private float moveDuration = 0.3f;

    private int currentIndex;
    private bool canControl;
    private bool isBusy;
    private bool isInUiView;
    private Coroutine activeCoroutine;

    public void Initialize(Transform newPlayerOrigin, Transform[] newLookPoints, Vector3 newCameraLocalOffset, int startIndex = 0)
    {
        playerOrigin = newPlayerOrigin;
        lookPoints = newLookPoints;
        cameraLocalOffset = newCameraLocalOffset;

        if (playerOrigin == null || lookPoints == null || lookPoints.Length == 0)
            return;

        currentIndex = Mathf.Clamp(startIndex, 0, lookPoints.Length - 1);
        ForceBaseView();
    }

    private void OnEnable()
    {
        canControl = true;
        isBusy = false;
        isInUiView = false;
    }

    private void OnDisable()
    {
        canControl = false;
        isBusy = false;
        isInUiView = false;

        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }
    }

    private void Update()
    {
        if (!canControl) return;
        if (isBusy) return;
        if (isInUiView) return;
        if (lookPoints == null || lookPoints.Length == 0) return;

        HandleKeyboardInput();
    }

    private void HandleKeyboardInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            PreviousPoint();
            return;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            NextPoint();
            return;
        }
    }

    public bool IsBusy()
    {
        return isBusy;
    }

    public Transform GetCurrentLookPoint()
    {
        if (lookPoints == null || lookPoints.Length == 0)
            return null;

        return lookPoints[currentIndex];
    }

    public void NextPoint()
    {
        if (lookPoints == null || lookPoints.Length == 0) return;

        int nextIndex = currentIndex + 1;
        if (nextIndex >= lookPoints.Length)
            nextIndex = 0;

        SetPoint(nextIndex);
    }

    public void PreviousPoint()
    {
        if (lookPoints == null || lookPoints.Length == 0) return;

        int prevIndex = currentIndex - 1;
        if (prevIndex < 0)
            prevIndex = lookPoints.Length - 1;

        SetPoint(prevIndex);
    }

    public void SetPoint(int index)
    {
        if (lookPoints == null || lookPoints.Length == 0) return;
        if (index < 0 || index >= lookPoints.Length) return;
        if (isInUiView) return;

        currentIndex = index;
        StartNewCoroutine(RotateToCurrentLookPoint());
    }

    public void EnterUiView(Transform uiCameraPoint)
    {
        if (uiCameraPoint == null) return;

        isInUiView = true;
        StartNewCoroutine(MoveToUiPoint(uiCameraPoint));
    }

    public void ExitUiView()
    {
        isInUiView = false;
        StartNewCoroutine(ReturnToBaseView());
    }

    public void ForceBaseView()
    {
        Vector3 basePos = GetBaseCameraPosition();
        transform.position = basePos;

        Transform target = GetCurrentLookPoint();
        if (target != null)
        {
            Vector3 dir = target.position - basePos;
            if (dir.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
            }
        }
    }

    private Vector3 GetBaseCameraPosition()
    {
        if (playerOrigin == null)
            return transform.position;

        return playerOrigin.position + playerOrigin.TransformDirection(cameraLocalOffset);
    }

    private IEnumerator RotateToCurrentLookPoint()
    {
        Transform target = GetCurrentLookPoint();
        if (target == null)
            yield break;

        isBusy = true;

        Vector3 basePos = GetBaseCameraPosition();
        transform.position = basePos;

        Quaternion startRot = transform.rotation;

        Vector3 dir = target.position - basePos;
        if (dir.sqrMagnitude < 0.0001f)
        {
            isBusy = false;
            activeCoroutine = null;
            yield break;
        }

        Quaternion endRot = Quaternion.LookRotation(dir.normalized, Vector3.up);

        float time = 0f;
        while (time < rotationDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / rotationDuration);

            transform.position = basePos;
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        transform.position = basePos;
        transform.rotation = endRot;

        isBusy = false;
        activeCoroutine = null;
    }

    private IEnumerator MoveToUiPoint(Transform targetPoint)
    {
        isBusy = true;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 endPos = targetPoint.position;
        Quaternion endRot = targetPoint.rotation;

        float time = 0f;
        while (time < moveDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / moveDuration);

            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        transform.position = endPos;
        transform.rotation = endRot;

        isBusy = false;
        activeCoroutine = null;
    }

    private IEnumerator ReturnToBaseView()
    {
        isBusy = true;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 endPos = GetBaseCameraPosition();
        Quaternion endRot = startRot;

        Transform target = GetCurrentLookPoint();
        if (target != null)
        {
            Vector3 dir = target.position - endPos;
            if (dir.sqrMagnitude > 0.0001f)
            {
                endRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
            }
        }

        float time = 0f;
        while (time < moveDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / moveDuration);

            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        transform.position = endPos;
        transform.rotation = endRot;

        isBusy = false;
        activeCoroutine = null;
    }

    private void StartNewCoroutine(IEnumerator routine)
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        activeCoroutine = StartCoroutine(routine);
    }
}