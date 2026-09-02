using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InfusionPedestal : MonoBehaviour
{
    [Header("Inserted Ingredient")]
    [SerializeField] IngredientInstance CurrentInsertedIngredient;


    [Header("Floating Settings")]
    [SerializeField] Transform FloatingTarget;
    [SerializeField] float FloatingStrength;
    [SerializeField] float DampingStrength;
    [SerializeField] float BobbingFrequency;
    [SerializeField] float BobbingAmplitude;
    [Space(10)]
    [SerializeField] float SpinSpeed = 30f;

    [Header("Events")]
    public UnityEvent IngredientSelected;
    public UnityEvent IngredientRemoved;

    bool ingredientPreviousGravity;
    float ingredientPreviousAngularDamping;

    float initialHoverOffset;
    float currentYRotation;

    public IngredientInstance GetInsertedIngredient()
    {
        return CurrentInsertedIngredient;
    }

    private void Start()
    {
        initialHoverOffset = Random.Range(-1f, 1f);
    }

    private void FixedUpdate()
    {
        if (CurrentInsertedIngredient)
        {
            float currentFloatOffset = Mathf.Sin(initialHoverOffset + Time.time * BobbingFrequency) * BobbingAmplitude;

            Vector3 floatTarget = new(FloatingTarget.position.x, FloatingTarget.position.y + currentFloatOffset, FloatingTarget.position.z);

            currentYRotation += SpinSpeed * Time.fixedDeltaTime;
            currentYRotation %= 360f; // Keep it within 0-360 range

            // 2. Combine base identity rotation (0,0,0) with the Y spin
            Quaternion targetRotation = Quaternion.Euler(0f, currentYRotation, 0f);

            // 3. Find the difference between where we are and where we want to be
            Quaternion rotationDifference = targetRotation * Quaternion.Inverse(transform.rotation);

            // 4. Convert the difference to an angle-axis vector
            rotationDifference.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);

            // Handle angle wrapping
            if (angleInDegrees > 180f)
                angleInDegrees -= 360f;

            // 5. Apply angular velocity to smoothly rotate the object
            Vector3 angularForce = (rotationAxis * (angleInDegrees * Mathf.Deg2Rad) * 5) - CurrentInsertedIngredient.AttachedRB.angularVelocity;

            CurrentInsertedIngredient.AttachedRB.AddTorque(angularForce, ForceMode.Acceleration);




            Vector3 offset = floatTarget - CurrentInsertedIngredient.transform.position;

            Vector3 force = (offset * FloatingStrength) - (CurrentInsertedIngredient.AttachedRB.linearVelocity * DampingStrength);

            CurrentInsertedIngredient.AttachedRB.AddForce(force, ForceMode.Acceleration);


            //CurrentInsertedIngredient.AttachedRB.AddTorque(Vector3.up * 0.05f, ForceMode.Acceleration);
        }
    }

    //This is called when we try to put an ingredient into the "socket"
    private void OnTriggerEnter(Collider other)
    {
        //If we already have something inserted, we aren't going to check anything
        if (CurrentInsertedIngredient) return;

        var addedIngredient = other.gameObject.GetComponent<IngredientInstance>();

        if (!addedIngredient)
            addedIngredient = other.gameObject.GetComponentInParent<IngredientInstance>();

        if (!addedIngredient) return;

        if (!addedIngredient.GetInteractable().isSelected)
        {
            PedestalSelectIngredient(addedIngredient);
        }
        else
        {
            addedIngredient.GetInteractable().selectExited.AddListener(HoveredIngredientReleased);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var removedIngredient = other.gameObject.GetComponent<IngredientInstance>();

        if (!removedIngredient)
            removedIngredient = other.gameObject.GetComponentInParent<IngredientInstance>();

        if (!removedIngredient) return;

        if(CurrentInsertedIngredient == removedIngredient)
        {
            PedestalIngredientRemoved(null);
        }
        else
        {
            removedIngredient.GetInteractable().selectExited.RemoveListener(HoveredIngredientReleased);
        }

    }
    void PedestalSelectIngredient(IngredientInstance ingredient)
    {
        CurrentInsertedIngredient = ingredient;

        CurrentInsertedIngredient.GetInteractable().selectEntered.AddListener(PedestalIngredientRemoved);

        ingredientPreviousGravity = CurrentInsertedIngredient.AttachedRB.useGravity;
        ingredientPreviousAngularDamping = CurrentInsertedIngredient.AttachedRB.angularDamping;

        CurrentInsertedIngredient.AttachedRB.useGravity = false;
        CurrentInsertedIngredient.AttachedRB.angularDamping = 6.5f;

        IngredientSelected?.Invoke();
    }

    void HoveredIngredientReleased(SelectExitEventArgs args)
    {
        if (!CurrentInsertedIngredient)
        {
            var droppedIngredient = args.interactableObject.transform.GetComponent<IngredientInstance>();

            if (!droppedIngredient)
                droppedIngredient = args.interactableObject.transform.GetComponentInParent<IngredientInstance>();

            PedestalSelectIngredient(droppedIngredient); 
        }

        args.interactableObject.selectExited.RemoveListener(HoveredIngredientReleased);
    }

    void PedestalIngredientRemoved(SelectEnterEventArgs args)
    {
        CurrentInsertedIngredient.GetInteractable().selectEntered.RemoveListener(PedestalIngredientRemoved);

        CurrentInsertedIngredient.AttachedRB.useGravity = ingredientPreviousGravity;
        CurrentInsertedIngredient.AttachedRB.angularDamping = ingredientPreviousAngularDamping;

        CurrentInsertedIngredient = null;

        IngredientRemoved?.Invoke();
    }

    public void ClearIngredient()
    {
        if(CurrentInsertedIngredient)
        {
            Destroy(CurrentInsertedIngredient.gameObject);
        }

        CurrentInsertedIngredient = null;
    }
}
