using System.Collections;
using UnityEngine;

public class Bone : MonoBehaviour
{
    [SerializeField] float speed = 6f;
    [SerializeField] float lifeTime = 4f;
    // How long it sits rooted (playing the rise telegraph) before it
    // detaches, starts flying, and becomes dangerous to touch.
    [SerializeField] float telegraphTime = 0.4f;

    Vector2 moveDir;
    float damage;
    EntityHealth attacker;
    LayerMask targetMask;
    Collider2D col;
    bool launched;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
    }

    public void Init(Vector2 direction, float dmg, EntityHealth attackerHealth, LayerMask mask)
    {
        moveDir = direction.normalized;
        damage = dmg;
        attacker = attackerHealth;
        targetMask = mask;

        Destroy(gameObject, telegraphTime + lifeTime);
        StartCoroutine(LaunchAfterTelegraph());
    }

    IEnumerator LaunchAfterTelegraph()
    {
        yield return new WaitForSeconds(telegraphTime);
        launched = true;
        if (col != null)
            col.enabled = true;
    }

    void Update()
    {
        if (!launched)
            return;

        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!launched)
            return;

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
