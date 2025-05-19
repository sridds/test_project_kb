using UnityEngine;
using System;
using DG.Tweening;

public class SpriteNumbers : MonoBehaviour
{
    [SerializeField]
    private Transform _holder;

    [SerializeField]
    private NumberSprite _prefab;

    [SerializeField]
    private float _numberKerning = 0.1f;

    private NumberSprite[] hitmarkers;

    public void SetValue(int damage)
    {
        string dmg = damage.ToString();
        hitmarkers = new NumberSprite[dmg.Length];

        float totalWidth = 0;
        float[] spriteWidths = new float[dmg.Length];

        // Get the total width of all sprites
        for (int i = 0; i < dmg.Length; i++)
        {
            int num = (int)Char.GetNumericValue(dmg[i]);
            spriteWidths[i] = _prefab.NumberSprites[num].bounds.size.x;
            totalWidth += spriteWidths[i];
        }

        totalWidth += _numberKerning * (dmg.Length - 1);

        float currentX = -totalWidth / 2;

        for (int i = 0; i < dmg.Length; i++)
        {
            // get value
            int num = (int)Char.GetNumericValue(dmg[i]);

            // Create new object
            NumberSprite hitmarker = Instantiate(_prefab, _holder.transform.position, Quaternion.identity);
            hitmarker.transform.parent = _holder;
            hitmarker.SetDigit(num, i);

            // Setup position
            float spriteWidth = spriteWidths[i];
            hitmarker.transform.localPosition = new Vector3(currentX + spriteWidth / 2, 0, 0);
            currentX += spriteWidth + _numberKerning;

            // Store in array for quick retrieval
            hitmarkers[i] = hitmarker;
        }
    }
}
