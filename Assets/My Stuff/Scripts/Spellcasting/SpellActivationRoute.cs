namespace AgeOfEnlightenment.Spellcasting
{
    using System;
	using UnityEngine;

	
	[Serializable]
	public class SpellActivationRoute
	{
        [SerializeField] RouteButtonState _PrimaryButtonState;
        [SerializeField] RouteButtonState _SecondaryButtonState;
    
        //Any required gesture or sequence of gestures
    }

    public enum RouteButtonState { Pressed, Hold, Released, Any }

}