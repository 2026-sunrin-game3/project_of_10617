using UnityEngine;
using UnityEngine.UI;

public class Boss : Enemy
{
    [SerializeField]
    PlayerController player;
    public float attackDist = 1.5f;
    [SerializeField] AttackRange defaultAttack;

    public float retreatTime = 0.6f;
    float retreatTimer;

    [SerializeField] Slider bossbar;
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected override void MobUpdate()
    {
        bossbar.value = health.health / health.maxHealth;
        if (retreatTimer > 0)
            retreatTimer -= Time.deltaTime;

        float dist = Vector2.Distance(player.transform.position, transform.position);
        bool moving = false;

        if (retreatTimer > 0)
        {
            float away = player.transform.position.x > transform.position.x ? -1 : 1;
            Move(Vector2.right * away);
            moving = true;
        }
        else if (dist <= attackDist)
        {
            if (atkCool <= 0)
            {
                retreatTimer = retreatTime;
                animator.SetTrigger("Attack");
            }
            Attack(0.5f, defaultAttack, transform.position);
        }
        else
        {
            Chase(player.transform);
            moving = true;
        }

        SetFacing(player.transform.position.x > transform.position.x ? 1 : -1);
        animator.SetBool("isMoving", moving);
    }

    void SetFacing(float dir)
    {
        direction = dir;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        transform.localScale = scale;
    }

    protected override void DrawGizmos()
    {
        Draw(defaultAttack);
    }
}
