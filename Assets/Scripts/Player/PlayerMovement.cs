using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rigid;
    private BoxCollider2D col_;
    EntityStat stat;
    public float jumpPower = 15f;
    [SerializeField] LayerMask groundMask_;
    [SerializeField] float groundDist_ = 0.05f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        stat = GetComponent<EntityStat>();
        col_ = GetComponent<BoxCollider2D>();
    }
    
    public void Move(Vector2 axis)
    {
        if (!enabled)
            return;

        float moveSpeed = stat.GetResultValue("moveSpeed");
        transform.Translate(axis.normalized * moveSpeed * Time.deltaTime);
    }

    public void SetVelocity(Vector2 dir)
    {
        rigid.linearVelocity = dir;
    }

    public bool OnGround()
    {
        Bounds b = col_.bounds;
        Vector2 center = new Vector2(b.center.x, b.min.y - groundDist_ * 0.5f);
        Vector2 size = new Vector2(b.size.x * 0.9f, groundDist_);
        Collider2D[] cast = Physics2D.OverlapBoxAll(center, size, 0f, groundMask_);

        return cast.Length > 0;
    }

    public bool Jump()
    {
        if (OnGround())
        {
            SetVelocity(Vector2.up * jumpPower);
            return true;
        }
        return false;
    }

    void OnDrawGizmos()
    {
        if (col_ == null)
            col_ = GetComponent<BoxCollider2D>();
        if (col_ == null)
            return;

        Bounds b = col_.bounds;
        Vector2 center = new Vector2(b.center.x, b.min.y - groundDist_ * 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(center, new Vector3(b.size.x * 0.9f, groundDist_));
    }
}    