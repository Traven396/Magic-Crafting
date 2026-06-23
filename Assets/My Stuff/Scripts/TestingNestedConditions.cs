using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Inspector;

public class TestingNestedConditions : MonoBehaviour
{
    [SerializeReference]
    public List<ILogic> subTesterClasses = new List<ILogic>();
}


public interface ILogic
{
    public bool Evaluate(SubTesterClass testableClass);
}

[System.Serializable]
public class LogicIDTest : ILogic
{
    [SerializeField]
    private int requiredID;
    public bool Evaluate(SubTesterClass testableClass)
    {
        return testableClass.requireID == requiredID;
    }
}
[System.Serializable]
public class LogicTagTest : ILogic
{
    [SerializeField]
    private string requiredTag;
    public bool Evaluate(SubTesterClass testableClass)
    {
        return testableClass.pieceRequire.CompareTag(requiredTag);
    }
}






[System.Serializable]
public class SubTesterClass
{
    public enum RequirementType { Type1, Type2, Type3 }

    public RequirementType Type;
    public int requireID;
    public RitualPlatePiece pieceRequire;
}


public class  SubSubTesterClass : SubTesterClass
{
    public int requireID;
}