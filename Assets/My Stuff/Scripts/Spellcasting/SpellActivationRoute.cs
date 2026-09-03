namespace AgeOfEnlightenment.Spellcasting
{
    using Alchemy.Inspector;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public enum RouteButtonState { Pressed, Hold, Released, Any }
    public enum SpellShootDirection { Left, Right, Up, Down, Forward, Back }
    [Serializable]
	public class SpellActivationRoute
	{
        /// <summary>
        /// An activation route is a sequence of RouteSteps that need to be completed to fully "cast" a mode of the spell.
        /// 
        /// This can be multiple different actions, or as simple as pressing a button.
        /// </summary>


        [Header("Route Settings")]
        [SerializeField] string _name;
        [SerializeField] float _durationBeforeExpire = 2;
        [SerializeField] List<RouteStep> _routeSteps;

        //We would have more than just the projectile settings here, but again prototyping
        [Title("Projectile Settings")]
        [SerializeField] GameObject _projectilePrefab;
        [SerializeField] float _projectileSpeed = 5f;
        [SerializeField] float _projectileLifetime = 10f;
        [SerializeField] SpellShootDirection _projectileShootDirection;

        [Title("Impact VFX")]
        [SerializeField] GameObject _impactPrefab;
        //Maybe here we determine the lifetime?
        //Or possibly if multiple are spawned?
        //Or if the scale is influenced by the projectile

        public string DisplayName => _name;
        public float MaximumSequenceSeconds => _durationBeforeExpire;
        public IReadOnlyList<RouteStep> ActivationSteps => _routeSteps;

        public GameObject ProjectilePrefab => _projectilePrefab;
        public float ProjectileSpeed => _projectileSpeed;
        public float ProjectileLifetime => _projectileLifetime;
        public SpellShootDirection ProjectileShootDirection => _projectileShootDirection;

        public GameObject ImpactPrefab => _impactPrefab;


        public bool IsValid()
        {
            if (_routeSteps.Count == 0) return false;
            if(_projectilePrefab == null) return false;

            return true;
        }

    }

    
}