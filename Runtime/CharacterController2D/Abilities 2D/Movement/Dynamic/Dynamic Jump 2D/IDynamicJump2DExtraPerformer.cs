using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IndieGabo.CharacterController2D.Abilities2D
{
    /// <summary>
    /// Any GameOject that wants to give information about jumping starting or being canceled
    /// through an event must implement this Interface.
    /// </summary>
    public interface IDynamicJump2DExtraPerformer
    {
        void Request();
        void Stop();
        void ExtraJumpPerform();
        UnityEvent ExtraJumpPerformed { get; }
    }
}
