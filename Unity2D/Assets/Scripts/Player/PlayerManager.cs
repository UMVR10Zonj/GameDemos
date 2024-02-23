using DG.Tweening;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject p1Plate;
    [SerializeField] private GameObject player2;
    [SerializeField] private GameObject p2Plate;

    private PlayerFSM p1FSM;
    private PlayerFSM p2FSM;
    private PlayerController p1Ctrller;
    private PlayerController p2Ctrller;
    private Vector3 p1RespawnPos;
    private Vector3 p2RespawnPos;

    private void Start()
    {
        p1FSM = player1.GetComponent<PlayerFSM>();
        p2FSM = player2.GetComponent<PlayerFSM>();
        p1Ctrller = player1.GetComponent<PlayerController>();
        p2Ctrller = player2.GetComponent<PlayerController>();
        p1RespawnPos = new Vector3(-4, 1.5f, 0);
        p2RespawnPos = new Vector3(4, 1.5f, 0);

        p1FSM.States[PlayerFSM.EState.ReSpawnable].OnEnterEvent += () =>
        {
            player1.SetActive(false);
            if (GameManager.Instance.isGaming)
            {
                SpawnPlayer("p1");
            }
        };
        p2FSM.States[PlayerFSM.EState.ReSpawnable].OnEnterEvent += () =>
        {
            player2.SetActive(false);
            if (GameManager.Instance.isGaming)
            {
                SpawnPlayer("p2");
            }
        };

        p1FSM.States[PlayerFSM.EState.Spawn].OnEnterEvent += () =>
        {
            player1.transform.DOMoveY(-4, 1).SetRelative().OnComplete(() =>
            {
                p1Plate.SetActive(false);
                p1FSM.SetState(PlayerFSM.EState.Idle);
            });
        };
        p2FSM.States[PlayerFSM.EState.Spawn].OnEnterEvent += () =>
        {
            player2.transform.DOMoveY(-4, 1).SetRelative().OnComplete(() =>
            {
                p2Plate.SetActive(false);
                p2FSM.SetState(PlayerFSM.EState.Idle);
            });
        };
    }

    public void SpawnPlayer(string player)
    {
        switch (player)
        {
            case "p1":
                player1.transform.position = p1RespawnPos;
                player1.GetComponent<SpriteRenderer>().DOFade(1, 0.1f);
                p1Ctrller.Reset();
                player1.SetActive(true);
                p1FSM.SetState(PlayerFSM.EState.Spawn);

                p1Plate.SetActive(true);
                break;

            case "p2":
                player2.transform.position = p2RespawnPos;
                player2.GetComponent<SpriteRenderer>().DOFade(1, 0.1f);
                p2Ctrller.Reset();
                player2.SetActive(true);
                p2FSM.SetState(PlayerFSM.EState.Spawn);

                p2Plate.SetActive(true);
                break;
        }
    }
}
