using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CauldronItemFloating : MonoBehaviour, ITriggerable
{
    public float FloatStrength = 1;
    public Collider WaterCollider;
    List<Rigidbody> floatingObjects = new();

    
    private void FixedUpdate()
    {
        float topOfCollider = WaterCollider.bounds.max.y;

        foreach (var obj in floatingObjects)
        {
            if (obj)
            {
                float forceFactor = 1f - (obj.position.y - topOfCollider) * FloatStrength;

                if (forceFactor > 0f)
                {
                    Vector3 uplift = -Physics.gravity * (forceFactor - obj.linearVelocity.y) * 0.95f;
                    obj.AddForce(uplift);
                }


                //obj.AddForce((topOfCollider - obj.position.y) * FloatStrength * Vector3.up, ForceMode.Force);
            }
        }


    }

    public void IngredientRemoved(IngredientInstance instance)
    {
        Rigidbody ingRB = instance.GetComponent<Rigidbody>();

        if (ingRB)
        {
            if (floatingObjects.Contains(ingRB))
            {
                floatingObjects.Remove(ingRB);
            }
        }
    }

    public void OnTriggerEnterCall(Collider other)
    {

        if (other.attachedRigidbody)
        {
            floatingObjects.Add(other.attachedRigidbody);

        }
    }

    public void OnTriggerExitCall(Collider other)
    {
        if (floatingObjects.Contains(other.attachedRigidbody))
        {
            floatingObjects.Remove(other.attachedRigidbody);
        }
    }

    public void OnTriggerStayCall(Collider other)
    {
        
    }
}
