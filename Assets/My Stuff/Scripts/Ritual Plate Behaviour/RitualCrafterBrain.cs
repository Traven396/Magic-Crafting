using Alchemy.Inspector;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class RitualCrafterBrain : MonoBehaviour
{
    [Title("Fun Stuff")]
    public Transform LeftHand;
    public Transform RightHand;
    public AudioClip ClapSound;
    public InputActionReference ClapAlternative;

    [Title("Important Stuff")]
    [SerializeField] private XRSocketInteractor centerSocket;
    [SerializeField] private XRSocketInteractor middleSocket;
    [SerializeField] private XRSocketInteractor outerSocket;

    [Title("Constellation Settings")]
    [SerializeField] GameObject StarGridParent;
    StarGridManager _GridManager;

    private RitualRecipe[] possibleRecipes;

    //This could function as a boolean to determine if we are waiting for a constellation
    RitualRecipe currentStartedRecipe;

    IngredientInstance CurrentCentralIngredient;
    IngredientInstance[] CurrentOuterIngredients;

    private float clapCooldown = 0f;

    private Vector3 previousHandLocationLeft, previousHandLocationRight;
    private Vector3 leftHandVelocity, rightHandVelocity;

    private RitualPlatePiece currentCenterPiece, currentMiddlePiece, currentOuterPiece;

    private void Awake()
    {
        _GridManager = StarGridParent.GetComponentInChildren<StarGridManager>();
    }

    private void Update()
    {
        if (previousHandLocationLeft != null) 
        { 
            leftHandVelocity = (LeftHand.position - previousHandLocationLeft) / Time.deltaTime;
        }
        if (previousHandLocationRight != null)
        {
            rightHandVelocity = (RightHand.position - previousHandLocationRight) / Time.deltaTime;
        }
        

        if (clapCooldown <= 0)
        {
            if (!centerSocket.hasSelection || !middleSocket.hasSelection || !outerSocket.hasSelection)
            {
                return;
            }

            if (currentStartedRecipe)
            {
                //If we are already in a recipe, stop checking things for the clap.
                return;
            }

            if (CheckClap() || ClapAlternative.action.WasPressedThisFrame())
            {
                clapCooldown = 1;

                CurrentCentralIngredient = GetCentralIngredient();
                CurrentOuterIngredients = GetOuterIngredients();

                CurrentRitualInfo ritualInfo = new CurrentRitualInfo(
                    CurrentCentralIngredient,
                    CurrentOuterIngredients,
                    currentOuterPiece,
                    currentMiddlePiece,
                    currentCenterPiece);

                if(ritualInfo.centralIngredient == null || ritualInfo.ingredients == null || ritualInfo.outerPiece == null || ritualInfo.middlePiece == null || ritualInfo.centerPiece == null)
                {
                    Debug.Log("Not all pieces or ingredients were valid.");
                    Debug.Log("Central Ingredient: " + ritualInfo.centralIngredient);

                    if(ritualInfo.ingredients != null)
                    {
                        foreach (IngredientItemSO ingredient in ritualInfo.ingredients)
                        {
                            Debug.Log("Outer Ingredient: " + ingredient);
                        } 
                    }

                    return;
                }
                foreach (RitualRecipe recipe in possibleRecipes)
                {
                    if (recipe.ValidRecipeConfiguration(ritualInfo))
                    {
                        currentStartedRecipe = recipe;
                        BeginRecipe();

                        break;
                    }
                }
            }
        }

        previousHandLocationLeft = LeftHand.position;
        previousHandLocationRight = RightHand.position;

        if (clapCooldown > 0)
            clapCooldown -= Time.deltaTime;
    }

    void BeginRecipe()
    {
        //If this is a recipe where we can actually put in a constellation, we open up the star grid.
        if (currentStartedRecipe.PossiblePatterns[0].InputConstellation)
            StarGridParent.SetActive(true);
        //If its not, then we are just going to make the result
        else
        {
            FinishRecipe(currentStartedRecipe.PossiblePatterns[0]);
        }
        //maybe make it face the player at some point? Would need to find the player

        //We levitate up the ingredients, make them ungrabbale? Move them around in a circle
        //We are enabling the StarGrid, and waiting for the player to draw a constellation


    }
    public void ConstellationChosen(StarPattern chosenConstellation)
    {
        var possibleList = currentStartedRecipe.PossiblePatterns.Where(pattern => pattern.InputConstellation == chosenConstellation);

        if (possibleList.Count() == 1)
        {
            FinishRecipe(possibleList.First());

            StarGridParent.SetActive(false);
            

        } else if (possibleList.Count() > 1)
        {
            Debug.Log("Multiple possible outputs for chosen constellation");
        }
        else
        {
            Debug.Log("Invalid constellation");
        }

        _GridManager.ClearGrid();
    }
    void FinishRecipe(RitualRecipe.RitualRecipeOutput output)
    {
        Instantiate(output.OutputItem, transform.position + (Vector3.up * 0.5f), Quaternion.identity);

        Destroy(CurrentCentralIngredient.gameObject);

        foreach (IngredientInstance ingredient in CurrentOuterIngredients)
        {
            Destroy(ingredient.gameObject);
        }
    }

    public void OuterSocketChanged()
    {
        if (outerSocket.hasSelection)
        {
            currentOuterPiece = outerSocket.interactablesSelected[0].transform.GetComponent<RitualPlatePiece>();
        } else
        {
            currentOuterPiece = null;
        }
        UpdatePossibleRecipes();
    }
    public void MiddleSocketChanged()
    {
        if (middleSocket.hasSelection)
        {
            currentMiddlePiece = middleSocket.interactablesSelected[0].transform.GetComponent<RitualPlatePiece>();
        }
        else
        {
            currentMiddlePiece = null;
        }
        UpdatePossibleRecipes();
    }
    public void CenterSocketChanged()
    {
        if (centerSocket.hasSelection)
        {
            currentCenterPiece = centerSocket.interactablesSelected[0].transform.GetComponent<RitualPlatePiece>();
        }
        else
        {
            currentCenterPiece = null;
        }
        UpdatePossibleRecipes();
    }
    private void UpdatePossibleRecipes()
    {
        possibleRecipes = RitualRecipeManager.Instance.GetRecipeList(currentCenterPiece, currentMiddlePiece, currentOuterPiece);
    }

    private IngredientInstance[] GetOuterIngredients() 
    {       
        if (outerSocket.hasSelection)
        {
            
            if (currentOuterPiece.TryGetIngredient(out IngredientInstance[] ingredients))
            {
                return ingredients;
            }
            
        }
        return null;
    }
    private IngredientInstance GetCentralIngredient()
    {
        if (centerSocket.hasSelection)
        {
            if (currentCenterPiece.TryGetIngredient(out IngredientInstance[] ingredient))
            {
                return ingredient[0];
            }   
        }
        return null;
    }
    private bool CheckClap()
    {
        float leftHandDotXVel = Vector3.Dot(leftHandVelocity, LeftHand.right);
        float rightHandDotXVel = Vector3.Dot(rightHandVelocity, -RightHand.right);

        if(leftHandDotXVel > 0.7f && rightHandDotXVel > 0.7f && Vector3.Distance(LeftHand.position, RightHand.position) < 0.15f && leftHandVelocity.magnitude > 1.3f && rightHandVelocity.magnitude > 1.3f)
        {
            AudioSource.PlayClipAtPoint(ClapSound, RightHand.position, .5f);
            return true;
        }
        else
        {
            return false;
        }
    }
}
