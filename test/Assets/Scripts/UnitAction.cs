using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hank.Battles
{
    public abstract class UnitAction : MonoBehaviour
    {
        public delegate void ActionComplete();
        public ActionComplete OnActionComplete;

        protected Coroutine currentActionCoroutine;

        public virtual void StartAction()
        {
            currentActionCoroutine = StartCoroutine(IExecuteAction());
        }

        public abstract IEnumerator IExecuteAction();
    }
}