using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace FoxheadDev.GestureDetection
{
    using NamedCondition = Tuple<string, Func<bool>>;

    public class PremadeGestureLibrary
    {

        /*
         * Some initial terminology so that these names make more sense. If you want to make your own, go hog wild lol
         * 
         *  Hand space is motion in reference to the hand's local space. So if you turn your hand to have the palm facing up, then the local "up" now becomes to the right. Essentially following your thumb
         * 
         *  A punch is a motion along the local Z axis, IE towards your fingers.
         *  A push is a motion along the local X axis, IE your palm.
         *  A slash is a motion along the local Y axis, IE your thumb
         * 
         *  A swing is only used for going up or down, it follows the Global Y axis
         *  
         */
        //The consistent velocity threshold to be used over and over
        public const float SmallGestureSpeed = 2f;
        public const float MediumGestureSpeed = 3f;
        public const float LargeGestureSpeed = 4f;

        /// <summary>
        /// A motion towards the positive Z axis of the hand. Some directions of this gesture are almost physically impossible, so be aware lol
        /// </summary>
        /// <param name="tracker">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedCondition PunchInViewDirection(PhysicsTracker tracker, GestureDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;
            
            switch (direction)
            {
                case GestureDirection.Left:
                    gesture = () => tracker.SelfSpaceVelocity.z >= minVelocity &&
                    tracker.ViewSpaceVelocity.x <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyZ() &&
                    tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.Right:
                    gesture = () => tracker.SelfSpaceVelocity.z >= minVelocity && 
                    tracker.ViewSpaceVelocity.x >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyZ() &&
                    tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.Up:
                    gesture = () => tracker.SelfSpaceVelocity.z >= minVelocity && 
                    tracker.ViewSpaceVelocity.y >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyZ() &&
                    tracker.ViewSpaceVelocity.MostlyY();
                    break;
                case GestureDirection.Down:
                    gesture = () => tracker.SelfSpaceVelocity.z >= minVelocity && 
                    tracker.ViewSpaceVelocity.y <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyZ() &&
                    tracker.ViewSpaceVelocity.MostlyY();
                    break;
                case GestureDirection.Forward:
                    gesture = () => tracker.SelfSpaceVelocity.z >= minVelocity && 
                    tracker.ViewSpaceVelocity.z >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyZ() &&
                    tracker.ViewSpaceVelocity.MostlyZ();
                    break;
                case GestureDirection.Back:
                    gesture = () => tracker.SelfSpaceVelocity.z >= minVelocity && 
                    tracker.ViewSpaceVelocity.z <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyZ() &&
                    tracker.ViewSpaceVelocity.MostlyZ();
                    break;
                case GestureDirection.InwardHoriz:
                        gesture = () => tracker.SelfSpaceVelocity.z >= minVelocity &&
                        tracker.ViewSpaceVelocity.x >= minVelocity &&
                        tracker.SelfSpaceVelocity.MostlyZ() &&
                        tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.OutwardHoriz:
                        gesture = () => tracker.SelfSpaceVelocity.z >= minVelocity &&
                        tracker.ViewSpaceVelocity.x <= -minVelocity &&
                        tracker.SelfSpaceVelocity.MostlyZ() &&
                        tracker.ViewSpaceVelocity.MostlyX();
                    break;
            }

            return Tuple.Create($"Punch {direction}", gesture);
        }

        /// <summary>
        /// A motion towards the negative Z axis of the hand. Kinda like yanking your hand back. Some directions of this gesture are almost physically impossible, so be aware lol
        /// </summary>
        /// <param name="tracker">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedCondition ReversePunchInViewDirection(PhysicsTracker tracker, GestureDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case GestureDirection.Left:
                    gesture = () => tracker.SelfSpaceVelocity.z <= -minVelocity && 
                    tracker.ViewSpaceVelocity.x <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyZ() &&
                    tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.Right:
                    gesture = () => tracker.SelfSpaceVelocity.z <= -minVelocity && 
                    tracker.ViewSpaceVelocity.x >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyZ() &&
                    tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.Up:
                    gesture = () => tracker.SelfSpaceVelocity.z <= -minVelocity && 
                    tracker.ViewSpaceVelocity.y >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyZ() &&
                    tracker.ViewSpaceVelocity.MostlyY();
                    break;
                case GestureDirection.Down:
                    gesture = () => tracker.SelfSpaceVelocity.z <= -minVelocity && 
                    tracker.ViewSpaceVelocity.y <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyZ() &&
                    tracker.ViewSpaceVelocity.MostlyY();
                    break;
                case GestureDirection.Forward:
                    gesture = () => tracker.SelfSpaceVelocity.z <= -minVelocity && 
                    tracker.ViewSpaceVelocity.z >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyZ() &&
                    tracker.ViewSpaceVelocity.MostlyZ();
                    break;
                case GestureDirection.Back:
                    gesture = () => tracker.SelfSpaceVelocity.z <= -minVelocity && 
                    tracker.ViewSpaceVelocity.z <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyZ() &&
                    tracker.ViewSpaceVelocity.MostlyZ();
                    break;
                case GestureDirection.InwardHoriz:
                        gesture = () => tracker.SelfSpaceVelocity.z <= -minVelocity &&
                        tracker.ViewSpaceVelocity.x <= -minVelocity &&
                        tracker.SelfSpaceVelocity.MostlyZ() &&
                        tracker.ViewSpaceVelocity.MostlyX();
                    
                    break;
                case GestureDirection.OutwardHoriz:
                        gesture = () => tracker.SelfSpaceVelocity.z <= -minVelocity && 
                        tracker.ViewSpaceVelocity.x >= minVelocity &&
                        tracker.SelfSpaceVelocity.MostlyZ() &&
                        tracker.ViewSpaceVelocity.MostlyX();
                    break;
            }

            return Tuple.Create($"Reverse Punch {direction}", gesture);
        }

        /// <summary>
        /// A motion towards the negative X axis of the hand (flipped for left hand, the system automatically corrects it). The direction towards your palm. Like a high five/Throwing gesture
        /// </summary>
        /// <param name="tracker">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedCondition PushInViewDirection(PhysicsTracker tracker, GestureDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case GestureDirection.Left:
                    gesture = () => tracker.SelfSpaceVelocity.x <= -minVelocity && 
                    tracker.ViewSpaceVelocity.x <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyX() &&
                    tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.Right:
                    gesture = () => tracker.SelfSpaceVelocity.x <= -minVelocity && 
                    tracker.ViewSpaceVelocity.x >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyX() &&
                    tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.Up:
                    gesture = () => tracker.SelfSpaceVelocity.x <= -minVelocity && 
                    tracker.ViewSpaceVelocity.y >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyX() &&
                    tracker.ViewSpaceVelocity.MostlyY();
                    break;
                case GestureDirection.Down:
                    gesture = () => tracker.SelfSpaceVelocity.x <= -minVelocity && 
                    tracker.ViewSpaceVelocity.y <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyX() &&
                    tracker.ViewSpaceVelocity.MostlyY();
                    break;
                case GestureDirection.Forward:
                    gesture = () => tracker.SelfSpaceVelocity.x <= -minVelocity && 
                    tracker.ViewSpaceVelocity.z >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyX() &&
                    tracker.ViewSpaceVelocity.MostlyZ();
                    break;
                case GestureDirection.Back:
                    gesture = () => tracker.SelfSpaceVelocity.x <= -minVelocity && 
                    tracker.ViewSpaceVelocity.z <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyX() &&
                    tracker.ViewSpaceVelocity.MostlyZ();
                    break;
                case GestureDirection.InwardHoriz:
                        gesture = () => tracker.SelfSpaceVelocity.x <= -minVelocity &&
                        tracker.ViewSpaceVelocity.x <= -minVelocity &&
                        tracker.SelfSpaceVelocity.MostlyX() &&
                        tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.OutwardHoriz:
                        gesture = () => tracker.SelfSpaceVelocity.x <= -minVelocity &&
                        tracker.ViewSpaceVelocity.x >= minVelocity &&
                        tracker.SelfSpaceVelocity.MostlyX() &&
                        tracker.ViewSpaceVelocity.MostlyX();
                    break;
            }

            return Tuple.Create($"Push {direction}", gesture);
        }

        /// <summary>
        /// A motion towards the positive X axis of the hand (flipped for left hand, the system automatically corrects it). The direction towards the back of your hand. Imagine backhand slapping someone, that gesture
        /// </summary>
        /// <param name="tracker">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedCondition ReversePushInViewDirection(PhysicsTracker tracker, GestureDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                // These first two cases on Left and Right have the weird conditional at the end because we are Flipping the X axis so that gestures are universal for each hand
                //The ? means that if the physics tracker IS the left hand, we use the >= side of it, and if its not the left hand we use the other
                case GestureDirection.Left:
                    gesture = () => tracker.SelfSpaceVelocity.x >= minVelocity && 
                    tracker.ViewSpaceVelocity.x <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyX() &&
                    tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.Right:
                    gesture = () => tracker.SelfSpaceVelocity.x >= minVelocity && 
                    tracker.ViewSpaceVelocity.x >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyX() &&
                    tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.Up:
                    gesture = () => tracker.SelfSpaceVelocity.x >= minVelocity && 
                    tracker.ViewSpaceVelocity.y >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyX() &&
                    tracker.ViewSpaceVelocity.MostlyY();
                    break;
                case GestureDirection.Down:
                    gesture = () => tracker.SelfSpaceVelocity.x >= minVelocity && 
                    tracker.ViewSpaceVelocity.y <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyX() &&
                    tracker.ViewSpaceVelocity.MostlyY();
                    break;
                case GestureDirection.Forward:
                    gesture = () => tracker.SelfSpaceVelocity.x >= minVelocity && 
                    tracker.ViewSpaceVelocity.z >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyX() &&
                    tracker.ViewSpaceVelocity.MostlyZ();
                    break;
                case GestureDirection.Back:
                    gesture = () => tracker.SelfSpaceVelocity.x >= minVelocity && 
                    tracker.ViewSpaceVelocity.z <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyX() &&
                    tracker.ViewSpaceVelocity.MostlyZ();
                    break;
                case GestureDirection.InwardHoriz:
                        gesture = () => tracker.SelfSpaceVelocity.x >= minVelocity &&
                        tracker.ViewSpaceVelocity.x <= -minVelocity &&
                        tracker.SelfSpaceVelocity.MostlyX() &&
                        tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.OutwardHoriz:
                        gesture = () => tracker.SelfSpaceVelocity.x >= minVelocity &&
                        tracker.ViewSpaceVelocity.x >= minVelocity &&
                        tracker.SelfSpaceVelocity.MostlyX() &&
                        tracker.ViewSpaceVelocity.MostlyX();
                    break;
            }

            return Tuple.Create($"Reverse Push {direction}", gesture);
        }

        /// <summary>
        /// A motion towards the positive Y axis of the hand. The direction towards your thumb. Imagine pulling something out of the ground
        /// </summary>
        /// <param name="tracker">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedCondition SlashInViewDirection(PhysicsTracker tracker, GestureDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case GestureDirection.Left:
                    gesture = () => tracker.SelfSpaceVelocity.y >= minVelocity && 
                    tracker.ViewSpaceVelocity.x <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyY() &&
                    tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.Right:
                    gesture = () => tracker.SelfSpaceVelocity.y >= minVelocity && 
                    tracker.ViewSpaceVelocity.x >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyY() &&
                    tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.Up:
                    gesture = () => tracker.SelfSpaceVelocity.y >= minVelocity && 
                    tracker.ViewSpaceVelocity.y >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyY() &&
                    tracker.ViewSpaceVelocity.MostlyY();
                    break;
                case GestureDirection.Down:
                    gesture = () => tracker.SelfSpaceVelocity.y >= minVelocity && 
                    tracker.ViewSpaceVelocity.y <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyY() &&
                    tracker.ViewSpaceVelocity.MostlyY();
                    break;
                case GestureDirection.Forward:
                    gesture = () => tracker.SelfSpaceVelocity.y >= minVelocity && 
                    tracker.ViewSpaceVelocity.z >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyY() &&
                    tracker.ViewSpaceVelocity.MostlyZ();
                    break;
                case GestureDirection.Back:
                    gesture = () => tracker.SelfSpaceVelocity.y >= minVelocity && 
                    tracker.ViewSpaceVelocity.z <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyY() &&
                    tracker.ViewSpaceVelocity.MostlyZ();
                    break;
                case GestureDirection.InwardHoriz:
                        gesture = () => tracker.SelfSpaceVelocity.y >= minVelocity && 
                        tracker.ViewSpaceVelocity.x <= -minVelocity &&
                        tracker.SelfSpaceVelocity.MostlyY() &&
                        tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.OutwardHoriz:
                        gesture = () => tracker.SelfSpaceVelocity.y >= minVelocity &&
                        tracker.ViewSpaceVelocity.x >= minVelocity &&
                        tracker.SelfSpaceVelocity.MostlyY() &&
                        tracker.ViewSpaceVelocity.MostlyX();
                    break;
            }

            return Tuple.Create($"Slash {direction}", gesture);
        }

        /// <summary>
        /// A motion towards the negative Y axis of the hand. The direction away your thumb. A sort of stabbing motion
        /// </summary>
        /// <param name="tracker">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedCondition ReverseSlashInViewDirection(PhysicsTracker tracker, GestureDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case GestureDirection.Left:
                    gesture = () => tracker.SelfSpaceVelocity.y <= -minVelocity && 
                    tracker.ViewSpaceVelocity.x <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyY() &&
                    tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.Right:
                    gesture = () => tracker.SelfSpaceVelocity.y <= -minVelocity && 
                    tracker.ViewSpaceVelocity.x >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyY() &&
                    tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.Up:
                    gesture = () => tracker.SelfSpaceVelocity.y <= -minVelocity && 
                    tracker.ViewSpaceVelocity.y >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyY() &&
                    tracker.ViewSpaceVelocity.MostlyY();
                    break;
                case GestureDirection.Down:
                    gesture = () => tracker.SelfSpaceVelocity.y <= -minVelocity && 
                    tracker.ViewSpaceVelocity.y <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyY() &&
                    tracker.ViewSpaceVelocity.MostlyY();
                    break;
                case GestureDirection.Forward:
                    gesture = () => tracker.SelfSpaceVelocity.y <= -minVelocity && 
                    tracker.ViewSpaceVelocity.z >= minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyY() &&
                    tracker.ViewSpaceVelocity.MostlyZ();
                    break;
                case GestureDirection.Back:
                    gesture = () => tracker.SelfSpaceVelocity.y <= -minVelocity && 
                    tracker.ViewSpaceVelocity.z <= -minVelocity &&
                    tracker.SelfSpaceVelocity.MostlyY() &&
                    tracker.ViewSpaceVelocity.MostlyZ();
                    break;
                case GestureDirection.InwardHoriz:
                        gesture = () => tracker.SelfSpaceVelocity.y <= -minVelocity && 
                        tracker.ViewSpaceVelocity.x <= -minVelocity &&
                        tracker.SelfSpaceVelocity.MostlyY() &&
                        tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.OutwardHoriz:
                        gesture = () => tracker.SelfSpaceVelocity.y <= -minVelocity && 
                        tracker.ViewSpaceVelocity.x <= -minVelocity &&
                        tracker.SelfSpaceVelocity.MostlyY() &&
                        tracker.ViewSpaceVelocity.MostlyX();
                    break;
            }

            return Tuple.Create($"Reverse Slash {direction}", gesture);
        }

        /// <summary>
        /// A motion compared to the Global Y axis. Regardless of any rotation or hand orientation. This only works Up and Down
        /// </summary>
        /// <param name="tracker">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against. The only ones that work for this are Up and Down</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedCondition SwingGlobal(PhysicsTracker tracker, GestureDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case GestureDirection.Left:
                    throw new Exception("A SwingGlobal gesture can only be checked against Up or Down.");
                case GestureDirection.Right:
                    throw new Exception("A SwingGlobal gesture can only be checked against Up or Down.");
                case GestureDirection.Up:
                    gesture = () => Vector3.Dot(tracker.Velocity, Vector3.up) >= minVelocity && 
                    tracker.Velocity.MostlyY();
                    break;
                case GestureDirection.Down:
                    gesture = () => Vector3.Dot(tracker.Velocity, Vector3.up) <= -minVelocity && 
                    tracker.Velocity.MostlyY();
                    break;
                case GestureDirection.Forward:
                    throw new Exception("A SwingGlobal gesture can only be checked against Up or Down.");
                case GestureDirection.Back:
                    throw new Exception("A SwingGlobal gesture can only be checked against Up or Down.");
            }

            return Tuple.Create($"Swing {direction}", gesture);
        }

        /// <summary>
        /// This is a gesture for just pulling your hand back in any direction, along the negative Z axis. The opposite gesture of a fistbump
        /// </summary>
        /// <param name="tracker">The Hand that should be checked for this gesture</param>
        /// <param name="velocity">The minimum velocity required to achieve the gesture. If not set it defaults to 2 units per second.</param>
        /// <returns></returns>
        public static NamedCondition ReversePunchGlobal(PhysicsTracker tracker, float? velocity = null)
        {
            float minVelocity = velocity ?? SmallGestureSpeed;

            Func<bool> gesture = () => tracker.SelfSpaceVelocity.z <= -minVelocity && 
            tracker.SelfSpaceVelocity.MostlyZ();

            return Tuple.Create($"Global Reverse Punch", gesture);
        }
        /// <summary>
        /// This is a gesture for just punching forward in any direction, along the negative Z axis. A fistbump style gesture
        /// </summary>
        /// <param name="tracker">The Hand that should be checked for this gesture</param>
        /// <param name="velocity">The minimum velocity required to achieve the gesture. If not set it defaults to 2 units per second.</param>
        /// <returns></returns>
        public static NamedCondition PunchGlobal(PhysicsTracker tracker, float? velocity = null)
        {
            float minVelocity = velocity ?? SmallGestureSpeed;

            Func<bool> gesture = () => tracker.SelfSpaceVelocity.z >= minVelocity &&
            tracker.SelfSpaceVelocity.MostlyZ();

            return Tuple.Create($"Global Punch", gesture);
        }

        /// <summary>
        /// This is a gesture for throwing your hand forward in any direction. This is only checking the SelfSpace velocity
        /// </summary>
        /// <param name="tracker">The Hand that should be checked for this gesture</param>
        /// <param name="velocity">The minimum velocity required to achieve the gesture. If not set it defaults to 2 units per second.</param>
        /// <returns></returns>
        public static NamedCondition PushGlobal(PhysicsTracker tracker, float? velocity = null)
        {
            float minVelocity = velocity ?? SmallGestureSpeed;

            Func<bool> gesture = () => tracker.SelfSpaceVelocity.x <= -minVelocity && 
            tracker.SelfSpaceVelocity.MostlyX();

            return Tuple.Create($"Global Push", gesture);
        }

        /// <summary>
        /// This is a gesture for throwing your hand backward in any direction. This is only checking the SelfSpace velocity
        /// </summary>
        /// <param name="tracker">The Hand that should be checked for this gesture</param>
        /// <param name="velocity">The minimum velocity required to achieve the gesture. If not set it defaults to 2 units per second.</param>
        /// <returns></returns>
        public static NamedCondition ReversePushGlobal(PhysicsTracker tracker, float? velocity = null)
        {
            float minVelocity = velocity ?? SmallGestureSpeed;

            Func<bool> gesture = () => tracker.SelfSpaceVelocity.x >= minVelocity &&
            tracker.SelfSpaceVelocity.MostlyX();

            return Tuple.Create($"Global Reverse Push", gesture);
        }

        /// <summary>
        /// This is a gesture for slashing your hand in any direction. This is only checking the SelfSpace velocity.
        /// </summary>
        /// <param name="tracker">The Hand that should be checked for this gesture</param>
        /// <param name="velocity">The minimum velocity required to achieve the gesture. If not set it defaults to 2 units per second.</param>
        /// <returns></returns>
        public static NamedCondition SlashGlobal(PhysicsTracker tracker, float? velocity = null)
        {
            float minVelocity = velocity ?? SmallGestureSpeed;

            Func<bool> gesture = () => tracker.SelfSpaceVelocity.y >= minVelocity &&
            tracker.SelfSpaceVelocity.MostlyY();

            return Tuple.Create($"Global Slash", gesture);
        }
        /// <summary>
        /// This is a gesture for slashing your hand in any direction, but reversed. This is only checking the SelfSpace velocity.
        /// </summary>
        /// <param name="tracker">The Hand that should be checked for this gesture</param>
        /// <param name="velocity">The minimum velocity required to achieve the gesture. If not set it defaults to 2 units per second.</param>
        /// <returns></returns>
        public static NamedCondition ReverseSlashGlobal(PhysicsTracker tracker, float? velocity = null)
        {
            float minVelocity = velocity ?? SmallGestureSpeed;

            Func<bool> gesture = () => tracker.SelfSpaceVelocity.y <= -minVelocity &&
            tracker.SelfSpaceVelocity.MostlyY();

            return Tuple.Create($"Global Slash", gesture);
        }
        /// <summary>
        /// Checks if the player has their hands close enough together. You can pass through how close this needs to be
        /// </summary>
        /// <param name="tracker">Which hand to check. This could technically be either one and it would return the same</param>
        /// <param name="closenessThreshold">How close, in Unity units, the hands need to be for this to be true. If not set it defaults to .3f</param>
        /// <returns></returns>
        //public static NamedConditionSet HandsCloseTogether(PhysicsTracker tracker, float? closenessThreshold = null)
        //{
        //    float threshold = closenessThreshold ?? .3f;

        //    Func<bool> gesture = () => tracker.DistanceToOtherHand <= threshold;

        //    return Tuple.Create($"Closeness Check", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        //}
        /// <summary>
        /// Checks if the player flicks their hand quick enough in a direction. This is entirely based off angular velocity/rotation. This is in relation to the hand itself
        /// </summary>
        /// <param name="tracker">The hand to check</param>
        /// <param name="direction">Which direction should the player flick. The clockwise and counterclockwise mean rotating your hand around the Z axis.</param>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public static NamedCondition SelfSpaceFlick(PhysicsTracker tracker, FlickDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed * 1.5f;
            Func<bool> gesture = null;

            switch (direction)
            {
                case FlickDirection.Left:
                    gesture = () => tracker.SelfSpaceAngularVelocity.y <= -minVelocity &&
                    tracker.SelfSpaceAngularVelocity.MostlyY();
                    break;
                case FlickDirection.Right:
                    gesture = () => tracker.SelfSpaceAngularVelocity.y >= minVelocity &&
                    tracker.SelfSpaceAngularVelocity.MostlyY();
                    break;
                case FlickDirection.Up:
                    gesture = () => tracker.SelfSpaceAngularVelocity.x <= -minVelocity &&
                    tracker.SelfSpaceAngularVelocity.MostlyX();
                    break;
                case FlickDirection.Down:
                    gesture = () => tracker.SelfSpaceAngularVelocity.x >= minVelocity &&
                    tracker.SelfSpaceAngularVelocity.MostlyX();
                    break;
                case FlickDirection.Clockwise:
                    gesture = () => tracker.SelfSpaceAngularVelocity.z >= -minVelocity &&
                    tracker.SelfSpaceAngularVelocity.MostlyZ();
                    break;
                case FlickDirection.Counterclockwise:
                    gesture = () => tracker.SelfSpaceAngularVelocity.z <= minVelocity &&
                    tracker.SelfSpaceAngularVelocity.MostlyZ();
                    break;
                case FlickDirection.Inward:
                        gesture = () => tracker.SelfSpaceAngularVelocity.y <= -minVelocity &&
                        tracker.SelfSpaceAngularVelocity.MostlyY();
                    break;
                case FlickDirection.Outward:
                        gesture = () => tracker.SelfSpaceAngularVelocity.y >= minVelocity &&
                        tracker.SelfSpaceAngularVelocity.MostlyY();
                    break;
            }

            return Tuple.Create($"Flick {direction}", gesture);
        }
        public static NamedCondition SelfSpaceFlick(PhysicsTracker tracker, GestureDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed * 1.5f;
            Func<bool> gesture = null;

            switch (direction)
            {
                case GestureDirection.Left:
                    gesture = () => tracker.SelfSpaceAngularVelocity.y <= -minVelocity &&
                    tracker.SelfSpaceAngularVelocity.MostlyY();
                    break;
                case GestureDirection.Right:
                    gesture = () => tracker.SelfSpaceAngularVelocity.y >= minVelocity &&
                    tracker.SelfSpaceAngularVelocity.MostlyY();
                    break;
                case GestureDirection.Up:
                    gesture = () => tracker.SelfSpaceAngularVelocity.x <= -minVelocity &&
                    tracker.SelfSpaceAngularVelocity.MostlyX();
                    break;
                case GestureDirection.Down:
                    gesture = () => tracker.SelfSpaceAngularVelocity.x >= minVelocity &&
                    tracker.SelfSpaceAngularVelocity.MostlyX();
                    break;
                //An X-Axis flick is one that can be used in either direction around the X-Axis. So this would be an Up or Down flick essentially.
                case GestureDirection.X_Axis:
                    gesture = () => tracker.SelfSpaceAngularVelocity.x.Abs() >= minVelocity &&
                    tracker.SelfSpaceAngularVelocity.MostlyX();
                    break;
                //A Y-Axis flick is flicking to either side of your hand. Like making a P.U. gesture
                case GestureDirection.Y_Axis:
                    gesture = () => tracker.SelfSpaceAngularVelocity.y.Abs() >= minVelocity &&
                    tracker.SelfSpaceAngularVelocity.MostlyY();
                    break;
                //A Z-Axis flick is one that should be used sparingly. Its rotating your hand quickly as if twisting something.
                case GestureDirection.Z_Axis:
                    gesture = () => tracker.SelfSpaceAngularVelocity.z.Abs() >= minVelocity &&
                    tracker.SelfSpaceAngularVelocity.MostlyZ();
                    break;
                default:
                    throw new ArgumentException("We cannot do a flick in the other directions yet. Gotta figure those numbers out bud");
            }

            return Tuple.Create($"Flick {direction}", gesture);
        }
        public static NamedCondition ZAxisFlick(PhysicsTracker tracker, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed * 1.5f;

            Func<bool> gesture = () => tracker.SelfSpaceAngularVelocity.z.Abs() >= minVelocity &&
            tracker.SelfSpaceAngularVelocity.MostlyZ();

            return Tuple.Create($"Z Axis Flick", gesture);
        }
        /// <summary>
        /// Checks if the hand palm direction vector is pointing the same direction as the viewspace up vector. Within a certain tolerance value
        /// </summary>
        /// <param name="tracker">The hand to check</param>
        /// <param name="tolerance">A value from 0 - 90, the difference between the two vectors must differ by max this much</param>
        /// <returns></returns>
        //public static NamedConditionSet PalmPointUpSelfSpace(PhysicsTracker tracker, float? tolerance = null)
        //{
        //    float threshold = tolerance ?? 15f;

        //    Func<bool> gesture = () => Vector3.Angle(tracker.UniveralPalm, Camera.main.transform.up) <= threshold;

        //    return Tuple.Create($"Palm Point Up", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        //}

        /// <summary>
        /// Requires the object to make a motion in the view direction (player's perspective) regardless of it's personal orientation.
        /// </summary>
        /// <param name="tracker">The hand to check</param>
        /// <param name="direction">The direction you are moving from the Player's perspective</param>
        /// <param name="velocity">An optional velocity requirement to pass and overwrite the base velocity.</param>
        /// <returns></returns>
        public static NamedCondition ViewSpaceMotion(PhysicsTracker tracker, GestureDirection direction, float? velocity = null)
        {
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case GestureDirection.Left:
                    gesture = () =>tracker.ViewSpaceVelocity.x <= -minVelocity &&
                    tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.Right:
                    gesture = () => tracker.ViewSpaceVelocity.x >= minVelocity &&
                    tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.Up:
                    gesture = () => tracker.ViewSpaceVelocity.y >= minVelocity &&
                    tracker.ViewSpaceVelocity.MostlyY();
                    break;
                case GestureDirection.Down:
                    gesture = () => tracker.ViewSpaceVelocity.y <= -minVelocity &&
                    tracker.ViewSpaceVelocity.MostlyY();
                    break;
                case GestureDirection.Forward:
                    gesture = () => tracker.ViewSpaceVelocity.z >= minVelocity &&
                    tracker.ViewSpaceVelocity.MostlyZ() &&
                    tracker.AccelerationStrength >= 0;
                    break;
                case GestureDirection.Back:
                    gesture = () => tracker.ViewSpaceVelocity.z <= -minVelocity &&
                    tracker.ViewSpaceVelocity.MostlyZ();
                    break;
                case GestureDirection.InwardHoriz:
                    gesture = () => tracker.ViewSpaceVelocity.x <= -minVelocity &&
                    tracker.ViewSpaceVelocity.MostlyX();
                    break;
                case GestureDirection.OutwardHoriz:
                    gesture = () => tracker.ViewSpaceVelocity.x >= minVelocity &&
                    tracker.ViewSpaceVelocity.MostlyX();
                    break;
            }

            return Tuple.Create($"ViewSpace Movement {direction}", gesture);
        }
    }
    public enum GestureType
    {
        Punch,
        Slash,
        Push,
        Flick,
        Orientless
    }
    public enum GestureVelocitySpace
    {
        View,
        Reverse_View,
        Global,
        Reverse_Global
    }
    public enum GestureDirection
    {
        Left,
        Right,
        Up,
        Down,
        Forward,
        Back,
        InwardHoriz,
        OutwardHoriz,
        X_Axis,
        Y_Axis,
        Z_Axis
    }
    public enum FlickDirection
    {
        Left,
        Right,
        Up,
        Down,
        Clockwise,
        Counterclockwise,
        Inward,
        Outward
    }
}