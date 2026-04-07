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
    private Vector3 aimOffset;
    private bool isDead = false;

    public Slider healthBar;
    public int maxHealth = 10;
    private int health = 10;
	public Vector3 targetPoint;
    public GameObject reticle;
    public InventoryUI inventoryUI;

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

        aimOffset = transform.forward * 2f;
        aimOffset.y = 0f;
        targetPoint = transform.position + aimOffset;

        if (reticle != null)
            reticle.transform.position = targetPoint + Vector3.up * 0.05f;
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

        if (isDead)
            return;

        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        switch (currentMode)
        {
            case PlayerMode.Normal:
                if (inventoryUI == null || !inventoryUI.isActiveAndEnabled)
                {
                    HandleNormalRotation();
                }
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
        if (isDead)
            return;

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
        if (inputActions == null) return;

        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector2 cursorDelta = inputActions.Player.Cursor.ReadValue<Vector2>();

        aimOffset += 0.02f * (
            cameraForward * cursorDelta.y +
            cameraRight * cursorDelta.x
        );

        aimOffset.y = 0f;

        targetPoint = transform.position + aimOffset;

        if (reticle != null)
        {
            reticle.transform.position = targetPoint + Vector3.up * 0.05f;
        }

        Vector3 lookDirection = aimOffset;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = targetRotation;
        }
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

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

        isDead = true;
        canMove = false;
        moveInput = Vector2.zero;

        if (interaction != null)
            interaction.enabled = false;

        if (inputActions != null)
            inputActions.Disable();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        loader.LoadLevel();
    }

    public void ResetAfterRespawn()
    {
        isDead = false;
        canMove = true;
        moveInput = Vector2.zero;

        if (interaction != null)
            interaction.enabled = true;

        if (inputActions != null)
            inputActions.Enable();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        aimOffset = transform.forward * 2f;
        aimOffset.y = 0f;
        targetPoint = transform.position + aimOffset;

        if (reticle != null)
            reticle.transform.position = targetPoint + Vector3.up * 0.05f;

        health = maxHealth;
        if (healthBar != null)
            healthBar.value = health;
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