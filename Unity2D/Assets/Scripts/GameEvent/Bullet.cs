using DG.Tweening;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private ParticleSystem portal;
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private float destroyTime;
    private Vector3 movePos;
    private float flyingTime;
    private void OnEnable()
    {
        movePos = Vector3.zero;
        flyingTime = 0;

        transform.position = transform.parent.position;

        var p1Dist = Vector3.Magnitude(GameManager.Instance.player1.position - transform.position);
        var p2Dist = Vector3.Magnitude(GameManager.Instance.player2.position - transform.position);

        movePos = (p1Dist < p2Dist) ? GameManager.Instance.player1.position : GameManager.Instance.player2.position;
        movePos -= transform.parent.position;

        movePos.Normalize();
    }
    private void FixedUpdate()
    {
        transform.position += movePos * moveSpeed * Time.fixedDeltaTime;
        flyingTime += Time.fixedDeltaTime;
        if (flyingTime > destroyTime) Explode();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Explode();
            if (movePos.x > 0)
            {
                other.GetComponent<Rigidbody2D>().AddForce(Vector2.one * 10, ForceMode2D.Impulse);
            }
            else
            {
                other.GetComponent<Rigidbody2D>().AddForce(-Vector2.right * 10 + Vector2.up * 10, ForceMode2D.Impulse);
            }
        }
    }

    private void Explode()
    {
        explosion.transform.position = transform.position;
        explosion.gameObject.SetActive(true);
        gameObject.SetActive(false);

        portal.transform.DOScale(0, 0.5f).SetDelay(1)
                        .OnComplete(() =>
                        {
                            transform.parent.gameObject.SetActive(false);
                            ObjectPool.ReleaseObject(transform.parent.gameObject);
                        });
    }
}
