using UnityEngine;

public class DatingActor : MonoBehaviour
{
    [SerializeField]
    private string _actorName;

    [Header("References")]
    [SerializeField]
    private SpriteRenderer _renderer;

    [SerializeField]
    private Sprite _defaultSprite;

    [SerializeField]
    private Sprite[] _talkAnimation;

    public void EnterScene()
    {

    }

    public void ExitScene()
    {

    }

    // tracks dialogue until completion
    public void SayLine(string line)
    {

    }
}
