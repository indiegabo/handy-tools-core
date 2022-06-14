using System.Collections;
using System.Collections.Generic;
using IndieGabo.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace IndieGabo.CharacterController2D.Abilities2D
{

    [CreateAssetMenu(fileName = "New DynamicDash2DSetup", menuName = "IndieGabo/CharacterController2D/Setups/DynamicDash2D")]
    public class DynamicDash2DSetup : ScriptableObject
    {

        [Tooltip("The speed wich will be applyed to X axis during dash.")]
        [SerializeField]
        protected float xSpeed = 20f;

        [Tooltip("The speed wich will be applyed to Y axis during dash.")]
        [SerializeField]
        protected float ySpeed = 0f;

        [Tooltip("Time in seconds of the dash duration.")]
        [SerializeField]
        protected float duration = 1f;

        [Tooltip("Minimun time in seconds between dashes.")]
        [SerializeField]
        protected float delay = 1f;

        [Tooltip("The gravity scale to be apllyed to RigidBody2D during dash.")]
        [SerializeField]
        protected float gravityScale = 0f;

        [Tooltip("Will only be able to dash when grounded.")]
        [SerializeField]
        protected bool mustBeGrounded = false;

        // Getters

        public float XSpeed => xSpeed;
        public float YSpeed => ySpeed;
        public float Duration => duration;
        public float Delay => delay;
        public float GravityScale => gravityScale;
        public bool MustBeGrounded => mustBeGrounded;
    }

}
