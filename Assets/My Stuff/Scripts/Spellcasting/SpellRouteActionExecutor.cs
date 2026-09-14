namespace AgeOfEnlightenment.Spellcasting
{
    using DG.Tweening;
    using FoxheadDev.GestureDetection;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.XR;

    /// <summary>
    /// This is the big class that will actually handle going through the events between the steps and having them do things
    /// 
    /// Spawning prefabs, projectiles, playing audio, haptics, anything you can imagine. All contained here
    /// </summary>
	public static class SpellRouteActionExecutor
	{
        public static void ExecuteActions(IReadOnlyList<RouteStepAction> actions, SpellActionContext context)
        {
            foreach (RouteStepAction action in actions)
            {
                ExecuteAction(action, context);
            }
        }

        public static void ExecuteAction(RouteStepAction action, SpellActionContext context)
        {
            if (action == null) return;
            
            if (context == null) return;
            
            if (context.CastOrigin == null) return;

            switch (action.Type)
            {
                case RouteStepActionType.SpawnObjectAtCastOrigin:
                    SpawnRuntimeObject(action, context);
                    break;

                case RouteStepActionType.DestroyObjects:
                    context.DestroyRuntimeObject(action.RuntimeObjectKey);
                    break;

                case RouteStepActionType.LaunchProjectile:
                    LaunchProjectile(action, context);
                    break;
                case RouteStepActionType.PlayAudio:
                    PlayAudio(action, context);
                    break;

                case RouteStepActionType.LogMessage:
                    Debug.Log(action.Message);
                    break;

                case RouteStepActionType.TweenObject:
                    CreateTween(action, context);
                    break;
                case RouteStepActionType.LaunchProjectileBurst:
                    LaunchProjectileBurst(action, context);
                    break;
            }

            // Future action types belong here:
            // PlayAudio(action, context);
            // SpawnShield(action, context);
            // StartBeam(action, context);
            // TeleportCaster(action, context);
            // ResolveAreaEffect(action, context);
        }
        private static void CreateTween(RouteStepAction action, SpellActionContext context)
        {
            Tweener createdTween = TweenTranslator.CreateTweener(action.TweenSettings, context);

            if (createdTween != null) 
            {
                context.SetRuntimeTween(action.TweenSettings.TweenKey, createdTween);
            }
        }
        private static void SpawnRuntimeObject(RouteStepAction action, SpellActionContext context)
        {
            if (action.Prefab == null)
            {
                Debug.LogError($"Action {action.Type} has no prefab.");

                return;
            }

            GameObject spawnedObject = SpawnObject(action, context);

            context.SetRuntimeObject(action.RuntimeObjectKey, spawnedObject);
        }

        private static GameObject SpawnObject(RouteStepAction action, SpellActionContext context)
        {
            GameObject spawnedObject = Object.Instantiate(action.Prefab, context.CastOrigin.position, context.CastOrigin.rotation);

            spawnedObject.transform.localScale = Vector3.one * action.SpawnSize;

            spawnedObject.layer = action.SpawnLayer;

            if (action.ChildOfSpawnpoint)
                spawnedObject.transform.parent = context.CastOrigin;

            return spawnedObject;
        }

        private static void LaunchProjectile(RouteStepAction action, SpellActionContext context)
        {
            if (context.ParentSession == null)
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

            Vector3 direction = GetShootDirection(action.ShootDirection, context.CastOrigin, context.CasterPhysicsTracker);
            Entity target = null;
            if (action.TargetedProjectile)
            {
                target = SpellTargetManager.GetTarget(action.TargetSettings, context);
            }

            context.ParentSession.LaunchProjectile(projectile, direction, action.ProjectileSpeed, action.ProjectileLifetime, action.ProjectileModifiers, action.ProjectileCallbacks, target);
        }
        private static void LaunchProjectileBurst(RouteStepAction action, SpellActionContext context)
        {
            if (context.ParentSession == null)
            {
                Debug.LogError("LaunchProjectile can only run after a SpellcastingSession has started.");
                return;
            }

            Entity target = null;

            if (action.TargetedProjectile)
            {
                target = SpellTargetManager.GetTarget(action.TargetSettings, context);
            }
            
            for (int i = 0; i < action.ProjectileCount; i++)
            {
                var projectile = SpawnObject(action, context).GetComponent<SpellProjectile>();

                if (!projectile)
                {
                    Debug.LogError("Spawned projectile does not have SpellProjectile attached.");
                    return;
                }


                Vector3 spread = new(Random.Range(-action.BurstSpread, action.BurstSpread), Random.Range(-action.BurstSpread, action.BurstSpread), Random.Range(-action.BurstSpread, action.BurstSpread));

                Vector3 direction = GetShootDirection(action.ShootDirection, context.CastOrigin, context.CasterPhysicsTracker);

                direction += context.CastOrigin.TransformDirection(spread);

                context.ParentSession.LaunchProjectile(projectile, direction, action.ProjectileSpeed, action.ProjectileLifetime, action.ProjectileModifiers, action.ProjectileCallbacks, target);
            }
        }
        private static Vector3 GetShootDirection(SpellShootDirection shootDirection, Transform castOrigin, PhysicsTracker tracker)
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

                case SpellShootDirection.PlayerDirection:
                    return Camera.main.transform.forward;

                case SpellShootDirection.PlayerDirectionWithCastVelocity:
                    return Vector3.Slerp(tracker.Velocity.normalized, Camera.main.transform.forward, 0.5f);

            }

            return castOrigin.forward;
        }
    
        private static void PlayAudio(RouteStepAction action, SpellActionContext context)
        {
            if (action.LoopAudio)
            {
                if (string.IsNullOrEmpty(action.LoopAudioParentKey))
                {
                    context.SetRuntimeObject(action.AudioRuntimeKey, SoundManagerSO.PlayLoopingClipAsChild(action.AudioClip, context.CastOrigin, action.Volume, 0.06f, 0.06f));
                }
                else
                {
                    if(context.TryGetRuntimeObject(action.LoopAudioParentKey, out GameObject audioParent))
                    {
                        context.SetRuntimeObject(action.AudioRuntimeKey, SoundManagerSO.PlayLoopingClipAsChild(action.AudioClip, audioParent.transform, action.Volume, 0.06f, 0.06f));
                    }
                    else
                    {
                        Debug.LogError("We cannot find a runtime object with the key " + action.LoopAudioParentKey);
                        return;
                    }
                }
            }
            else
            {
                SoundManagerSO.PlayClipAtPoint(action.AudioClip, context.CastOrigin.position, action.Volume, 0.06f, 0.06f);
            }
        }
    
    
    }
}