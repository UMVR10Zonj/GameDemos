using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DoorCtrl : MonoBehaviour
{
    [SerializeField] private Transform doorFrame;
    [SerializeField] private Transform doorL;
    [SerializeField] private Transform doorR;
    [SerializeField] private Transform doorlight;
    [SerializeField] private List<AudioClip> effClips;
    private AudioSource EffPlayer;

    public Tween Close()
    {
        EffPlayer = GetComponent<AudioSource>();
        EffPlayer.clip = effClips[0];
        EffPlayer.Play();
        doorlight.GetComponent<Image>().color = Color.red;

        doorFrame.DOScale(Vector3.one, 1);
        doorL.DOLocalMoveX(-280, 1.5f).SetEase(Ease.InOutBounce).SetDelay(1f);
        doorR.DOLocalMoveX(280, 1.5f).SetEase(Ease.InOutBounce).SetDelay(1f);
        return doorlight.DOScale(1, 0.5f).SetDelay(2.5f);
    }
    public Tween Open()
    {
        doorlight.GetComponent<Image>().DOColor(Color.green, 0.5f);
        doorL.DOLocalMoveX(-1110, 1.5f).SetEase(Ease.OutBounce).SetDelay(0.5f);
        doorR.DOLocalMoveX(1175, 1.5f).SetEase(Ease.OutBounce).SetDelay(0.5f);
        return doorFrame.DOScale(Vector3.one * 2, 1).SetDelay(1.5f)
                        .OnComplete(() => doorlight.localScale = Vector3.zero);
    }
}
