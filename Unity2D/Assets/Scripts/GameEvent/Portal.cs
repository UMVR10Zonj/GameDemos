using UnityEngine;
using DG.Tweening;

public class Portal : MonoBehaviour
{
    [SerializeField] private Transform linkedPortal;
    private float p1Distance;
    private float p2Distance;
    private bool transporting;

    private void Update()
    {
        p1Distance = Vector3.Magnitude(GameManager.Instance.player1.position - transform.position);
        p2Distance = Vector3.Magnitude(GameManager.Instance.player2.position - transform.position);
    }

    private void FixedUpdate()
    {
        if (transporting) return;

        if (p1Distance < 2)
        {
            transporting = true;
            GameManager.Instance.player1.GetComponent<PlayerFSM>().SetState(PlayerFSM.EState.Transporting);
            GameManager.Instance.player1.DOScale(0, 0.5f);
            GameManager.Instance.player1.DOLocalMove(transform.position, 0.5f).SetDelay(0.5f);

            GameManager.Instance.player1.position = linkedPortal.position;
            GameManager.Instance.player1.DOScale(1, 0.5f).SetDelay(1).OnComplete(() =>
            {
                transporting = false;
                GameManager.Instance.player1.GetComponent<PlayerFSM>().SetState(PlayerFSM.EState.Idle);
            });
        }
        else if (p2Distance < 2)
        {
            transporting = true;
            GameManager.Instance.player2.GetComponent<PlayerFSM>().SetState(PlayerFSM.EState.Transporting);
            GameManager.Instance.player2.DOScale(0, 0.5f);
            GameManager.Instance.player2.DOLocalMove(transform.position, 0.5f).SetDelay(0.5f);

            GameManager.Instance.player2.position = linkedPortal.position;
            GameManager.Instance.player2.DOScale(1, 0.5f).SetDelay(1).OnComplete(() =>
            {
                transporting = false;
                GameManager.Instance.player2.GetComponent<PlayerFSM>().SetState(PlayerFSM.EState.Idle);
            });
        }
    }
}
