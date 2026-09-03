namespace AgeOfEnlightenment.Spellcasting
{
    using System;
    using UnityEngine;

    public class SpellcastingSession
    {
        SpellDefinitionSO _spellDefintion;
        SpellActivationRoute _route;
        MasterSpellcaster _originalCaster;
        Transform _castOrigin;

        SpellProjectile _projectile;

        public float ChargeTime { get; private set; }

        //This will be something much more complex once there is more spell types.
        public bool IsComplete => _projectile == null;

        public SpellcastingSession(SpellDefinitionSO spellDefinition, SpellActivationRoute route, MasterSpellcaster originalCaster, Transform castOrigin)
        {
            _spellDefintion = spellDefinition;
            _route = route;
            _originalCaster = originalCaster;
            _castOrigin = castOrigin;


            //We would also pass along the stats of when the spell was cast. But prototype mode
        }


        public void Begin()
        {
            //Normally at this point we would have large branching depending on what kind of spell was just cast, but for now every spell is a projectile.
            FireProjectile();
        }

        public void Notify_ProjectileHit(SpellProjectile projectile, Collision collision)
        {
            Debug.Log($"Yo this guy {projectile.gameObject.name} just hit {collision.gameObject.name} and I was told about it");
        }
        public void Notify_ProjectileExpire(SpellProjectile projectile)
        {

        }

        public void Tick(float deltaTime)
        {
            ChargeTime += deltaTime;

        }

        void FireProjectile()
        {
            GameObject projectileObject = GameObject.Instantiate(_route.ProjectilePrefab, _castOrigin.position, _castOrigin.rotation);

            if(!projectileObject.TryGetComponent(out SpellProjectile spellProjectile))
            {
                Debug.LogError($"Route {_route.DisplayName} needs a projectile perfab with SpellProjectile on it");
                GameObject.Destroy(projectileObject);
                return;
            }

            _projectile = spellProjectile;

            Vector3 shootDirection = Vector3.zero;

            switch (_route.ProjectileShootDirection)
            {
                case SpellShootDirection.Left:
                    shootDirection = -_castOrigin.right;
                    break;
                case SpellShootDirection.Right:
                    shootDirection = _castOrigin.right;
                    break;
                case SpellShootDirection.Up:
                    shootDirection = _castOrigin.up;
                    break;
                case SpellShootDirection.Down:
                    shootDirection = -_castOrigin.up;
                    break;
                case SpellShootDirection.Forward:
                    shootDirection = _castOrigin.forward;
                    break;
                case SpellShootDirection.Back:
                    shootDirection = -_castOrigin.forward;
                    break;
            }

            _projectile.Initialize(this, shootDirection, _route.ProjectileSpeed, _route.ProjectileLifetime, _route.ImpactPrefab);
        }

    }
}