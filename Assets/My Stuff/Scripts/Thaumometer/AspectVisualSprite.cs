using UnityEngine;
using UnityEngine.UI;

public class AspectVisualSprite : MonoBehaviour
{
    private Image _image;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void SetAspect(AspectTag aspect)
    {
        _image.sprite = aspect.Aspect.Icon;
        _image.color = aspect.Aspect.AssociatedColor;
    }
}
