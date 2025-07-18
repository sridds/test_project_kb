using UnityEngine;

namespace Hank.Unused
{
    public abstract class Item : ScriptableObject
    {
        public string ItemName;
        public string ItemDescription;
        public Sprite ItemUISprite;
    }
}