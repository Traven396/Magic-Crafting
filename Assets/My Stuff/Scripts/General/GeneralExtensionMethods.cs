using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public static class GeneralExtensionMethods
{
    public static void IgnoreCollision(this Rigidbody rigidbody1, Rigidbody rigidbody2, bool ignoreCollisions = true)
    {
        var rb1Colliders = rigidbody1.GetAttachedColliders();
        var rb2Colliders = rigidbody2.GetAttachedColliders();

        for (int i = 0; i < rb1Colliders.Length; i++) 
        { 
            for(int j = 0; j < rb2Colliders.Length; j++)
                Physics.IgnoreCollision(rb1Colliders[i], rb2Colliders[j], ignoreCollisions);
        }
    }
    /// <summary>
    /// Returns every collider attached to the rigidbody as well as on any children of it.
    /// </summary>
    /// <param name="rb"></param>
    /// <returns>An Array of all colliders</returns>
    public static Collider[] GetAttachedColliders(this Rigidbody rb)
    {
        Collider[] colliders = rb.GetComponentsInChildren<Collider>();

        colliders.Concat(rb.GetComponents<Collider>());

        return colliders;
    }

    public static void TurnToTrigger(this XRBaseInteractable interactable)
    {
        foreach (Collider col in interactable.colliders)
        {
            col.isTrigger = true;
        }
    }
    public static void ResetFromTrigger(this XRBaseInteractable interactable)
    {
        foreach (Collider col in interactable.colliders)
        {
            col.isTrigger = false;
        }
    }

    public static void TurnToTrigger(this IXRSelectInteractable interactable)
    {
        foreach (Collider col in interactable.colliders)
        {
            col.isTrigger = true;
        }
    }
    public static void ResetFromTrigger(this IXRSelectInteractable interactable)
    {
        foreach (Collider col in interactable.colliders)
        {
            col.isTrigger = false;
        }
    }
}
