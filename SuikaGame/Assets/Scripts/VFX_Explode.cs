using System.Collections;
using UnityEngine;

public class VFX_Explode : MonoBehaviour
{
    [SerializeField] private Transform guazhi;

    private SpriteRenderer spRGuazhi;
    private Color newColor;
    private float alpha;
    private float timeflow;

    private void OnEnable()
    {
        spRGuazhi = guazhi.GetComponent<SpriteRenderer>();
        StartCoroutine(Scale());
    }

    private IEnumerator Scale()
    {
        timeflow = 0;
        while (timeflow < 0.8f)
        {
            alpha = spRGuazhi.color.a;
            alpha += 0.01f;
            alpha = Mathf.Clamp01(alpha);

            newColor = spRGuazhi.color;
            newColor.a = alpha;

            spRGuazhi.color = newColor;

            if (guazhi.localScale.x < 2)
                guazhi.localScale += Vector3.one * 3f * Time.deltaTime;
            timeflow += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}