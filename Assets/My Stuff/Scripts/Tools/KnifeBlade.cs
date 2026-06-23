using UnityEngine;

public class KnifeBlade : MonoBehaviour
{
    private KnifeTool parentKnife;
    private Collider currentProcessable;
    private void Awake()
    {
        parentKnife = GetComponentInParent<KnifeTool>();
    }


    private void OnTriggerEnter(Collider other)
    {
        var parentStuff = other.GetComponentInParent<IngredientKnifeProcessable>();
        if (parentStuff)
        {
            if (currentProcessable == null)
            {
                currentProcessable = other;
                parentKnife.KnifeEnter(parentStuff);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other == currentProcessable) {
            parentKnife.KnifeStay();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var parentStuff = other.GetComponentInParent<IngredientKnifeProcessable>();
        if (parentStuff)
        {
            if (currentProcessable == other)
            {
                currentProcessable = null;
            }
        }
    }
}
