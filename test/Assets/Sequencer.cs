using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.Collections;

public class Sequencer : MonoBehaviour
{
    private List<SequencerAction> sequenceActions = new List<SequencerAction>();
    private CancellationTokenSource cancellationTokenSource;
    private int currentActionIndex = 0;

    public event Action OnSequenceStarted;
    public event Action OnSequenceCompleted;
    public event Action<SequencerAction> OnActionStarted;

    private void Awake()
    {
        InitializeSequence();
    }

    private void InitializeSequence()
    {
        sequenceActions.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out SequencerAction action))
            {
                sequenceActions.Add(action);
            }
        }
    }

    public async void PlaySequence()
    {

    }
}

public abstract class SequencerAction : MonoBehaviour
{
    public virtual async Task ExecuteActionAsync(Sequencer sequencer, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine(ExecuteWithCallback(() => tcs.SetResult(true)));

        await tcs.Task;
    }

    protected virtual IEnumerator ExecuteActionCoroutine(Sequencer sequencer)
    {
        yield return null;
    }

    private IEnumerator ExecuteWithCallback(Action onComplete)
    {
        yield return StartCoroutine(ExecuteActionCoroutine(null));
        onComplete?.Invoke();
    }
}