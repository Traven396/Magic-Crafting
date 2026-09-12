namespace AgeOfEnlightenment.Spellcasting
{
    using Alchemy.Inspector;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The MasterSpellcaster is the input agnostic designator and receiver of input.
    /// It keeps track of SpellcastingSessions that are currently active so that they can carry out their entire lifespan with no issues
    /// 
    /// It only creates a Session if the SpellSequenceTracker tells it that a SpellRoute has been fully completed
    /// </summary>
    public class MasterSpellcaster : MonoBehaviour
    {
        [Header("Spellcasting")]
        [SerializeField] private SpellDefinitionSO _activeSpell;
        [SerializeField] private WandGestureInput _gestureInput;

        [SerializeField] Transform _castOriginPoint;
        //This is temporary. In the future this will be a parent class of anything gesture.
        [SerializeField] WandGestureInput GestureInput;
        private SpellSequenceTracker _sequenceTracker;
        private SpellcastingSession _latestSession;

        List<SpellcastingSession> _activeSessions = new();
        List<SpellcastingSession> _completedSessions = new();


        private void Awake()
        {
            RebuildSequenceTracker();
        }

        private void OnEnable()
        {
            if (_gestureInput != null) _gestureInput.GestureRecognized += ReceiveGesture;
        }

        private void OnDisable()
        {
            if (_gestureInput != null) _gestureInput.GestureRecognized -= ReceiveGesture;
        }

        private void Update()
        {
            //if (_currentSession == null) _sequenceTracker?.Tick(Time.time);

            _sequenceTracker?.Tick(Time.time);

            if(_activeSessions.Count > 0)
            {
                _activeSessions.ForEach(session =>
                {
                    session.Tick(Time.deltaTime);

                    //If a session has completed it's task then we add it to a list so we can remove them all at once
                    if (session.IsComplete)
                        _completedSessions.Add(session);
                });

                //We remove from our tracked sessions the one's that have completed already
                _completedSessions.ForEach(complete => _activeSessions.Remove(complete));
                _completedSessions.Clear();
            }

            //if (_currentSession == null) return;

            //_currentSession.Tick(Time.deltaTime);

            //if (_currentSession.IsComplete) _currentSession = null;
        }

        public void SetSpellDefinition(SpellDefinitionSO spellDefinition)
        {
            if (_latestSession != null) return;
            if (_sequenceTracker != null && _sequenceTracker.HasActiveAttempt) return;

            _activeSpell = spellDefinition;

            RebuildSequenceTracker();
        }

        public void SetCastOrigin(Transform castOriginPoint)
        {
            if (_latestSession != null) return;
            if (_sequenceTracker != null && _sequenceTracker.HasActiveAttempt) return;

            _castOriginPoint = castOriginPoint;
        }

        public void SpellButton_Press(SpellButton button)
        {
            //if (_currentSession != null) return;

            _sequenceTracker?.ButtonPressed(button, Time.time);
        }

        public void SpellButton_Release(SpellButton button)
        {
            //if (_currentSession != null) return;

            _sequenceTracker?.ButtonReleased(button, Time.time);
        }

        public void ReceiveGesture(GestureSpec gesture)
        {
            //if (_currentSession != null) return;

            _sequenceTracker?.GestureRecognized(gesture, Time.time);
        }

        private void RebuildSequenceTracker()
        {
            if (_activeSpell == null)
            {
                _sequenceTracker = null;

                return;
            }

            _sequenceTracker = new SpellSequenceTracker(_activeSpell, _castOriginPoint, GestureInput.GetTracker());
            _sequenceTracker.FinalStepReached += BeginCastingSession;

            if (_gestureInput != null) _gestureInput.SetGestureSpecifications(_activeSpell.GetGestureSpecifications());
        }

        private void BeginCastingSession(SpellActivationAttempt attempt, RouteStep outcome)
        {

            if (_castOriginPoint == null)
            {
                Debug.LogError($"Spell {_activeSpell.name} tried to cast without a cast origin.");

                return;
            }

            _latestSession = new SpellcastingSession(_activeSpell, outcome, this, attempt.ActionContext);
            _latestSession.Begin();

            _activeSessions.Add(_latestSession);
        }

        public void CancelAllCasting()
        {
            _sequenceTracker.CancelCurrentAttempt();
        }
    }

}