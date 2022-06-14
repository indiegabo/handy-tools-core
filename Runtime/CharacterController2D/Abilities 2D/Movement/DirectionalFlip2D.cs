
using IndieGabo.CharacterController2D.Enums;
using IndieGabo.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using IndieGabo.CharacterController2D.Abilities2D;

namespace IndieGabo.CharacterController2D.Abilities2D
{
    [AddComponentMenu("IndieGabo/Character Controller 2D/Abilities 2D/DirectionalFlip2D")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class DirectionalFlip2D : Ability2D<DirectionalFlip2DSetup>, IFacingDirectionProvider
    {
        #region  Editor   

        [Foldout("Available Events:")]
        [Space]
        [InfoBox("You can use these to directly set listeners about wich horizontal direction this GameObject is flipped towards.")]
        [SerializeField]
        public UnityEvent<HorizontalDirections> directionFacingUpdate;

        #endregion

        #region Components

        protected Rigidbody2D rb;

        #endregion

        #region Properties

        public float currentDirection { get; protected set; }

        #endregion

        #region Getters

        public UnityEvent<HorizontalDirections> DirectionFacingUpdate => directionFacingUpdate;

        #endregion

        #region Mono
        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();
            InitialFlip();
        }

        #endregion

        #region Logic

        public virtual void EvaluateAndFlip(float subjectDirection)
        {
            if (!ShouldFlip(subjectDirection)) return;

            switch (setup.Strategy)
            {
                case FlipStrategy.Rotating:
                    FlipRotating(subjectDirection);
                    break;
                case FlipStrategy.Scaling:
                    FlipScaling(subjectDirection);
                    break;
                default:
                    FlipRotating(subjectDirection);
                    break;
            }

            if (Mathf.Sign(currentDirection) < 0) { DirectionFacingUpdate.Invoke(HorizontalDirections.Left); return; }
            if (Mathf.Sign(currentDirection) > 0) { DirectionFacingUpdate.Invoke(HorizontalDirections.Right); return; }
        }

        /// <summary>
        /// Evaluates if Game Object should be flippedand
        /// flips it's rotation on Y axis based on the subjectDirection
        /// param.
        /// </summary>
        /// <param name="subjectDirection"> The normalized direction to be evaluateed </param>
        public virtual void FlipRotating(float subjectDirection)
        {
            currentDirection *= -1;
            transform.Rotate(0f, -180f, 0f);
        }

        /// <summary>
        /// Evaluates if Game Object should be flipped and
        /// flips it's scale on X axis based on the subjectDirection
        /// param.
        /// </summary>
        /// <param name="subjectDirection"> The normalized direction to be evaluateed </param>
        public virtual void FlipScaling(float subjectDirection)
        {
            currentDirection *= -1;
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }

        /// <summary>
        /// Executes an initial Flip of the GameObject
        /// based on the startingDirection chosen on
        /// inspector.
        /// </summary>
        protected virtual void InitialFlip()
        {
            switch (setup.StartingDirection)
            {
                case HorizontalDirections.Right:
                case HorizontalDirections.None:
                    transform.Rotate(0f, 0f, 0f);
                    currentDirection = 1;
                    break;
                case HorizontalDirections.Left:
                    transform.Rotate(0f, -180f, 0f);
                    currentDirection = -1;
                    break;
            }
        }

        /// <summary>
        /// Evalates if GameObject should be Flipped
        /// </summary>
        /// <param name="subjectDirection"></param>
        /// <returns></returns>
        protected virtual bool ShouldFlip(float subjectDirection)
        {
            return subjectDirection > 0 && currentDirection < 0 || subjectDirection < 0 && currentDirection > 0;
        }

        #endregion

        #region IGComponent
#pragma warning disable 0414

        [Foldout("About this component")]
        [ReadOnly, Label("Name"), SerializeField]
        public string componentName = "IndieGabo's  Directional Flip 2D";

        [ReadOnly, Label("Info"), TextArea(1, 30), SerializeField, Space, Foldout("About this component")]
        public string info = "This component gives a GameObject the ability to flip based on a given direction. It can either rotate its Y axis or invert its scale on the X axis.";

        [field: SerializeField, ReadOnly, Label("Feed Requirements"), TextArea(1, 30), Space, InfoBox("You MUST feed these functions for this component to work.", EInfoBoxType.Warning), Foldout("About this component")]
        public string requirements = "None. You are good to go.";

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
