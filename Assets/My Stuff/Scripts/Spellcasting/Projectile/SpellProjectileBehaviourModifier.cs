namespace AgeOfEnlightenment.Spellcasting
{
    using System;
    using UnityEngine;
    using UnityEngine.UIElements;
    using static UnityEngine.GraphicsBuffer;

    [Serializable]
    public abstract class SpellProjectileBehaviourModifier
    {
        public abstract void Launch(SpellProjectile projectile);
        public abstract void Tick(SpellProjectile projectile, float deltaTime);

        public abstract void LateTick(SpellProjectile projectile, float deltaTime);
    }

    [Serializable]
    public class HomingProjectileModifier : SpellProjectileBehaviourModifier
    {
        [SerializeField] float HomingStrength;

        public override void Launch(SpellProjectile projectile)
        {

        }

        public override void Tick(SpellProjectile projectile, float deltaTime)
        {
            if (projectile.CurrentTarget)
            {
                Vector3 direction = (projectile.CurrentTarget.transform.TransformPoint(projectile.CurrentTarget.Rigidbody.centerOfMass) - projectile.Rigidbody.position).normalized;

                Quaternion targetRotation = Quaternion.LookRotation(direction);

                Quaternion nextRotation = Quaternion.RotateTowards(projectile.Rigidbody.rotation, targetRotation, HomingStrength * deltaTime);

                projectile.Rigidbody.MoveRotation(nextRotation);
                projectile.Rigidbody.linearVelocity = projectile.transform.forward * projectile.Speed; 
            }
        }
        public override void LateTick(SpellProjectile projectile, float deltaTime)
        {

        }
    }
    [Serializable]
    public class LookTowardsLaunchModifier : SpellProjectileBehaviourModifier
    {
        public override void Launch(SpellProjectile projectile)
        {
            projectile.transform.forward = projectile.Rigidbody.linearVelocity.normalized;
        }

        public override void Tick(SpellProjectile projectile, float deltaTime)
        {
        }
        public override void LateTick(SpellProjectile projectile, float deltaTime)
        {
        }
    }
    [Serializable]
    public class DebugModifier : SpellProjectileBehaviourModifier
    {
        public override void LateTick(SpellProjectile projectile, float deltaTime)
        {
            Debug.Log("We lately tick");
        }

        public override void Launch(SpellProjectile projectile)
        {
            Debug.Log("We just launched. Woo");
        }

        public override void Tick(SpellProjectile projectile, float deltaTime)
        {
            Debug.Log("We are ticking");
        }
    }
}