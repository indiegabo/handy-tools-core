using System.Collections;
using System.Collections.Generic;
using IndieGabo.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using IndieGabo.CharacterController2D.Enums;

namespace IndieGabo.CharacterController2D.Abilities2D
{
    [CreateAssetMenu(fileName = "New DirectionalFlip2DSetup", menuName = "IndieGabo/CharacterController2D/Setups/DirectionalFlip2D")]
    public class DirectionalFlip2DSetup : ScriptableObject
    {

        #region Editor

        [Tooltip("If the game object should be flipped scaling negatively on X axis or rotating Y axis 180º")]
        [SerializeField]
        protected FlipStrategy strategy = FlipStrategy.Rotating;

        [Tooltip("Use this to set wich direction GameObject should start flipped towards.")]
        [SerializeField]
        protected HorizontalDirections startingDirection = HorizontalDirections.Right;

        #endregion

        #region Getters

        public FlipStrategy Strategy => strategy;
        public HorizontalDirections StartingDirection => startingDirection;

        #endregion

    }
}
