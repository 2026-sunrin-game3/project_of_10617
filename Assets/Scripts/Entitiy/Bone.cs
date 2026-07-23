using UnityEngine;

public class Bone : MonoBehaviour
{
    [SerializeField] float speed = 6f;
    [SerializeField] float lifeTime = 4f;

    Vector2 moveDir;
    float damage;
    EntityHealth attacker;
    LayerMask targetMask;

    public void Init(Vector2 direction, float dmg, EntityHealth attackerHealth, LayerMask mask)
    {
        moveDir = direction.normalized;
        damage = dmg;
        attacker = attackerHealth;
        targetMask = mask;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetMask.value) == 0)
            return;

        EntityHealth hp = other.GetComponent<EntityHealth>();
        if (hp != null)
        {
            hp.GetDamage(damage, attacker);
            Destroy(gameObject);
        }
    }
}
