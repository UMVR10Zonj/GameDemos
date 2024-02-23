using DG.Tweening;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private void OnEnable()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(1).position = Vector3.zero;
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(2).position = Vector3.zero;

        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).DOScale(0.5f, 0.5f)
                             .OnComplete(() => transform.GetChild(1).gameObject.SetActive(true));
    }
}
