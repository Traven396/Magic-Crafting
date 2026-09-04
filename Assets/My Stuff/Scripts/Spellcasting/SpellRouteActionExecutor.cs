namespace AgeOfEnlightenment.Spellcasting
{
    using System.Collections.Generic;
    using UnityEngine;

	/// <summary>
	/// This is the big class that will actually handle going through the events between the steps and having them do things
	/// 
	/// Spawning prefabs, projectiles, playing audio, haptics, anything you can imagine. All contained here
	/// </summary>
	public static class SpellRouteActionExecutor
	{
        public static void ExecuteActions(IReadOnlyList<RouteAction> actions, SpellActionContext context)
        {
            foreach (RouteAction action in actions)
            {
                ExecuteAction(action, context);
            }
        }

        public static void ExecuteAction(RouteAction action, SpellActionContext context)
        {
            if (action == null) return;
            
            if (context == null) return;
            
            if (context.CastOrigin == null) return;
            


            switch (action.Type)
            {
                case RouteActionType.SpawnPreviewAtCastOrigin:
                    SpawnPrefab(action, context);
                    break;

                case RouteActionType.DestroyObjects:
                    context.DestroyRuntimeObject(action.RuntimeObjectKey);
                    break;

                case RouteActionType.SpawnProjectileAtOrigin:
                    SpawnPrefab(action, context);
                    break;

                case RouteActionType.LaunchProjectile:
                    LaunchProjectile(action, context);
                    break;
            }

            // Future action types belong here:
            // PlayAudio(action, context);
            // SpawnShield(action, context);
            // StartBeam(action, context);
            // TeleportCaster(action, context);
            // ResolveAreaEffect(action, context);
        }

        private static void SpawnPrefab(RouteAction action, SpellActionContext context)
        {
            if (action.Prefab == null)
            {
                Debug.LogError($"Action {action.Type} has no prefab.");

                return;
            }

            GameObject spawnedObject = Object.Instantiate(action.Prefab, context.CastOrigin.position, context.CastOrigin.rotation);

            context.SetRuntimeObject(action.RuntimeObjectKey, spawnedObject);
        }

        private static void LaunchProjectile(RouteAction action, SpellActionContext context)
        {
            if (context.AssociatedCastingSession == null)
            {
                Debug.LogError("LaunchProjectile can only run after a SpellcastingSession has started.");

                return;
            }

            if (!context.TryGetRuntimeObject(action.RuntimeObjectKey, out GameObject projectileObject))
            {
                Debug.LogError($"No runtime object exists with key {action.RuntimeObjectKey}.");

                return;
            }

            if (!projectileObject.TryGetComponent(out SpellProjectile projectile))
            {
                Debug.LogError($"Runtime object {action.RuntimeObjectKey} has no SpellProjectile.");

                return;
            }

            Vector3 direction = GetShootDirection(action.ShootDirection, context.CastOrigin);

            context.AssociatedCastingSession.LaunchProjectile(projectile, direction, action.ProjectileSpeed, action.ProjectileLifetime, action.ProjectileCallbacks);
        }

        private static Vector3 GetShootDirection(SpellShootDirection shootDirection, Transform castOrigin)
        {
            switch (shootDirection)
            {
                case SpellShootDirection.Left:
                    return -castOrigin.right;

                case SpellShootDirection.Right:
                    return castOrigin.right;

                case SpellShootDirection.Up:
                    return castOrigin.up;

                case SpellShootDirection.Down:
                    return -castOrigin.up;

                case SpellShootDirection.Forward:
                    return castOrigin.forward;

                case SpellShootDirection.Back:
                    return -castOrigin.forward;
            }

            return castOrigin.forward;
        }
    }
}