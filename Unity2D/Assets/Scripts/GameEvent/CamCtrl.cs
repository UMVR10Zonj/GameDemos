using DG.Tweening;
using UnityEngine;

public class CamCtrl : MonoBehaviour
{
    #region Singleton

    private static CamCtrl Instance = null;
    private CamCtrl() { }
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

    public static bool isRotating = false;

    [SerializeField] private PlayerFSM p1Fsm;
    [SerializeField] private PlayerFSM p2Fsm;
    [SerializeField] private Transform arrow;
    [SerializeField] private Material matArrow;
    [SerializeField] private AudioSource EffPlayer;
    [SerializeField] private AudioClip effWarn;

    private Camera cam;
    private Vector3 ori_Pos;
    private float ori_Rot;
    private float ori_Orthosize;
    private Vector3 p1StartPos = new Vector3(-15.6f, -9.15f, -1);

    #endregion

    private void Start()
    {
        cam = Camera.main;
        ori_Pos = cam.transform.position;
        ori_Rot = cam.transform.rotation.z;
        ori_Orthosize = cam.orthographicSize;
    }

    public static void Shake(float duration, float strength) => Instance.OnShake(duration, strength);
    public static void RotateL() => Instance.OnRotate(Vector3.forward * -180);
    public static void RotateR() => Instance.OnRotate(Vector3.forward * 180);
    public static void FocusPlayer(string player) => Instance.OnFocusPlayer(player);
    public static void EndFocus() => Instance.OnEndFocus();

    private void OnShake(float duration, float strength)
    {
        if (isRotating) return;
        ori_Pos = cam.transform.position;
        transform.DOShakePosition(duration, strength).OnComplete(() => transform.position = ori_Pos);
        transform.DOShakeRotation(duration, Vector3.forward * strength).OnComplete(() => transform.rotation = Quaternion.Euler(0, 0, ori_Rot));
    }
    private void OnRotate(Vector3 rotation)
    {
        arrow.localScale = (rotation.z > 0) ? Vector3.one : Vector3.one - Vector3.right * 2;
        arrow.gameObject.SetActive(true);

        arrow.DORotate(-Vector3.forward * 180 * arrow.localScale.x, 6).SetRelative();

        EffPlayer.clip = effWarn;
        EffPlayer.Play();

        Sequence tmp = DOTween.Sequence().SetRecyclable();

        tmp.Append(matArrow.DOFade(1, 1f));
        tmp.Append(matArrow.DOFade(0, 1f));
        tmp.Append(matArrow.DOFade(1, 1f));
        tmp.Append(matArrow.DOFade(0, 1f));
        tmp.Append(matArrow.DOFade(1, 1f));
        tmp.Append(matArrow.DOFade(0, 1f));

        tmp.OnComplete(() =>
        {
            arrow.gameObject.SetActive(false);
            isRotating = true;
            p1Fsm.SetState(PlayerFSM.EState.UnCtrlable);
            p2Fsm.SetState(PlayerFSM.EState.UnCtrlable);

            Sequence rot = DOTween.Sequence();

            rot.Append(cam.DOOrthoSize(17.5f, 3)).SetEase(Ease.Linear);
            rot.Append(transform.DORotate(rotation, 3).SetRelative());
            rot.Append(cam.DOOrthoSize(ori_Orthosize, 3)).SetEase(Ease.Linear);
            rot.OnComplete(() =>
            {
                GameManager.FlipGravity();
                ori_Rot += rotation.z;
                p1Fsm.SetState(PlayerFSM.EState.Idle);
                p2Fsm.SetState(PlayerFSM.EState.Idle);
                isRotating = false;
            });
        });
    }
    private void OnFocusPlayer(string player)
    {
        Vector3 pos = Vector3.zero;
        switch (player)
        {
            case "p1":
                pos = p1StartPos;
                break;
            case "p2":
                pos = p1StartPos;
                pos.x *= -1;
                break;
        }
        cam.DOOrthoSize(5.85f, 0.3f);
        transform.DOMove(pos, 0.3f);
    }
    private void OnEndFocus()
    {
        cam.DOOrthoSize(ori_Orthosize, 0.3f);
        transform.DOMove(-Vector3.forward, 0.3f);
    }
}
