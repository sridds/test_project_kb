using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DatingScene : MonoBehaviour
{
    [SerializeField]
    private SceneDialogue[] _dialogues;

    private IEnumerator BeginScene()
    {
        _dialogues[0].Actor.EnterScene();

        yield return null;
    }
}

[System.Serializable]
public struct SceneDialogue
{
    public DatingActor Actor;

    [TextArea]
    public string Line;
}
