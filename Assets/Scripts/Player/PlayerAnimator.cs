using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator animator;
    EntityStat stat;

    public float direction;

    void Start()
    {
        animator = GetComponent<Animator>();
        stat = GetComponent<EntityStat>();
    }

    public void SetMoving(bool val, Vector2 axis)
    {
        animator.SetBool("isMoving", val);

        float moveRate = stat.GetResultValue("moveSpeed") / stat.GetBaseValue("moveSpeed");

        animator.SetFloat("moveSpeed", moveRate);
        if (val)
        {
            if (axis.x > 0)
                direction = 1;
            else if (axis.x < 0)
                direction = -1;

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            transform.localScale = scale;
        }
    }
    
    public void Jump()
    {
        animator.SetTrigger("Jump");
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }
}