namespace AgeOfEnlightenment.Spellcasting
{
    using Alchemy.Inspector;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Spells/Base Spell", fileName = "New Spell")]
    public class SpellDefinitionSO : ScriptableObject
    {

        [SerializeField] List<SpellActivationRoute> _ActivationRoutes;








        enum SpellType
        {
            Projectile,
            AreaOfEffect
        }
        enum SpellShootDirection { Left, Right, Up, Down, Forward, Back }


        [Title("Old Methodology")]
        [SerializeField] SpellType _SpellType;
        [SerializeField] GameObject _ProjectilePrefab;


        [Title("Projectile Settings")]
        [SerializeField] SpellShootDirection _SpellShootDirection;
        [SerializeField] float _ProjectileSpeed;
        [SerializeField] float _ProjectileLifetime;

        [SerializeField] bool _RequireCharging;
        [ShowIf("_RequireCharging")]
        [SerializeField] float _ChargeTime;

        [SerializeField] bool _SpawnPreviewOnPress;
        [ShowIf("_SpawnPreviewOnPress")]
        [SerializeField] GameObject _PreviewPrefab;

        public void Activate_Press(Transform castPoint)
        {
            //if (_ActivationType == SpellActivationType.Press)
            //{

            //    GameObject projectile = Instantiate(_ProjectilePrefab, castPoint.position, castPoint.rotation);

            //    switch (_SpellShootDirection)
            //    {
            //        case SpellShootDirection.Left:
            //            projectile.GetComponent<SpellProjectile>().Launch(-castPoint.right, _ProjectileSpeed, _ProjectileLifetime);
            //            break;
            //        case SpellShootDirection.Right:
            //            projectile.GetComponent<SpellProjectile>().Launch(castPoint.right, _ProjectileSpeed, _ProjectileLifetime);
            //            break;
            //        case SpellShootDirection.Up:
            //            projectile.GetComponent<SpellProjectile>().Launch(castPoint.up, _ProjectileSpeed, _ProjectileLifetime);
            //            break;
            //        case SpellShootDirection.Down:
            //            projectile.GetComponent<SpellProjectile>().Launch(-castPoint.up, _ProjectileSpeed, _ProjectileLifetime);
            //            break;
            //        case SpellShootDirection.Forward:
            //            projectile.GetComponent<SpellProjectile>().Launch(castPoint.forward, _ProjectileSpeed, _ProjectileLifetime);
            //            break;
            //        case SpellShootDirection.Back:
            //            projectile.GetComponent<SpellProjectile>().Launch(-castPoint.forward, _ProjectileSpeed, _ProjectileLifetime);
            //            break;
            //    }
            //}
        }

        public void Activate_Hold(Transform castPoint)
        {

        }

        public void Activate_Release(Transform castPoint)
        {
            //if(_ActivationType == SpellActivationType.Release) {

            //    GameObject projectile = Instantiate(_ProjectilePrefab, castPoint.position, castPoint.rotation);

            //    switch (_SpellShootDirection)
            //    {
            //        case SpellShootDirection.Left:
            //            projectile.GetComponent<SpellProjectile>().Launch(-castPoint.right, _ProjectileSpeed, _ProjectileLifetime);
            //            break;
            //        case SpellShootDirection.Right:
            //            projectile.GetComponent<SpellProjectile>().Launch(castPoint.right, _ProjectileSpeed, _ProjectileLifetime);
            //            break;
            //        case SpellShootDirection.Up:
            //            projectile.GetComponent<SpellProjectile>().Launch(castPoint.up, _ProjectileSpeed, _ProjectileLifetime);
            //            break;
            //        case SpellShootDirection.Down:
            //            projectile.GetComponent<SpellProjectile>().Launch(-castPoint.up, _ProjectileSpeed, _ProjectileLifetime);
            //            break;
            //        case SpellShootDirection.Forward:
            //            projectile.GetComponent<SpellProjectile>().Launch(castPoint.forward, _ProjectileSpeed, _ProjectileLifetime);
            //            break;
            //        case SpellShootDirection.Back:
            //            projectile.GetComponent<SpellProjectile>().Launch(-castPoint.forward, _ProjectileSpeed, _ProjectileLifetime);
            //            break;
            //    }
            //}
        }
    }

}