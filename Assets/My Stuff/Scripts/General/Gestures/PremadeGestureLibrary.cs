using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace FoxheadDev.GestureDetection
{
    using NamedConditionSet = Tuple<string, Func<bool>[]>;
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
        /// <param name="playerHand">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedConditionSet PunchInViewDirection(PhysicsTracker playerHand, ViewSpaceDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;
            
            switch (direction)
            {
                case ViewSpaceDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity &&
                    playerHand.ViewSpaceVelocity.x <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyZ() &&
                    playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && 
                    playerHand.ViewSpaceVelocity.x >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyZ() &&
                    playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && 
                    playerHand.ViewSpaceVelocity.y >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyZ() &&
                    playerHand.ViewSpaceVelocity.MostlyY();
                    break;
                case ViewSpaceDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && 
                    playerHand.ViewSpaceVelocity.y <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyZ() &&
                    playerHand.ViewSpaceVelocity.MostlyY();
                    break;
                case ViewSpaceDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && 
                    playerHand.ViewSpaceVelocity.z >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyZ() &&
                    playerHand.ViewSpaceVelocity.MostlyZ();
                    break;
                case ViewSpaceDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && 
                    playerHand.ViewSpaceVelocity.z <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyZ() &&
                    playerHand.ViewSpaceVelocity.MostlyZ();
                    break;
                case ViewSpaceDirection.InwardHoriz:
                        gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity &&
                        playerHand.ViewSpaceVelocity.x >= minVelocity &&
                        playerHand.SelfSpaceVelocity.MostlyZ() &&
                        playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.OutwardHoriz:
                        gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity &&
                        playerHand.ViewSpaceVelocity.x <= -minVelocity &&
                        playerHand.SelfSpaceVelocity.MostlyZ() &&
                        playerHand.ViewSpaceVelocity.MostlyX();
                    break;
            }

            return Tuple.Create($"Punch {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// A motion towards the negative Z axis of the hand. Kinda like yanking your hand back. Some directions of this gesture are almost physically impossible, so be aware lol
        /// </summary>
        /// <param name="playerHand">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedConditionSet ReversePunchInViewDirection(PhysicsTracker playerHand, ViewSpaceDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case ViewSpaceDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.x <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyZ() &&
                    playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.x >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyZ() &&
                    playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.y >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyZ() &&
                    playerHand.ViewSpaceVelocity.MostlyY();
                    break;
                case ViewSpaceDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.y <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyZ() &&
                    playerHand.ViewSpaceVelocity.MostlyY();
                    break;
                case ViewSpaceDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.z >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyZ() &&
                    playerHand.ViewSpaceVelocity.MostlyZ();
                    break;
                case ViewSpaceDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.z <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyZ() &&
                    playerHand.ViewSpaceVelocity.MostlyZ();
                    break;
                case ViewSpaceDirection.InwardHoriz:
                        gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity &&
                        playerHand.ViewSpaceVelocity.x <= -minVelocity &&
                        playerHand.SelfSpaceVelocity.MostlyZ() &&
                        playerHand.ViewSpaceVelocity.MostlyX();
                    
                    break;
                case ViewSpaceDirection.OutwardHoriz:
                        gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && 
                        playerHand.ViewSpaceVelocity.x >= minVelocity &&
                        playerHand.SelfSpaceVelocity.MostlyZ() &&
                        playerHand.ViewSpaceVelocity.MostlyX();
                    break;
            }

            return Tuple.Create($"Reverse Punch {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// A motion towards the negative X axis of the hand (flipped for left hand, the system automatically corrects it). The direction towards your palm. Like a high five/Throwing gesture
        /// </summary>
        /// <param name="playerHand">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedConditionSet PushInViewDirection(PhysicsTracker playerHand, ViewSpaceDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case ViewSpaceDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.x <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyX() &&
                    playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.x >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyX() &&
                    playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.y >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyX() &&
                    playerHand.ViewSpaceVelocity.MostlyY();
                    break;
                case ViewSpaceDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.y <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyX() &&
                    playerHand.ViewSpaceVelocity.MostlyY();
                    break;
                case ViewSpaceDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.z >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyX() &&
                    playerHand.ViewSpaceVelocity.MostlyZ();
                    break;
                case ViewSpaceDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.z <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyX() &&
                    playerHand.ViewSpaceVelocity.MostlyZ();
                    break;
                case ViewSpaceDirection.InwardHoriz:
                        gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity &&
                        playerHand.ViewSpaceVelocity.x <= -minVelocity &&
                        playerHand.SelfSpaceVelocity.MostlyX() &&
                        playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.OutwardHoriz:
                        gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity &&
                        playerHand.ViewSpaceVelocity.x >= minVelocity &&
                        playerHand.SelfSpaceVelocity.MostlyX() &&
                        playerHand.ViewSpaceVelocity.MostlyX();
                    break;
            }

            return Tuple.Create($"Push {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// A motion towards the positive X axis of the hand (flipped for left hand, the system automatically corrects it). The direction towards the back of your hand. Imagine backhand slapping someone, that gesture
        /// </summary>
        /// <param name="playerHand">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedConditionSet ReversePushInViewDirection(PhysicsTracker playerHand, ViewSpaceDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                // These first two cases on Left and Right have the weird conditional at the end because we are Flipping the X axis so that gestures are universal for each hand
                //The ? means that if the physics tracker IS the left hand, we use the >= side of it, and if its not the left hand we use the other
                case ViewSpaceDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && 
                    playerHand.ViewSpaceVelocity.x <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyX() &&
                    playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && 
                    playerHand.ViewSpaceVelocity.x >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyX() &&
                    playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && 
                    playerHand.ViewSpaceVelocity.y >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyX() &&
                    playerHand.ViewSpaceVelocity.MostlyY();
                    break;
                case ViewSpaceDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && 
                    playerHand.ViewSpaceVelocity.y <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyX() &&
                    playerHand.ViewSpaceVelocity.MostlyY();
                    break;
                case ViewSpaceDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && 
                    playerHand.ViewSpaceVelocity.z >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyX() &&
                    playerHand.ViewSpaceVelocity.MostlyZ();
                    break;
                case ViewSpaceDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && 
                    playerHand.ViewSpaceVelocity.z <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyX() &&
                    playerHand.ViewSpaceVelocity.MostlyZ();
                    break;
                case ViewSpaceDirection.InwardHoriz:
                        gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity &&
                        playerHand.ViewSpaceVelocity.x <= -minVelocity &&
                        playerHand.SelfSpaceVelocity.MostlyX() &&
                        playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.OutwardHoriz:
                        gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity &&
                        playerHand.ViewSpaceVelocity.x >= minVelocity &&
                        playerHand.SelfSpaceVelocity.MostlyX() &&
                        playerHand.ViewSpaceVelocity.MostlyX();
                    break;
            }

            return Tuple.Create($"Reverse Push {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// A motion towards the positive Y axis of the hand. The direction towards your thumb. Imagine pulling something out of the ground
        /// </summary>
        /// <param name="playerHand">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedConditionSet SlashInViewDirection(PhysicsTracker playerHand, ViewSpaceDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case ViewSpaceDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && 
                    playerHand.ViewSpaceVelocity.x <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyY() &&
                    playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && 
                    playerHand.ViewSpaceVelocity.x >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyY() &&
                    playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && 
                    playerHand.ViewSpaceVelocity.y >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyY() &&
                    playerHand.ViewSpaceVelocity.MostlyY();
                    break;
                case ViewSpaceDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && 
                    playerHand.ViewSpaceVelocity.y <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyY() &&
                    playerHand.ViewSpaceVelocity.MostlyY();
                    break;
                case ViewSpaceDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && 
                    playerHand.ViewSpaceVelocity.z >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyY() &&
                    playerHand.ViewSpaceVelocity.MostlyZ();
                    break;
                case ViewSpaceDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && 
                    playerHand.ViewSpaceVelocity.z <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyY() &&
                    playerHand.ViewSpaceVelocity.MostlyZ();
                    break;
                case ViewSpaceDirection.InwardHoriz:
                        gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && 
                        playerHand.ViewSpaceVelocity.x <= -minVelocity &&
                        playerHand.SelfSpaceVelocity.MostlyY() &&
                        playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.OutwardHoriz:
                        gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity &&
                        playerHand.ViewSpaceVelocity.x >= minVelocity &&
                        playerHand.SelfSpaceVelocity.MostlyY() &&
                        playerHand.ViewSpaceVelocity.MostlyX();
                    break;
            }

            return Tuple.Create($"Slash {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// A motion towards the negative Y axis of the hand. The direction away your thumb. A sort of stabbing motion
        /// </summary>
        /// <param name="playerHand">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedConditionSet ReverseSlashInViewDirection(PhysicsTracker playerHand, ViewSpaceDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case ViewSpaceDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.x <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyY() &&
                    playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.x >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyY() &&
                    playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.y >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyY() &&
                    playerHand.ViewSpaceVelocity.MostlyY();
                    break;
                case ViewSpaceDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.y <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyY() &&
                    playerHand.ViewSpaceVelocity.MostlyY();
                    break;
                case ViewSpaceDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.z >= minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyY() &&
                    playerHand.ViewSpaceVelocity.MostlyZ();
                    break;
                case ViewSpaceDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && 
                    playerHand.ViewSpaceVelocity.z <= -minVelocity &&
                    playerHand.SelfSpaceVelocity.MostlyY() &&
                    playerHand.ViewSpaceVelocity.MostlyZ();
                    break;
                case ViewSpaceDirection.InwardHoriz:
                        gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && 
                        playerHand.ViewSpaceVelocity.x <= -minVelocity &&
                        playerHand.SelfSpaceVelocity.MostlyY() &&
                        playerHand.ViewSpaceVelocity.MostlyX();
                    break;
                case ViewSpaceDirection.OutwardHoriz:
                        gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && 
                        playerHand.ViewSpaceVelocity.x <= -minVelocity &&
                        playerHand.SelfSpaceVelocity.MostlyY() &&
                        playerHand.ViewSpaceVelocity.MostlyX();
                    break;
            }

            return Tuple.Create($"Reverse Slash {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// A motion compared to the Global Y axis. Regardless of any rotation or hand orientation. This only works Up and Down
        /// </summary>
        /// <param name="playerHand">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against. The only ones that work for this are Up and Down</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedConditionSet SwingGlobal(PhysicsTracker playerHand, ViewSpaceDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case ViewSpaceDirection.Left:
                    throw new Exception("A SwingGlobal gesture can only be checked against Up or Down.");
                case ViewSpaceDirection.Right:
                    throw new Exception("A SwingGlobal gesture can only be checked against Up or Down.");
                case ViewSpaceDirection.Up:
                    gesture = () => Vector3.Dot(playerHand.Velocity, Vector3.up) >= minVelocity && 
                    playerHand.Velocity.MostlyY();
                    break;
                case ViewSpaceDirection.Down:
                    gesture = () => Vector3.Dot(playerHand.Velocity, Vector3.up) <= -minVelocity && 
                    playerHand.Velocity.MostlyY();
                    break;
                case ViewSpaceDirection.Forward:
                    throw new Exception("A SwingGlobal gesture can only be checked against Up or Down.");
                case ViewSpaceDirection.Back:
                    throw new Exception("A SwingGlobal gesture can only be checked against Up or Down.");
            }

            return Tuple.Create($"Swing {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// This is a gesture for just pulling your hand back in any direction, along the negative Z axis. The opposite gesture of a fistbump
        /// </summary>
        /// <param name="playerHand">The Hand that should be checked for this gesture</param>
        /// <param name="velocity">The minimum velocity required to achieve the gesture. If not set it defaults to 2 units per second.</param>
        /// <returns></returns>
        public static NamedConditionSet ReversePunchGlobal(PhysicsTracker playerHand, float? velocity = null)
        {
            float minVelocity = velocity ?? SmallGestureSpeed;

            Func<bool> gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && 
            playerHand.SelfSpaceVelocity.MostlyZ();

            return Tuple.Create($"Global Reverse Punch", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }
        /// <summary>
        /// This is a gesture for just punching forward in any direction, along the negative Z axis. A fistbump style gesture
        /// </summary>
        /// <param name="playerHand">The Hand that should be checked for this gesture</param>
        /// <param name="velocity">The minimum velocity required to achieve the gesture. If not set it defaults to 2 units per second.</param>
        /// <returns></returns>
        public static NamedConditionSet PunchGlobal(PhysicsTracker playerHand, float? velocity = null)
        {
            float minVelocity = velocity ?? SmallGestureSpeed;

            Func<bool> gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity &&
            playerHand.SelfSpaceVelocity.MostlyZ();

            return Tuple.Create($"Global Punch", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// This is a gesture for throwing your hand forward in any direction. This is only checking the SelfSpace velocity
        /// </summary>
        /// <param name="playerHand">The Hand that should be checked for this gesture</param>
        /// <param name="velocity">The minimum velocity required to achieve the gesture. If not set it defaults to 2 units per second.</param>
        /// <returns></returns>
        public static NamedConditionSet PushGlobal(PhysicsTracker playerHand, float? velocity = null)
        {
            float minVelocity = velocity ?? SmallGestureSpeed;

            Func<bool> gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && 
            playerHand.SelfSpaceVelocity.MostlyX();

            return Tuple.Create($"Global Push", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// This is a gesture for throwing your hand backward in any direction. This is only checking the SelfSpace velocity
        /// </summary>
        /// <param name="playerHand">The Hand that should be checked for this gesture</param>
        /// <param name="velocity">The minimum velocity required to achieve the gesture. If not set it defaults to 2 units per second.</param>
        /// <returns></returns>
        public static NamedConditionSet ReversePushGlobal(PhysicsTracker playerHand, float? velocity = null)
        {
            float minVelocity = velocity ?? SmallGestureSpeed;

            Func<bool> gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity &&
            playerHand.SelfSpaceVelocity.MostlyX();

            return Tuple.Create($"Global Reverse Push", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// This is a gesture for slashing your hand in any direction. This is only checking the SelfSpace velocity.
        /// </summary>
        /// <param name="playerHand">The Hand that should be checked for this gesture</param>
        /// <param name="velocity">The minimum velocity required to achieve the gesture. If not set it defaults to 2 units per second.</param>
        /// <returns></returns>
        public static NamedConditionSet SlashGlobal(PhysicsTracker playerHand, float? velocity = null)
        {
            float minVelocity = velocity ?? SmallGestureSpeed;

            Func<bool> gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity &&
            playerHand.SelfSpaceVelocity.MostlyY();

            return Tuple.Create($"Global Slash", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }
        /// <summary>
        /// This is a gesture for slashing your hand in any direction, but reversed. This is only checking the SelfSpace velocity.
        /// </summary>
        /// <param name="playerHand">The Hand that should be checked for this gesture</param>
        /// <param name="velocity">The minimum velocity required to achieve the gesture. If not set it defaults to 2 units per second.</param>
        /// <returns></returns>
        public static NamedConditionSet ReverseSlashGlobal(PhysicsTracker playerHand, float? velocity = null)
        {
            float minVelocity = velocity ?? SmallGestureSpeed;

            Func<bool> gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity &&
            playerHand.SelfSpaceVelocity.MostlyY();

            return Tuple.Create($"Global Slash", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }
        /// <summary>
        /// Checks if the player has their hands close enough together. You can pass through how close this needs to be
        /// </summary>
        /// <param name="playerHand">Which hand to check. This could technically be either one and it would return the same</param>
        /// <param name="closenessThreshold">How close, in Unity units, the hands need to be for this to be true. If not set it defaults to .3f</param>
        /// <returns></returns>
        //public static NamedConditionSet HandsCloseTogether(PhysicsTracker playerHand, float? closenessThreshold = null)
        //{
        //    float threshold = closenessThreshold ?? .3f;

        //    Func<bool> gesture = () => playerHand.DistanceToOtherHand <= threshold;

        //    return Tuple.Create($"Closeness Check", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        //}
        /// <summary>
        /// Checks if the player flicks their hand quick enough in a direction. This is entirely based off angular velocity/rotation. This is in relation to the hand itself
        /// </summary>
        /// <param name="playerHand">The hand to check</param>
        /// <param name="direction">Which direction should the player flick. The clockwise and counterclockwise mean rotating your hand around the Z axis.</param>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public static NamedConditionSet SelfSpaceFlick(PhysicsTracker playerHand, FlickDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed * 1.5f;
            Func<bool> gesture = null;

            switch (direction)
            {
                case FlickDirection.Left:
                    gesture = () => playerHand.SelfSpaceAngularVelocity.y <= -minVelocity &&
                    playerHand.SelfSpaceAngularVelocity.MostlyY();
                    break;
                case FlickDirection.Right:
                    gesture = () => playerHand.SelfSpaceAngularVelocity.y >= minVelocity &&
                    playerHand.SelfSpaceAngularVelocity.MostlyY();
                    break;
                case FlickDirection.Up:
                    gesture = () => playerHand.SelfSpaceAngularVelocity.x <= -minVelocity &&
                    playerHand.SelfSpaceAngularVelocity.MostlyX();
                    break;
                case FlickDirection.Down:
                    gesture = () => playerHand.SelfSpaceAngularVelocity.x >= minVelocity &&
                    playerHand.SelfSpaceAngularVelocity.MostlyX();
                    break;
                case FlickDirection.Clockwise:
                    gesture = () => playerHand.SelfSpaceAngularVelocity.z >= -minVelocity &&
                    playerHand.SelfSpaceAngularVelocity.MostlyZ();
                    break;
                case FlickDirection.Counterclockwise:
                    gesture = () => playerHand.SelfSpaceAngularVelocity.z <= minVelocity &&
                    playerHand.SelfSpaceAngularVelocity.MostlyZ();
                    break;
                case FlickDirection.Inward:
                        gesture = () => playerHand.SelfSpaceAngularVelocity.y <= -minVelocity &&
                        playerHand.SelfSpaceAngularVelocity.MostlyY();
                    break;
                case FlickDirection.Outward:
                        gesture = () => playerHand.SelfSpaceAngularVelocity.y >= minVelocity &&
                        playerHand.SelfSpaceAngularVelocity.MostlyY();
                    break;
            }

            return Tuple.Create($"Flick {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }
        public static NamedConditionSet ZAxisFlick(PhysicsTracker playerHand, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed * 1.5f;

            Func<bool> gesture = () => playerHand.SelfSpaceAngularVelocity.z.Abs() >= minVelocity &&
            playerHand.SelfSpaceAngularVelocity.MostlyZ();

            return Tuple.Create($"Z Axis Flick", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }
        /// <summary>
        /// Checks if the hand palm direction vector is pointing the same direction as the viewspace up vector. Within a certain tolerance value
        /// </summary>
        /// <param name="playerHand">The hand to check</param>
        /// <param name="tolerance">A value from 0 - 90, the difference between the two vectors must differ by max this much</param>
        /// <returns></returns>
        //public static NamedConditionSet PalmPointUpSelfSpace(PhysicsTracker playerHand, float? tolerance = null)
        //{
        //    float threshold = tolerance ?? 15f;

        //    Func<bool> gesture = () => Vector3.Angle(playerHand.UniveralPalm, Camera.main.transform.up) <= threshold;

        //    return Tuple.Create($"Palm Point Up", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        //}
    }
    public enum ViewSpaceDirection
    {
        Left,
        Right,
        Up,
        Down,
        Forward,
        Back,
        InwardHoriz,
        OutwardHoriz
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