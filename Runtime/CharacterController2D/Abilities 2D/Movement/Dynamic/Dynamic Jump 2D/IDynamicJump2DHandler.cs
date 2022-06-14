using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IndieGabo.CharacterController2D.Abilities2D
{
    /// <summary>
    /// Any component that implements this interface will be able to handle dynamic jump
    /// by invoking its methods..
    /// </summary>
    public interface IDynamicJump2DHandler
    {
        UnityEvent SendJumpRequest { get; }
        UnityEvent SendJumpStop { get; }
    }
}
