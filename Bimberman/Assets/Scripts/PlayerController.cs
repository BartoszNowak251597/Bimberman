using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController playerInstance;

    public float moveSpeed = 5f;
    public Camera playerCamera;
    public bool canMove = true;
    public PlayerInventory inventory;

    private Rigidbody rb;
    private PlayerInputActions inputActions;

    private Vector2 moveInput;
    private Vector2 mousePosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (inputActions == null)
            inputActions = new PlayerInputActions();

        playerInstance = this;
    }

    private void OnEnable()
    {
        if (inputActions == null)
            inputActions = new PlayerInputActions();

        inputActions.Enable();
    }

    void OnDisable()
    {
        if (inputActions != null)
            inputActions.Disable();
    }

    void Update()
    {
        if (!canMove)
        {
            moveInput = Vector2.zero;
            return;
        }

        if (inputActions == null)
            return;

        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        mousePosition = inputActions.Player.Look.ReadValue<Vector2>();

        RotateToMouse();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {

        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movement = cameraForward * moveInput.y + cameraRight * moveInput.x;

        if (movement.sqrMagnitude > 1f)
            movement.Normalize();

        Vector3 newPosition = rb.position + moveSpeed * movement * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    void RotateToMouse()
    {
        if (playerCamera == null) return;

        Ray ray = playerCamera.ScreenPointToRay(mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 targetPoint = ray.GetPoint(rayDistance);
            Vector3 lookDirection = targetPoint - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = targetRotation;
            }
        }
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
    }
}