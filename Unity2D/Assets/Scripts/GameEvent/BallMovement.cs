using DG.Tweening;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public float moveSpeed;
    public float spinSpeed;
    private AudioSource HitWallSound;

    private Rigidbody2D rb;
    private Vector3 ballPos;
    private Vector2 moveDirection;

    private float maxX;
    private float maxY;

    private void OnEnable()
    {
        transform.DOScale(Vector3.one, 3f).SetEase(Ease.InOutBounce).OnComplete(() =>
        {
            // NOTE: 發射至隨機向量
            moveDirection = Random.insideUnitCircle.normalized;
            GetComponent<Rigidbody2D>().velocity = moveDirection * moveSpeed;
        });
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        var col2D = GetComponent<CircleCollider2D>();
        HitWallSound = GetComponent<AudioSource>();
        maxX = Camera.main.orthographicSize * Camera.main.aspect;
        maxX -= col2D.radius;
        maxY = Camera.main.orthographicSize;
        maxY -= col2D.radius;
        spinRot = 0;
    }

    private void FixedUpdate() => ScreenBounds();

    private float spinRot;
    private bool isSaveVelocity = false;
    private Vector2 tmpV = Vector2.zero;
    private void ScreenBounds()
    {
        if (CamCtrl.isRotating && !isSaveVelocity)
        {
            isSaveVelocity = true;
            tmpV = rb.velocity;
            rb.velocity = Vector2.zero;
        }
        else if (CamCtrl.isRotating)
        {
            return;
        }
        else if (!CamCtrl.isRotating && isSaveVelocity)
        {
            isSaveVelocity = false;
            rb.velocity = tmpV;
        }

        ballPos = transform.position;

        if (ballPos.x > maxX || ballPos.x < -maxX)
        {
            moveDirection.x = -moveDirection.x;
            rb.velocity = moveDirection * moveSpeed;
            CamCtrl.Shake(0.5f, 0.5f);
            HitWallSound.Play();
        }

        if (ballPos.y > maxY || ballPos.y < -maxY)
        {
            moveDirection.y = -moveDirection.y;
            rb.velocity = moveDirection * moveSpeed;
            CamCtrl.Shake(0.5f, 0.5f);
            HitWallSound.Play();
        }
        spinRot += Time.fixedDeltaTime * spinSpeed;
        transform.rotation = Quaternion.Euler(0, 0, spinRot);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<PlayerFSM>().CurrState() == PlayerFSM.EState.Transporting) return;
            other.GetComponent<PlayerFSM>().SetState(PlayerFSM.EState.Dead);
        }
    }
}
