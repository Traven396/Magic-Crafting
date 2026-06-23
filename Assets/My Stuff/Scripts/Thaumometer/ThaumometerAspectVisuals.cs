using Alchemy.Inspector;
using UnityEngine;

public class ThaumometerAspectVisuals : MonoBehaviour
{
    [Title("References")]
    public Transform AspectDisplayPoint;
    public GameObject AspectUIPrefab;
    

    private IngredientInstance _currentIngredient;


    private GameObject[] currentAspectUIVisuals = new GameObject[0];


    public void CreateUIVisuals(IngredientInstance currentIngredient)
    {
        _currentIngredient = currentIngredient;

        currentAspectUIVisuals = new GameObject[currentIngredient.Tags.Count];

        for (int i = 0; i < currentIngredient.Tags.Count; i++)
        {
            currentAspectUIVisuals[i] = Instantiate(AspectUIPrefab, AspectDisplayPoint);
            // Set the visual to the correct aspect tag
            // newUIVisual.GetComponent<ThaumometerAspectUIVisual>().SetAspectTag(currentIngredient.Tags[i]);
            if(DiscoveriesSingleton.Instance.IsIngredientDiscovered(currentIngredient))
            {
                currentAspectUIVisuals[i].GetComponent<AspectVisualSprite>().SetAspect(currentIngredient.Tags[i]);
            }
        }

    }

    public void UpdateUIVisuals(IngredientInstance currentIngredient, int currentTag)
    {
        currentAspectUIVisuals[currentTag].GetComponent<AspectVisualSprite>().SetAspect(currentIngredient.Tags[currentTag]);
        Debug.Log("We received the event call");
    }

    public void ClearUIVisuals()
    {
        _currentIngredient = null;

        foreach (var item in currentAspectUIVisuals)
        {
            Destroy(item);
        }
        currentAspectUIVisuals = new GameObject[0];
    }


    
}
