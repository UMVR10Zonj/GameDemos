using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem nova;
    [SerializeField] private ParticleSystem sparkleElplode;
    [SerializeField] private ParticleSystem portal;
    [SerializeField] private SpriteRenderer blinkWhite;
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private ParticleSystem trail;

    private Animation anim;
    private PlayerFSM fsm;
    private AudioSource EffPlayer;

    private void Start()
    {
        nova.Pause();
        sparkleElplode.Pause();

        anim = GetComponent<Animation>();
        fsm = GetComponent<PlayerFSM>();
        EffPlayer = GetComponent<AudioSource>();
        EffPlayer.Pause();

        fsm.States[PlayerFSM.EState.Dead].OnEnterEvent += () => DeadVFX();
        fsm.States[PlayerFSM.EState.Spawn].OnEnterEvent += () => Blink();
    }

    public void SpawnOnStart()
    {
        portal.gameObject.SetActive(true);
        portal.transform.DOScaleX(0.6f, 1).SetEase(Ease.InOutBounce)
                        .OnComplete(() =>
                        {
                            StartCoroutine(PlayStartSpawn(() =>
                            {
                                portal.transform.DOScaleX(0, 0.5f);

                                transform.DOMoveY(1, 0.2f).SetRelative();
                                transform.DORotate(Vector3.forward * -3600, 1f).SetRelative()
                                         .OnComplete(() => transform.DOMoveY(-1, 0.2f).SetRelative());
                                trail.transform.DOScale(1, 0.1f);
                            }));
                        });
    }
    // NOTE: Start Anim player2 jump shake
    public void JumpShake()
    {
        EffPlayer.Play();
        CamCtrl.Shake(0.08f, 0.5f);
    }
    public void DeadVFX()
    {
        StartCoroutine(PlayParticle(nova, () => nova.transform.position = Vector3.zero));
        Blink();

        transform.DOScale(new Vector3(0.3f, 1.3f, 1), 0.7f).SetEase(EaseScale)
                 .OnComplete(() => transform.DOScale(Vector3.one, 0.3f)
                                            .OnComplete(() =>
                                            {
                                                GetComponent<SpriteRenderer>().DOFade(0, 0.1f);
                                                StartCoroutine(PlayParticle(sparkleElplode, () =>
                                                {
                                                    sparkleElplode.transform.position = Vector3.zero;
                                                }));

                                                fsm.SetState(PlayerFSM.EState.ReSpawnable);
                                            }));
    }

    private Sequence Blink()
    {
        Sequence tmpSeq = DOTween.Sequence().SetRecyclable();
        for (int i = 0; i < 3; i++)
        {
            tmpSeq.Append(blinkWhite.DOFade(1, 0.2f));
            tmpSeq.Append(blinkWhite.DOFade(0, 0.2f));
        }
        return tmpSeq;
    }

    private float EaseScale(float time, float duration, float overshootOrAmplitude, float period)
    {
        float t = time / duration;
        return scaleCurve.Evaluate(t);
    }

    /// <summary>
    /// do action on particle end
    /// </summary>
    /// <param name="p">to play particle</param>
    /// <param name="action">to do action on particle end</param>
    /// <returns></returns>
    private IEnumerator PlayParticle(ParticleSystem p, Action action)
    {
        p.transform.position = transform.position;
        p.Play();
        var eff = p.GetComponent<AudioSource>();
        if (eff != null) eff.Play();

        while (p.isPlaying)
        {
            yield return null;
        }

        action.Invoke();
    }

    private IEnumerator PlayStartSpawn(Action action)
    {
        anim.Play();

        while (anim.isPlaying)
        {
            yield return null;
        }

        action.Invoke();
    }

    public void Sound_Doing()
    {
        EffPlayer.Play();
    }
}
