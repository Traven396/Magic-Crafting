using System;
using UnityEngine;

public class TemperatureController : MonoBehaviour
{
    [Header("Temperature")]
    [SerializeField] float temperature;
    [SerializeField] float ambientTemperature;
    [SerializeField] float maxTemperature = 9999f;

    [Header("Passive Cooling")]
    [SerializeField] float passiveCoolRate = 1f;
    [SerializeField] bool pausePassiveCooling;

    public float Temperature => temperature;
    public int TemperatureInt => Mathf.RoundToInt(temperature);
    public float AmbientTemperature => ambientTemperature;
    public float MaxTemperature => maxTemperature;
    public bool PausePassiveCooling
    {
        get => pausePassiveCooling;
        set => pausePassiveCooling = value;
    }

    public bool IsAtAmbient => Mathf.Approximately(temperature, ambientTemperature);

    public event Action<float> TemperatureChanged;
    public event Action CooledToAmbient;

    void Update()
    {
        if (pausePassiveCooling || IsAtAmbient)
            return;

        float previousTemperature = temperature;
        temperature = Mathf.MoveTowards(temperature, ambientTemperature, passiveCoolRate * Time.deltaTime);

        if (Mathf.Approximately(previousTemperature, temperature))
            return;

        TemperatureChanged?.Invoke(temperature);

        if (IsAtAmbient)
            CooledToAmbient?.Invoke();
    }

    public void RaiseTemperature(float amount)
    {
        if (amount <= 0f)
            return;

        SetTemperature(temperature + amount);
    }

    public void LowerTemperature(float amount)
    {
        if (amount <= 0f)
            return;

        SetTemperature(temperature - amount);
    }

    public void SetTemperature(float value)
    {
        float clampedValue = Mathf.Clamp(value, ambientTemperature, maxTemperature);

        if (Mathf.Approximately(temperature, clampedValue))
            return;

        temperature = clampedValue;
        TemperatureChanged?.Invoke(temperature);

        if (IsAtAmbient)
            CooledToAmbient?.Invoke();
    }

    public void ResetToAmbient()
    {
        SetTemperature(ambientTemperature);
    }

    public void ResumePassiveCooling()
    {
        pausePassiveCooling = false;
    }

    public void SetPassiveCoolRate(float rate)
    {
        passiveCoolRate = rate;
    }
}
