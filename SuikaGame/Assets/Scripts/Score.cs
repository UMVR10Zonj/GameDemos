using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static Score Instance;
    public Transform tScore;
    public Sprite[] numbers;

    private Image numK, numH, numT, numO;
    private int k, h, t, o;

    private int showScore;
    private WaitForSeconds wait005s;
    private WaitUntil waitUntilNeedDo;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        numK = tScore.GetChild(0).GetComponent<Image>();
        numH = tScore.GetChild(1).GetComponent<Image>();
        numT = tScore.GetChild(2).GetComponent<Image>();
        numO = tScore.GetChild(3).GetComponent<Image>();
        numK.gameObject.SetActive(false);
        numH.gameObject.SetActive(false);
        numT.gameObject.SetActive(false);
        numO.sprite = numbers[0];
        k = 0;
        h = 0;
        t = 0;
        o = 0;
        showScore = 0;
        wait005s = new WaitForSeconds(0.05f);
        waitUntilNeedDo = new WaitUntil(() => showScore < Main.Instance.score);
    }

    private void Start()
    {
        StartCoroutine(IRefreshScore());
    }

    public void ResetScore()
    {
        showScore = 0;

        k = 0;
        h = 0;
        t = 0;
        o = 0;

        numO.sprite = numbers[o];
        numT.sprite = numbers[t];
        numH.sprite = numbers[h];
        numK.sprite = numbers[k];

        numK.gameObject.SetActive(false);
        numH.gameObject.SetActive(false);
        numT.gameObject.SetActive(false);
    }

    private IEnumerator IRefreshScore()
    {
        while (true)
        {
            yield return waitUntilNeedDo;
            showScore++;

            o++;
            if (o > 9)
            {
                numT.gameObject.SetActive(true);
                o = 0;
                t++;
            }
            if (h > 9)
            {
                numK.gameObject.SetActive(true);
                h = 0;
                k++;
            }
            if (t > 9)
            {
                numH.gameObject.SetActive(true);
                t = 0;
                h++;
            }

            numO.sprite = numbers[o];
            numT.sprite = numbers[t];
            numH.sprite = numbers[h];
            numK.sprite = numbers[k];

            yield return wait005s;
        }
    }
}