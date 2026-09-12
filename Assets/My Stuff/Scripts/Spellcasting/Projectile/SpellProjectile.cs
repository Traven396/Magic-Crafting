namespace AgeOfEnlightenment.Spellcasting
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using UnityEngine;

    public class SpellProjectile : MonoBehaviour
    {
        SpellcastingSession _ownerSession;
        bool _hasResolved;


        List<SpellProjectileBehaviourModifier> _modifiers = new();



        Rigidbody _rigidbody;
        Entity _currentTarget;
        float _speed;


        public Entity CurrentTarget => _currentTarget;
        public float Speed => _speed;
        public Rigidbody Rigidbody => _rigidbody;

        private void Awake()
        {
            GetComponent<Collider>().enabled = false;
        }

        public void ActivateAndLaunch(SpellcastingSession session, Vector3 shootDirection, float speed, float lifeTime, List<SpellProjectileBehaviourModifier> modifiers, Entity target = null)
        {
            _ownerSession = session;

            _speed = speed;

            transform.parent = null;

            _modifiers = modifiers;

            if(!_rigidbody) _rigidbody = GetComponent<Rigidbody>();

            if (!_rigidbody)
            {
                Debug.LogError($"{name} doesn't have a Rigidbody");
                return;
            }

            _rigidbody.isKinematic = false;

            _rigidbody.linearVelocity = shootDirection.normalized * speed;

            Destroy(gameObject, lifeTime);

            GetComponent<Collider>().enabled = true;

            if (target != null)
                _currentTarget = target;



            _modifiers.ForEach(mod => mod.Launch(this));
        }

        private void Update()
        {
            _modifiers.ForEach(mod => mod.Tick(this, Time.deltaTime));
        }
        private void LateUpdate()
        {
            _modifiers.ForEach(mod => mod.LateTick(this, Time.deltaTime));
        }


        private void OnCollisionEnter(Collision collision)
        {
            //We only want to call the collision method once for this projectile
            if (_hasResolved) return;

            _hasResolved = true;
            _ownerSession.Notify_ProjectileHit(this, collision);

            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_hasResolved) return;

            _hasResolved = true;

            if(_ownerSession != null) _ownerSession.Notify_ProjectileExpire(this);
        }
    }

}