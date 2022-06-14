using System.Collections;
using System.Collections.Generic;
using IndieGabo.CharacterController2D.Checkers2D;
using UnityEngine;
using UnityEngine.Events;
using IndieGabo.NaughtyAttributes;
using IndieGabo.CharacterController2D.Data;

namespace IndieGabo.CharacterController2D.Abilities2D
{
    [AddComponentMenu("IndieGabo/Character Controller 2D/Abilities 2D/DynamicDash2D")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class DynamicDash2D : DynamicMovementPerformer2D<DynamicDash2DSetup>, IDynamicDash2DPerformer
    {

        #region Editor

        [Header("Perform Approach")]
        [InfoBox("If you uncheck this it means you will have to call the Perform() method inside the Physics Update of any component you create to handle this one.")]
        [Tooltip("In case you want to handle when and how the Perform() method is called, you should turn this off")]
        [SerializeField]
        [Space]
        protected bool autoPerform = true;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IGroundingProvider you can mark this and it will subscribe to its events. GroundingChecker2D, for example, implements it.")]
        [SerializeField]
        protected bool seekGroundingProvider = false;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an ISlopeDataProvider you can mark this and it will subscribe to its events. SlopeChecker2D, for example, implements it.")]
        [SerializeField]
        protected bool seekSlopeDataProvider = false;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IDashHandler you can mark this and it will subscribe to its events. PCActions, for example, implements it.")]
        [SerializeField]
        protected bool seekDashHandler = false;

        [Foldout("Available Events")]
        [Label("Dash Performed")]
        [SerializeField]
        [Space]
        protected UnityEvent dashPerformed;

        #endregion

        #region Updaters

        protected IGroundingProvider groundingProvider;
        protected ISlopeDataProvider slopeDataProvider;
        protected IDynamicDash2DHandler dashHandler;

        #endregion

        #region Properties

        public bool dashing { get; protected set; } = false;
        protected bool grounded = false;
        protected SlopeData slopeData;
        protected float dashStartedAt;
        protected bool dashLocked = false;
        protected float currentDashTimer = 0;
        protected float currentDirectionSign = 0;

        #endregion

        #region Getters

        protected bool CanStartDashing => GroundingIsOk && !dashLocked && Time.fixedTime > dashStartedAt + setup.Duration + setup.Delay;
        protected bool GroundingIsOk => setup.MustBeGrounded ? grounded : true;

        // Events
        public UnityEvent DashPerformed => dashPerformed;

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();
        }

        protected virtual void Start()
        {
            SubscribeSeekers();
        }

        protected virtual void FixedUpdate()
        {
            if (!autoPerform || !dashing) return;

            if (slopeData != null && slopeData.onSlope)
            {
                Perform(slopeData);
            }
            else
            {
                Perform();
            }
        }
        protected virtual void OnEnable()
        {
            SubscribeSeekers();
        }

        protected virtual void OnDisable()
        {
            UnsubscribeSeekers();
        }

        #endregion

        #region  Logic

        /// <summary>
        /// Starts the jump process so Ascend can be called each physics frame
        /// </summary>
        protected void StartDash()
        {
            dashing = true;
            DashPerformed.Invoke();
        }

        /// <summary>
        /// Starts the jump process so Ascend can be called each physics frame
        /// </summary>
        public void SetUpDash(float directionSign)
        {
            currentDirectionSign = directionSign;
            currentDashTimer = 0;
            dashStartedAt = Time.fixedTime;
            rb.velocity = Vector2.zero;
        }

        /// <summary>
        /// Should be called on Fixed (Physics) Update.
        /// </summary>
        public void Perform()
        {
            if (currentDashTimer > setup.Duration) { Stop(); return; }
            ApplyHorizontalVelocityWithGravity(setup.XSpeed, currentDirectionSign, setup.GravityScale);
            ApplyVerticalVelocity(setup.YSpeed);
            currentDashTimer += Time.fixedDeltaTime;
        }

        public void Perform(SlopeData slopeData)
        {
            if (currentDashTimer > setup.Duration) { Stop(); return; }
            ApplyHorizontalVelocity(setup.XSpeed, currentDirectionSign, slopeData);
            currentDashTimer += Time.fixedDeltaTime;
        }

        /// <summary>
        /// Stops jump in progress if any.
        /// </summary>
        public void Stop()
        {
            dashing = false;
            ApplyGravityScale(defaultGravityScale);
        }


        #endregion

        #region Callbacks

        /// <summary>
        /// Call this to request a Jump
        /// </summary>
        public void Request()
        {
            if (!CanStartDashing) return;
            StartDash();
        }

        /// <summary>
        /// Call this in order to Lock jump and
        /// prevent new jumps to occur based on
        /// shouldLock boolean.
        /// </summary>
        /// <param name="shouldLock"></param>
        public void LockDash(bool shouldLock)
        {
            dashLocked = shouldLock;
        }

        public void UpdateGronding(bool newGrounding)
        {
            grounded = newGrounding;
        }

        public void UpdateSlopeData(SlopeData newSlopeData)
        {
            slopeData = newSlopeData;
        }

        #endregion

        #region Update Seeking

        /// <summary>
        /// Subscribes to events based on components wich implements
        /// the correct interfaces
        /// </summary>
        protected virtual void SubscribeSeekers()
        {
            UnsubscribeSeekers();

            if (seekGroundingProvider)
            {
                groundingProvider = GetComponent<IGroundingProvider>();
                if (groundingProvider == null)
                    CC2DLog.Warning("Component DynamicDash2D might not work properly. It is marked to seek for an IGroundingProvider but it could not find any.");
                groundingProvider?.GroundingUpdate.AddListener(UpdateGronding);
            }

            if (seekGroundingProvider)
            {
                slopeDataProvider = GetComponent<ISlopeDataProvider>();
                if (slopeDataProvider == null)
                    CC2DLog.Warning("Component DynamicDash2D might not work properly. It is marked to seek for an ISlopeDataProvider but it could not find any.");
                slopeDataProvider?.SlopeDataUpdate.AddListener(UpdateSlopeData);
            }

            if (seekDashHandler)
            {
                dashHandler = GetComponent<IDynamicDash2DHandler>();
                if (dashHandler == null)
                    CC2DLog.Warning("Component DynamicDash2D might not work properly. It is marked to seek for an IDashHandler but it could not find any.");
                dashHandler?.SendDashRequest.AddListener(Request);
            }
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        protected virtual void UnsubscribeSeekers()
        {
            groundingProvider?.GroundingUpdate.RemoveListener(UpdateGronding);
            slopeDataProvider?.SlopeDataUpdate.RemoveListener(UpdateSlopeData);
            dashHandler?.SendDashRequest.RemoveListener(Request);
        }

        #endregion

        #region IGComponent
#pragma warning disable 0414

        [Header("About this component"), Foldout("About this component")]
        [ReadOnly, Label("Name"), SerializeField, Space]
        public string componentName = "IndieGabo's  Dash 2D";

        [ReadOnly, Label("Info"), TextArea(1, 30), SerializeField, Space, Foldout("About this component")]
        public string info = "This component gives a GameObject the ability to dash as long as anything with its reference requests it.";

        [field: SerializeField, ReadOnly, Label("Feed Requirements"), TextArea(1, 30), Space, InfoBox("You MUST feed these functions for this component to work.", EInfoBoxType.Warning), Foldout("About this component")]
        public string requirements = "On FixedUpdate: \n"
                                        + "UpdateGronding(bool newGrounding) \n\n"
                                        + "On Demand:"
                                        + "DashRequested() \n"
                                        + "StopDash()";

        public string DocsUrl => "https://docs.com";

        [Button, Tooltip("Opens this component's documentation webpage")]
        public virtual void OpenDocs()
        {
            Application.OpenURL(DocsUrl);
        }

#pragma warning restore 0414
        #endregion
    }
}
