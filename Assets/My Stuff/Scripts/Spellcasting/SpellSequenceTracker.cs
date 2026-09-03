namespace AgeOfEnlightenment.Spellcasting
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Analytics;


    /// <summary>
    /// This class is responsible for tracking inputs (buttons and gestures, as well as anything in the future I add I suppose) and then detecting when a SpellRoute has had
    /// all of it's requirements met. It then notifies the MasterSpellcaster that a route has been chosen, which then creates that SpellcastingSession
    /// </summary>
    public class SpellSequenceTracker
    {
        class RouteProgress
        {
            public int NextStepIndex;
            public float StartedAt;
        }

        SpellDefinitionSO _spellDefinition;
        HashSet<SpellButton> _heldButtons = new();
        Dictionary<SpellActivationRoute, RouteProgress> _routeProgress = new Dictionary<SpellActivationRoute, RouteProgress>();

        public SpellSequenceTracker(SpellDefinitionSO spellDefinition)
        {
            _spellDefinition = spellDefinition;

            foreach (var route in _spellDefinition.ActivationRoutes)
            {
                _routeProgress.Add(route, new RouteProgress());
            }
        }

        public SpellActivationRoute ButtonPressed(SpellButton button, float currentTime)
        {
            _heldButtons.Add(button);

            return CheckForCompletedRoute(RouteEvent.ButtonPressed(button), currentTime);

        }

        public SpellActivationRoute ButtonReleased(SpellButton button, float currentTime)
        {
            _heldButtons.Remove(button);

            return CheckForCompletedRoute(RouteEvent.ButtonReleased(button), currentTime);
        }

        //We would do a similar thing here for when we recognize a gesture.


        public void Tick(float currentTime)
        {
            foreach (var route in _spellDefinition.ActivationRoutes)
            {
                if (!route.IsValid()) continue;

                RouteProgress progress = _routeProgress[route];

                if (progress.NextStepIndex == 0) continue;

                if (RouteTimedOut(route, progress, currentTime)) ResetRouteProgress(progress);
            }
        }




        SpellActivationRoute CheckForCompletedRoute(RouteEvent routeEvent, float currentTime)
        {
            foreach (var route in _spellDefinition.ActivationRoutes)
            {
                if (!route.IsValid()) continue;

                RouteProgress progress = _routeProgress[route];

                if (RouteTimedOut(route, progress, currentTime)) ResetRouteProgress(progress);

                if (RouteNextStepMatch(route, progress, routeEvent))
                {
                    if(AdvanceStep(route, progress, currentTime))
                    {
                        //If we made it into here, then we have fully completed the route, so we reset tracking any others, and return which route was chosen
                        ResetAllRoutes();
                        return route;
                    }
                }
            }

            //If we are all the way out here then none of the routes are completed yet, so null is returned

            return null;
        }

        bool RouteNextStepMatch(SpellActivationRoute route, RouteProgress progress, RouteEvent routeEvent)
        {
            //If the next step we are trying to check is longer than the actual number of steps, then its false
            if (progress.NextStepIndex >= route.ActivationSteps.Count) return false;

            RouteStep nextStep = route.ActivationSteps[progress.NextStepIndex];
            
            return nextStep.StepMatch(routeEvent, _heldButtons);
        }

        bool AdvanceStep(SpellActivationRoute route, RouteProgress progress, float currentTime)
        {
            //If the step index is 0, then we are completing the first step of this route so we need to save it's time
            if (progress.NextStepIndex == 0) progress.StartedAt = currentTime;

            progress.NextStepIndex++;

            //If we have reached the end of the step count, then this route is fully completed and we will return it
            return progress.NextStepIndex >= route.ActivationSteps.Count;
        }


        //Compare the current time to when we started tracking the route. If the difference is too much for that route then it has timed out and should stop being tracked.
        bool RouteTimedOut(SpellActivationRoute route, RouteProgress progress, float currentTime)
        {
            if (progress.NextStepIndex == 0) return false;
            if (route.MaximumSequenceSeconds <= 0) return false;

            return currentTime - progress.StartedAt > route.MaximumSequenceSeconds;
        }

        void ResetRouteProgress(RouteProgress progress)
        {
            progress.NextStepIndex = 0;
            progress.StartedAt = 0;
        }
        void ResetAllRoutes()
        {
            foreach (RouteProgress progress in _routeProgress.Values)
            {
                ResetRouteProgress(progress);
            }
        }
    }
}