using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Skill
{
    public string SkillName;
    public string SkillDescription;
    public Sprite SkillIcon;
    public int Cost;
}

public class BattleSkillMenu : MonoBehaviour
{
    [SerializeField] private Skill[] _testSkills;

    [SerializeField] private TextMeshProUGUI _skillDescriptionText;
    [SerializeField] private TextMeshProUGUI _skillBonusText;

    private int skillIndex;

    public void MoveUp()
    {
        skillIndex--;
    }
    public void MoveDown()
    {
        skillIndex++;
    }
}

public class MenuItem
{
    private TextMeshProUGUI _text;
    private Image _icon;

    public void SetupMenuItem(string text, Sprite icon)
    {
        _text.text = text;
        _icon.sprite = icon;
    }
}
