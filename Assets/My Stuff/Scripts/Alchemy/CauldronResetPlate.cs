using UnityEngine;
using UnityEngine.UI;

public class CauldronResetPlate : MonoBehaviour
{
    [SerializeField] Image CircleImage;

    public bool handValid = false;

    CauldronBrain _cauldron;

    private void Awake()
    {
        _cauldron = GetComponentInParent<CauldronBrain>();
    }

    private void Update()
    {
        if (_cauldron.refreshTimer > 0)
            CircleImage.fillAmount = _cauldron.refreshTimer / _cauldron.ResetWaterTimer;
        else
            CircleImage.fillAmount = 0;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            handValid = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            handValid = false;
    }
}
