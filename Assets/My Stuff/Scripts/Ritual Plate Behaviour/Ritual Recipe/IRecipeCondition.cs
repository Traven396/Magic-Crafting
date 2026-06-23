using UnityEngine;

public interface IRecipeCondition
{
    public bool Evaluate(CurrentRitualInfo ritualInfo);
}

public enum ConditionType
{
    ContainsUnique,
    GreaterThan,
    GreaterThanOrEqual,
    Equal,
    LessThan,
    LessThanOrEqual
}
