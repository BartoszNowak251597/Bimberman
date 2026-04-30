using TMPro;
using UnityEngine;

namespace Crafting
{
    public class PlayerMouseInteractor : MonoBehaviour
    {
        public Camera cam;
        public float interactDistance = 5f;
        public LayerMask interactLayer;
        public TextMeshProUGUI interactionText;

        [Header("Dragging")]
        public float dragHeight = 0.5f;
        public float dragSmooth = 15f;

        [Header("Cauldron Lift")]
        public Transform cauldronCenter;
        public float cauldronLiftRadius = 2f;
        public float maxLiftHeight = 1.5f;
        [Range(0f, 1f)] public float cauldronPullStrength = 0.35f;

        [Header("Drop Into Cauldron")]
        public Cauldron cauldron;
        public float cauldronDropRadius = 0.9f;

        [Header("Drop Under Tap")]
        public TapDispenser tapDispenser;
        public Transform tapCenter;
        public float tapDropRadius = 1.2f;

        private IInteractable currentInteractable;
        private IInteractable pressedInteractable;

        private IngredientData heldIngredient;
        private BottleData heldBottle;
        private IngredientWorldItem heldWorldIngredient;
        private FillableBottle heldFillableBottle;

        private Plane dragPlane;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            dragPlane = new Plane(Vector3.up, new Vector3(0f, dragHeight, 0f));
        }

        private void Update()
        {
            HandleHover();
            UpdateDraggedItem();

            if (Input.GetMouseButtonDown(0) && currentInteractable != null)
            {
                pressedInteractable = currentInteractable;

                pressedInteractable.OnClick(this);
                pressedInteractable.OnPressStart(this);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (pressedInteractable != null)
                {
                    pressedInteractable.OnPressEnd(this);
                    pressedInteractable = null;
                }

                TryReleaseHeldItem();
            }
        }

        private void HandleHover()
        {
            if (cam == null)
                return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            IInteractable newInteractable = null;

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
            {
                newInteractable = hit.collider.GetComponentInParent<IInteractable>();
            }

            if (currentInteractable != newInteractable)
            {
                currentInteractable?.OnHoverExit();
                currentInteractable = newInteractable;
                currentInteractable?.OnHoverEnter();
            }

            if (interactionText != null)
            {
                interactionText.text = currentInteractable != null
                    ? currentInteractable.GetInteractionText(this)
                    : "";
            }
        }

        private void UpdateDraggedItem()
        {
            Transform draggedTransform = GetDraggedTransform();

            if (draggedTransform == null || cam == null)
                return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 targetPos = ray.GetPoint(enter);
                targetPos.y = dragHeight;

                if (cauldronCenter != null && heldWorldIngredient != null)
                {
                    Vector2 itemPos2D = new Vector2(targetPos.x, targetPos.z);
                    Vector2 cauldronPos2D = new Vector2(
                        cauldronCenter.position.x,
                        cauldronCenter.position.z
                    );

                    float distanceToCauldron = Vector2.Distance(itemPos2D, cauldronPos2D);

                    if (distanceToCauldron < cauldronLiftRadius)
                    {
                        float t = 1f - (distanceToCauldron / cauldronLiftRadius);

                        float liftedHeight = Mathf.Lerp(
                            dragHeight,
                            dragHeight + maxLiftHeight,
                            t
                        );

                        targetPos.y = liftedHeight;

                        targetPos.x = Mathf.Lerp(
                            targetPos.x,
                            cauldronCenter.position.x,
                            t * cauldronPullStrength
                        );

                        targetPos.z = Mathf.Lerp(
                            targetPos.z,
                            cauldronCenter.position.z,
                            t * cauldronPullStrength
                        );
                    }
                }

                draggedTransform.position = Vector3.Lerp(
                    draggedTransform.position,
                    targetPos,
                    Time.deltaTime * dragSmooth
                );
            }
        }

        private Transform GetDraggedTransform()
        {
            if (heldWorldIngredient != null)
                return heldWorldIngredient.transform;

            if (heldFillableBottle != null)
                return heldFillableBottle.transform;

            return null;
        }

        private void TryReleaseHeldItem()
        {
            if (heldWorldIngredient != null)
            {
                if (CanDropIntoCauldron())
                {
                    cauldron.AddHeldIngredientFromInteractor(this);
                    return;
                }

                DropHeldIngredient();
                return;
            }

            if (heldFillableBottle != null)
            {
                if (CanDropUnderTap())
                {
                    FillableBottle bottle = heldFillableBottle;

                    ClearHeldBottleReferencesOnly();

                    bool startedPouring = tapDispenser.TryPourTest(bottle);

                    if (startedPouring)
                        return;

                    PickupFillableBottle(bottle);
                }

                DropHeldFillableBottle();
            }
        }

        private bool CanDropIntoCauldron()
        {
            if (cauldron == null || cauldronCenter == null)
                return false;

            if (!HasIngredient())
                return false;

            Vector2 itemPos2D = new Vector2(
                heldWorldIngredient.transform.position.x,
                heldWorldIngredient.transform.position.z
            );

            Vector2 cauldronPos2D = new Vector2(
                cauldronCenter.position.x,
                cauldronCenter.position.z
            );

            float distance = Vector2.Distance(itemPos2D, cauldronPos2D);

            return distance <= cauldronDropRadius;
        }

        private bool CanDropUnderTap()
        {
            if (tapDispenser == null)
                return false;

            if (heldFillableBottle == null)
                return false;

            Transform center = tapCenter != null ? tapCenter : tapDispenser.transform;

            Vector2 bottlePos2D = new Vector2(
                heldFillableBottle.transform.position.x,
                heldFillableBottle.transform.position.z
            );

            Vector2 tapPos2D = new Vector2(
                center.position.x,
                center.position.z
            );

            float distance = Vector2.Distance(bottlePos2D, tapPos2D);

            return distance <= tapDropRadius;
        }

        public bool HasIngredient()
        {
            return heldIngredient != null;
        }

        public bool HasBottle()
        {
            return heldBottle != null || heldFillableBottle != null;
        }

        public bool HasFillableBottle()
        {
            return heldFillableBottle != null;
        }

        public IngredientData GetHeldIngredient()
        {
            return heldIngredient;
        }

        public BottleData GetHeldBottle()
        {
            return heldBottle;
        }

        public IngredientWorldItem GetHeldWorldIngredient()
        {
            return heldWorldIngredient;
        }

        public FillableBottle GetHeldFillableBottle()
        {
            return heldFillableBottle;
        }

        public void PickupWorldIngredient(IngredientWorldItem item)
        {
            if (item == null)
                return;

            heldWorldIngredient = item;
            heldIngredient = item.data;

            heldBottle = null;
            heldFillableBottle = null;

            item.SetHeld(true);

            Rigidbody rb = item.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }

        public void DropHeldIngredient()
        {
            if (heldWorldIngredient == null)
                return;

            heldWorldIngredient.SetHeld(false);

            Rigidbody rb = heldWorldIngredient.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            heldWorldIngredient = null;
            heldIngredient = null;
        }

        public void PickupFillableBottle(FillableBottle bottle)
        {
            if (bottle == null)
                return;

            heldFillableBottle = bottle;
            heldBottle = bottle.data;

            heldIngredient = null;
            heldWorldIngredient = null;

            bottle.SetHeld(true);
        }

        public void DropHeldFillableBottle()
        {
            if (heldFillableBottle == null)
                return;

            heldFillableBottle.SetHeld(false);

            heldFillableBottle = null;
            heldBottle = null;
        }

        public void ClearHeldBottleReferencesOnly()
        {
            heldFillableBottle = null;
            heldBottle = null;
        }

        public void HoldBottle(BottleData bottle)
        {
            heldBottle = bottle;

            heldIngredient = null;
            heldWorldIngredient = null;
            heldFillableBottle = null;
        }

        public void ClearHeldReferencesOnly()
        {
            heldWorldIngredient = null;
            heldIngredient = null;
        }

        public void ClearHands()
        {
            heldIngredient = null;
            heldBottle = null;
            heldWorldIngredient = null;
            heldFillableBottle = null;
        }
    }
}