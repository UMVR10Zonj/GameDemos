using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    #region Field Decleare

    public static Main Instance;

    public static object[] Fruits;
    public static object[] VFX_FruitExplode;
    // NOTE: collided gameobject queue
    public static ConcurrentQueue<GameObject> CQueGo;

    public Transform Bucket;
    [SerializeField] GameObject BlackScreen;
    [SerializeField] GameObject showEnd;
    [SerializeField] GameObject showScore;
    [SerializeField] Text TxtScore;
    [SerializeField] Text TxtHighestScore;
    [SerializeField] SpriteRenderer deadLine;

    [NonSerialized] public int score;
    [NonSerialized] public int HighestScore;
    [NonSerialized] public bool isWarnning;
    [NonSerialized] public bool isBlinking;
    [NonSerialized] public bool isRunGame;

    public Fruit cfruit;
    public WaitForSeconds wait1s;
    public WaitForSeconds wait02s;
    private WaitUntil waitBlink;

    #endregion

    #region Mearge Fruit Event

    public delegate void onMeargeFruit(GameObject go);
    public event onMeargeFruit MrgFruitEvent;

    FruitCollision colInfo;
    public void MeargeFruit(GameObject fruit)
    {
        colInfo = fruit.GetComponent<FruitCollision>();
        int fruitKind = System.Int32.Parse(fruit.name[7].ToString()) + 1;

        score += fruitKind;

        Destroy(fruit);
        Destroy(colInfo.PareFruit);

        if (fruitKind > 9) return;

        var bronFruit = Instantiate((GameObject)Fruits[fruitKind], colInfo.point, transform.rotation, Bucket);
        var vfx = Instantiate((GameObject)VFX_FruitExplode[fruitKind - 1], bronFruit.transform.position, bronFruit.transform.rotation, Bucket);
        MrgFruitEvent?.Invoke(fruit);
    }

    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;

        Fruits = Resources.LoadAll("Prefab/Fruits", typeof(GameObject));
        VFX_FruitExplode = Resources.LoadAll("Prefab/VFX/FruitExplode", typeof(GameObject));
        CQueGo = new ConcurrentQueue<GameObject>();

        cfruit = new Fruit();
        wait1s = new WaitForSeconds(1);
        wait02s = new WaitForSeconds(0.2f);
        waitBlink = new WaitUntil(() => isBlinking);

        HighestScore = 0;

        StartCoroutine(setupGame());
    }

    private string mouseLeftBtn = "Fire1";
    private void Update()
    {
        if (!isRunGame) return;
        if (!cfruit.isControllabl) return;

        // NOTE: Player Input Handler
        if (Input.GetButtonDown(mouseLeftBtn))
        {
            cfruit.SetTargetPosition();
            return;
        }
        if (Input.GetButton(mouseLeftBtn))
        {
            cfruit.SetTargetPosition();
            cfruit.Move();
            return;
        }
        if (Input.GetButtonUp(mouseLeftBtn))
        {
            if (cfruit.NeedMove())
                StartCoroutine(cfruit.KeepMoving());

            cfruit.Drop();
            StartCoroutine(CreateFruit());
            return;
        }
    }

    private void FixedUpdate()
    {
        // NOTE: Collition Handler use Mearge Fruit Event
        if (CQueGo.TryDequeue(out GameObject go))
        {
            MeargeFruit(go);
        }
    }

    public void setDLTransparent()
    {
        Color newColor = deadLine.color;
        newColor.a = 0;
        deadLine.color = newColor;
    }

    public void GameReset()
    {
        score = 0;
        Score.Instance.ResetScore();

        isWarnning = false;
        isBlinking = false;

        cfruit = new Fruit();

        showEnd.SetActive(true);
        showScore.SetActive(false);

        setDLTransparent();
        BlackScreen.SetActive(false);
        StartCoroutine(setupGame());
    }

    public void GameEnd()
    {
        isRunGame = false;
        showEnd.SetActive(false);
        showScore.SetActive(true);

        TxtScore.text = score.ToString();
        if (score > HighestScore) HighestScore = score;
        TxtHighestScore.text = HighestScore.ToString();

        StopAllCoroutines();
    }

    private IEnumerator setupGame()
    {
        yield return new WaitForSeconds(0.5f);
        score = 0;
        isRunGame = true;
        isWarnning = false;
        isBlinking = false;
        StartCoroutine(Blink());
        StartCoroutine(ClearBucket());
        yield break;
    }

    private IEnumerator CreateFruit()
    {
        yield return wait1s;
        cfruit = new Fruit();
    }

    private IEnumerator Blink()
    {
        yield return waitBlink;

        bool isIncreasing = true;

        Color newColor;
        float newAlpha;

        while (true)
        {
            yield return waitBlink;

            newColor = deadLine.color;
            newAlpha = newColor.a + (isIncreasing ? 1 : -1) * 4 * Time.deltaTime;

            if (newAlpha > 1.0f)
            {
                newAlpha = 1.0f;
                isIncreasing = !isIncreasing;
            }
            else if (newAlpha < 0.0f)
            {
                newAlpha = 0.0f;
                isIncreasing = !isIncreasing;
            }

            newColor.a = newAlpha;
            deadLine.color = newColor;

            yield return null;
        }
    }

    private IEnumerator ClearBucket()
    {
        yield return new WaitUntil(() => !isRunGame);

        int dotime = Bucket.childCount;

        for (int i = 0; i <= dotime - 1; i++)
        {
            int fruitKind = System.Int32.Parse(Bucket.GetChild(0).name[7].ToString());
            var vfx = Instantiate((GameObject)VFX_FruitExplode[fruitKind], Bucket.GetChild(0).transform.position, Bucket.GetChild(0).transform.rotation, Bucket);
            Destroy(Bucket.GetChild(0).gameObject);
            yield return wait02s;
        }

        yield return wait1s;
        BlackScreen.SetActive(true);
    }
}

public class Fruit
{
    public GameObject fruit;
    public bool isControllabl = true;

    private Rigidbody2D fruitRig;
    private Vector3 TargetPos;
    private CircleCollider2D fruitCol;

    public Fruit()
    {
        fruit = GameObject.Instantiate((GameObject)Main.Fruits[UnityEngine.Random.Range(0, 5)], Main.Instance.Bucket);
        fruitRig = fruit.GetComponent<Rigidbody2D>();
        fruitCol = fruit.GetComponent<CircleCollider2D>();
        fruitRig.gravityScale = 0;
    }

    public void Drop()
    {
        fruitRig.gravityScale = 4;
        isControllabl = false;
    }

    public void Move()
    {
        if (NeedMove())
            fruit.transform.position = Vector3.MoveTowards(fruit.transform.position, TargetPos, 20 * Time.deltaTime);
    }

    // NOTE: 檢查 滑鼠點擊位置 與 水果目前位置 之間距離是否小於等於0
    public bool NeedMove() =>
        Vector3.Magnitude(TargetPos - fruit.transform.position) <= 0 ? false : true;

    public IEnumerator KeepMoving()
    {
        while (NeedMove())
        {
            Move();
            yield return null;
        }
        yield break;
    }

    public void SetTargetPosition()
    {
        // NOTE: 取得滑鼠點擊位置並轉成 world space
        TargetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // NOTE: 水果在畫面邊邊的絕對值 公式(5.4f - 水果 prefab 的半徑 * scale 值)
        //       5.4f 是滑鼠在視窗邊邊的 position
        var MinMax = Mathf.Abs(5.4f - fruitCol.radius * 1.5f);

        // NOTE: 設定水果的 X軸 在 MinMax 之間
        TargetPos.x = Mathf.Clamp(TargetPos.x, -MinMax, MinMax);

        // NOTE: 固定Z軸，不固定會跑到攝影機的Z軸 導致攝影機看不見
        TargetPos.z = 9.9f;

        // NOTE: 固定Y軸
        TargetPos.y = fruit.transform.position.y;
    }
}