namespace AgeOfEnlightenment.Spellcasting
{
    using FoxheadDev.GestureDetection;
    using System;
    using UnityEngine;

    using NamedCondition = System.Tuple<string, System.Func<bool>>;

    public static class GestureTranslator
	{
		//This class is for converting the inspector set values into an actual Func<bool> that we can test for gestures

		public static NamedCondition CreatePredicatedGesture(GestureSpec gesture, PhysicsTracker tracker)
		{
            if (gesture == null) throw new ArgumentNullException(nameof(gesture));
			//We need to be sure that there is an actual tracker to be checking against
			if (tracker == null) throw new ArgumentNullException(nameof(tracker));
			//Make sure the gesture is actually valid
			if(!gesture.IsValid()) throw new ArgumentException("Gesture is not valid.", nameof(gesture));

			float? speedOverride = gesture.OverrideSpeed ? gesture.MinimumSpeed : null;


            return CreateCondition(gesture, tracker, speedOverride);
        }

		static NamedCondition CreateCondition(GestureSpec gesture, PhysicsTracker tracker, float? speedOverride)
		{
			switch (gesture.Type)
			{
				case GestureType.Punch:
					switch (gesture.VelocitySpace)
					{
						case GestureVelocitySpace.View:
                            return PremadeGestureLibrary.PunchInViewDirection(tracker, gesture.Direction, speedOverride);
							
						case GestureVelocitySpace.Reverse_View:
                            return PremadeGestureLibrary.ReversePunchInViewDirection(tracker, gesture.Direction, speedOverride);
                            
						case GestureVelocitySpace.Global:
                            return PremadeGestureLibrary.PunchGlobal(tracker, speedOverride);

                        case GestureVelocitySpace.Reverse_Global:
                            return PremadeGestureLibrary.ReversePunchGlobal(tracker, speedOverride);
					}
					break;
				case GestureType.Slash:
                    switch (gesture.VelocitySpace)
                    {
                        case GestureVelocitySpace.View:
                            return PremadeGestureLibrary.SlashInViewDirection(tracker, gesture.Direction, speedOverride);

                        case GestureVelocitySpace.Reverse_View:
                            return PremadeGestureLibrary.ReverseSlashInViewDirection(tracker, gesture.Direction, speedOverride);
                            
                        case GestureVelocitySpace.Global:
                            return PremadeGestureLibrary.SlashGlobal(tracker, speedOverride);
                            
                        case GestureVelocitySpace.Reverse_Global:
                            return PremadeGestureLibrary.ReverseSlashGlobal(tracker, speedOverride);
                            
                    }
                    break;
				case GestureType.Push:
                    switch (gesture.VelocitySpace)
                    {
                        case GestureVelocitySpace.View:
                            return PremadeGestureLibrary.PushInViewDirection(tracker, gesture.Direction, speedOverride);

                        case GestureVelocitySpace.Reverse_View:
                            return PremadeGestureLibrary.ReversePushInViewDirection(tracker, gesture.Direction, speedOverride);

                        case GestureVelocitySpace.Global:
                            return PremadeGestureLibrary.PushGlobal(tracker, speedOverride);

                        case GestureVelocitySpace.Reverse_Global:
                            return PremadeGestureLibrary.ReversePushGlobal(tracker, speedOverride);

                    }
                    break;
				case GestureType.Flick:
                    return PremadeGestureLibrary.SelfSpaceFlick(tracker, gesture.Direction, speedOverride);
                    
			}

            throw new ArgumentOutOfRangeException(nameof(gesture.Type));
		}
	}

}