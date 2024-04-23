using System.Collections;
using UnityEngine;

public class FruitVFX : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(Scale());
    }

    private IEnumerator Scale()
    {
        while (transform.localScale.x < 1.8f)
        {
            transform.localScale += Vector3.one * 2f * Time.deltaTime;
            yield return null;
        }
        while (transform.localScale.x > 1.5f)
        {
            if (transform.localScale.x <= 1.5f)
            {
                transform.localScale = new Vector3(1.5f, 1.5f, 0);
                yield break;
            }
            transform.localScale -= Vector3.one * 1.8f * Time.deltaTime;
            yield return null;
        }
    }

}