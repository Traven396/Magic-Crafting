namespace AgeOfEnlightenment.Spellcasting
{
    using UnityEngine;

    public class SpellProjectile : MonoBehaviour
    {
        Rigidbody _rigidbody;

        SpellcastingSession _ownerSession;
        bool _hasResolved;

        GameObject _impactEffect;

        private void Awake()
        {
            GetComponent<Collider>().enabled = false;
        }

        public void Initialize(SpellcastingSession session, Vector3 shootDirection, float speed, float lifeTime, GameObject impactVFX)
        {
            _ownerSession = session;

            if(!_rigidbody) _rigidbody = GetComponent<Rigidbody>();

            if (!_rigidbody)
            {
                Debug.LogError($"{name} doesn't have a Rigidbody");
                return;
            }

            _impactEffect = impactVFX;

            _rigidbody.linearVelocity = shootDirection.normalized * speed;
            Destroy(gameObject, lifeTime);

            GetComponent<Collider>().enabled = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            //We only want to call the collision method once for this projectile
            if (_hasResolved) return;

            _hasResolved = true;
            _ownerSession.Notify_ProjectileHit(this, collision);
            Debug.Log($"Ok, we hit something, and its name is {collision.gameObject.name}");

            if (_impactEffect)
            {
                Instantiate(_impactEffect, collision.GetContact(0).point, Quaternion.identity);
            }
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_hasResolved) return;

            _hasResolved = true;

            if(_ownerSession != null) _ownerSession.Notify_ProjectileExpire(this);
        }


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