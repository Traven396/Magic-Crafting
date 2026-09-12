namespace AgeOfEnlightenment.Spellcasting
{
    using FoxheadDev.GestureDetection;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Analytics;


    /// <summary>
    /// This class is responsible for tracking inputs (buttons and gestures, as well as anything in the future I add I suppose) and then detecting when a SpellRoute has had
    /// all of it's requirements met. It then notifies the MasterSpellcaster that a route has been chosen, which then creates that SpellcastingSession
    /// </summary>
    public class SpellSequenceTracker
    {
        SpellDefinitionSO _spellDefinition;
        Transform _castOrigin;
        PhysicsTracker _casterPhysicsTracker;

        HashSet<SpellButton> _heldButtons = new();

        float currentStepCooldown;
        SpellActivationAttempt _currentAttempt;
        public bool HasActiveAttempt => _currentAttempt != null;

        public event Action<SpellActivationAttempt, RouteStep> StepEntered;
        public event Action<SpellActivationAttempt, RouteStep> FinalStepReached;
        public event Action<SpellActivationAttempt> AttemptCancelled;

        public SpellSequenceTracker(SpellDefinitionSO spellDefinition, Transform castOrigin, PhysicsTracker casterTracker)
        {
            _spellDefinition = spellDefinition;
            _castOrigin = castOrigin;
            _casterPhysicsTracker = casterTracker;
        }

        public void ButtonPressed(SpellButton button, float currentTime)
        {
            _heldButtons.Add(button);

            if (currentStepCooldown <= 0)
                ProcessEvent(RouteEvent.ButtonPressed(button), currentTime);
        }

        public void ButtonReleased(SpellButton button, float currentTime)
        {
            _heldButtons.Remove(button);

            if (currentStepCooldown <= 0)
                ProcessEvent(RouteEvent.ButtonReleased(button), currentTime);
        }

        public void GestureRecognized(GestureSpec gesture, float currentTime)
        {
            if(currentStepCooldown <= 0)
                ProcessEvent(RouteEvent.GestureRecognized(gesture), currentTime);
        }

        private void ProcessEvent(RouteEvent routeEvent, float currentTime)
        {
            if (_currentAttempt == null)
            {
                TryStartAttempt(routeEvent, currentTime);
                return;
            }

            RouteStep matchingChild = FindMatchingChild(_currentAttempt.CurrentStep, routeEvent);

            if (matchingChild != null)
            {
                AdvanceToStep(matchingChild, currentTime);
                return;
            }

            if (!_currentAttempt.CurrentStep.RequiredButtonsAreHeld(_heldButtons)) CancelCurrentAttempt();
        }

        public void Tick(float currentTime)
        {
            if(currentStepCooldown > 0)
                currentStepCooldown -= Time.deltaTime;

            if (_currentAttempt == null) return;
            if (_currentAttempt.HasTimedOut(currentTime))
            {
                CancelCurrentAttempt();
                return;
            }

            if (!_currentAttempt.CurrentStep.RequiredButtonsAreHeld(_heldButtons))
            {
                CancelCurrentAttempt();
                return;
            }

            TryAdvanceChargeStep(currentTime);
        }

        private void TryStartAttempt(RouteEvent routeEvent, float currentTime)
        {
            SpellActivationRoute matchingRoute = null;

            foreach (SpellActivationRoute route in _spellDefinition.ActivationRoutes)
            {
                if (route == null || !route.IsValid()) continue;
                
                if (!route.FirstStep.StepMatch(routeEvent, _heldButtons)) continue;
                
                if (matchingRoute != null)
                {
                    Debug.LogError($"Spell {_spellDefinition.name} has overlapping root route conditions.");
                    return;
                }

                matchingRoute = route;
            }

            if (matchingRoute == null) return;


            
            _currentAttempt = new SpellActivationAttempt(matchingRoute, _castOrigin, _casterPhysicsTracker);

            AdvanceToStep(matchingRoute.FirstStep, currentTime);
        }


        //We cycle through all of the current possible next steps and see if we reached the charge time for one of them.
        private void TryAdvanceChargeStep(float currentTime)
        {
            RouteStep chargeStep = null;

            foreach (RouteStep child in _currentAttempt.CurrentStep.NextSteps)
            {
                if (child.EventType != RouteEventType.ChargeTimeReached) continue;
                if (!child.RequiredButtonsAreHeld(_heldButtons)) continue;
                if (currentTime - _currentAttempt.TimeCurrentStepEnteredAt < child.RequiredChargeSeconds) continue;

                if (chargeStep != null)
                {
                    Debug.LogError($"Route {_currentAttempt.CurrentStep.Name} has overlapping charge branches.");
                    return;
                }

                chargeStep = child;
            }

            if (chargeStep != null) AdvanceToStep(chargeStep, currentTime);
        }


        //We look through the possible children steps and find one that has a valid condition based on what event we just fired
        private RouteStep FindMatchingChild(RouteStep parentStep, RouteEvent routeEvent)
        {
            RouteStep matchingChild = null;

            foreach (RouteStep child in parentStep.NextSteps)
            {
                if (!child.StepMatch(routeEvent, _heldButtons)) continue;

                if (matchingChild != null)
                {
                    Debug.LogError($"Route step {parentStep.Name} has overlapping child conditions.");
                    return null;
                }

                matchingChild = child;
            }

            return matchingChild;
        }

        //This method is the one that moves to the next step in the tree.
        private void AdvanceToStep(RouteStep step, float currentTime)
        {
            currentStepCooldown = step.Cooldown;

            if (step.FinalStep)
            {
                _currentAttempt.EnterFinalStep(step, currentTime);
                FinalStepReached?.Invoke(_currentAttempt, step);

                _currentAttempt = null;

                return;
            }

            _currentAttempt.EnterNonTerminalStep(step, currentTime);

            StepEntered?.Invoke(_currentAttempt, step);
        }


        public void CancelCurrentAttempt()
        {
            if (_currentAttempt == null) return;



            _currentAttempt.Cleanup();

            AttemptCancelled?.Invoke(_currentAttempt);

            _currentAttempt = null;
        }
    }

    /// <summary>
    /// This class is what we use to track the progression through the different steps before we actually reach a final step and cast something
    /// 
    /// It is basically just a fancy way of remembering numbers for where we are in the tree
    /// </summary>
    public class SpellActivationAttempt
    {
        public SpellActivationRoute Route { get; }
        public RouteStep CurrentStep { get; private set; }
        public float TimeCurrentStepEnteredAt { get; private set; }
        public SpellActionContext ActionContext { get; }

        public SpellActivationAttempt(SpellActivationRoute route, Transform castOrigin, PhysicsTracker tracker)
        {
            Route = route;
            ActionContext = new SpellActionContext(castOrigin, tracker);

            
        }

        public void EnterNonTerminalStep(RouteStep step, float currentTime)
        {
            //Now this might seem backwards, but what we are doing is finishing the OLD step, and then starting the new one we are moving to.
            SpellRouteActionExecutor.ExecuteActions(step.OnStepCompletedActions, ActionContext);

            CurrentStep = step;
            TimeCurrentStepEnteredAt = currentTime;

            SpellRouteActionExecutor.ExecuteActions(step.OnStepStartActions, ActionContext);
        }
        public void Cleanup()
        {
            ActionContext.DestroyAllRuntimeObjects();
        }
        public void EnterFinalStep(RouteStep step, float currentTime)
        {
            
            CurrentStep = step;
            TimeCurrentStepEnteredAt = currentTime;

            SpellRouteActionExecutor.ExecuteActions(step.OnStepStartActions, ActionContext);
        }

        public bool HasTimedOut(float currentTime)
        {
            if (CurrentStep == null) return false;
            if (CurrentStep.TimeoutSeconds <= 0f) return false;

            return currentTime - TimeCurrentStepEnteredAt > CurrentStep.TimeoutSeconds;
        }

        public void Cancel()
        {
            ActionContext.DestroyAllRuntimeObjects();
        }
    }

}