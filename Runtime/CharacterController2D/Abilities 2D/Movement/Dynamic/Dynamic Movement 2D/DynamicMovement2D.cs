using System.Collections;
using System.Collections.Generic;
using IndieGabo.CharacterController2D.Abilities2D;
using IndieGabo.CharacterController2D.Data;
using UnityEngine;
using IndieGabo.NaughtyAttributes;

namespace IndieGabo.CharacterController2D.Abilities2D
{
    [AddComponentMenu("IndieGabo/Character Controller 2D/Abilities 2D/DynamicMovement2D")]
    public class DynamicMovement2D : DynamicMovementPerformer2D<DynamicMovement2DSetup>
    {

        #region Mono

        protected override void Awake()
        {
            base.Awake();
        }

        #endregion

        #region Logic

        /// <summary>
        /// Makes the character stand still
        /// </summary>
        public virtual void Stand()
        {
            ApplyHorizontalVelocity(0f, 0f);
        }

        /// <summary>
        /// Makes the character stand still considering slopes
        /// </summary>
        /// <param name="slopeData"></param>
        public virtual void Stand(SlopeData slopeData)
        {
            ApplyHorizontalVelocity(0f, 0f, slopeData, setup.FullFriction);
        }

        /// <summary>
        /// Moves character along X axis based on xSpeed    
        /// </summary>
        /// <param name="directionSign"></param>
        public virtual void MoveHorizontally(float directionSign)
        {
            ApplyHorizontalVelocity(setup.XSpeed, directionSign);
        }

        /// <summary>
        /// Moves character along X axis based on xSpeed
        /// </summary>
        /// <param name="directionSign"></param>
        public virtual void GroundedMoveHorizontally(float directionSign)
        {
            if (setup.HasStartingtImpulse)
            {
                if (Mathf.Abs(rb.velocity.x) < setup.XSpeed)
                {
                    ApplyHorizontalForce(setup.AccelerationRate, directionSign);
                }
                else
                {
                    ApplyHorizontalVelocity(setup.XSpeed, directionSign);
                }
            }
            else
            {
                ApplyHorizontalVelocity(setup.XSpeed, directionSign);
            }
            EvaluateAndApplyLinearDrag(directionSign);
        }

        /// <summary>
        /// Moves character along X axis based on xSpeed considering slopes
        /// </summary>
        /// <param name="directionSign"></param>
        /// <param name="slopeData"></param>
        /// <param name="ignoreSlopes"></param>
        public virtual void GroundedMoveHorizontally(float directionSign, SlopeData slopeData, bool ignoreSlopes = false)
        {
            if (setup.HasStartingtImpulse && !slopeData.onSlope)
            {
                if (Mathf.Abs(rb.velocity.x) < setup.XSpeed)
                {
                    ApplyHorizontalForce(setup.AccelerationRate, directionSign);
                }
                else
                {
                    ApplyHorizontalVelocity(setup.XSpeed, directionSign, slopeData, setup.FullFriction, ignoreSlopes);
                }
            }
            else
            {
                ApplyHorizontalVelocity(setup.XSpeed, directionSign, slopeData, setup.FullFriction, ignoreSlopes);
            }

            EvaluateAndApplyLinearDrag(directionSign, slopeData);
        }

        /// <summary>
        /// Set a Rigidbody.velocity.x based on a given speed
        /// </summary>
        /// <param name="speed"> The desired speed </param>
        public virtual void MoveHorizontallyApplyingGravity(float directionSign, float gravityScale)
        {
            ApplyHorizontalVelocityWithGravity(setup.XSpeed, directionSign, gravityScale);
        }

        /// <summary>
        /// Pushs the character along X axis towards given direction sign using the amount of force given
        /// </summary>
        /// <param name="force"></param>
        /// <param name="directionSign"></param>
        public virtual void PushHorizontally(float force, float directionSign)
        {
            ApplyHorizontalForce(force, directionSign);
        }

        /// <summary>
        /// Applies vertical speed to the character
        /// </summary>
        /// <param name="speed"></param>
        public virtual void MoveVertically(float speed)
        {
            ApplyVerticalVelocity(speed);
        }

        /// <summary>
        /// Evaluates if linear drag of rigidbody should be changed. Applies the new drag value
        /// case it should.
        /// </summary>
        /// <param name="directionSign"></param>
        /// <param name="slopeData"></param>
        protected virtual void EvaluateAndApplyLinearDrag(float directionSign, SlopeData slopeData = null)
        {
            if ((slopeData != null && slopeData.onSlope) || !setup.HasStartingtImpulse)
            {
                rb.drag = 0f;
                return;
            }

            if ((Mathf.Abs(directionSign) <= 0.4f && rb.velocity.x != 0) || IsChangingDirection(directionSign))
            {
                rb.drag = setup.StartingImpulseDrag;
            }
            else
            {
                rb.drag = 0f;
            }
        }

        /// <summary>
        /// Simple evaluation of if the character is changing direction
        /// </summary>
        /// <param name="directionSign"></param>
        /// <returns></returns>
        public virtual bool IsChangingDirection(float directionSign)
        {
            return (rb.velocity.x > 0f && directionSign < 0f) || (rb.velocity.x < 0f && directionSign > 0f);
        }

        #endregion

        #region IGComponent
#pragma warning disable 0414

        [Header("About this component"), Foldout("About this component")]
        [ReadOnly, Label("Name"), SerializeField, Space]
        public string componentName = "IndieGabo's  Dynamic Movement 2D";

        [ReadOnly, Label("Info"), TextArea(1, 30), SerializeField, Space, Foldout("About this component")]
        public string info = "This component gives a GameObject to move using it's RigidBody2D set to Dynamic.";

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
