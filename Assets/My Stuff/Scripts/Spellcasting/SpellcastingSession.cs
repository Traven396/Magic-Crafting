namespace AgeOfEnlightenment.Spellcasting
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.Timeline.Actions;
    using UnityEngine;

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
            SpellRouteActionExecutor.ExecuteActions(_lastStepCompleted.OnStepCompletedActions, _actionContext);
            
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

            FinishProjectile(projectile);
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

            FinishProjectile(projectile);
        }

        void FinishProjectile(SpellProjectile projectile)
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

        public void LaunchProjectile(SpellProjectile projectile, Vector3 direction, float speed, float lifetime, List<SpellProjectileCallbackAction> callbacks)
        {
            _activeProjectiles.Add(projectile);

            if(callbacks != null)
                if(callbacks.Count > 0)
                    _callbacks.Add(projectile, callbacks);

            projectile.Initialize(this, direction, speed, lifetime);
        }

    }

    //This class is for keeping track of any mid-cast objects that we have. Previews, VFX, and other flair.
    //This is seperate because at this point we might not even have a finished spell, so a SpellcastingSession isnt possible
    public class SpellActionContext
    {
        private Dictionary<string, GameObject> _runtimeObjects = new();

        public Transform CastOrigin { get; private set; }
        public SpellcastingSession AssociatedCastingSession { get; private set; }

        public SpellActionContext(Transform castOrigin)
        {
            CastOrigin = castOrigin;
        }

        public void AttachCastingSession(SpellcastingSession castingSession)
        {
            AssociatedCastingSession = castingSession;
        }

        public void SetRuntimeObject(string key, GameObject runtimeObject)
        {
            DestroyRuntimeObject(key);

            _runtimeObjects[key] = runtimeObject;
        }

        public bool TryGetRuntimeObject(string key, out GameObject runtimeObject)
        {
            return _runtimeObjects.TryGetValue(key, out runtimeObject);
        }

        public void DestroyRuntimeObject(string key)
        {
            if (!_runtimeObjects.TryGetValue(key, out GameObject runtimeObject)) return;

            if (runtimeObject != null) UnityEngine.Object.Destroy(runtimeObject);

            _runtimeObjects.Remove(key);
        }

        public void DestroyAllRuntimeObjects()
        {
            foreach (GameObject runtimeObject in _runtimeObjects.Values)
            {
                if (runtimeObject != null) UnityEngine.Object.Destroy(runtimeObject);
            }

            _runtimeObjects.Clear();
        }
    }
}