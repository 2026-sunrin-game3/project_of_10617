using System.Collections;
using UnityEngine;

public class GasterBlaster : MonoBehaviour
{
    [SerializeField] float chargeTime = 0.6f;
    [SerializeField] float fireTime = 0.3f;
    [SerializeField] Vector2 beamSize = new Vector2(6f, 1.2f);

    Vector2 fireDir;
    float damage;
    EntityHealth attacker;
    LayerMask targetMask;
    SpriteRenderer sprite;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Init(Vector2 direction, float dmg, EntityHealth attackerHealth, LayerMask mask)
    {
        fireDir = direction.normalized;
        damage = dmg;
        attacker = attackerHealth;
        targetMask = mask;

        // The skull art faces left (snout toward -X) by default. A 180-degree
        // rotation would mirror it AND flip it upside down (a Z rotation is a
        // point reflection, not a mirror), so use flipX to mirror left/right
        // only, and keep the beam's own hitbox oriented off fireDir directly
        // rather than off the transform, which never actually rotates.
        sprite.flipX = fireDir.x > 0;

        StartCoroutine(FireRoutine());
    }

    IEnumerator FireRoutine()
    {
        yield return new WaitForSeconds(chargeTime);

        Vector2 center = (Vector2)transform.position + fireDir * beamSize.x * 0.5f;
        float angle = Mathf.Atan2(fireDir.y, fireDir.x) * Mathf.Rad2Deg;
        var hits = Physics2D.OverlapBoxAll(center, beamSize, angle, targetMask);
        foreach (var hit in hits)
        {
            EntityHealth hp = hit.GetComponent<EntityHealth>();
            if (hp != null)
                hp.GetDamage(damage, attacker);
        }

        yield return new WaitForSeconds(fireTime);
        Destroy(gameObject);
    }
}
