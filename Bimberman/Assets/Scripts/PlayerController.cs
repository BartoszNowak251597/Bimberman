using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController playerInstance;

    public enum PlayerMode
    {
        Normal,
        StationaryPOV
    }

    [Header("General")]
    public PlayerMode currentMode = PlayerMode.Normal;
    public float moveSpeed = 5f;
    public Camera playerCamera;
    public bool canMove = true;
    public PlayerInventory inventory;
    public Interaction interaction;
    public LevelLoader loader;

    private Rigidbody rb;
    private PlayerInputActions inputActions;

    private Vector2 moveInput;

    public Slider healthBar;
    public int maxHealth = 10;
    private int health = 10;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (inputActions == null)
            inputActions = new PlayerInputActions();

        playerInstance = this;

        healthBar = GameObject.Find("Healthbar").GetComponent<Slider>();
        health = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
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
        if (inputActions == null)
            return;

        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        switch (currentMode)
        {
            case PlayerMode.Normal:
                HandleNormalRotation();
                break;

            case PlayerMode.StationaryPOV:
                break;
        }

        if (!canMove)
        {
            moveInput = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        if (currentMode == PlayerMode.Normal && canMove)
        {
            Move();
        }
    }

    void Move()
    {
        if (playerCamera == null) return;

        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movement = cameraForward * moveInput.y + cameraRight * moveInput.x;

        if (movement.sqrMagnitude > 1f)
            movement.Normalize();

        Vector3 newPosition = rb.position + moveSpeed * movement * Time.fixedDeltaTime * Time.timeScale;
        rb.MovePosition(newPosition);
    }

    void HandleNormalRotation()
    {
        RotateToMouse();
    }

    void RotateToMouse()
    {
        if (playerCamera == null) return;

        Vector2 mousePosition = inputActions.Player.Look.ReadValue<Vector2>();

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

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.value = health;

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player has died.");
        gameObject.SetActive(false);
        loader.LoadLevel();
    }

    public void SetMode(PlayerMode mode)
    {
        currentMode = mode;

        switch (mode)
        {
            case PlayerMode.Normal:
                SetMovementEnabled(true);

                if (interaction != null)
                    interaction.enabled = true;
                break;

            case PlayerMode.StationaryPOV:
                SetMovementEnabled(false);

                if (interaction != null)
                    interaction.enabled = true;
                break;
        }
    }

    public void EnterStationMode()
    {
        SetMovementEnabled(false);

        if (interaction != null)
        {
            interaction.enabled = false;

            if (interaction.interactionPrompt != null)
                interaction.interactionPrompt.SetActive(false);
        }
    }

    public void ExitStationMode()
    {
        SetMovementEnabled(false);

        if (interaction != null)
        {
            interaction.enabled = true;
        }
    }
}