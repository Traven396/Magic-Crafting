using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(IngredientInstance))]
public abstract class IngredientProcessingMethod : MonoBehaviour
{
    public GameObject ProcessOutput;
    //How many of the output item do you get for completeing this?
    [SerializeField] [Min(1)] public int OutputAmount;
    
    //The amount if how much each step takes to complete
    [SerializeField] [Min(0.1f)] public float RequiredProcessAmount = 5;
    //The steps are how many times the process needs to be completed
    [SerializeField] [Min(1)] public int RequiredProcessSteps = 5;

    public UnityEvent OnStepComplete;
    public UnityEvent OnProcessComplete;

    public abstract void Process(float progress);
}
