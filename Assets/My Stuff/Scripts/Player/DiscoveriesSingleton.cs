using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiscoveriesSingleton : MonoBehaviour
{
    [SerializeField] private List<IngredientItemSO> _DiscoveredIngredients = new();
    
    private static DiscoveriesSingleton _instance;

    public static DiscoveriesSingleton Instance { get { return _instance; } }
    public UnityEvent OnIngredientDiscovered = new();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public List<IngredientItemSO> GetAllDiscoveredIngredients()
    {
        return _DiscoveredIngredients;
    }

    public void AddDiscoveredIngredient(IngredientInstance newIngredient)
    {
        if (!_DiscoveredIngredients.Contains(newIngredient.Item))
        {
            _DiscoveredIngredients.Add(newIngredient.Item);
            _DiscoveredIngredients.Sort((a, b) => a.name.CompareTo(b.name));
            OnIngredientDiscovered.Invoke();
        }
    }
    public bool IsIngredientDiscovered(IngredientInstance ingredient)
    {
        return _DiscoveredIngredients.Contains(ingredient.Item);
    }


}
