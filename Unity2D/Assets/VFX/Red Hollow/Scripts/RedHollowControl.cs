using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedHollowControl : MonoBehaviour
{
    [Range(0.0f, 1.0f)] public float hue = 0;
    [SerializeField] private List<AudioClip> effclips;
    private HueControl hc;
    private Animator animator;
    private AudioSource eff;


    private void Start()
    {
        hc = transform.GetChild(0).GetComponent<HueControl>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        eff = GetComponent<AudioSource>();
    }
    // private void FixedUpdate()
    // {
    //     hc.hue = hue;
    // }

    public void Play_Charging()
    {
        eff.clip = effclips[2];
        eff.Play();
        animator.Play("Red Hollow - Charging");
    }
    public void Finish_Charging()
    {
        eff.clip = effclips[1];
        eff.Play();
        animator.Play("Red Hollow - Charged");
    }
    public void Burst_Beam()
    {
        animator.Play("Red Hollow - Burst");
        eff.clip = effclips[0];
        eff.Play();
        CamCtrl.Shake(3f, 0.5f);
    }
    public void Dead()
    {
        animator.Play("Red Hollow - Dead");
    }

    public void Play()
    {
        StartCoroutine(PlayAnim());
    }
    float m_hue;
    private IEnumerator PlayAnim()
    {
        m_hue = 0;
        hc.hue = 0;
        Play_Charging();
        yield return new WaitForSeconds(2f);
        while (m_hue < 0.5f)
        {
            m_hue += 0.5f * Time.deltaTime;
            hc.hue = m_hue;
            yield return null;
        }

        Finish_Charging();
        yield return new WaitForSeconds(2f);
        if (CamCtrl.isRotating) yield return null;

        Burst_Beam();
        Transform p1 = GameManager.Instance.player1;
        Transform p2 = GameManager.Instance.player2;
        for (float i = 1.5f; i > 0; i -= Time.deltaTime)
        {
            if (p1.GetComponent<PlayerFSM>().CurrState() == PlayerFSM.EState.Transporting) yield return null;
            if (p2.GetComponent<PlayerFSM>().CurrState() == PlayerFSM.EState.Transporting) yield return null;
            var p1Dist = Vector3.Magnitude(p1.position - transform.position);
            var p2Dist = Vector3.Magnitude(p2.position - transform.position);

            if (p1Dist < 8f) p1.GetComponent<PlayerFSM>().SetState(PlayerFSM.EState.Dead);
            if (p2Dist < 8f) p2.GetComponent<PlayerFSM>().SetState(PlayerFSM.EState.Dead);
            yield return null;
        }

        Dead();
    }
}
