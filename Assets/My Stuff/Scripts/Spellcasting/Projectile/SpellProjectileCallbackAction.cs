namespace AgeOfEnlightenment.Spellcasting
{
    using System;
    using UnityEngine;

	[Serializable]
	public abstract class SpellProjectileCallbackAction
	{
        public enum ProjectileCallbackType { Impact, Expire, Both }

        [SerializeField] public ProjectileCallbackType _callbackType;

        public abstract void ProjectileCollide(SpellProjectile projectile, Collision collision);

        public abstract void ProjectileExpire(SpellProjectile projectile);
	}

    [Serializable]
    public class SpellProjectile_SpawnObject : SpellProjectileCallbackAction
    {
        [SerializeField] GameObject Prefab;
        public override void ProjectileCollide(SpellProjectile projectile, Collision collision)
        {
            GameObject.Instantiate(Prefab, collision.GetContact(0).point, Quaternion.identity);
        }

        public override void ProjectileExpire(SpellProjectile projectile)
        {
            GameObject.Instantiate(Prefab, projectile.transform.position, Quaternion.identity);
        }
    }

    [Serializable]
    public class SpellProjectile_PlayAudio : SpellProjectileCallbackAction
    {
        [SerializeField] AudioClip Clip;
        [SerializeField] float Volume;
        [SerializeField] float PitchRange;
        [SerializeField] float VolumeRange;
        public override void ProjectileCollide(SpellProjectile projectile, Collision collision)
        {
            SoundManagerSO.PlayClipAtPoint(Clip, collision.GetContact(0).point, Volume, PitchRange, VolumeRange);
        }

        public override void ProjectileExpire(SpellProjectile projectile)
        {
            SoundManagerSO.PlayClipAtPoint(Clip, projectile.transform.position, Volume, PitchRange, VolumeRange);
        }
    }
    [Serializable]
    public class SpellProjectile_AddForce : SpellProjectileCallbackAction
    {
        public override void ProjectileCollide(SpellProjectile projectile, Collision collision)
        {
            throw new NotImplementedException();
        }

        public override void ProjectileExpire(SpellProjectile projectile)
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public class SpellProjectile_DealDamage : SpellProjectileCallbackAction
    {
        public override void ProjectileCollide(SpellProjectile projectile, Collision collision)
        {
            throw new NotImplementedException();
        }

        public override void ProjectileExpire(SpellProjectile projectile)
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public class SpellProjectile_DebugInfo : SpellProjectileCallbackAction
    {
        public override void ProjectileCollide(SpellProjectile projectile, Collision collision)
        {
            Debug.Log($"The projectile of {projectile.gameObject.name} has collided and this method is being called");
        }

        public override void ProjectileExpire(SpellProjectile projectile)
        {
            Debug.Log($"The projectile of {projectile.gameObject.name} has expired and this method is being called");
        }
    }
}