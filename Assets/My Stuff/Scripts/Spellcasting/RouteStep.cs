namespace AgeOfEnlightenment.Spellcasting
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using UnityEngine;


    public enum SpellButton { Primary, Secondary, Tertiary }
    public enum  RouteEventType
    {
        ButtonPressed,
        ButtonReleased,
        GestureRecognized
    }

    public struct RouteEvent
    {
        public RouteEventType Type { get; }
        public SpellButton Button { get; }
        //Here is where we would designate the gesture. 

        private RouteEvent(RouteEventType type, SpellButton button)
        {
            Type = type;
            Button = button;
        }

        public static RouteEvent ButtonPressed(SpellButton button) => new RouteEvent(RouteEventType.ButtonPressed, button);
        public static RouteEvent ButtonReleased(SpellButton button) => new RouteEvent(RouteEventType.ButtonReleased, button);

        //Gesture recognized event creator. 
    }

    [Serializable]
	public class RouteStep
	{
        [SerializeField] private RouteEventType Type;
        [SerializeField] private SpellButton Button;
        [SerializeField] List<SpellButton> RequiredButtonsToBeHeld;


        public bool StepMatch(RouteEvent routeEvent, HashSet<SpellButton> heldButtons)
        {
            //If this isnt even the correct event type, the it doesn't match
            if (routeEvent.Type != Type) return false;

            //If the event we want is a button being pressed and we didnt get the right button, then it doesn't match
            if (Type == RouteEventType.ButtonPressed && routeEvent.Button != Button) return false;

            //The opposite is also false
            if (Type == RouteEventType.ButtonReleased && routeEvent.Button != Button) return false;

            foreach (SpellButton button in RequiredButtonsToBeHeld) 
            {
                //If our required buttons to be held are not all in the held buttons, then it doesn't match
                if (!heldButtons.Contains(button)) return false;
            }

            //If we made it this far then it should be a match
            return true;
        }

    }

}