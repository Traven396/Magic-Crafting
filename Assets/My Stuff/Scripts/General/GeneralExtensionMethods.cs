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

    //This is a class full of methods that I can use globally.


    /// <summary>
    /// Gets the absolute value of a float.
    /// </summary>
    public static float Abs(this float num) => Mathf.Abs(num);
    /// <summary>
    /// Returns true if the vector's X component is its largest component
    /// </summary>
    public static bool MostlyX(this Vector3 vec) => vec.x.Abs() > vec.y.Abs() && vec.x.Abs() > vec.z.Abs();

    /// <summary>
    /// Returns true if the vector's Y component is its largest component
    /// </summary>
    public static bool MostlyY(this Vector3 vec) => vec.y.Abs() > vec.x.Abs() && vec.y.Abs() > vec.z.Abs();

    /// <summary>
    /// Returns true if the vector's Z component is its largest component
    /// </summary>
    public static bool MostlyZ(this Vector3 vec) => vec.z.Abs() > vec.x.Abs() && vec.z.Abs() > vec.y.Abs();

    /// <summary>
    /// Just turns the X value to the negative of what it is. I use this to flip things for left and right
    /// </summary>
    public static Vector3 InvertX(this Vector3 vec) => new(-vec.x, vec.y, vec.z);
    /// <summary>
    /// Just turns the Y value to the negative of what it is. I use this to flip things for left and right
    /// </summary>
    public static Vector3 InvertY(this Vector3 vec) => new(vec.x, -vec.y, vec.z);
}
