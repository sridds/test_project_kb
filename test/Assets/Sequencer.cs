using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.Collections;
using NaughtyAttributes;

public class Sequencer : MonoBehaviour
{
    private List<SequencerAction> sequenceActionList = new List<SequencerAction>();

    public event Action OnSequenceStarted;
    public event Action OnSequenceCompleted;
    public event Action<SequencerAction> OnActionStarted;

    private bool isRunning;
    private int currentActionIndex;

    private void Awake()
    {
        InitializeSequence();
    }

    private void InitializeSequence()
    {
        sequenceActionList.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out SequencerAction action))
            {
                sequenceActionList.Add(action);
            }
        }
    }

    public async void PlaySequence()
    {
        if(sequenceActionList.Count == 0)
        {
            Debug.LogWarning("Failed to play sequencer, no actions to execute");
            return;
        }

        if(isRunning)
        {
            Debug.LogWarning("Failed to play sequencer, sequencer is already running!");
            return;
        }

        currentActionIndex = 0;
        await ExecuteSequenceAsync();
    }

    private async Task ExecuteSequenceAsync()
    {
        isRunning = true;
        OnSequenceStarted?.Invoke();

        while(currentActionIndex < sequenceActionList.Count)
        {
            if (sequenceActionList[currentActionIndex].ExecutionMode == EActionExecutionMode.Sequential)
            {
                await ExecuteSequentialActionAsync(sequenceActionList[currentActionIndex]);
            }
            else if (sequenceActionList[currentActionIndex].ExecutionMode == EActionExecutionMode.Parallel)
            {
                _ = ExecuteSequentialActionAsync(sequenceActionList[currentActionIndex]);
            }

            currentActionIndex++;
        }

        isRunning = false;
        OnSequenceCompleted?.Invoke();
    }

    private async Task ExecuteSequentialActionAsync(SequencerAction action)
    {
        OnActionStarted?.Invoke(action);
        Debug.Log($"Executing sequential action: {action.name}");

        await action.ExecuteActionAsync(this);
    }
}

public enum EActionExecutionMode
{
    Sequential,     // Wait for the action to complete
    Parallel,       // Start the action and immediately continue to the next
}

public abstract class SequencerAction : MonoBehaviour
{
    [SerializeField] protected EActionExecutionMode _executionMode;
    [SerializeField] protected float _delay;

    public EActionExecutionMode ExecutionMode => _executionMode;

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