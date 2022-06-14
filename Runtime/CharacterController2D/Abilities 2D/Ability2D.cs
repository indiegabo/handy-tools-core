using System.Collections;
using System.Collections.Generic;
using IndieGabo.NaughtyAttributes;
using UnityEngine;

namespace IndieGabo.CharacterController2D.Abilities2D
{
    [DefaultExecutionOrder(300)]
    public abstract class Ability2D<T> : MonoBehaviour
    {
        #region Editor

        [Required("Without a proper setup this won't work.")]
        [SerializeField]
        protected T setup;

        #endregion

        #region Mono 

        protected virtual void Awake()
        {
            if (setup == null)
            {
                CC2DLog.Danger($"{GetType().Name} setup is null. Please assign a proper setup to this ability.");
            }
        }

        #endregion

        #region Setup Stuff

        public virtual void ChangeSetup(T newSetup)
        {
            setup = newSetup;
        }

        #endregion
    }

}
