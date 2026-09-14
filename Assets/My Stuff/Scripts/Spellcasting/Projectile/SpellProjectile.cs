namespace AgeOfEnlightenment.Spellcasting
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class SpellProjectile : MonoBehaviour
    {
        [SerializeField] Transform TrailParent;
        SpellcastingSession _ownerSession;
        bool _hasResolved;


        List<SpellProjectileBehaviourModifier> _modifiers = new();



        Rigidbody _rigidbody;
        Entity _currentTarget;
        float _speed;


        public Entity CurrentTarget => _currentTarget;
        public float Speed => _speed;
        public Rigidbody Rigidbody => _rigidbody;


        float longestTrail;

        private void Awake()
        {
            GetComponent<Collider>().enabled = false;

            if (TrailParent)
            {
                var childTrails = TrailParent.GetComponentsInChildren<TrailRenderer>();

                longestTrail = childTrails.Max(tr => tr.time);
            }
        }

        public void ActivateAndLaunch(SpellcastingSession session, Vector3 shootDirection, float speed, float lifeTime, List<SpellProjectileBehaviourModifier> modifiers, Entity target = null)
        {
            _ownerSession = session;

            _speed = speed;

            transform.parent = null;

            _modifiers = new(modifiers);

            if(!_rigidbody) _rigidbody = GetComponent<Rigidbody>();

            if (!_rigidbody)
            {
                Debug.LogError($"{name} doesn't have a Rigidbody");
                return;
            }

            _rigidbody.isKinematic = false;

            _rigidbody.linearVelocity = shootDirection.normalized * speed;

            Invoke(nameof(DestroyProjectile), lifeTime);

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

        void DestroyProjectile()
        {
            TrailParent.parent = null;

            Destroy(TrailParent.gameObject, longestTrail);

            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            //We only want to call the collision method once for this projectile
            if (_hasResolved) return;

            _hasResolved = true;
            _ownerSession.Notify_ProjectileHit(this, collision);

            DestroyProjectile();
        }

        private void OnDestroy()
        {
            if (_hasResolved) return;

            _hasResolved = true;

            if(_ownerSession != null) _ownerSession.Notify_ProjectileExpire(this);
        }
    }

}