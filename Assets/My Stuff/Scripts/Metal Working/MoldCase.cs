using Alchemy.Inspector;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using AgeOfEnlightenment.Stats;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
public class MoldCase : MonoBehaviour, ILiquidReceiver
{
    [Header("Recipe Settings")]
    [SerializeField] List<ItemStatModifier> statModifierList;
    [SerializeField] GameObject MoldOutputPrefab;
    //We would have some variable here for a SO of a MoldOutput thing.
    //We would pass along all of the metals to it and it would do the complicated math for determining how those affect the stats
    //It would have a list of "Stat Modifiers" or something, and each of these would have a metal and what stats that affected
    //Depending on how much of different metals you add it would calculate the average for what the final stat would be.
    [Header("Mold Settings")]
    [SerializeField] int MaxLiquidFill = 125;
    [SerializeField] float AnimationTime = 1;
    [SerializeField] int CooldownTime = 300;
    [SerializeField] TemperatureController temperatureController;

    [Space(10)]
    [Header("References")]
    [SerializeField] Transform LeftHinge;
    [SerializeField] Transform RightHinge;
    [Space(10)]
    [SerializeField] Image FillGauge;
    [SerializeField] Image FullIndicator;
    [SerializeField] ParticleSystem FinishedParticles;

    Tween leftTween;
    Tween rightTween;

    bool isOpen = false;

    bool isLocked = false;

    bool isAcceptingInput = true;
    [Header("Internal State")]
    [SerializeField] List<ContainedLiquidMetal> internalMetalList = new();

    public float CurrentTemperature => temperatureController.Temperature;
    int CurrentFillAmount => internalMetalList.Sum(liquid => liquid.amount);

    Gradient fillGradient;
    private void Start()
    {
        if (temperatureController == null)
            temperatureController = GetComponent<TemperatureController>();

        temperatureController.SetPassiveCoolRate(1f);
        temperatureController.TemperatureChanged += OnTemperatureChanged;
        temperatureController.CooledToAmbient += OnCooledToAmbient;

        fillGradient = new Gradient();

        var colors = new GradientColorKey[3];
        colors[0] = new GradientColorKey(Color.red, 0.0f);
        colors[1] = new GradientColorKey(Color.yellow, 0.5f);
        colors[2] = new GradientColorKey(Color.green, 1.0f);


        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

        fillGradient.SetKeys(colors, alphas);

        FullIndicator.enabled = false;
    }

    void OnDestroy()
    {
        if (temperatureController == null)
            return;

        temperatureController.TemperatureChanged -= OnTemperatureChanged;
        temperatureController.CooledToAmbient -= OnCooledToAmbient;
    }

    void OnTemperatureChanged(float _)
    {
        UpdateVisuals();
    }
    void SuccesfulCraft()
    {
        FinishedParticles.Play();

        var spawnedWand = Instantiate(MoldOutputPrefab, transform);

        spawnedWand.GetComponent<WandFrame>().InitializeFrameStats(statModifierList, internalMetalList);

        spawnedWand.GetComponent<XRGrabInteractable>().selectEntered.AddListener(ResultRemoved);
    }
    void FailureCraft()
    {

    }

    void ResultRemoved(SelectEnterEventArgs args)
    {
        args.interactableObject.selectEntered.RemoveListener(ResultRemoved);

        isAcceptingInput = true;
    }

    void OnCooledToAmbient()
    {
        if (CurrentFillAmount >= MaxLiquidFill)
        {
            //This is a successful craft. We spawn the object then need to pass along the information of the metals stats to it
            ResetVisuals();

            SuccesfulCraft();
        }
        else
        {
            //This will be a recipe fail. The mold was not filled enough. Inside will just be metal scraps
            FailureCraft();
        }

        internalMetalList.Clear();
        isLocked = false;
    }

    public void TryAddLiquid(IngredientItemSO ingredient, int addedAmount)
    {
        //If we are open then adding liquid does nothing, it would fall out
        if (isOpen)
            return;

        if (!isAcceptingInput)
            return;
        
        //We set isLocked to true no matter what since even if we dont add liquid we are still trying to.
        //and even if we only add a small bit to it thats enough to warrant locking it
        isLocked = true;

        

        int cappedAddAmount = Mathf.Clamp(addedAmount, 0, MaxLiquidFill - CurrentFillAmount);

        if (cappedAddAmount == 0)
        {
            isAcceptingInput = false;
            return;
        }

        temperatureController.SetTemperature(CooldownTime);

        var sameIngredient = internalMetalList.Where(id => id.ingredient == ingredient).ToArray();
        
        if (sameIngredient.Any()) 
        {
            int index = internalMetalList.FindIndex(id => id.ingredient == ingredient);
            var updated = internalMetalList[index];

            updated.amount += cappedAddAmount;

            internalMetalList[index] = updated;
        }
        else
        {
            ContainedLiquidMetal thingToAdd = new ContainedLiquidMetal();
            thingToAdd.amount = cappedAddAmount;
            thingToAdd.ingredient = ingredient;

            internalMetalList.Add(thingToAdd);
        }

        UpdateVisuals();
    }
    
    void ResetVisuals()
    {
        FillGauge.fillAmount = 0;
        FillGauge.color = fillGradient.Evaluate(0);
        FullIndicator.enabled = false;
    }
    void UpdateVisuals()
    {
        float fillPercentage = (float)CurrentFillAmount / MaxLiquidFill;
        FillGauge.fillAmount = fillPercentage;

        FillGauge.color = fillGradient.Evaluate(fillPercentage);

        if(fillPercentage == 1 && !FullIndicator.isActiveAndEnabled)
        {
            FullIndicator.color = fillGradient.Evaluate(0);
            FullIndicator.enabled = true;
        } 
        else if(fillPercentage < 1 && FullIndicator.isActiveAndEnabled)
        {
            FullIndicator.enabled = false;
        }

        if (FullIndicator.isActiveAndEnabled)
        {
            var tempValue = 1 - temperatureController.Temperature / CooldownTime;
            FullIndicator.color = fillGradient.Evaluate(tempValue);
        }
    }
    
    [Button]
    public void CaseToggle()
    {
        if (!isLocked)
        {
            if (isOpen)
            {
                CloseCase();
            }
            else
            {
                OpenCase();
            } 
        }
    }

    void OpenCase()
    {
        isOpen = true;

        var tweenSpeed = AnimationTime * (1 - (LeftHinge.localRotation.eulerAngles.y / 60));

        if(leftTween != null && rightTween != null)
        {
            leftTween.Kill(false);
            rightTween.Kill(false);
        }

        leftTween = LeftHinge.DOLocalRotate(Vector3.up * 60, tweenSpeed);
        rightTween = RightHinge.DOLocalRotate(Vector3.up * 60, tweenSpeed);
    }
    void CloseCase()
    {
        isOpen = false;

        if (leftTween != null && rightTween != null)
        {
            leftTween.Kill(false);
            rightTween.Kill(false);
        }

        var tweenSpeed = AnimationTime * (LeftHinge.localRotation.eulerAngles.y / 60);

        leftTween = LeftHinge.DOLocalRotate(Vector3.zero, tweenSpeed);
        rightTween = RightHinge.DOLocalRotate(Vector3.zero, tweenSpeed);
    }

}
