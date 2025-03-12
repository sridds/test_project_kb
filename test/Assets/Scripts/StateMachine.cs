using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hank.Systems.StateMachine
{
    public class StateMachine
    {
        public StateNode CurrentState { get { return current; } }

        StateNode current;
        Dictionary<Type, StateNode> nodes = new();
        HashSet<ITransition> anyTransitions = new();

        public void Update()
        {
            current.State?.Update();

            var transition = GetTransition();
            if (transition != null)
                ChangeState(transition.To);
        }

        public void FixedUpdate()
        {
            current.State?.FixedUpdate();
        }

        public void SetState(IState state)
        {
            ChangeState(state);
        }

        public void SetStateImmediate(IState state)
        {
            current = nodes[state.GetType()];
            current.State?.OnEnter();
        }

        public bool HasTransition(IState from, IState to)
        {
            if (!nodes.ContainsKey(from.GetType())) return false;

            foreach (var transition in nodes[from.GetType()].Transitions)
            {
                if (transition.To == to)
                {
                    return true;
                }
            }

            return false;
        }

        void ChangeState(IState state)
        {
            if (state == current.State) return;

            var previousState = current.State;
            var nextState = nodes[state.GetType()].State;

            previousState?.OnExit();
            nextState?.OnEnter();

            current = nodes[state.GetType()];
        }

        ITransition GetTransition()
        {
            // Are there any transitions from any state?
            foreach (var transition in anyTransitions)
            {
                if (transition.Condition.Evaluate())
                {
                    return transition;
                }
            }

            // Are there any transitions in the current state?
            foreach (var transition in current.Transitions)
            {
                if (transition.Condition.Evaluate())
                {
                    return transition;
                }
            }

            return null;
        }

        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }

        StateNode GetOrAddNode(IState state)
        {
            var node = nodes.GetValueOrDefault(state.GetType());

            if (node == null)
            {
                node = new StateNode(state);
                nodes.Add(state.GetType(), node);
            }

            return node;
        }

        public class StateNode
        {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }

            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }

            public void AddTransition(IState to, IPredicate condition)
            {
                Transitions.Add(new Transition(to, condition));
            }
        }
    }

    public interface IState
    {
        void OnEnter();
        void OnExit();
        void Update();
        void FixedUpdate();
    }

    public interface IPredicate
    {
        bool Evaluate();
    }

    public interface ITransition
    {
        IState To { get; }
        IPredicate Condition { get; }
    }

    public class FuncPredicate : IPredicate
    {
        readonly Func<bool> func;

        public FuncPredicate(Func<bool> func) => this.func = func;

        public bool Evaluate() => func.Invoke();
    }

    public class EventPredicate : IPredicate
    {
        bool eventFiredFlag;

        public EventPredicate(ref Action action)
        {
            action += () => eventFiredFlag = true;
        }

        public bool Evaluate()
        {
            if (eventFiredFlag)
            {
                eventFiredFlag = false;
                return true;
            }

            return false;
        }
    }

    public class Transition : ITransition
    {
        public IState To { get; }
        public IPredicate Condition { get; }

        public Transition(IState to, IPredicate condition)
        {
            To = to;
            Condition = condition;
        }
    }

    public abstract class BaseState : IState
    {
        protected StateMachine stateMachine;

        protected BaseState(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
    }
}

