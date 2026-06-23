using Alchemy.Inspector;
using System.Collections;
using UnityEngine;

public class CauldronEffects : MonoBehaviour
{
    [Title("Universal Step Settings")]
    [SerializeField] CauldronStepEffects CleanEffects;
    [SerializeField] CauldronStepEffects DirtyEffects;
    [SerializeField] CauldronStepEffects FoamingWaterEffects;
    [SerializeField] int FoamTransitionFrameCount;
    [Title("Sound Effects")]
    [SerializeField] AudioClip StepProgressSound;
    [SerializeField] float StepProgressSoundVolume;
    [SerializeField] AudioClip RecipeFailSound;
    [SerializeField] float RecipeFailSoundVolume;
    [Title("Particle Systems")]
    [SerializeField] ParticleSystem RisingBubblePS;
    [SerializeField] ParticleSystem SurfaceBubblePS;

    private SkinnedMeshRenderer _renderer;
    private Material _liquidMaterial;
    private ParticleSystem.MainModule _bubblePSMain;
    private ParticleSystem.EmissionModule _bubblePSEmission;

    int shallowWaterColorID;
    int deepWaterColorID;
    int depthID;
    int depthStrengthID;


    int noiseColorID;
    int noiseDensityID;
    int noiseSpeedID;
    int noiseDistanceID;

    int noiseEdge1ID;
    int noiseEdge2ID;

    int stirAmountID;

    private CauldronStepEffects _lastAppliedEffects;

    private void Awake()
    {
        _renderer = GetComponent<SkinnedMeshRenderer>();
        _liquidMaterial = _renderer.materials[1];

        _bubblePSMain = RisingBubblePS.main;

        _bubblePSEmission = RisingBubblePS.emission;

        shallowWaterColorID = Shader.PropertyToID("_ShallowWaterColor");
        deepWaterColorID = Shader.PropertyToID("_DeepWaterColor");
        depthID = Shader.PropertyToID("_Depth");
        depthStrengthID = Shader.PropertyToID("_DepthStrength");


        noiseColorID = Shader.PropertyToID("_NoiseColor");
        noiseDensityID = Shader.PropertyToID("_NoiseDensity");
        noiseSpeedID = Shader.PropertyToID("_NoiseSpeed");
        noiseDistanceID = Shader.PropertyToID("_NoiseDistance");

        noiseEdge1ID = Shader.PropertyToID("_Edge1");
        noiseEdge2ID = Shader.PropertyToID("_Edge2");


        stirAmountID = Shader.PropertyToID("_RotationAmount");

        _lastAppliedEffects = CleanEffects;
    }
    //Method to play a sound when an ingredient is consumed
    //also playing a little particle effect there

    //Method to completely restore the water to its original state
    public void RefreshWater()
    {
        //At some point make another private CauldronStepEffects varaible and have it save all the properties of the material at awake, that way we dont need
        //make a copy of the material and then manually set it all back
        _lastAppliedEffects = CleanEffects;

        CauldronStepEffect_Update(CleanEffects);

        _renderer.SetBlendShapeWeight(0, 0);
        
        RisingBubblePS.Play();

        _bubblePSMain.playOnAwake = true;
    }


    public void StirLiquid(float newStirArmount)
    {
        _liquidMaterial.SetFloat(stirAmountID, newStirArmount);
    }

    //Method for when the recipe fails and we muddy the water
    [Button]
    public void RecipeFail()
    {
        CauldronStepEffect_Update(DirtyEffects);

        SoundManagerSO.PlayClipAtPoint(RecipeFailSound, transform.position, RecipeFailSoundVolume);
    }

    public void IngredientConvert(IngredientInstance converted, float amount)
    {
        _renderer.SetBlendShapeWeight(0, amount);

        //Move the bubbles down
        if (amount == 100)
        {
            RisingBubblePS.Stop();
            RisingBubblePS.Clear();

            _bubblePSMain.playOnAwake = false;
            Debug.Log("Stop PS");
        }


        //Maybe play a particle effect at where the ingredient consumed
        //Or a sound effect instance
    }

    //Method for whenever a step is completed to play a sound effect
    [Button]
    public void CauldronStepCompleted(CauldronStepEffects stepEffects)
    {
        CauldronStepEffect_Update(stepEffects);

        SoundManagerSO.PlayClipAtPoint(StepProgressSound, transform.position + Vector3.up, StepProgressSoundVolume);
    }


    private void CauldronStepEffect_Update(CauldronStepEffects stepEffects)
    {
        if (stepEffects.ShallowWaterColor == Color.clear)
            return;

        _lastAppliedEffects = stepEffects;

        //Depth Settings
        _liquidMaterial.SetColor(shallowWaterColorID, stepEffects.ShallowWaterColor);
        _liquidMaterial.SetColor(deepWaterColorID, stepEffects.DeepWaterColor);
        _liquidMaterial.SetFloat(depthID, stepEffects.Depth);
        _liquidMaterial.SetFloat(depthStrengthID, stepEffects.DepthStrength);

        //Noise setttings
        _liquidMaterial.SetColor(noiseColorID, stepEffects.NoiseColor);
        _liquidMaterial.SetFloat(noiseDensityID, stepEffects.NoiseDensity);
        _liquidMaterial.SetFloat(noiseSpeedID, stepEffects.NoiseSpeed);
        _liquidMaterial.SetFloat(noiseDistanceID, stepEffects.NoiseDistance);


        Material[] materials = _renderer.materials;
        materials[1] = _liquidMaterial;

        _renderer.materials = materials;

        //Particle System Settings
        _bubblePSMain.startColor = stepEffects.BubbleColor;
        //Need to add stuff for the other bubble settings
    }

    //Will start a somewhat quick transition to the foamy water version from wherever we were before
    [Button]
    public void CauldronFoam()
    {
        StopCoroutine(nameof(StopFoam));
        StartCoroutine(nameof(StartFoam));
    }
    IEnumerator StartFoam()
    {
        //Slowly transition the material settings to the foamy version
        //Once we get within a certain threshold of completed then we turn on the surface bubbles and off the rising
        float noiseSpeedStep = Mathf.Abs(_lastAppliedEffects.NoiseSpeed - FoamingWaterEffects.NoiseSpeed) / (float)FoamTransitionFrameCount;
        float noiseDensityStep = Mathf.Abs(_lastAppliedEffects.NoiseDensity - FoamingWaterEffects.NoiseDensity) / (float)FoamTransitionFrameCount;
        float noiseDistanceStep = Mathf.Abs(_lastAppliedEffects.NoiseDistance - FoamingWaterEffects.NoiseDistance) / (float)FoamTransitionFrameCount;

        float noiseEdge1Step = Mathf.Abs(_lastAppliedEffects.Edge1 - FoamingWaterEffects.Edge1) / (float)FoamTransitionFrameCount;
        float noiseEdge2Step = Mathf.Abs(_lastAppliedEffects.Edge2 - FoamingWaterEffects.Edge2) / (float)FoamTransitionFrameCount;

        while (true)
        {
            var movedNoiseSpeed = Mathf.MoveTowards(_liquidMaterial.GetFloat(noiseSpeedID), FoamingWaterEffects.NoiseSpeed, noiseSpeedStep);
            _liquidMaterial.SetFloat(noiseSpeedID, movedNoiseSpeed);

            var movedNoiseDensity = Mathf.MoveTowards(_liquidMaterial.GetFloat(noiseDensityID), FoamingWaterEffects.NoiseDensity, noiseDensityStep);
            _liquidMaterial.SetFloat(noiseDensityID, movedNoiseDensity);

            var movedNoiseDistance = Mathf.MoveTowards(_liquidMaterial.GetFloat(noiseDistanceID), FoamingWaterEffects.NoiseDistance, noiseDistanceStep);
            _liquidMaterial.SetFloat(noiseDistanceID, movedNoiseDistance);

            var movedNoiseEdge1 = Mathf.MoveTowards(_liquidMaterial.GetFloat(noiseEdge1ID), FoamingWaterEffects.Edge1, noiseEdge1Step);
            _liquidMaterial.SetFloat(noiseEdge1ID, movedNoiseEdge1);

            var movedNoiseEdge2 = Mathf.MoveTowards(_liquidMaterial.GetFloat(noiseEdge2ID), FoamingWaterEffects.Edge2, noiseEdge2Step);
            _liquidMaterial.SetFloat(noiseEdge2ID, movedNoiseEdge2);


            if (_liquidMaterial.GetFloat(noiseEdge1ID) != FoamingWaterEffects.Edge1)
                yield return null;
            else
            {
                SurfaceBubblePS.gameObject.SetActive(true);
                RisingBubblePS.gameObject.SetActive(false);
                break; 
            }
        }

        
    }
    //If the player removes the item early or the conversion completes then we remove the foam and go back to what it was before
    [Button]
    public void CauldronUnfoam()
    {
        StopCoroutine(nameof(StartFoam));
        StartCoroutine(nameof(StopFoam));
    }
    IEnumerator StopFoam()
    {
        //Begin transitioning back to the last applied settings
        //Quickly turn off the surface bubbles and back on the rising ones
        SurfaceBubblePS.gameObject.SetActive(false);
        RisingBubblePS.gameObject.SetActive(true);

        float noiseSpeedStep = Mathf.Abs(_lastAppliedEffects.NoiseSpeed - FoamingWaterEffects.NoiseSpeed) / (float)FoamTransitionFrameCount;
        float noiseDensityStep = Mathf.Abs(_lastAppliedEffects.NoiseDensity - FoamingWaterEffects.NoiseDensity) / (float)FoamTransitionFrameCount;
        float noiseDistanceStep = Mathf.Abs(_lastAppliedEffects.NoiseDistance - FoamingWaterEffects.NoiseDistance) / (float)FoamTransitionFrameCount;

        float noiseEdge1Step = Mathf.Abs(_lastAppliedEffects.Edge1 - FoamingWaterEffects.Edge1) / (float)FoamTransitionFrameCount;
        float noiseEdge2Step = Mathf.Abs(_lastAppliedEffects.Edge2 - FoamingWaterEffects.Edge2) / (float)FoamTransitionFrameCount;

        while (true)
        {
            var movedNoiseSpeed = Mathf.MoveTowards(_liquidMaterial.GetFloat(noiseSpeedID), _lastAppliedEffects.NoiseSpeed, noiseSpeedStep);
            _liquidMaterial.SetFloat(noiseSpeedID, movedNoiseSpeed);

            var movedNoiseDensity = Mathf.MoveTowards(_liquidMaterial.GetFloat(noiseDensityID), _lastAppliedEffects.NoiseDensity, noiseDensityStep);
            _liquidMaterial.SetFloat(noiseDensityID, movedNoiseDensity);

            var movedNoiseDistance = Mathf.MoveTowards(_liquidMaterial.GetFloat(noiseDistanceID), _lastAppliedEffects.NoiseDistance, noiseDistanceStep);
            _liquidMaterial.SetFloat(noiseDistanceID, movedNoiseDistance);

            var movedNoiseEdge1 = Mathf.MoveTowards(_liquidMaterial.GetFloat(noiseEdge1ID), _lastAppliedEffects.Edge1, noiseEdge1Step);
            _liquidMaterial.SetFloat(noiseEdge1ID, movedNoiseEdge1);

            var movedNoiseEdge2 = Mathf.MoveTowards(_liquidMaterial.GetFloat(noiseEdge2ID), _lastAppliedEffects.Edge2, noiseEdge2Step);
            _liquidMaterial.SetFloat(noiseEdge2ID, movedNoiseEdge2);


            if (_liquidMaterial.GetFloat(noiseEdge1ID) != _lastAppliedEffects.Edge1)
                yield return null;
            else
                break;
        }

    }
}
