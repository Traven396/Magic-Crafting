using UnityEngine;

public class IngredientCuttable : IngredientKnifeProcessable
{

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<KnifeTool>(out var knife))
        {
            
        }
    }
    

    public override void Process(float progress)
    {
        
    }
}
