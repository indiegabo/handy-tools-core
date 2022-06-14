using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IndieGabo.NaughtyAttributes;
using UnityEngine;
using System.Reflection;

namespace IndieGabo.FSM
{
    /// <summary>
    /// Represents a State controlled by the StateMachine class.
    /// </summary>
    [DefaultExecutionOrder(200)]
    public abstract class State : MonoBehaviour
    {

        #region Fields

        [Tooltip("Set this to get visual feedback on the Machine's component inspector")]
        [SerializeField] new protected string name;

        /// <summary>
        /// The state machine. Set inside InternalLoad() method wich is called by the machine.
        /// </summary>
        protected StateMachine machine;

        /// <summary>
        /// The machine's actor. Set inside InternalLoad() method wich is called by the machine.
        /// </summary>
        protected Actor actor;

        #endregion

        #region Properties

        /// <summary>
        /// List of transitions of this state.
        /// </summary>
        public List<StateTransition> transitions { get; protected set; } = new List<StateTransition>();

        /// <summary>
        /// Either the state name defined on Inspector or the type of the state class
        /// </summary>
        public string Name => (name != null && name != "") ? name : GetType().ToString();

        public Action OnEnterMethod = () => { };
        public Action OnExitMethod = () => { };
        public Action OnLoadMethod = () => { };
        public Action TickMethod = () => { };
        public Action LateTickMethod = () => { };
        public Action FixedTickMethod = () => { };

        #endregion

        protected virtual void Awake()
        {
        }

        /// <summary>
        /// Adds a transition into the available transitions of this state
        /// </summary>
        /// <param name="Condition"> A bool returning callback wich evaluates if the state should become active or not </param>
        /// <param name="state"> The state wich should become active based on condition </param>
        /// <param name="priority"> Priority level </param>
        protected virtual void AddTransition(Func<bool> Condition, State state, int priority = 0)
        {
            StateTransition transition = new StateTransition(Condition, state, priority);
            AddTransition(transition);
        }

        /// <summary>
        /// Adds a transition into the available transitions of this state
        /// </summary>
        /// <param name="transition"> The State Trasitions to be add </param>
        protected virtual void AddTransition(StateTransition transition)
        {
            transitions.Add(transition);
        }

        /// <summary>
        /// </summary>
        public virtual void InternalLoad()
        {
            machine = GetComponent<StateMachine>();
            actor = machine.actor;
            LoadMethods();
        }

        /// <summary>
        /// Sorts transitions based on priority. Descending
        /// </summary>
        public virtual void SortTransitions()
        {
            transitions = transitions.OrderByDescending(transition => transition.priority).ToList();
        }

        protected virtual void LoadMethods()
        {

            System.Type stateType = this.GetType();
            MethodInfo mi;

            mi = stateType.GetMethod("OnEnter", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                OnEnterMethod = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

            mi = stateType.GetMethod("OnExit", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                OnExitMethod = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

            mi = stateType.GetMethod("OnLoad", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                OnLoadMethod = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

            mi = stateType.GetMethod("Tick", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                TickMethod = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

            mi = stateType.GetMethod("LateTick", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                LateTickMethod = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

            mi = stateType.GetMethod("FixedTick", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                FixedTickMethod = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

        }
    }
}