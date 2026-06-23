using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RitualIngredientInput : MonoBehaviour
{
    private List<IngredientInstance> _ingredients = new();

    public bool GetValidIngredient(out IngredientInstance chosenIngredient)
    {
        
        if(_ingredients.Count == 1) 
        {
            chosenIngredient = _ingredients[0];
            return true;
        }
        else
        {
            chosenIngredient = null;
            return false;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.parent)
        {
            if (other.transform.parent.TryGetComponent(out IngredientInstance ingredient))
            {
                if (!_ingredients.Contains(ingredient))
                    _ingredients.Add(ingredient); 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent)
        {
            if (other.transform.parent.TryGetComponent(out IngredientInstance ingredient))
            {
                _ingredients.Remove(ingredient);
            } 
        }
    }


}
