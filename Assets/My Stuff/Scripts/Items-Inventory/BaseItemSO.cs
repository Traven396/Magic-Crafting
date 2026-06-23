using System;
using UnityEngine;

[System.Serializable]
public class BaseItemSO : ScriptableObject
{
    [SerializeField] private string Name;
    protected virtual ItemType _Type { get; }

    public ItemType Type { get { return _Type; } }

    public virtual string GetName()
    {
        return Name;
    }

    
}
public enum ItemType
{
    Ingredient,
    Result
}