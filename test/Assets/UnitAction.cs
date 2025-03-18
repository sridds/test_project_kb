using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class UnitAction : MonoBehaviour
{
    public IEnumerator IExecute()
    {
        Initalize();

        yield return StartCoroutine(IExecuteAction());

        Cleanup();
    }

    protected abstract IEnumerator IExecuteAction();

    public virtual void Initalize() { }

    public virtual void Cleanup() { }
}
