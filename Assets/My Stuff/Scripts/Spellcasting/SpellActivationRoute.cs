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
        /// This is the container for a specific Route that the spell can take. Its basically a container for the entire branching Route system with a clearly defined Start point
        /// </summary>
        [Header("Route Settings")]
        [SerializeField] string _name;
        [SerializeField] RouteStep _firstStep;

        public string Name => _name;
        public RouteStep FirstStep => _firstStep;

        public bool IsValid()
        {
            if (_firstStep == null) return false;

            return _firstStep.IsValid();
        }

        public void GatherPossibleGestures(List<GestureSpec> output)
        {
            if (_firstStep != null) _firstStep.GatherPossibleGestures(output);
        }
    }

    
}