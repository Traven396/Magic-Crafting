namespace AgeOfEnlightenment.Spellcasting
{
    using DG.Tweening;
    using FoxheadDev.GestureDetection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class SpellcastingSession
    {
        SpellDefinitionSO _spellDefintion;
        RouteStep _lastStepCompleted;
        MasterSpellcaster _originalCaster;

        SpellActionContext _actionContext;

        List<SpellProjectile> _activeProjectiles = new();

        Dictionary<SpellProjectile, List<SpellProjectileCallbackAction>> _callbacks = new();

        //This will be something much more complex once there is more spell types.
        bool _IsComplete;
        public bool IsComplete => _IsComplete;

        public SpellcastingSession(SpellDefinitionSO spellDefinition, RouteStep route, MasterSpellcaster originalCaster, SpellActionContext context)
        {
            _spellDefintion = spellDefinition;
            _lastStepCompleted = route;
            _originalCaster = originalCaster;
            _actionContext = context;

            _actionContext.AttachCastingSession(this);

            //We would also pass along the stats of when the spell was cast. But prototype mode
        }


        public void Begin()
        {
            
        }

        public void Notify_ProjectileHit(SpellProjectile projectile, Collision collision)
        {
            if (_callbacks.TryGetValue(projectile, out List<SpellProjectileCallbackAction> callback))
            {
                foreach (SpellProjectileCallbackAction call in callback)
                {
                    if (call._callbackType == SpellProjectileCallbackAction.ProjectileCallbackType.Impact || call._callbackType == SpellProjectileCallbackAction.ProjectileCallbackType.Both)
                    {
                        call.ProjectileCollide(projectile, collision);
                    }
                }
            }

            CompleteProjectle(projectile);
        }
        public void Notify_ProjectileExpire(SpellProjectile projectile)
        {
            if (_callbacks.TryGetValue(projectile, out List<SpellProjectileCallbackAction> callback))
            {
                foreach (SpellProjectileCallbackAction call in callback)
                {
                    if(call._callbackType == SpellProjectileCallbackAction.ProjectileCallbackType.Expire || call._callbackType == SpellProjectileCallbackAction.ProjectileCallbackType.Both)
                    {
                        call.ProjectileExpire(projectile);
                    }
                }
            }

            CompleteProjectle(projectile);
        }

        void CompleteProjectle(SpellProjectile projectile)
        {
            if(_activeProjectiles.Contains(projectile))
                _activeProjectiles.Remove(projectile);
            
            if (_callbacks.ContainsKey(projectile))
                _callbacks.Remove(projectile);

            if (_activeProjectiles.Count == 0) _IsComplete = true;
        }

        public void Tick(float deltaTime)
        {
            

        }

        public void LaunchProjectile(SpellProjectile projectile, Vector3 direction, float speed, float lifetime, List<SpellProjectileBehaviourModifier> modifiers, List<SpellProjectileCallbackAction> callbacks, Entity target = null)
        {
            _activeProjectiles.Add(projectile);

            if(callbacks != null)
                if(callbacks.Count > 0)
                    _callbacks.Add(projectile, callbacks);

            projectile.ActivateAndLaunch(this, direction, speed, lifetime, modifiers, target);
        }
    }

    //This class is for keeping track of any mid-cast objects that we have. Previews, VFX, and other flair.
    //This is seperate because at this point we might not even have a finished spell, so a SpellcastingSession isnt possible
    public class SpellActionContext
    {
        private Dictionary<string, GameObject> _runtimeObjects = new();
        private Dictionary<string, Tweener> _runtimeTweens = new();

        public Transform CastOrigin { get; private set; }
        public PhysicsTracker CasterPhysicsTracker { get; private set; }
        public SpellcastingSession ParentSession { get; private set; }

        public SpellActionContext(Transform castOrigin, PhysicsTracker tracker)
        {
            CastOrigin = castOrigin;
            CasterPhysicsTracker = tracker;
        }

        public void AttachCastingSession(SpellcastingSession castingSession)
        {
            ParentSession = castingSession;
        }

        public void SetRuntimeObject(string key, GameObject runtimeObject)
        {
            if (!String.IsNullOrEmpty(key))
            {
                DestroyRuntimeObject(key);

                _runtimeObjects[key] = runtimeObject; 
            }
        }

        public bool TryGetRuntimeObject(string key, out GameObject runtimeObject)
        {
            return _runtimeObjects.TryGetValue(key, out runtimeObject);
        }

        public void DestroyRuntimeObject(string key)
        {
            if (!TryGetRuntimeObject(key, out GameObject runtimeObject)) return;

            if (runtimeObject != null) 
            {
                DestroyRuntimeTweensFromObject(runtimeObject);
                UnityEngine.Object.Destroy(runtimeObject);
            }

            _runtimeObjects.Remove(key);
        }

        public void DestroyAllRuntimeObjects()
        {
            foreach (GameObject runtimeObject in _runtimeObjects.Values)
            {
                if (runtimeObject != null) 
                {
                    DestroyRuntimeTweensFromObject(runtimeObject);
                    UnityEngine.Object.Destroy(runtimeObject);
                }
            }

            _runtimeObjects.Clear();
        }
    
    
        public void SetRuntimeTween(string key, Tweener tween)
        {
            if (!String.IsNullOrEmpty(key))
            {
                DestroyRuntimeTween(key);

                _runtimeTweens[key] = tween;
            }
        }
        public bool TryGetRuntimeTween(string key, out Tweener tween)
        {
            return _runtimeTweens.TryGetValue(key, out tween);
        }
        void DestroyRuntimeTweensFromObject(GameObject runtimeObject)
        {
            var tweens = _runtimeTweens.Where(tw => tw.Value.target as Transform == runtimeObject.transform).ToList();

            foreach (var tween in tweens)
            {
                tween.Value.Kill();
                _runtimeTweens.Remove(tween.Key);
            }

        }
        public void DestroyRuntimeTween(string key)
        {
            if (!TryGetRuntimeTween(key, out Tweener runtimeTween)) return;

            if (runtimeTween != null)
            {
                runtimeTween.Kill();
            }

            _runtimeTweens.Remove(key);
        }
    }
}