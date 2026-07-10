using UnityEngine;

public class PlayerBattle : MonoBehaviour
{
    public EntityHealth health;
    public EntityStat stat;
    [System.Serializable]

    public struct AttakRange
    {
        public Vector2 offset, size;

        public bool drawGizmos;
    }

    public AttakRange defaultAttack;
    [SerializeField] LayerMask enemyMask;

    void Start()
    {
        health = GetComponent<EntityHealth>();
        stat = GetComponent<EntityStat>();
    }

    public void Attack()
    {
        var col = Physics2D.OverlapBoxAll((Vector2)transform.position + defaultAttack.offset, defaultAttack.size, 0, enemyMask);

        foreach (var target in col)
        {
            EntityHealth hp = target.GetComponent<EntityHealth>();
            if (hp != null)
            {
                hp.GetDamage(3, health);
            }
        }
    }

    void Draw(AttakRange range)
    {
        if (!range.drawGizmos)
            return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + range.offset, range.size);
    }

    void OnDrawGizmos()
    {
        Draw(defaultAttack);
    }

}
