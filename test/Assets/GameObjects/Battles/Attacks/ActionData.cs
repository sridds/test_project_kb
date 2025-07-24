using UnityEngine;

[CreateAssetMenu(fileName = "NewActionData", menuName = "ActionData")]
public class ActionData : ScriptableObject
{
    public float mana;
    public float damage;
    public float flatKnockback;
    public float scalingKnockback;

    public virtual void Use()
    {

    }
}
