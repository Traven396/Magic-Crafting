using System;
using UnityEngine;

public class LiquidInput : MonoBehaviour
{
    ILiquidReceiver Receiver;

    private void Awake()
    {
        Receiver = GetComponentInParent<ILiquidReceiver>();
    }

    //I want this script to be usable on many different objects for pouring liquids
    //This would mainly be used later on for alchemy stuff I guess but I want the option for many liquids

    //This is placed onto a collider that different LiquidStream objects might be contacting. When it is contacted the original script will call on this a
    // TryAddLiquid method or something. This would then call on a connected script that implements an interface? It would pass along the liquid its trying to add into the parent script to add it


    public void TryAddLiquid(IngredientItemSO ingredient, int addedAmount)
    {
        if (Receiver == null)
            return;

        Receiver.TryAddLiquid(ingredient, addedAmount);
    }
}

public interface ILiquidReceiver
{
    void TryAddLiquid(IngredientItemSO ingredient, int addedAmount);
}
