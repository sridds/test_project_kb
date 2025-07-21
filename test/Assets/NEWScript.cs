using System.Collections;
using UnityEngine;
using System;

public class NEWScript : MonoBehaviour
{
    [SerializeField] Sprite theSprite;
    GameObject theObject;
    float timer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 15)
        {
            theObject = new GameObject();
            theObject.AddComponent<SpriteRenderer>().sprite = theSprite;
            theObject.transform.position = new Vector3(15, 0, 0);
            StartCoroutine(Move());
            timer = -1000000;

            transform.Rotate(new Vector3(Time.time * 10f, 0, 0));

        }
        

    }

    IEnumerator Move()
    {
        float elapsed = 0;
        while (elapsed < 0.6f)
        {
            theObject.transform.position = Vector3.Lerp(new Vector3(15, 0, 0), new Vector3(-15, 0, 0), elapsed / 0.6f);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
