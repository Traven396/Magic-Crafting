namespace AgeOfEnlightenment.Spellcasting
{
    using Alchemy.Inspector;
    using FoxheadDev.GestureDetection;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using UnityEngine;
    using static UnityEditor.Searcher.Searcher.AnalyticsEvent;
    using static UnityEngine.InputManagerEntry;


    public enum SpellButton { Primary, Secondary, Tertiary }
    public enum RouteEventType
    {
        ButtonPressed,
        ButtonReleased,
        GestureRecognized,
        ChargeTimeReached
    }

    public enum RouteActionType
    {
        None,
        Custom,
        SpawnObjectAtCastOrigin,
        DestroyObjects,
        PlayAudio,
        LaunchProjectile
    }
    public enum SpellShootDirection { Left, Right, Up, Down, Forward, Back, PlayerDirection, PlayerDirectionWithCastVelocity }
    //This is a way for in the inspector for me to designate what kind of gesture I want to have the player perform.
    //Maybe in the future I can make this more beginner friendly? But for now I know what the different spaces are and everything
    [Serializable]
    public class GestureSpec
    {
        [Tooltip("Slash means local-Y (Thumb)" + "\n" + "Punch means local-Z (Fingers)" + "\n" + "Push means local-X (Palm: Right = +)")]
        [SerializeField] GestureType _gestureType;
        bool isFlick => _gestureType == GestureType.Flick;
        [SerializeField] GestureDirection _direction;
        [HideIf("isFlick")] [SerializeField] GestureVelocitySpace _velocitySpace;

        [SerializeField] bool _overrideMinimumSpeed;

        [ShowIf("_overrideMinimumSpeed")]
        [SerializeField] float _minimumSpeed;


        public GestureType Type => _gestureType;
        public GestureDirection Direction => _direction;
        public GestureVelocitySpace VelocitySpace => _velocitySpace;
        public bool OverrideSpeed => _overrideMinimumSpeed;
        public float MinimumSpeed => _minimumSpeed;

        public bool GestureMatches(GestureSpec other)
        {
            if (other == null) return false;
            if (_gestureType != other._gestureType) return false;
            //The flick gesture only needs to read in the Self space, so if we are setting it to flick we dont need to check the velocity space
            if (_gestureType != GestureType.Flick && _velocitySpace != other._velocitySpace) return false;
            if (_direction != other._direction) return false;
            if (_overrideMinimumSpeed != other._overrideMinimumSpeed) return false;
            if (_overrideMinimumSpeed && !Mathf.Approximately(_minimumSpeed, other._minimumSpeed)) return false;

            return true;
        }

        public bool IsValid()
        {
            if (_overrideMinimumSpeed && _minimumSpeed <= 0f) return false;

            return true;
        }

        public string GetKey()
        {
            if (_gestureType != GestureType.Flick)
                return $"{_gestureType}|{_velocitySpace}|{_direction}|{_overrideMinimumSpeed}|{_minimumSpeed}";
            else
                return $"{_gestureType}|{_direction}|{_overrideMinimumSpeed}|{_minimumSpeed}";
        }
    }


    // A way to have a universal way of firing manual events to the spell.
    public struct RouteEvent
    {
        public RouteEventType Type { get; }
        public SpellButton Button { get; }
        public GestureSpec Gesture { get; }

        private RouteEvent(RouteEventType type, SpellButton button, GestureSpec gesture)
        {
            Type = type;
            Button = button;
            Gesture = gesture;
        }

        public static RouteEvent ButtonPressed(SpellButton button) => new RouteEvent(RouteEventType.ButtonPressed, button, null);
        public static RouteEvent ButtonReleased(SpellButton button) => new RouteEvent(RouteEventType.ButtonReleased, button, null);
        public static RouteEvent GestureRecognized(GestureSpec gesture) => new RouteEvent(RouteEventType.GestureRecognized, default, gesture);
        public static RouteEvent ChargeReached() => new RouteEvent(RouteEventType.ChargeTimeReached, default, null);
    }


    #region Targeting

    public enum TargetMethod
    {
        Raycast,
        SphereCast,
        CapsuleCast,
        BoxCast,
        OverlapSphere,
        OverlapBox,
        OverlapCapsule
    }
    public enum TargetSelection
    {
        Closest,
        Furthest,
        All
    }
    public enum TargetSource
    {
        Caster,
        CastOrigin,
        PlayerView
    }

    //At some point I will add target filters so that things can be more complex. Like "non-player" and it would be all others
    //But for now we keep it simple

    [Serializable]
    public struct TargetingSettings
    {
        public EntityType EntityFilter;
        public TargetMethod Method;
        public TargetSource Source;
        public SpellShootDirection Direction;
        public TargetSelection Selection;
        public LayerMask IgnoredLayers;
        

        bool sizedType => Method != TargetMethod.Raycast;
        bool distanceType => Method != TargetMethod.OverlapSphere || Method != TargetMethod.OverlapCapsule || Method != TargetMethod.OverlapBox;
        [Space(10)]
        [ShowIf("distanceType")] public float CastDistance;
        [ShowIf("sizedType")] public float CastSize;

    } 
    #endregion

    [Serializable]
    public class RouteAction
    {

        //These are the actions that can happen when a route is reached or finished.
        //They allow for progressive changes to the spell as the player "moves" through it
        [HideInInspector] public string name = "";
        
        [OnValueChanged("ChangeNameStringForOrganization")]
        [SerializeField] RouteActionType _type;


        bool gameObjectType => _type == RouteActionType.SpawnObjectAtCastOrigin || _type == RouteActionType.DestroyObjects;
        bool taggedItemType => _type == RouteActionType.SpawnObjectAtCastOrigin || _type == RouteActionType.LaunchProjectile || _type == RouteActionType.DestroyObjects;
        bool projectileType => _type == RouteActionType.LaunchProjectile;
        bool audioType => _type == RouteActionType.PlayAudio;
        
        // ^ all of these are variables I need to designate so I can only have parts of the settings appear that are necessary to what the Action is doing. It is merely to just make it look nice
        //very tedious to set up though ngl
        
        
        [Header("Runtime Object Settings")]
        //This is so that if I want certain parts of a spell to spawn more objects or anything, I have a way to have them affect the other's without passing an actual reference
        //An action can spawn something tagged as "Target" and then if other steps need to change or destroy it they just need to ask for "Target"
        [ShowIf("taggedItemType")] [SerializeField] string _runtimeObjectKey = "Preview";
        [ShowIf("gameObjectType")] [AssetsOnly] [SerializeField] GameObject _objectPrefab;
        [ShowIf("gameObjectType")] [SerializeField] bool _childOfSpawnpoint;
        [ShowIf("gameObjectType")][SerializeField] float _spawnSize = 1; 
        [ShowIf("gameObjectType")][SerializeField] int _spawnLayer;



        [Header("Projectile Settings")]
        [ShowIf("projectileType")] [SerializeField] SpellShootDirection _shootDirection;
        [ShowIf("projectileType")] [SerializeField] float _projectileSpeed = 5f;
        [ShowIf("projectileType")] [SerializeField] float _projectileLifetime = 10f;
        [ShowIf("projectileType")] [SerializeField] bool _targetedProjectile;
        [ShowIf("_targetedProjectile")] [SerializeField] TargetingSettings _targetSettings;

        [ListViewSettings(ShowFoldoutHeader = false, ShowBoundCollectionSize = false)]
        [Header("Projectile Modifiers")]
        [ShowIf("projectileType")][SerializeReference] List<SpellProjectileBehaviourModifier> _projectileModifiers = new();

        [ListViewSettings (ShowFoldoutHeader = false, ShowBoundCollectionSize = false)]
        [Space(5f)]
        [Header("Projectile Callbacks")]
        [ShowIf("projectileType")] [SerializeReference] List<SpellProjectileCallbackAction> _projectileCallbacks = new();

        [Header("Audio Settings")]
        [ShowIf("audioType")] [SerializeField] AudioClip _audioClip;
        [ShowIf("audioType")] [SerializeField] float _volume;


        
        public RouteActionType Type => _type;
        public string RuntimeObjectKey => _runtimeObjectKey;
        public bool ChildOfSpawnpoint => _childOfSpawnpoint;
        public float SpawnSize => _spawnSize;
        public int SpawnLayer => _spawnLayer;

        public GameObject Prefab => _objectPrefab;
        public SpellShootDirection ShootDirection => _shootDirection;
        public float ProjectileSpeed => _projectileSpeed;
        public float ProjectileLifetime => _projectileLifetime;
        public bool TargetedProjectile => _targetedProjectile;
        public TargetingSettings TargetSettings => _targetSettings;


        public List<SpellProjectileBehaviourModifier> ProjectileModifiers => _projectileModifiers;
        public List<SpellProjectileCallbackAction> ProjectileCallbacks => _projectileCallbacks;


        public AudioClip AudioClip => _audioClip;
        public float Volume => _volume;


        public bool IsValid()
        {
            if (_type == RouteActionType.SpawnObjectAtCastOrigin && _objectPrefab == null) return false;
            if (_type == RouteActionType.LaunchProjectile && _projectileSpeed <= 0f) return false;
            if (_type == RouteActionType.LaunchProjectile && _projectileLifetime <= 0f) return false;

            return true;
        }
        //This method is called whenever the Type variable is changed, and it sets the name variable to what we want it to be so that it shows in the list
        void ChangeNameStringForOrganization(RouteActionType _newType)
        {
            name = _newType.ToString();
        }
    }
    
    [Serializable]
	public class RouteStep
	{
        //A route step is a new 'step' in the spell. Obviously
        //Basically every unique action the player must do when casting the spell.

        //For example
        // Press Trigger -> Punch wand forward -> Slash wand down -> Release trigger

        //All of the above would be unique RouteStep, and for each we can specify those triggers in here.
        //ALSO we can designate actions that happen when the RouteStep starts and when it finishes, that way we can play audio or spawn necessary objects as needed for the spells
        [Header("Identity")]
        [SerializeField] string _name;
        [SerializeField] float _cooldown;

        [Header("Entry Triggers")]
        [SerializeField] private RouteEventType _eventType;

        bool buttonEventType => _eventType == RouteEventType.ButtonPressed || _eventType == RouteEventType.ButtonReleased;
        [SerializeField] [ShowIf("buttonEventType")] SpellButton _button;

        bool gestureEventType => _eventType == RouteEventType.GestureRecognized;
        [SerializeField] [ShowIf("gestureEventType")] GestureSpec _gesture;

        bool chargeEventType => _eventType == RouteEventType.ChargeTimeReached;
        [SerializeField][ShowIf("chargeEventType")] float _requiredCharge;

        [ListViewSettings(ShowBoundCollectionSize = false)]
        [SerializeField] List<SpellButton> RequiredButtonsToBeHeld;
        [Tooltip("Optionally we can have some routes have a time limit on them. If this is set to 0 the route can last forever though")]
        [SerializeField] float _timeoutSeconds;


        [Header("Actions")]
        [ListViewSettings(ShowBoundCollectionSize = false)]
        [SerializeField] List<RouteAction> _onStartedActions;

        [ListViewSettings(ShowBoundCollectionSize = false)]
        [SerializeField] List<RouteAction> _onCompletionActions;




        [Header("Branching Paths")]
        [SerializeField] bool _finalStep;
        [ListViewSettings (ShowBoundCollectionSize = false)]
        [HideIf("_finalStep")] [SerializeField] List<RouteStep> _nextSteps;








        public string Name => _name;
        public float Cooldown => _cooldown;
        public RouteEventType EventType => _eventType;
        public GestureSpec Gesture => _gesture;
        public float RequiredChargeSeconds => _requiredCharge;
        public float TimeoutSeconds => _timeoutSeconds;
        public bool FinalStep => _finalStep;
        public IReadOnlyList<RouteAction> OnStepStartActions => _onStartedActions;
        public IReadOnlyList<RouteAction> OnStepCompletedActions => _onCompletionActions;
        public IReadOnlyList<RouteStep> NextSteps => _nextSteps;


        public bool StepMatch(RouteEvent routeEvent, HashSet<SpellButton> heldButtons)
        {
            if (routeEvent.Type != _eventType) return false;
            if (_eventType == RouteEventType.ButtonPressed && routeEvent.Button != _button) return false;
            if (_eventType == RouteEventType.ButtonReleased && routeEvent.Button != _button) return false;
            if (_eventType == RouteEventType.GestureRecognized && (_gesture == null || routeEvent.Gesture == null || !_gesture.GestureMatches(routeEvent.Gesture))) return false;
            if (!RequiredButtonsAreHeld(heldButtons)) return false;

            return true;
        }
        public bool RequiredButtonsAreHeld(HashSet<SpellButton> heldButtons)
        {
            foreach (SpellButton requiredButton in RequiredButtonsToBeHeld)
            {
                if (!heldButtons.Contains(requiredButton)) return false;
            }

            return true;
        }

        public bool IsValid()
        {
            if (_eventType == RouteEventType.GestureRecognized && (_gesture == null || !_gesture.IsValid())) return false;
            if (_eventType == RouteEventType.ChargeTimeReached && _requiredCharge < 0f) return false;
            if (_finalStep && _nextSteps.Count > 0) return false;
            if (!_finalStep && _nextSteps.Count == 0) return false;

            foreach (RouteAction action in _onCompletionActions)
            {
                if (action == null || !action.IsValid()) return false;
            }

            foreach (RouteAction action in _onStartedActions)
            {
                if (action == null || !action.IsValid()) return false;
            }

            foreach (RouteStep nextStep in _nextSteps)
            {
                if (nextStep == null || !nextStep.IsValid()) return false;
            }

            return true;
        }


        public void GatherPossibleGestures(List<GestureSpec> output)
        {
            if (_eventType == RouteEventType.GestureRecognized && _gesture != null) output.Add(_gesture);

            foreach (RouteStep nextStep in _nextSteps)
            {
                if (nextStep != null) nextStep.GatherPossibleGestures(output);
            }
        }

    }

}