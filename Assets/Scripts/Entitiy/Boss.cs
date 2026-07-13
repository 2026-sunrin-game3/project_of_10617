using UnityEngine;

public class Boss : Enemy
{
    [SerializeField]
    PlayerController player;
    
    [SerializeField] AttackRange defaultAttack;

    public float attackDist = 1.5f;
    protected override void MobUpdate()
    {
        if (Vector2.Distance(player.transform.position, transform.position) <= attackDist)
        {
            Attack(0.5f, defaultAttack, transform.position);
        }
        else
        {
            Chase(player.transform);
        }
    }

    protected override void DrawGizmos()
    {
        Draw(defaultAttack);
    }
}
