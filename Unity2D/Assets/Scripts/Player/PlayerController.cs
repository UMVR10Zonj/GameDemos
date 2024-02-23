using System;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Field Declaration

    [Header("Controller")]
    [SerializeField] private ControllerType inputType;

    #region Move

    [Header("Move")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float moveSpeed;
    [Tooltip("From 0 speed to Max Speed Frame")]
    [SerializeField] private float MaxSFrame;
    [Tooltip("From Max Speed to 0 speed Frame")]
    [SerializeField] private float MinSFrame;
    [SerializeField] private float wallChk;
    private float speed;
    private float moveVelocity
    {
        get => speed;
        set
        {
            if (value >= maxSpeed)
            {
                speed = maxSpeed;
            }
            else if (value <= 0)
            {
                speed = 0;
            }
            else
            {
                speed = value;
            }
        }
    }
    // -1面向左, 1 面向右
    private float face;
    private float AxisH;

    #endregion
    #region Jump

    [Header("Jump")]
    [SerializeField] public float jumpHight;
    [SerializeField][Range(0, 20)] private float fallSpeed;
    [SerializeField] private Transform groundChkObj;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AnimationCurve riseCurv;
    private Rigidbody2D rb;
    float jumpVelocity = 0;
    private Vector2 groundChkSize;
    [NonSerialized] public Vector2 m_Gravity;

    #endregion

    private InputHandler ih;
    private PlayerFSM fsm;

    #endregion

    private void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        ih = new InputHandler();
        fsm = GetComponent<PlayerFSM>();
        groundChkSize = new Vector2(0.8f, 0.2f);
        m_Gravity = new Vector2(0, -Physics2D.gravity.y);
    }

    private void Awake() => Init();
    private void Start()
    {
        fsm.States[PlayerFSM.EState.Dead].OnEnterEvent += () =>
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        };
        fsm.States[PlayerFSM.EState.Spawn].OnEnterEvent += () =>
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        };
    }
    // NOTE: Input
    private void Update()
    {
        if (!GameManager.Instance.isGaming) return;
        if (fsm.CurrState() == PlayerFSM.EState.UnCtrlable) return;
        if (fsm.CurrState() == PlayerFSM.EState.Dead) return;
        if (fsm.CurrState() == PlayerFSM.EState.Spawn) return;
        if (fsm.CurrState() == PlayerFSM.EState.Transporting) return;

        MoveInputHandler();
        JumpInputHandler();
    }
    // NOTE: Animation
    private void FixedUpdate()
    {
        if (!GameManager.Instance.isGaming) return;
        if (fsm.CurrState() == PlayerFSM.EState.UnCtrlable) return;
        if (fsm.CurrState() == PlayerFSM.EState.Dead) return;
        if (fsm.CurrState() == PlayerFSM.EState.Spawn) return;
        if (fsm.CurrState() == PlayerFSM.EState.Transporting) return;

        MoveAnimHandler();
        JumpAnimHandler();
    }

    public void Reset()
    {
        moveVelocity = 0;
        jumpVelocity = 0;
    }

    private void MoveInputHandler()
    {
        AxisH = ih.GetAxisH(inputType);
        if (Physics2D.gravity.y > 0) AxisH *= -1;
        if (AxisH != 0)
        {
            if (AxisH > 0)
            {
                face = 1;
                transform.localScale = Vector3.one;
            }
            else
            {
                face = -1;
                transform.localScale = Vector3.one - Vector3.right * 2;
            }
            moveVelocity += maxSpeed / MaxSFrame;
        }
        else if (AxisH == 0)
        {
            moveVelocity -= maxSpeed / MinSFrame;
        }
    }
    private void MoveAnimHandler()
    {
        if (moveVelocity > 0 && WallhitChk() == null)
        {
            fsm.SetState(PlayerFSM.EState.Move);
            transform.position += Vector3.right * face * moveVelocity * Time.fixedDeltaTime;
            // rb.position += Vector2.right * face * moveVelocity * Time.fixedDeltaTime;
        }
        if (moveVelocity == 0 && fsm.CurrState() != PlayerFSM.EState.Spawn)
        {
            fsm.SetState(PlayerFSM.EState.Idle);
        }
    }
    private Collider2D WallhitChk()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * face, wallChk, groundLayer);
        return hit.collider;
    }

    private void JumpInputHandler()
    {
        if (ih.GetJumpBtnDown(inputType) && isGrounded())
        {
            Jump();
        }
    }
    private void JumpAnimHandler()
    {
        transform.position += Vector3.up * jumpVelocity * Time.deltaTime;

        if (rb.velocity.y != 0)
        {
            rb.velocity -= m_Gravity * fallSpeed * Time.deltaTime;
        }
    }
    private bool isGrounded()
    {
        return Physics2D.OverlapCapsule(groundChkObj.position, groundChkSize, CapsuleDirection2D.Horizontal, 0, groundLayer) ? true : false;
    }

    private void Jump()
    {
        fsm.SetState(PlayerFSM.EState.Jump);
        transform.DOScaleX(0.8f, 0.3f).OnComplete(() => transform.DOScaleX(1, 0.1f));
        // transform.DOScaleY(1.5f, 0.3f).OnComplete(() => transform.DOScaleY(1, 0.1f));
        DOTween.To(() => jumpVelocity, (x) => jumpVelocity = x, jumpHight, 0.2f)
               .SetRelative()
               .SetEase(EaseJump)
               .OnComplete(() => jumpVelocity = 0);
    }
    private float EaseJump(float time, float duration, float overshootOrAmplitude, float period)
    {
        float t = time / duration;
        return riseCurv.Evaluate(t);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // if (other.gameObject.name == gameObject.name) return;

        // NOTE: Handle Step Attack
        if (other.gameObject.name == "Player1" || other.gameObject.name == "Player2")
        {
            var otherFsm = other.transform.GetComponent<PlayerFSM>();
            if (otherFsm.CurrState() == PlayerFSM.EState.Spawn || fsm.CurrState() == PlayerFSM.EState.Spawn) return;
            if (otherFsm.CurrState() == PlayerFSM.EState.Dead || fsm.CurrState() == PlayerFSM.EState.Dead) return;

            if (m_Gravity.y > 0 && transform.position.y - other.transform.position.y > 0.5f ||
                m_Gravity.y < 0 && other.transform.position.y - transform.position.y > 0.5f)
            {
                CamCtrl.Shake(0.3f, 0.3f);
                Jump();
            }
            else if (m_Gravity.y > 0 && transform.position.y - other.transform.position.y < -0.5f ||
                     m_Gravity.y < 0 && other.transform.position.y - transform.position.y < -0.5f)
            {
                fsm.SetState(PlayerFSM.EState.Dead);
            }
        }

        if (other.gameObject.tag == "DeadArea")
        {
            fsm.SetState(PlayerFSM.EState.Dead);
        }
    }
}
