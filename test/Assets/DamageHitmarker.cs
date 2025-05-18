using UnityEngine;
using System;

public class DamageHitmarker : MonoBehaviour
{
    [SerializeField]
    private Sprite[] _numberSprites;

    [SerializeField]
    private float _numberKerning = 0.1f;

    private void Start()
    {
        SetValue(UnityEngine.Random.Range(10000, 50000));
    }

    public void SetValue(int damage)
    {
        string dmg = damage.ToString();

        float totalWidth = 0;
        float[] spriteWidths = new float[dmg.Length];

        for (int i = 0; i < dmg.Length; i++)
        {
            int num = (int)Char.GetNumericValue(dmg[i]);
            spriteWidths[i] = _numberSprites[num].bounds.size.x;
            totalWidth += spriteWidths[i];
        }

        totalWidth += _numberKerning * (dmg.Length - 1);

        float currentX = -totalWidth / 2;

        for (int i = 0; i < dmg.Length; i++)
        {
            // get value
            int num = (int)Char.GetNumericValue(dmg[i]);

            GameObject go = new GameObject(num.ToString());
            go.transform.parent = transform;

            // setup renderer
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = _numberSprites[num];
            renderer.color = Color.red;

            float spriteWidth = spriteWidths[i];
            go.transform.localPosition = new Vector3(currentX + spriteWidth / 2, 0, 0);

            currentX += spriteWidth + _numberKerning;
        }
    }
}
