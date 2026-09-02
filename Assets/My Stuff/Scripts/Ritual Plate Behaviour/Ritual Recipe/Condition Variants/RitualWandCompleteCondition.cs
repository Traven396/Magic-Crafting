using System;
using UnityEngine;

[Serializable]
public class RitualWandCompleteCondition : IRecipeCondition
{
    public bool Evaluate(CurrentRitualInfo ritualInfo)
    {
        ProtoWand centralWand = ritualInfo.centralIngredient.GetComponent<ProtoWand>();

        if (!centralWand)
            return false;

        if(!centralWand.ValidWand)
            return false;

        return true;
    }
}
