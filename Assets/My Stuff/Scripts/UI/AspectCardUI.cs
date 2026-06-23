using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AspectCardUI : MonoBehaviour
{
    [SerializeField] private Image AspectImage;
    [SerializeField] private TMP_Text AspectText;

    public void SetAspect(AspectTag aspect)
    {
        AspectImage.sprite = aspect.Aspect.Icon;
        AspectImage.color = aspect.Aspect.AssociatedColor;

        AspectText.text = aspect.Strength.ToString();
        AspectText.color = aspect.Aspect.AssociatedColor;
    }
}
