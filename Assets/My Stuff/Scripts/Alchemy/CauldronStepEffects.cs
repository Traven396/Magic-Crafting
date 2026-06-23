using Alchemy.Inspector;
using System;
using UnityEngine;

[Serializable]
public class CauldronStepEffects
{
    [Title("Depth Settings (Leave shallow blank for ignored step)")]
    public Color ShallowWaterColor;
    public Color DeepWaterColor;
    public float Depth = -0.2f;
    [Range(0, 2)]
    public float DepthStrength = 1.5f;

    [Title("Noise Settings")]
    public Color NoiseColor;
    public float NoiseDensity = 5.5f;
    public float NoiseSpeed = 0.03f;
    public float NoiseDistance = 0.8f;
    public float Edge1 = -0.3f;
    public float Edge2 = 0.6f;

    [Title("Particle System Settings")]
    public Color BubbleColor;
    //Size
    //Emission amount
    //Different particle for final result. Like sparkles/lightning/fire etc
    //Smoke color?
    //Smoke amount
    //Smoke size


    //Sounds? Like adding a new ambient sound, or changing the step progress sound
}
