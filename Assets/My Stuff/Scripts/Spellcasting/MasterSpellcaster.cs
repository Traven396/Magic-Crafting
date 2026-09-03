namespace AgeOfEnlightenment.Spellcasting
{
    using Alchemy.Inspector;
    using UnityEngine;

    /// <summary>
    /// The MasterSpellcaster is the input agnostic designator and receiver of input.
    /// It keeps track of SpellcastingSessions that are currently active so that they can carry out their entire lifespan with no issues
    /// 
    /// It only creates a Session if the SpellSequenceTracker tells it that a SpellRoute has been fully completed
    /// </summary>
    public class MasterSpellcaster : MonoBehaviour
    {
        [Title("Spellcasting Settings")]
        [SerializeField] SpellDefinitionSO _ActiveSpell;
        Transform _CastOriginPoint;
        Transform _CasterTransform;

        SpellSequenceTracker _SequenceTracker;


        private SpellcastingSession _currentSession;
        public SpellcastingSession CurrentSession => _currentSession;

        private void Awake()
        {
            if(_ActiveSpell != null)
            {
                _SequenceTracker = new SpellSequenceTracker(_ActiveSpell);
            }
        }
        private void Update()
        {
            _SequenceTracker?.Tick(Time.time);

            if (_currentSession == null) return;

            _currentSession.Tick(Time.deltaTime);

            if (_currentSession.IsComplete) _currentSession = null;
        }

        public void SetSpellDefinition(SpellDefinitionSO spellDefinition)
        {
            if (_currentSession != null) return;

            _ActiveSpell = spellDefinition;

            _SequenceTracker = new SpellSequenceTracker(_ActiveSpell);
        }

        public void SetCastOrigin(Transform castOriginPoint)
        {
            if(_currentSession != null) return;

            _CastOriginPoint = castOriginPoint;
        }

        public void SpellButton_Press(SpellButton button)
        {
            if (_SequenceTracker == null) return;

            TryStartSession(_SequenceTracker.ButtonPressed(button, Time.time));
        }

        public void SpellButton_Release(SpellButton button)
        {
            if (_SequenceTracker == null) return;

            TryStartSession(_SequenceTracker.ButtonReleased(button, Time.time));
        }

        private void TryStartSession(SpellActivationRoute route)
        {
            if (route == null) return;
            if (_currentSession != null) return;
            if (_CastOriginPoint == null) return;

            _currentSession = new SpellcastingSession(_ActiveSpell, route, this, _CastOriginPoint);
            _currentSession.Begin();
        }
    }

}