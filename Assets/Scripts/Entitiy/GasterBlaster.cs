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

    public void Init(Vector2 direction, float dmg, EntityHealth attackerHealth, LayerMask mask)
    {
        fireDir = direction.normalized;
        damage = dmg;
        attacker = attackerHealth;
        targetMask = mask;

        // The skull art faces left (mouth opens toward -X), not along
        // transform.right, so offset by 180 to line the mouth - and the
        // beam, which fires along transform.right - up with fireDir.
        float angle = Mathf.Atan2(fireDir.y, fireDir.x) * Mathf.Rad2Deg + 180f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        StartCoroutine(FireRoutine());
    }

    IEnumerator FireRoutine()
    {
        yield return new WaitForSeconds(chargeTime);

        Vector2 center = (Vector2)transform.position + (Vector2)(transform.right * beamSize.x * 0.5f);
        var hits = Physics2D.OverlapBoxAll(center, beamSize, transform.eulerAngles.z, targetMask);
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
