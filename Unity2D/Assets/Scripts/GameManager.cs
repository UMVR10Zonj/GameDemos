using System.Collections;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager Instance;
    private GameManager() { }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion
    #region Field Declaration

    public bool isGaming;
    [SerializeField] private AudioSource BgmPlayer;
    [SerializeField] private AudioClip WinBgm;
    private AudioSource Effplayer;

    [Header("--------Env")]
    [SerializeField] private GameObject t1;
    [SerializeField] private GameObject t2;
    [SerializeField] private GameObject t3;

    [Header("--------Players")]
    public Transform player1;
    public Transform player2;
    private PlayerFSM p1Fsm;
    private PlayerFSM p2Fsm;
    private PlayerVFX p1Vfx;
    private PlayerVFX p2Vfx;

    [Header("--------UI")]
    public TopBar TopBar;
    [SerializeField] private TextMeshProUGUI countThr;
    [SerializeField] private DoorCtrl doorCtrller;
    [SerializeField] private Transform settle;
    [SerializeField] private Image winner;
    [SerializeField] private Image crown;
    [SerializeField] private Animation animCrown;

    [Header("--------Event")]
    [SerializeField] private RedHollowControl hollow;
    [SerializeField] private Transform spikeBall;
    [SerializeField] private ParticleSystem portalB;
    [SerializeField] private ParticleSystem portalY;
    [SerializeField] private ParticleSystem fireball;
    [SerializeField] private GameObject rocket;

    #endregion

    private void Init()
    {
        TopBar.Reset();
        StartCoroutine(StartAnim());
        BgmPlayer.volume = 0.15f;
        portalB.gameObject.SetActive(false);
        portalY.gameObject.SetActive(false);
    }

    private void Start()
    {
        p1Fsm = player1.GetComponent<PlayerFSM>();
        p2Fsm = player2.GetComponent<PlayerFSM>();
        p1Vfx = player1.GetComponent<PlayerVFX>();
        p2Vfx = player2.GetComponent<PlayerVFX>();
        Effplayer = GetComponent<AudioSource>();

        ObjectPool.SetupNewPool(ObjectPool.EType.FX_Rocket, rocket, 10);

        #region DeadCount Event

        p1Fsm.States[PlayerFSM.EState.Dead].OnEnterEvent += () => TopBar.AddDead("p1", 1);
        p2Fsm.States[PlayerFSM.EState.Dead].OnEnterEvent += () => TopBar.AddDead("p2", 1);

        #endregion
        #region GamingEvent

        // NOTE: GameEnd
        TopBar.timer.NowTime
                    .Where(time => time <= 0)
                    .Subscribe(_ =>
                    {
                        isGaming = false;
                        DOTween.To(SetVolum, BgmPlayer.volume, 0, 1).OnComplete(() => BgmPlayer.Pause());
                        doorCtrller.Close().OnComplete(() =>
                        {
                            BgmPlayer.volume = 0.7f;
                            BgmPlayer.clip = WinBgm;
                            BgmPlayer.Play();

                            winner.color = (TopBar.p1DeadCount < TopBar.p2DeadCount) ? Color.red : Color.cyan;
                            settle.DOMoveY(0, 1).SetEase(Ease.OutBounce);
                            winner.transform.DOLocalMoveY(200, 0.5f).SetRelative().SetDelay(1);
                            winner.transform.DOLocalRotate(Vector3.forward * 1800, 1).SetRelative().SetDelay(1)
                                            .OnComplete(() =>
                                            {
                                                winner.transform.DOLocalRotate(Vector3.zero, 0.1f);
                                                crown.DOFade(1, 0.1f);
                                                animCrown.Play();
                                            });
                            winner.transform.DOLocalMoveY(-200, 0.5f).SetRelative().SetDelay(1.5f);
                        });
                    });

        // NOTE: Rotate Screen
        TopBar.timer.NowTime
                    .Where(time => time < TopBar.timer.RemainTime && time > 1 && Mathf.FloorToInt(time) % 30 == 0)
                    .Subscribe(_ =>
                    {
                        if (Random.Range(0, 2) == 0)
                        {
                            CamCtrl.RotateL();
                        }
                        else
                        {
                            CamCtrl.RotateR();
                        }
                    });

        // NOTE: Spike Ball
        TopBar.timer.NowTime
                    .Where(time => time < 180 && time > 1 && Mathf.FloorToInt(time) % 60 == 0)
                    .Subscribe(_ =>
                    {
                        StartCoroutine(SummonSpikeBall());
                        portalB.gameObject.SetActive(true);
                        portalY.gameObject.SetActive(true);
                    });

        // NOTE: Hollow Burst
        TopBar.timer.NowTime
                    .Where(time => time < 180 && time > 1 && Mathf.FloorToInt(time) % 20 == 0)
                    .Subscribe(_ =>
                    {
                        hollow.transform.position = (Random.Range(0, 2) == 0) ? player1.position : player2.position;
                        hollow.Play();
                    });

        // NOTE: Spawn Rocket
        TopBar.timer.NowTime
                    .Where(time => time < 180 && time > 1 && Mathf.FloorToInt(time) % 2 == 0)
                    .Subscribe(_ =>
                    {
                        if (CamCtrl.isRotating) return;
                        var a = ObjectPool.GetObject(ObjectPool.EType.FX_Rocket);
                        a.transform.position = (Random.Range(0, 2) == 0) ? player1.transform.position : player2.transform.position;
                        a.transform.position += Vector3.right * Random.Range(-5f, 6f) + Vector3.up * Random.Range(-5f, 6f);
                        a.SetActive(true);
                    });
        TopBar.timer.NowTime
                    .Where(time => time < 150 && time > 1 && Mathf.FloorToInt(time) % 3 == 0)
                    .Subscribe(_ =>
                    {
                        if (CamCtrl.isRotating) return;
                        var fx_rocket = ObjectPool.GetObject(ObjectPool.EType.FX_Rocket);
                        fx_rocket.transform.position = (Random.Range(0, 2) == 0) ? player1.transform.position : player2.transform.position;
                        fx_rocket.transform.position += Vector3.right * Random.Range(-5f, 6f) + Vector3.up * Random.Range(-5f, 6f);
                        fx_rocket.SetActive(true);
                    });
        TopBar.timer.NowTime
                    .Where(time => time < 150 && time > 1 && Mathf.FloorToInt(time) % 5 == 0)
                    .Subscribe(_ =>
                    {
                        if (CamCtrl.isRotating) return;
                        var fx_rocket = ObjectPool.GetObject(ObjectPool.EType.FX_Rocket);
                        fx_rocket.transform.position = (Random.Range(0, 2) == 0) ? player1.transform.position : player2.transform.position;
                        fx_rocket.transform.position += Vector3.right * Random.Range(-5f, 6f) + Vector3.up * Random.Range(-5f, 6f);
                        fx_rocket.SetActive(true);
                    });

        #endregion

        Init();
    }
    private void FixedUpdate()
    {
        // NOTE: Obj and skybox Rotation.
        RenderSettings.skybox.SetFloat("_Rotation", 2 * Time.time);
        t1.transform.Rotate(-Vector3.forward * 20 * Time.fixedDeltaTime);
        t2.transform.Rotate(Vector3.forward * 20 * Time.fixedDeltaTime);
        t3.transform.Rotate(Vector3.forward * Time.fixedDeltaTime);

        if (Vector3.Magnitude(player1.position) > 30) player1.position = Vector3.zero;
        if (Vector3.Magnitude(player2.position) > 30) player2.position = Vector3.zero;
    }

    private void SetVolum(float v)
    {
        BgmPlayer.volume = v;
    }

    private WaitForSeconds wait1Sec = new WaitForSeconds(1);
    private WaitForSeconds wait4Sec = new WaitForSeconds(4);
    private IEnumerator StartAnim()
    {
        yield return wait1Sec;
        CamCtrl.FocusPlayer("p1");
        yield return wait1Sec;
        p1Vfx.SpawnOnStart();
        yield return wait4Sec;
        CamCtrl.EndFocus();
        player1.GetComponent<Rigidbody2D>().gravityScale = 1;

        yield return wait1Sec;
        CamCtrl.FocusPlayer("p2");
        yield return wait1Sec;
        p2Vfx.SpawnOnStart();
        yield return wait4Sec;
        CamCtrl.EndFocus();
        player2.GetComponent<Rigidbody2D>().gravityScale = 1;

        Effplayer.Play();

        Sequence countDown = DOTween.Sequence();
        countDown.Append(countThr.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutBack).OnComplete(() =>
        {
            countThr.transform.localScale = Vector3.zero;
            countThr.text = 2.ToString();
            Effplayer.Play();
        }));
        countDown.Append(countThr.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutBack).OnComplete(() =>
        {
            countThr.transform.localScale = Vector3.zero;
            countThr.text = 1.ToString();
            Effplayer.Play();
        }));
        countDown.Append(countThr.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutBack).OnComplete(() =>
        {
            countThr.transform.localScale = Vector3.zero;
            countThr.text = "START";
        }));
        countDown.Append(countThr.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutBack).OnComplete(() =>
        {
            countThr.transform.localScale = Vector3.zero;

        }));
        countDown.OnComplete(() =>
        {
            TopBar.timer.StartTimer();
            isGaming = true;
        });
    }

    private IEnumerator SummonSpikeBall()
    {
        spikeBall.gameObject.SetActive(true);
        yield return new WaitForSeconds(30);
        spikeBall.DOScale(Vector3.zero, 1).OnComplete(() => spikeBall.gameObject.SetActive(false));
        portalB.gameObject.SetActive(false);
        portalY.gameObject.SetActive(false);
    }

    public static void FlipGravity() => Instance.OnFlipGravity();
    private void OnFlipGravity()
    {
        Physics2D.gravity *= -1;

        var p1con = player1.GetComponent<PlayerController>();
        p1con.jumpHight *= -1;
        p1con.m_Gravity *= -1;

        var p2con = player2.GetComponent<PlayerController>();
        p2con.jumpHight *= -1;
        p2con.m_Gravity *= -1;

        player1.DORotate(Vector3.forward * 180, 0.5f).SetRelative();
        player2.DORotate(Vector3.forward * 180, 0.5f).SetRelative();
    }
}
