namespace AgeOfEnlightenment.Spellcasting
{
    using System;
    using System.Collections.Generic;
    using FoxheadDev.GestureDetection;
    using UnityEngine;

    public class WandGestureInput : MonoBehaviour
    {
        private class GestureProbe
        {
            public GestureSpec Specification;
            public Func<bool> Predicate;
            public bool WasMatching;

            public GestureProbe(GestureSpec specification, Func<bool> predicate)
            {
                Specification = specification;
                Predicate = predicate;
            }
        }

        [SerializeField] private Transform _trackedTransform;
        [SerializeField] private Transform _mainBodyTransform;

        private readonly Dictionary<string, GestureProbe> _gestureProbes = new Dictionary<string, GestureProbe>();

        private PhysicsTracker _physicsTracker;
        public event Action<GestureSpec> GestureRecognized;

        private void Awake()
        {
            EnsurePhysicsTracker();
        }
        //private void OnEnable()
        //{
        //    GestureRecognized += DebugGesture;
        //}
        //private void OnDisable()
        //{
        //    GestureRecognized -= DebugGesture;
        //}
        public PhysicsTracker GetTracker()
        {
            if (_physicsTracker == null)
                EnsurePhysicsTracker();

            return _physicsTracker;
        }
        void DebugGesture(GestureSpec spec)
        {
            Debug.Log("Valid gesture: " + _physicsTracker.AccelerationStrength.ToString("#.00"));
        }
        private void Update()
        {
            if (_physicsTracker == null) return;

            _physicsTracker.Update(_trackedTransform.position, _trackedTransform.rotation, Time.deltaTime);

            foreach (GestureProbe gestureProbe in _gestureProbes.Values)
            {
                bool matchesNow = gestureProbe.Predicate();

                if (matchesNow && !gestureProbe.WasMatching) GestureRecognized?.Invoke(gestureProbe.Specification);

                gestureProbe.WasMatching = matchesNow;
            }
        }

        public void SetGestureSpecifications(IEnumerable<GestureSpec> gestureSpecifications)
        {
            if (!EnsurePhysicsTracker()) return;

            _gestureProbes.Clear();

            foreach (GestureSpec gestureSpecification in gestureSpecifications)
            {
                if (gestureSpecification == null || !gestureSpecification.IsValid()) continue;

                string key = gestureSpecification.GetKey();

                if (_gestureProbes.ContainsKey(key)) continue;


                /////////////////////////////////////////////////////////////////////////////////////////
                //Leaving an indicator here. We are returned a NamedCondition from CreatePredicatedGesture, but we are then just using its func for simplicities sake

                Func<bool> predicate = GestureTranslator.CreatePredicatedGesture(gestureSpecification, _physicsTracker).Item2;

                _gestureProbes.Add(key, new GestureProbe(gestureSpecification, predicate));
            }
        }

        private bool EnsurePhysicsTracker()
        {
            if (_physicsTracker != null) return true;

            if (_trackedTransform == null) _trackedTransform = transform;
            if (_mainBodyTransform == null) _mainBodyTransform = transform.root;
            if (_trackedTransform == null || _mainBodyTransform == null) return false;

            _physicsTracker = new PhysicsTracker(_trackedTransform, _mainBodyTransform);

            return true;
        }
    }
}