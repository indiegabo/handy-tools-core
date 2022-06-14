using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using IndieGabo.NaughtyAttributes;
using IndieGabo.CharacterController2D.Data;
using IndieGabo.CharacterController2D.Checkers2D;

namespace IndieGabo.CharacterController2D.Abilities2D
{
    [AddComponentMenu("IndieGabo/Character Controller 2D/Abilities 2D/DynamicJump2D")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class DynamicJump2D : DynamicMovementPerformer2D<DynamicJump2DSetup>, IDynamicJump2DPerformer, IDynamicJump2DExtraPerformer
    {
        #region Editor

        [Header("Debug")]
        [Tooltip("Turn this on and get some visual feedback. Do not forget to turn your Gizmos On")]
        [SerializeField]
        protected bool debugOn = false;

        [ShowIf("debugOn")]
        [ReadOnly]
        [SerializeField]
        protected int extraJumpsLeft;

        [Header("Perform Approach")]
        [InfoBox("If you uncheck this it means you will have to call the Perform() method inside the Physics Update of any component you create to handle this one.")]
        [Tooltip("In case you want to handle when and how the Perform() method is called, you should turn this off")]
        [SerializeField]
        [Space]
        protected bool autoPerform = true;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IGroundingProvider you can mark this and it will subscribe to its events. GroundingChecker2D, for example, implements it.")]
        [SerializeField] protected bool seekGroundingProvider = false;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IWallHitDataProvider you can mark this and it will subscribe to its events. WallHitChecker2D, for example, implements it.")]
        [SerializeField] protected bool seekWallHitDataProvider = false;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IMovementDirectionUpdater you can mark this and it will subscribe to its events. PCActions, for example, implements it.")]
        [SerializeField] protected bool seekMovementDirectionProvider = false;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IJumpHandler you can mark this and it will subscribe to its events. PCActions, for example, implements it.")]
        [SerializeField] protected bool seekJumpHandler = false;


        [Foldout("Available Events")]
        [Label("Jump Performed")]
        [Space]
        [SerializeField]
        protected UnityEvent jumpPerformed;

        [Foldout("Available Events")]
        [Label("Extra Jump Performed")]
        [SerializeField]
        protected UnityEvent extraJumpPerformed;

        #endregion

        #region Updaters

        protected IGroundingProvider groundingProvider;
        protected IWallHitDataProvider wallHitDataProvider;
        protected IMovementDirectionProvider movementDirectionProvider;
        protected IDynamicJump2DHandler jumpHandler;

        #endregion

        #region Fields

        #endregion

        #region Properties

        public bool jumping { get; protected set; } = false; // performing jump or not?
        public bool extraJumping { get; protected set; } = false; // performing extra jump or not?
        protected bool grounded = false; // is character grounded?
        protected WallHitData wallHitData; // Data about the wall hit
        protected Vector2 movementDirection = Vector2.zero; // Direction of movement
        protected float jumpRequestedAt; // time when jump was requested
        protected float jumpStartedAt; // err... come on... this one you know what is for
        protected float canResetAt; // When jump count can be reset
        protected float resetRequestedAt; // last time something requested a reset on extra jumps.
        protected bool jumpRequestPersists = false; // If true This means whatever is requesting jump did not call StopJump() yet. Usually jump button still pressed.
        protected bool jumpLocked = false; // If true character will not jump even if jump button is pressed.
        protected float currentJumpTimer = 0; // Used to calculate how long character has been jumping
        protected float coyoteTimeCounter = 0; // Used to calculate how long character can still jump in case of not being grounded anymore
        protected float resetGhostingTime = 0.15f; // The amount of time in seconds before allowing jump count to be reset

        #endregion

        #region Getters

        protected bool CanStartJump => (CoyoteCheck || OnWall) && !jumpLocked;
        protected bool CanStartExtraJump => setup.HasExtraJumps && extraJumpsLeft > 0 && !jumpLocked;

        protected bool CoyoteCheck => setup.HasCoyoteTime ? coyoteTimeCounter > 0f : grounded;
        protected bool OnWall => setup.CanWallJump && SlidingOnWall;
        protected bool SlidingOnWall => wallHitData != null && (movementDirection.x < 0 && wallHitData.leftHitting || movementDirection.x > 0 && wallHitData.rightHitting);

        // Events
        public UnityEvent JumpPerformed => jumpPerformed;
        public UnityEvent ExtraJumpPerformed => extraJumpPerformed;

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();
            ResetJumpCount();
        }

        protected virtual void Start()
        {
            SubscribeSeekers();
        }

        protected virtual void Update()
        {
            if (setup.HasCoyoteTime && grounded)
            {
                coyoteTimeCounter = setup.CoyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }
        }

        protected virtual void FixedUpdate()
        {
            EvaluateAndApplyJumpCountReset();
            if (autoPerform)
            {
                if (jumping)
                {
                    Perform();
                }
                else if (extraJumping)
                {
                    ExtraJumpPerform();
                }
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

        protected void PrepareJump()
        {
            currentJumpTimer = 0;
            jumpStartedAt = Time.fixedTime;
            canResetAt = jumpStartedAt + resetGhostingTime;
            rb.drag = 0;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        /// <summary>
        /// Starts the jump process so Ascend can be called each physics frame
        /// </summary>
        protected void StartJump()
        {
            PrepareJump();
            jumping = true;
            JumpPerformed.Invoke();
        }

        protected void StartExtraJump()
        {
            PrepareJump();
            extraJumping = true;
            extraJumpsLeft--;
            ExtraJumpPerformed.Invoke();
        }

        /// <summary>
        /// Should be called on Fixed (Physics) Update.
        /// </summary>
        public void Perform()
        {
            if (!jumpRequestPersists || currentJumpTimer > setup.Duration) { Stop(); return; }

            ApplyAscension(setup.Force, setup.Duration);
        }

        /// <summary>
        /// Should be called on Fixed (Physics) Update.
        /// </summary>
        public void ExtraJumpPerform()
        {
            if (!jumpRequestPersists || currentJumpTimer > setup.ExtraJumpDuration) { Stop(); return; }

            ApplyAscension(setup.ExtraJumpForce, setup.ExtraJumpDuration);
        }

        protected void ApplyAscension(float force, float duration)
        {
            float proportionCompleted = currentJumpTimer / duration;
            float thisFrameForce = Mathf.Lerp(force, 0f, proportionCompleted);
            ApplyVerticalForce(thisFrameForce);
            currentJumpTimer += Time.fixedDeltaTime;
        }

        /// <summary>
        /// Resets available jumps count upon grounding.
        /// </summary>
        protected void EvaluateAndApplyJumpCountReset()
        {
            if (resetRequestedAt < canResetAt || Time.fixedTime < canResetAt) return;

            ResetJumpCount();
        }

        /// <summary>
        /// Called to allow jump count to be reset.
        /// </summary>
        protected virtual void ResetJumpCount()
        {
            extraJumpsLeft = setup.ExtraJumps;
        }

        /// <summary>
        /// Call this to reset the jump count.
        /// </summary>
        public virtual void RequestJumpCountReset()
        {
            resetRequestedAt = Time.fixedTime;
        }

        /// <summary>
        /// Call this to request a Jump. 
        /// Character will ascend until duration is reached or StopJump() is called.
        /// </summary>
        public virtual void Request()
        {
            jumpRequestPersists = true;
            jumpRequestedAt = Time.time;
            StartCoroutine(EvaluateJumpRequest());
        }

        /// <summary>
        /// This will keep trying to jump Start a jump while the request persists, the character 
        /// not currently performing a jump and the jump buffer time
        /// </summary>
        protected virtual IEnumerator EvaluateJumpRequest()
        {
            while (jumpRequestPersists && !jumping && !extraJumping && Time.time <= jumpRequestedAt + setup.JumpBufferTime)
            {
                if (CanStartJump)
                {
                    StartJump();
                }
                else if (CanStartExtraJump)
                {
                    StartExtraJump();
                }
                yield return null;
            }
        }

        /// <summary>
        /// Stops jump in progress if any.
        /// </summary>
        public void Stop()
        {
            jumpRequestPersists = false;
            jumping = false;
            extraJumping = false;
            coyoteTimeCounter = 0f;
        }

        #endregion

        #region Update Seeking

        /// <summary>
        /// Call this to update grounding.
        /// </summary>
        /// <param name="newGrounding"></param>
        public void UpdateGrounding(bool newGrounding)
        {
            grounded = newGrounding;

            if (grounded)
            {
                RequestJumpCountReset();
            }
        }

        /// <summary>
        /// Call this to update DynamicJump2D about walls being hit.
        /// </summary>
        /// <param name="newWallHitData"></param>
        public void UpdateWallHitData(WallHitData newWallHitData)
        {
            wallHitData = newWallHitData;

            if (SlidingOnWall)
            {
                RequestJumpCountReset();
            }

        }

        /// <summary>
        /// Call this to update grounding.
        /// </summary>
        /// <param name="newMovementDirection"></param>
        public void UpdateMovementDirection(Vector2 newMovementDirection)
        {
            movementDirection = newMovementDirection;
        }

        /// <summary>
        /// Call this in order to Lock jump and
        /// prevent new jumps to occur based on
        /// shouldLock boolean.
        /// </summary>
        /// <param name="shouldLock"></param>
        public void LockJump(bool shouldLock)
        {
            jumpLocked = shouldLock;
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
                    CC2DLog.Warning("Component Jump 2D might not work properly. It is marked to seek for an IGroundingProvider but it could not find any.");
                groundingProvider?.GroundingUpdate.AddListener(UpdateGrounding);
            }

            if (seekWallHitDataProvider)
            {
                wallHitDataProvider = GetComponent<IWallHitDataProvider>();
                if (wallHitDataProvider == null)
                    CC2DLog.Warning("Component Jump 2D might not work properly. It is marked to seek for an IWallHitDataProvider but it could not find any.");
                wallHitDataProvider?.WallHitDataUpdate.AddListener(UpdateWallHitData);
            }

            if (seekMovementDirectionProvider)
            {
                movementDirectionProvider = GetComponent<IMovementDirectionProvider>();
                if (movementDirectionProvider == null)
                    CC2DLog.Warning("Component Jump 2D might not work properly. It is marked to seek for an IMovementDirectionUpdater but it could not find any.");
                movementDirectionProvider?.MovementDirectionUpdate.AddListener(UpdateMovementDirection);
            }

            if (seekJumpHandler)
            {
                jumpHandler = GetComponent<IDynamicJump2DHandler>();
                if (jumpHandler == null)
                    CC2DLog.Warning("Component Jump 2D might not work properly. It is marked to seek for an IJumpHandler but it could not find any.");
                jumpHandler?.SendJumpRequest.AddListener(Request);
                jumpHandler?.SendJumpStop.AddListener(Stop);
            }
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        protected virtual void UnsubscribeSeekers()
        {
            groundingProvider?.GroundingUpdate.RemoveListener(UpdateGrounding);
            wallHitDataProvider?.WallHitDataUpdate.RemoveListener(UpdateWallHitData);
            movementDirectionProvider?.MovementDirectionUpdate.RemoveListener(UpdateMovementDirection);
            jumpHandler?.SendJumpRequest.RemoveListener(Request);
            jumpHandler?.SendJumpStop.RemoveListener(Stop);
        }

        #endregion

        #region IGComponent
#pragma warning disable 0414

        [Header("About this component"), Foldout("About this component")]
        [ReadOnly, Label("Name"), SerializeField, Space]
        public string componentName = "IndieGabo's  Jump 2D";

        [ReadOnly, Label("Info"), TextArea(1, 30), SerializeField, Space, Foldout("About this component")]
        public string info = "This component gives a GameObject the ability to jump as long as anything with its reference requests it.";

        [field: SerializeField, ReadOnly, Label("Feed Requirements"), TextArea(1, 30), Space, InfoBox("You MUST feed these functions for this component to work.", EInfoBoxType.Warning), Foldout("About this component")]
        public string requirements = "On FixedUpdate: \n"
                                        + "UpdateGronding(bool newGrounding) \n\n"
                                        + "On Demand:\n"
                                        + "JumpRequested() \n"
                                        + "StopJump()";

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
