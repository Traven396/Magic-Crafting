namespace AgeOfEnlightenment.Spellcasting
{
    using UnityEngine;

    public class SpellProjectile : MonoBehaviour
    {


        public void Launch(Vector3 direction, float speed, float lifetime)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(direction.normalized * speed, ForceMode.VelocityChange);
                Destroy(gameObject, lifetime);
            }
        }
    }

}