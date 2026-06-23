using System;
using UnityEngine;

[Serializable]
public class AspectTag
{
    public AspectSO Aspect;
    public int Strength;


    public AspectTag(AspectSO aspect, int strength)
    {
        Aspect = aspect;
        Strength = strength;
    }
    public AspectTag()
    {

    }



}
